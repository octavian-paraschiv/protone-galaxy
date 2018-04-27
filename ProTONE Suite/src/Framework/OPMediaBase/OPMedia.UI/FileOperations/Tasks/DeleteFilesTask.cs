using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OPMedia.Core;

namespace OPMedia.UI.FileTasks
{
    public class DeleteFilesTask : BaseFileTask
    {
        public DeleteFilesTask(List<string> srcFiles) :
            base(FileTaskType.Delete, srcFiles, string.Empty)
        {
        }

        protected override bool DeleteObject(string path)
        {
            _support.CheckIfCanContinue(path);

            try
            {
                if (Directory.Exists(path))
                {
                    _support.DeleteFolder(path);
                }
                else if (File.Exists(path))
                {
                    DeleteConnectedFiles(path);
                    _support.DeleteFile(path, true);
                }
            }
            catch { }

            return true;
        }

        protected virtual void DeleteConnectedFiles(string file) { }
    }
}
