using System;
using System.Collections.Generic;
using System.Text;
using OPMedia.UI.Wizards;
using System.IO;
//using OPMedia.Runtime.Playlists;
using System.Windows.Forms;
using OPMedia.Core;
using OPMedia.Runtime.ProTONE.Playlists;
using OPMedia.Core.TranslationSupport;
using OPMedia.Runtime.ProTONE.Rendering;
using OPMedia.Core.Logging;
using OPMedia.UI;
using System.Threading;
using OPMedia.Runtime.ProTONE;
using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.UI.Controls;
using OPMedia.Core.ComTypes;
using OPMedia.Addons.Builtin.Navigation.CatalogExplorer.DataLayer;
using OPMedia.Runtime.FileInformation;

namespace OPMedia.Addons.Builtin.CatalogExplorer.ImportWizard.Tasks
{
    public class Task : BackgroundTask
    {
        private bool finished = false;

        public string SourcePath = string.Empty;
        public string CatalogPath = string.Empty;
        public string EntryDescription = string.Empty;
        public string CatalogDescription = string.Empty;

        public long InsertionPointID = 0;

        int _steps = 0;
        int _currentStep = 0;

        #region BackgroundTask overrides

        public override int CurrentStep
        {
            get { return _currentStep; }
        }

        public override int TotalSteps
        {
            get
            {
                return _steps;
            }
        }

        public override bool IsFinished
        {
            get
            {
                return finished;
            }
        }

        public override StepDetail RunNextStep()
        {
            StepDetail detail = new StepDetail();
            detail.Description = string.Format("Importing from {0} ...", SourcePath);

            _abortScan.Reset();

            try
            {
                Catalog cat = new Catalog(CatalogPath);
                CatalogItem parent = cat.GetByItemID(InsertionPointID);

                ScanFolder(cat, this.SourcePath, parent);

                finished = true;

                bool isAborted = _abortScan.WaitOne(0);
                if (!isAborted)
                {
                    cat.CatalogDescription = CatalogDescription;
                    cat.Save(CatalogPath);
                }

                detail.Results = (isAborted) ? Translator.Translate("TXT_ABORTED") : Translator.Translate("TXT_SUCCESS");
                detail.IsSuccess = !isAborted;
            }
            catch(Exception ex)
            {
                detail.Results = ex.Message;
                detail.IsSuccess = false;
            }
            finally
            {
                finished = true;
            }

            return detail;
        }

        public override void Reset()
        {
            finished = false;

            _steps = 0;
            _currentStep = 0;

            try
            {
                _steps += PathUtils.EnumDirectories(_abortScan, SourcePath, "*", SearchOption.AllDirectories).Count;
                _steps += PathUtils.EnumFiles(_abortScan, SourcePath, "*", SearchOption.AllDirectories).Count;
            }
            catch(Exception ex)
            {
                ConfirmScanAbortOnException(ex);
            }
        }

        #endregion

        #region Scanning

        ManualResetEvent _abortScan = new ManualResetEvent(false);

        public void ScanFolder(Catalog cat, string dir, CatalogItem parent)
        {
            if (!CanContinue()) 
                return;

            ReportPseudoStepInit("TXT_SCANNING: "  + dir);

            try
            {
                DriveInfo di = new DriveInfo(Path.GetPathRoot(dir));

                CatalogItem ci = new CatalogItem(cat);

                if (di.DriveType == DriveType.Removable || di.DriveType == DriveType.CDRom)
                {
                    ci.OrigItemPath = dir.Replace(Path.GetPathRoot(dir), "$:/");
                    if (PathUtils.DirectorySeparator != "/")
                        ci.OrigItemPath = ci.OrigItemPath.Replace(PathUtils.DirectorySeparator, "/");
                }
                else
                {
                    ci.OrigItemPath = dir;
                }

                ci.IsRoot = (parent == null);
                ci.RootItemLabel = di.VolumeLabel;
                ci.RootSerialNumber = Kernel32.GetVolumeSerialNumber(di.RootDirectory.FullName);

                ci.Name = GetName(dir, di);
                
                if (parent != null)
                {
                    ci.ParentItemID = parent.ItemID;
                }
                                
                if (ci.IsRoot)
                {
                    // This is a disk
                    ci.ItemType = (byte)di.DriveType;
                    ci.Description = EntryDescription;
                }
                else
                {
                    ci.ItemType = cat.CatalogItemType_GetByTypeCode("FLD").TypeID; // is a folder
                }

                ci.Save();

                Application.DoEvents();

                List<string> subDirs = PathUtils.EnumDirectories(_abortScan, dir);
                foreach (string subDir in subDirs)
                    ScanFolder(cat, subDir, ci);

                List<string> files = PathUtils.EnumFiles(_abortScan, dir);
                foreach (string file in files)
                    ScanFile(cat, file, ci);

                RaiseTaskProgressEvent(null, _currentStep++);
            }
            catch(Exception ex)
            {
                ConfirmScanAbortOnException(ex);
            }
        }

        private string GetName(string dir, DriveInfo rootDrive)
        {
            string retVal = PathUtils.GetDirectoryTitle(dir);
            string root = Path.GetPathRoot(dir);

            string rootSpec = root.ToString().TrimEnd(PathUtils.DirectorySeparator.ToCharArray());
            string dirSpec = dir.TrimEnd(PathUtils.DirectorySeparator.ToCharArray());

            if (rootSpec == dirSpec)
            {
                retVal = rootDrive.VolumeLabel;
            }
            return retVal;
        }

        private void ScanFile(Catalog cat, string file, CatalogItem parent)
        {
            if (!CanContinue()) 
                return;

            ReportPseudoStepInit("TXT_SCANNING: " + file);

            try
            {
                CatalogItem ci = new CatalogItem(cat);
                ci.IsRoot = false;
                ci.ItemType = cat.CatalogItemType_GetByTypeCode("FIL").TypeID; // is a file
                ci.Name = Path.GetFileName(file);

                DriveInfo di = new DriveInfo(Path.GetPathRoot(file));
                if (di.DriveType == DriveType.Removable || di.DriveType == DriveType.CDRom)
                {
                    ci.OrigItemPath = file.Replace(Path.GetPathRoot(file), "$:/");
                    if (PathUtils.DirectorySeparator != "/")
                        ci.OrigItemPath = ci.OrigItemPath.Replace(PathUtils.DirectorySeparator, "/");
                }
                else
                {
                    ci.OrigItemPath = file;
                }

                ci.RootItemLabel = di.VolumeLabel;
                ci.RootSerialNumber = Kernel32.GetVolumeSerialNumber(di.RootDirectory.FullName);
                ci.ParentItemID = parent.ItemID;

                try
                {
                    NativeFileInfo nfi = NativeFileInfoFactory.FromPath(file);
                    if (nfi != null)
                    {
                        ci.Description = nfi.Details;
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                }

                ci.Save();

                RaiseTaskProgressEvent(null, _currentStep++);
            }
            catch (Exception ex)
            {
                ConfirmScanAbortOnException(ex);
            }

            Application.DoEvents();
        }
        #endregion

        private void ConfirmScanAbortOnException(Exception ex)
        {
            if (ex is ThreadAbortException)
            {
                _abortScan.Set();
            }
            else
            {
                MainThread.Post(delegate(object x)
                {
                    string message = Translator.Translate("TXT_SCAN_ERROR_MSG", SourcePath, ex.Message);

                    TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Paused);

                    if (MessageDisplay.Query(message, "TXT_SCAN_ERROR", MessageBoxIcon.Exclamation) != DialogResult.Yes)
                        _abortScan.Set();
                    else
                        _abortScan.Reset();

                    TaskbarThumbnailManager.Instance.SetProgressStatus(TaskbarProgressBarStatus.Normal);
                });
            }
        }

        private bool CanContinue()
        {
            do
            {
                if (_abortScan.WaitOne(0)) 
                    return false;

                Thread.Sleep(50);
            }
            while (IsExecutionPaused(50));

            return true;
        }

        private void ReportPseudoStepInit(string desc)
        {
            StepDetail detail = new StepDetail();
            detail.Description = desc;

            RaiseTaskStepInitEvent(detail);
        }
    }
}
