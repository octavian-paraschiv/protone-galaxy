using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using OPMedia.Core.Logging;
using OPMedia.Core.Utilities;
using System.Runtime.InteropServices;

namespace OPMedia.Core
{
    public static class ApplicationInfo
    {
        public static string _appName = null;

        public static DateTime? BuildDateTime { get; private set; }

        static ApplicationInfo()
        {
            try
            {
                ReadBuildDateFromCoffHeader();
            }
            catch
            {
                BuildDateTime = null;
            }
        }

        private static void ReadBuildDateFromCoffHeader()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

            if (!File.Exists(path))
                return;

            var headerDefinition = typeof(IMAGE_FILE_HEADER);

            var buffer = new byte[Math.Max(Marshal.SizeOf(headerDefinition), 4)];
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fileStream.Position = 0x3C;
                fileStream.Read(buffer, 0, 4);
                fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                fileStream.Read(buffer, 0, 4); // "PE\0\0"
                fileStream.Read(buffer, 0, buffer.Length);
            }
            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                var addr = pinnedBuffer.AddrOfPinnedObject();
                var coffHeader = (IMAGE_FILE_HEADER)Marshal.PtrToStructure(addr, headerDefinition);

                var epoch = new DateTime(1970, 1, 1);
                var sinceEpoch = new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond);
                var buildDate = epoch + sinceEpoch;

                BuildDateTime = buildDate;
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }

        public static void RegisterAppName(Assembly asm)
        {
            if (IsSuiteApplication == false)
            {
                try
                {
                    _appName = asm.GetName().Name;
                }
                catch
                {
                    _appName = null;
                }
            }
        }

        public static string ApplicationLaunchPath
        {
            get
            {
                return Application.ExecutablePath;
            }
        }

        public static string ApplicationName
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_appName))
                    {
                        return Assembly.GetEntryAssembly().GetName().Name;
                    }
                }
                catch 
                {
                    _appName = "Unknown";
                }

                return _appName;
            }
        }

        public static string ApplicationBinary
        {
            get
            {
                return Path.GetFileName(ApplicationLaunchPath);
            }
        }


        public static string AltLogsFolder
        {
            get
            {
                try
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        ApplicationName);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (Directory.Exists(path))
                    {
                        return path;
                    }
                }
                catch
                {
                }

                return PathUtils.CurrentDir;
            }
        }

        public static bool IsSuiteApplication
        {
            get
            {
                return ApplicationName.StartsWith(Constants.SuiteAppPrefix);
            }
        }
    }
}
