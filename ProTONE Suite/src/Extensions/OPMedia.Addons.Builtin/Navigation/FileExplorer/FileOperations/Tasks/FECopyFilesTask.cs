using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.UI.FileTasks;
using OPMedia.UI.FileOperations.Tasks;
using System.IO;

namespace OPMedia.Addons.Builtin.Navigation.FileExplorer.FileOperations.Tasks
{
    public class FECopyFilesTask : CopyFilesTask
    {
        public FECopyFilesTask(List<string> srcFiles, string srcFolder)
            : base(srcFiles, srcFolder)
        {
        }

        protected override FileTaskSupport InitSupport()
        {
            return new FEFileTaskSupport(this);
        }

        protected override void CopyConnectedFiles(string srcPath, string destPath)
        {
            List<string> linkedFiles = _support.GetChildFiles(srcPath, this.TaskType);
            if (linkedFiles != null && linkedFiles.Count > 0)
            {
                foreach (string linkedFile in linkedFiles)
                {
                    _support.CheckIfCanContinue(linkedFile);

                    try
                    {
                        string destinationPath = this.GetDestinationPath(linkedFile);
                        _support.CopyFile(linkedFile, destinationPath);
                    }
                    catch
                    {
                    }
                }
            }

            string parentFile = _support.GetParentFile(srcPath, this.TaskType);
            if (!string.IsNullOrEmpty(parentFile))
            {
                _support.CheckIfCanContinue(parentFile);

                try
                {
                    string destinationPath = this.GetDestinationPath(parentFile);
                    _support.CopyFile(parentFile, destinationPath);
                }
                catch
                {
                }
            }
        }
    }
}
