using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.UI.FileOperations.Tasks;
using OPMedia.UI.FileTasks;
using System.IO;
using OPMedia.Core.Configuration;
using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.Runtime.ProTONE.ExtendedInfo;
using OPMedia.Core;
using OPMedia.Runtime.ProTONE.Configuration;

namespace OPMedia.Addons.Builtin.Navigation.FileExplorer.FileOperations.Tasks
{
    public class FEFileTaskSupport : FileTaskSupport
    {
        public FEFileTaskSupport(BaseFileTask task)
            : base(task)
        {
        }

        public override List<string> GetChildFiles(string file, FileTaskType taskType)
        {
            List<string> list = new List<string>();

            if (ProTONEConfig.UseLinkedFiles)
            {
                string fileType = PathUtils.GetExtension(file).ToUpperInvariant();
                string[] childFileTypes = ProTONEConfig.GetChildFileTypes(fileType);

                if (childFileTypes != null && childFileTypes.Length > 0)
                {
                    foreach (string childFileType in childFileTypes)
                    {
                        // This will find files like "FileName.PFT" and change them into "FileName.CFT"
                        string childFilePath = Path.ChangeExtension(file, childFileType);
                        if (File.Exists(childFilePath) && !list.Contains(childFilePath))
                        {
                            list.Add(childFilePath);
                        }

                        // This will find files like "FileName.PFT" and change them into "FileName.PFT.CFT"
                        // (i.e. handle double type extension case like for Bookmark files)
                        string childFileType2 = string.Format("{0}.{1}", PathUtils.GetExtension(file), childFileType);
                        string childFilePath2 = Path.ChangeExtension(file, childFileType2);
                        if (File.Exists(childFilePath2) && !list.Contains(childFilePath2))
                        {
                            list.Add(childFilePath2);
                        }
                    }
                }
            }

            return list;
        }

        public override string GetParentFile(string file, FileTaskType taskType)
        {
            if (ProTONEConfig.UseLinkedFiles)
            {
                string parentFilePath = "";

                if (Path.HasExtension(file))
                {
                    // Check whether the child file is a double extension file
                    // In this case the parent file should have same name but w/o the second extension part.
                    parentFilePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                    if (File.Exists(parentFilePath))
                        return parentFilePath;
                }

                string fileType = Path.GetExtension(file).ToUpperInvariant();
                string[] parentFileTypes = ProTONEConfig.GetParentFileTypes(fileType);

                if (parentFileTypes != null && parentFileTypes.Length > 0)
                {
                    foreach (string parentFileType in parentFileTypes)
                    {
                        parentFilePath = Path.ChangeExtension(file, parentFileType);
                        if (File.Exists(parentFilePath))
                        {
                            return parentFilePath;
                        }
                    }
                }
            }

            return null;
        }
    }
}
