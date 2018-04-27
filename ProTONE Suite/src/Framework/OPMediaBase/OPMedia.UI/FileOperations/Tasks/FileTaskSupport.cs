using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core;
using OPMedia.Core.TranslationSupport;
using System.Windows.Forms;
using OPMedia.UI.FileTasks;
using System.Threading;
using System.IO;
using OPMedia.Runtime.FileInformation;
using System.ComponentModel;
using OPMedia.UI.Controls;
using OPMedia.Core.ComTypes;
using System.Diagnostics;
using OPMedia.Core.Logging;

namespace OPMedia.UI.FileOperations.Tasks
{
    public class FileTaskSupport
    {
        BaseFileTask _task = null;

        bool _skipConfirmations = false;

        public bool SkipConfirmations
        {
            get
            {
                return _skipConfirmations;
            }
        }

        public bool RequiresRefresh { get; private set; }

        #region Constructor

        public FileTaskSupport(BaseFileTask task)
        {
            _task = task;
        }

        ~FileTaskSupport()
        {
            _task = null;
        }

        #endregion

        #region Task pause / interrupt support

        ManualResetEvent _fileTaskWaitEvent = new ManualResetEvent(false);
        object _lock = new object();

        public void RequestAbort()
        {

            try
            {
                _fileTaskWaitEvent.Set();
                TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Paused);

                if (MessageDisplay.Query(Translator.Translate("TXT_CONFIRM_ABORT"),
                    Translator.Translate("TXT_CONFIRM"), MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CanContinue = false;
                }
            }
            finally
            {
                _fileTaskWaitEvent.Reset();
                TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Normal);
            }

        }

        ManualResetEvent _canContinueEvent = new ManualResetEvent(true);
        public bool CanContinue 
        {
            get
            {
                return _canContinueEvent.WaitOne(0);
            }

            private set
            {
                if (value)
                    _canContinueEvent.Set();
                else
                    _canContinueEvent.Reset();
            }
        }

        public void CheckIfCanContinue(string path)
        {
            while (_fileTaskWaitEvent.WaitOne(10))
                Application.DoEvents();

            if (CanContinue == false)
                throw new TaskInterruptedException(path);
        }

        #endregion

        #region Confirmation support

        #region Generic confirmations

        protected class ConfirmationData
        {
            public bool ConfirmationResult { get; set; }
            public bool FlagValue { get; set; }

            public ConfirmationData()
            {
                ConfirmationResult = FlagValue = false;
            }
        }

        protected ConfirmationData ConfirmObjectAction(string tag, string objectName)
        {
            try
            {
                _fileTaskWaitEvent.Set();
                TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Paused);

                ConfirmationData retVal = new ConfirmationData();

                MainThread.Send(delegate(object x)
                {
                    DialogResult dr = DialogResult.Abort;

                    if (_task.ObjectsCount == 1)
                    {
                        dr = MessageDisplay.Query(
                        Translator.Translate(tag, objectName), "TXT_CONFIRM");
                    }
                    else
                    {
                        dr = MessageDisplay.QueryWithCancelAndAbort(
                        Translator.Translate(tag, objectName),
                        "TXT_CONFIRM", (_task.ObjectsCount > 1));
                    }

                    switch (dr)
                    {
                        case DialogResult.Abort:
                            CanContinue = false;
                            break;

                        case DialogResult.No:
                            retVal.ConfirmationResult = false;
                            break;

                        case DialogResult.Yes:
                            retVal.ConfirmationResult = true;
                            break;

                        case DialogResult.OK: // YES ALL
                            retVal.ConfirmationResult = true;
                            retVal.FlagValue = true;
                            break;
                    }
                });

                return retVal;
            }
            finally
            {
                _fileTaskWaitEvent.Reset();
                TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Normal);
            }
        }

        protected bool ConfirmObjectAction(string tag, string objectName, ref bool flagValue)
        {
            ConfirmationData data = ConfirmObjectAction(tag, objectName);
            flagValue = data.FlagValue;
            return data.ConfirmationResult;
        }

        #endregion

        #region Overwrite confirmations

        protected bool _overwrite = false;
        protected bool _overwriteReadOnly = false;
        public bool CanOverwrite(string path)
        {
            if (!_overwrite)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_OVERWRITE", path, ref _overwrite))
                    return false;
            }

            if (PathUtils.ObjectHasAttribute(path, FileAttributes.ReadOnly) && !_overwriteReadOnly)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_OVERWRITE_RO", path, ref _overwriteReadOnly))
                    return false;
            }

            return true;
        }

        #endregion

        #region Delete confirmations

        protected bool _delete = false;
        protected bool _deleteROHS = false;
        public bool CanDelete(string path)
        {
            if (!_delete)
            {
                string confirmMsg = PathUtils.ObjectHasAttribute(path, FileAttributes.Directory) ?
                    "TXT_CONFIRM_DELETE_EMPTYDIR" : "TXT_CONFIRM_DELETE";

                if (!ConfirmObjectAction(confirmMsg, path, ref _delete))
                    return false;
            }

            if (PathUtils.ObjectHasAttribute(path, FileAttributes.ReadOnly | FileAttributes.System | FileAttributes.Hidden)
                && !_deleteROHS)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_DELETE_ROHS", path, ref _deleteROHS))
                    return false;
            }

            return true;
        }

        protected bool _deleteNonEmptyFolders = false;
        public bool CanDeleteNonEmptyFolder(string path)
        {
            if (!_deleteNonEmptyFolders)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_DELETE_DIR", path, ref _deleteNonEmptyFolders))
                    return false;
            }

            return true;
        }

        #endregion

        #region Move confirmations

        protected bool _moveNonEmptyFolders = false;
        public bool CanMoveNonEmptyFolder(string path)
        {
            if (!_moveNonEmptyFolders)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_MOVE_DIR", path, ref _moveNonEmptyFolders))
                    return false;
            }

            return true;
        }

        protected bool _move = false;
        protected bool _moveROHS = false;
        public bool CanMove(string path)
        {
            if (!_move)
            {
                string confirmMsg = PathUtils.ObjectHasAttribute(path, FileAttributes.Directory) ?
                    "TXT_CONFIRM_MOVE_EMPTYDIR" : "TXT_CONFIRM_MOVE";

                if (!ConfirmObjectAction(confirmMsg, path, ref _move))
                    return false;
            }

            if (PathUtils.ObjectHasAttribute(path, FileAttributes.ReadOnly | FileAttributes.System | FileAttributes.Hidden)
                && !_moveROHS)
            {
                if (!ConfirmObjectAction("TXT_CONFIRM_MOVE_ROHS", path, ref _moveROHS))
                    return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region File system support

        #region File operations

        public bool CopyFile(string srcFile, string destFile)
        {
            try
            {
                if (_skipConfirmations || !File.Exists(destFile) || this.CanOverwrite(destFile))
                {
                    string dir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        Log(FSAction.NewFolder, dir);

                        // File system changed => refresh will be required
                        this.RequiresRefresh = true;
                    }

                    _transferredAtomCount = 0;
                    FileRoutines.CopyFile(srcFile, destFile, Kernel32.CopyFileOptions.None, new Kernel32.CopyFileCallback(this.CopyFileCallback));
                    _task.FireTaskProgress(ProgressEventType.Progress, srcFile, UpdateProgressData.FileDone);
                    
                    Log(FSAction.Copy, srcFile, destFile);

                    // File system changed => refresh will be required
                    this.RequiresRefresh = true;
                }
                return true;
            }
            catch (Exception exception)
            {
                Win32Exception winEx = exception as Win32Exception;
                if (winEx == null || winEx.NativeErrorCode != WinError.S_OK)
                {
                    _task.AddToErrorMap(srcFile, exception.Message);
                }

                return false;
            }
        }

        public void MoveFile(string srcFile, string destFile, bool needConfirmation)
        {
            if (_skipConfirmations || !needConfirmation || this.CanMove(destFile))
            {
                if (PathUtils.PathsAreOnSameRoot(srcFile, destFile))
                {
                    bool fileExists = File.Exists(destFile);
                    if (_skipConfirmations || !needConfirmation || !fileExists || this.CanOverwrite(destFile))
                    {
                        string dir = Path.GetDirectoryName(destFile);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            Log(FSAction.NewFolder, dir);
                        }
                        if (fileExists)
                        {
                            File.SetAttributes(destFile, FileAttributes.Normal);
                            File.Delete(destFile);

                            if (this.CopyFile(srcFile, destFile))
                            {
                                this.DeleteFile(srcFile, false);
                            }
                        }
                        else
                            File.Move(srcFile, destFile);

                        Log(FSAction.Move, srcFile, destFile);

                        // File system changed => refresh will be required
                        this.RequiresRefresh = true;
                    }
                }
                else if (this.CopyFile(srcFile, destFile))
                {
                    this.DeleteFile(srcFile, false);
                }
            }
        }

        public void DeleteFile(string file, bool needConfirmation)
        {
            if (_skipConfirmations || !needConfirmation || CanDelete(file))
            {
                DeleteFileSystemObject(file);
            }
        }

        int _transferredAtomCount = 0;

        public Kernel32.CopyFileCallbackAction CopyFileCallback(
           string source, string destination, object state,
           long totalFileSize, long totalBytesTransferred)
        {
            int atomCount = (int)(totalBytesTransferred / FileRoutines.FILESIZE_NOTIFY_ATOM);
            if (atomCount != _transferredAtomCount)
            {
                _transferredAtomCount = atomCount;
                UpdateProgressData data = new UpdateProgressData(totalFileSize, totalBytesTransferred);
                _task.FireTaskProgress(ProgressEventType.Progress, source, data);
            }

            while (_fileTaskWaitEvent.WaitOne(10))
                Application.DoEvents();

            return CanContinue ? 
                Kernel32.CopyFileCallbackAction.Continue : 
                Kernel32.CopyFileCallbackAction.Cancel;
        }

        #endregion

        #region Folder operations

        public void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                if (PathUtils.IsEmptyFolder(path))
                {
                    // empty folder
                    if (_skipConfirmations || CanDelete(path))
                    {
                        //Directory.Delete(path);

                        DeleteFileSystemObject(path);
                        this.RequiresRefresh = true;
                    }
                }
                else if (_skipConfirmations || CanDeleteNonEmptyFolder(path))
                {
                    DeleteFileSystemObject(path);
                    this.RequiresRefresh = true;
                }
            }
        }

        internal void MoveTo(string dir, string destinationPath)
        {
            Directory.Move(dir, destinationPath);
            this.RequiresRefresh = true;
            Log(FSAction.MoveFolder, dir, destinationPath);

        }

        #endregion

        #region Generic file system operations

        public void DeleteFileSystemObject(string fsi)
        {
            try
            {
                if (File.Exists(fsi))
                {
                    _task.FireTaskProgress(ProgressEventType.Progress, fsi, UpdateProgressData.Empty);

                    File.SetAttributes(fsi, FileAttributes.Normal);
                    File.Delete(fsi);
                    Log(FSAction.Delete, fsi);

                    _task.FireTaskProgress(ProgressEventType.Progress, fsi, UpdateProgressData.FileDone);
                }
                else if (Directory.Exists(fsi))
                {
                    PathUtils.DeleteFolderTree(fsi,
                        (delFSI, isFolder) =>
                        {
                            _task.FireTaskProgress(ProgressEventType.Progress, delFSI, UpdateProgressData.Empty);

                            if (isFolder)
                                Log(FSAction.DeleteFolder, delFSI);
                            else
                                Log(FSAction.Delete, delFSI);

                            _task.FireTaskProgress(ProgressEventType.Progress, delFSI, UpdateProgressData.FileDone);
                        });
                }


                // File system changed => refresh will be required
                this.RequiresRefresh = true;
            }
            catch (Exception ex)
            {
                _task.AddToErrorMap(fsi, ex.Message);
            }
        }

        #endregion

        public virtual List<string> GetChildFiles(string path, FileTaskType taskType)
        {
            return null;
        }
        public virtual string GetParentFile(string path, FileTaskType taskType)
        {
            return null;
        }

        #endregion

        #region Logging

        private void Log(FSAction fsla, object obj1, object obj2 = null)
        {
            if (obj2 == null)
                Logger.LogTrace($"FS: {fsla}: {obj1}");
            else
                Logger.LogTrace($"FS: {fsla}: {obj1} -> {obj2}");
        }

        #endregion

        public enum FSAction
        {
            Copy = 0,
            Move,
            Delete,

            NewFolder,
            MoveFolder,
            DeleteFolder
        }
    }
}
