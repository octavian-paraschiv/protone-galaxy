using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OPMedia.Core;

namespace OPMedia.UI.FileTasks
{
    public class CopyFilesTask : BaseFileTask
    {
        public CopyFilesTask(List<string> srcFiles, string srcFolder) :
            base(FileTaskType.Copy, srcFiles, srcFolder)
        {
        }

        protected override bool CopyObject(string path)
        {
            _support.CheckIfCanContinue(path);

            try
            {
                string destinationPath = this.GetDestinationPath(path);

                if (Directory.Exists(path))
                {
                    List<string> subObjects = PathUtils.EnumFileSystemEntries(path);
                    if (subObjects != null && subObjects.Count > 0)
                    {
                        foreach (string subObj in subObjects)
                        {
                            CopyObject(subObj);
                        }
                    }
                }
                else
                {
                    CopyConnectedFiles(path, destinationPath); 
                    _support.CopyFile(path, destinationPath);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected virtual void CopyConnectedFiles(string srcFile, string destPath) {}
    }
}
