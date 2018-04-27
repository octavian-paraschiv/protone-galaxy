using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.IO;
using OPMedia.Core;
using System.Security.Permissions;
using System.ComponentModel;

namespace OPMedia.Runtime.FileInformation
{
    public sealed class FileRoutines
    {
        public const long FILESIZE_NOTIFY_ATOM = 10 * 1024 * 1024;
        public const long FILESIZE_TRANSFER_ATOM = 1 * 1024 * 1024;

        public static void CopyFile(string source, string destination,
            Kernel32.CopyFileOptions options, Kernel32.CopyFileCallback callback)
        {
            CopyFile(source, destination, options, callback, null);
        }

        public static void CopyFile(string source, string destination,
            Kernel32.CopyFileOptions options, Kernel32.CopyFileCallback callback, object state)
        {
            if (source == null) 
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if ((options & ~Kernel32.CopyFileOptions.All) != 0)
                throw new ArgumentOutOfRangeException("options");

            //new FileIOPermission(FileIOPermissionAccess.Read, source).Demand();
            //new FileIOPermission(FileIOPermissionAccess.Write, destination).Demand();

            if (new FileInfo(source).Length <= FILESIZE_TRANSFER_ATOM)
            {
                File.Copy(source, destination, true);
            }
            else
            {
                Kernel32.CopyProgressRoutine cpr = callback == null ?
                    null : new Kernel32.CopyProgressRoutine(new Kernel32.CopyProgressData(
                        source, destination, callback, state).CallbackHandler);

                bool cancel = false;
                if (!Kernel32.CopyFileEx(source, destination, cpr, IntPtr.Zero, ref cancel, (int)options))
                {
                    int err = Kernel32.GetLastError();
                    if (err != WinError.S_OK)
                        throw new Win32Exception(err);
                }
            }
        }
    }
}
