using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.UI.FileTasks;
using System.IO;

namespace OPMedia.Addons.Builtin.Navigation.FileExplorer.FileOperations.Tasks
{
    public class FEMoveFilesTask : MoveFilesTask
    {
        public FEMoveFilesTask(List<string> srcFiles, string srcFolder)
            : base(srcFiles, srcFolder)
        {
        }

        protected override UI.FileOperations.Tasks.FileTaskSupport InitSupport()
        {
            return new FEFileTaskSupport(this);
        }

        protected override void MoveConnectedFiles(string srcPath, string destPath)
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
                        _support.MoveFile(linkedFile, destinationPath, false);
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
                    _support.MoveFile(parentFile, destinationPath, true);
                }
                catch
                {
                }
            }
        }
    }
}
