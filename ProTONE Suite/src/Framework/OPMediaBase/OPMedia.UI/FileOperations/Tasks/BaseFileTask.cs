using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OPMedia.Core;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using OPMedia.Core.TranslationSupport;
using OPMedia.Core.Logging;
using OPMedia.UI.FileOperations.Tasks;

namespace OPMedia.UI.FileTasks
{
    #region Enums

    public enum FileTaskType
    {
        None = 0,
        Copy,
        Move,
        Delete
    }

    public enum ProgressEventType
    {
        Started = 0,
        KeepAlive,
        Progress,
        Aborted,
        Finished,
    }

    #endregion

    #region Delegates

    public delegate void FileTaskProgressDG(ProgressEventType eventType, string file, UpdateProgressData data);

    #endregion

    #region Helper classes

    public class TaskInterruptedException : Exception
    {
        public TaskInterruptedException(string msg)
            : base(msg)
        {
        }
    }

    public class UpdateProgressData
    {
        public static UpdateProgressData Empty = new UpdateProgressData();
        public static UpdateProgressData FileDone = new UpdateProgressData(100, 100);

        public long TotalFileSize { get; private set; }
        public long TotalBytesTransferred { get; private set; }

        private UpdateProgressData()
        {
            this.TotalFileSize = 0;
            this.TotalBytesTransferred = 0;
        }

        public UpdateProgressData(long totalFileSize, long totalBytesTransferred)
        {
            this.TotalFileSize = totalFileSize;
            this.TotalBytesTransferred = totalBytesTransferred;
        }

        public override string ToString()
        {
            return $"t/T: {TotalBytesTransferred}/{TotalFileSize}";
        }
    }

    #endregion

    public abstract class BaseFileTask
    {
        public event FileTaskProgressDG FileTaskProgress = null;

        public FileTaskType TaskType { get; private set; }

        public string UpperCaseTaskType
        {
            get
            {
                return this.TaskType.ToString().ToUpperInvariant();
            }
        }

        public Dictionary<string, string> ErrorMap { get; private set; }

        public bool IsFinished { get; private set; }

        public List<string> SrcFiles { get; private set; }
        public string SrcFolder { get; private set; }
        public string DestFolder { get; private set; }
        public int ObjectsCount { get; private set; }

        public long ProcessedObjects { get; private set; }

        protected Thread _fileTaskThread;
        protected FileTaskSupport _support = null;

        protected System.Timers.Timer _timer = null;

        private bool _requiresRefresh = false;
        public bool RequiresRefresh
        {
            get
            {
                return (_support == null) ? _requiresRefresh : _support.RequiresRefresh;
            }
        }

        public bool CanContinue
        {
            get
            {
                return (_support == null) ? false : _support.CanContinue;
            }
        }

        public BaseFileTask(FileTaskType type, List<string> srcFiles, string srcFolder)
        {
            this.TaskType = type;
            this.ErrorMap = new Dictionary<string, string>();

            this.SrcFiles = srcFiles;
            this.SrcFolder = srcFolder;

            _timer = new System.Timers.Timer(500);
            _timer.AutoReset = true;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

            _support = InitSupport();
            this.IsFinished = false;
        }

        protected virtual FileTaskSupport InitSupport()
        {
            return new FileTaskSupport(this);
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPath))
            {
                FireTaskProgress(ProgressEventType.KeepAlive, _currentPath, UpdateProgressData.Empty);
            }
        }

        public void FireTaskProgress(ProgressEventType eventType, string file, UpdateProgressData data)
        {
            if (eventType == ProgressEventType.Progress &&
                data == UpdateProgressData.FileDone)
            {
                ProcessedObjects++;
            }

            if (FileTaskProgress != null)
            {
                FileTaskProgress(eventType, file, data);
            }
        }

        public void AddToErrorMap(string path, string error)
        {
            if (ErrorMap.ContainsKey(path))
            {
                ErrorMap[path] = error;
            }
            else
            {
                ErrorMap.Add(path, error);
            }
        }

        protected string GetDestinationPath(string srcPath)
        {
            if (TaskType == FileTaskType.Copy || TaskType == FileTaskType.Move)
            {
                string diffPath = srcPath.Replace(SrcFolder, string.Empty);
                return Path.Combine(DestFolder, diffPath.TrimStart(PathUtils.DirectorySeparatorChars));
            }

            return string.Empty;
        }


        public void RunTask(string destFolder)
        {
            this.DestFolder = destFolder ?? string.Empty;

            _fileTaskThread = new Thread(new ThreadStart(RunTaskAsync));
            _fileTaskThread.Priority = ThreadPriority.Normal;
            _fileTaskThread.Start();
        }

        public void RequestAbort()
        {
            _support.RequestAbort();
        }

        string _currentPath = string.Empty;
        private void RunTaskAsync()
        {
            try
            {
                ProcessedObjects = 0;
                ObjectsCount = 0;
                ErrorMap.Clear();

                List<string> allLinkedFiles = new List<string>();
                foreach (string path in SrcFiles)
                {
                    if (File.Exists(path))
                    {
                        List<String> linkedFiles = _support.GetChildFiles(path, TaskType);
                        if (linkedFiles != null && linkedFiles.Count > 0)
                        {
                            var linkedFilesLowercase = (from s in linkedFiles
                                                        select s.ToLowerInvariant()).ToList();

                            allLinkedFiles.AddRange(linkedFilesLowercase);
                        }
                    }
                }

                List<string> srcFilesClone = new List<string>(SrcFiles);

                foreach (string srcFile in srcFilesClone)
                {
                    if (allLinkedFiles.Contains(srcFile.ToLowerInvariant()))
                        SrcFiles.Remove(srcFile);
                }

                foreach (string path in SrcFiles)
                {
                    if (Directory.Exists(path))
                    {
                        List<string> entries = PathUtils.EnumFileSystemEntries(path, "*", SearchOption.AllDirectories);
                        if (entries != null && entries.Count > 0)
                        {
                            ObjectsCount += entries.Count;
                        }
                    }
                    else
                    {
                        ObjectsCount++;
                    }
                }

                ObjectsCount += allLinkedFiles.Count;

                FireTaskProgress(ProgressEventType.Started, string.Empty, UpdateProgressData.Empty);

                _currentPath = string.Empty;
                _timer.Start();

                try
                {
                    foreach (string path in SrcFiles)
                    {
                        _currentPath = path;
                        FireTaskProgress(ProgressEventType.Progress, path, UpdateProgressData.Empty);

                        switch (TaskType)
                        {
                            case FileTaskType.Copy:
                                CopyObject(path);
                                break;

                            case FileTaskType.Move:
                                MoveObject(path);
                                break;

                            case FileTaskType.Delete:
                                DeleteObject(path);
                                break;
                        }
                    }
                }
                catch (TaskInterruptedException)
                {
                    FireTaskProgress(ProgressEventType.Aborted, string.Empty, UpdateProgressData.Empty);
                    return;
                }
                finally
                {
                    _timer.Stop();
                    _currentPath = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                IsFinished = true;
            }

            FireTaskProgress(ProgressEventType.Finished, string.Empty, UpdateProgressData.Empty);
            _requiresRefresh = _support.RequiresRefresh;
            _support = null;
        }

        #region Overridables

        protected virtual bool CopyObject(string path) { return true; }
        protected virtual bool MoveObject(string path) { return true; }
        protected virtual bool DeleteObject(string path) { return true; }
        
        #endregion
    }
}
