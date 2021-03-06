using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using OPMedia.Core.Logging;
using OPMedia.Core.TranslationSupport;
using System.Drawing.Design;
using OPMedia.Core;
using OPMedia.Core.Utilities;

namespace OPMedia.Runtime.FileInformation
{
    public class NativeFileInfo
    {
        public static readonly NativeFileInfo Empty = new NativeFileInfo();

        FileSystemInfo fsi = null;

        [Browsable(false)]
        public FileSystemInfo FileSystemInfo
        {get { return fsi; } }

        [Browsable(false)]
        public bool IsValid
        { get { return fsi != null; } }

        [TranslatableDisplayName("TXT_PARENT")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [Browsable(true)]
        public string Parent
        {
            get
            {
                if (IsValid)
                {
                    return System.IO.Path.GetDirectoryName(fsi.FullName);
                }

                return null;
            }
        }

        [TranslatableDisplayName("TXT_PATH")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public virtual string Path
        { 
            get 
            {
                if (IsValid)
                {
                    if (fsi != null)
                        return fsi.FullName;

                }

                return null; 
            } 
        }

        [TranslatableDisplayName("TXT_NAME")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public virtual string Name
        {
            get
            {
                if (IsValid)
                {
                    if (fsi != null)
                        return fsi.Name;
                }

                return null;
            } 
        }

        [TranslatableDisplayName("TXT_EXTENSION")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public virtual string Extension
        { get { return IsValid ? fsi.Extension : null; } }

        [TranslatableDisplayName("TXT_ATTRIBUTES")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [Editor("OPMedia.UI.Controls.FileAttributesBrowser, OPMedia.UI", typeof(UITypeEditor))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [SingleSelectionBrowsable]
        public FileAttributes? Attributes
        { 
            get 
            {
                if (IsValid) 
                    return fsi.Attributes; 
                else 
                    return null; 
            } 

            set 
            {
                if (IsValid)
                {
                    if (value.HasValue)
                        fsi.Attributes = value.Value;
                    else
                        fsi.Attributes ^= fsi.Attributes;

                    object arg = new string[] { this.Path };
                    EventDispatch.DispatchEvent(EventNames.RefreshItems, arg);
                }
            } 
        }
        
        [TranslatableDisplayName("TXT_FILESIZE")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public long? Size
        { get { if (IsValid) return (fsi as FileInfo).Length; else return null; } }

        [TranslatableDisplayName("TXT_CREATIONDATE")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public string DateCreated
        { get { if (IsValid) return StringUtils.BuildTimeString(fsi.CreationTime.ToLocalTime()); else return null; } }

        [TranslatableDisplayName("TXT_LASTACCESSDATE")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public string DateAccesed
        { get { if (IsValid) return StringUtils.BuildTimeString(fsi.LastAccessTime.ToLocalTime()); else return null; } }

        [TranslatableDisplayName("TXT_LASTCHANGEDATE")]
        [TranslatableCategory("TXT_FILESYSTEMINFO")]
        [SingleSelectionBrowsable]
        public string DateModified
        { get { if (IsValid) return StringUtils.BuildTimeString(fsi.LastWriteTime.ToLocalTime()); else return null; } }

        [Browsable(false)]
        public virtual Dictionary<string, string> ExtendedInfo
        {
            get { return null; }
            set { }
        }

        [Browsable(false)]
        public string Details
        {
            get
            {
                return GetDetailsInner();
            }
        }

        protected virtual string GetDetailsInner()
        {
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj is NativeFileInfo)
            {
                return string.Equals((obj as NativeFileInfo).Path, this.Path);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (string.IsNullOrEmpty(Path) ? 0 : Path.GetHashCode());
        }

        public NativeFileInfo()
        {
        }

        public NativeFileInfo(string path, bool throwExceptionOnInvalid)
        {
            LoadFromPath(path, throwExceptionOnInvalid);
        }

        protected void LoadFromPath(string path, bool throwExceptionOnInvalid)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists)
                {
                    fsi = di;
                    return;
                }


                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    fsi = fi;
                    return;
                }

                throw new FileNotFoundException("File not found: " + path);
            }
            catch
            {
                fsi = null;

                if (throwExceptionOnInvalid)
                {
                    throw;
                }
            }
        }
    }

    public class SingleSelectionBrowsableAttribute : Attribute
    {
    }
}
