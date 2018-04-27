using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace OPMedia.Core.Win32
{
    public static class Regedit
    {
        public static string InstallPathOverride
        {
            get
            {
                string retVal = null;

                try
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OPMedia Research\ProTONE Suite");
                    if (key != null)
                    {
                        retVal = key.GetValue("InstallPathOverride", null) as string;
                    }
                }
                catch
                {
                    retVal = null;
                }

                return retVal;
            }
        }

        public static string InstallLanguageID
        {
            get
            {
                string retVal = null;

                try
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OPMedia Research\ProTONE Suite");
                    if (key != null)
                    {
                        retVal = key.GetValue("InstallLanguageID", "en") as string;
                    }
                }
                catch
                {
                    retVal = null;
                }

                return retVal;
            }
        }
    }
}
