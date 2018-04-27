using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.UI.FileTasks;
using System.IO;

namespace OPMedia.Addons.Builtin.Navigation.FileExplorer.FileOperations.Tasks
{
    public class FEDeleteFilesTask : DeleteFilesTask
    {
        public FEDeleteFilesTask(List<string> srcFiles)
            : base(srcFiles)
        {
        }

        protected override UI.FileOperations.Tasks.FileTaskSupport InitSupport()
        {
            return new FEFileTaskSupport(this);
        }

        protected override void DeleteConnectedFiles(string path)
        {
            List<string> linkedFiles = _support.GetChildFiles(path, this.TaskType);
            if (linkedFiles != null && linkedFiles.Count > 0)
            {
                foreach (string linkedFile in linkedFiles)
                {
                    _support.CheckIfCanContinue(linkedFile);

                    try
                    {
                        _support.DeleteFile(linkedFile, false);
                    }
                    catch
                    {
                    }
                }
            }

            string parentFile = _support.GetParentFile(path, this.TaskType);
            if (!string.IsNullOrEmpty(parentFile))
            {
                _support.CheckIfCanContinue(parentFile);

                try
                {
                    _support.DeleteFile(parentFile, true);
                }
                catch
                {
                }
            }
        }
    }
}
