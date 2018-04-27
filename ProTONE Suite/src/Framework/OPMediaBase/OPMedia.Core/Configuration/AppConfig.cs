using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OPMedia.Core;
using System.IO;
using System.Reflection;
using OPMedia.Core.Logging;
using System.ComponentModel;
using OPMedia.Core.TranslationSupport;
using OPMedia.Core.GlobalEvents;
using OPMedia.Core.Utilities;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.Security.Principal;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OPMedia.Core.Win32;

namespace OPMedia.Core.Configuration
{
    public enum ProxyType
    {
        NotDefined = -1,
        NoProxy = 0,
        HttpProxy,
        Socks4Proxy,
        Socks5Proxy,
        InternetExplorerProxy,
        ApplicationProxy,

        GlobalProxy,
    }

    public class ProxySettings
    {
        public static ProxySettings Empty = new ProxySettings();
        public ProxyType ProxyType = ProxyType.NoProxy;
        public int ProxyPort = 8080;

        public string ProxyAddress { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }

        private ProxySettings()
        {
            EventDispatch.RegisterHandler(this);
            InitDefaults();
        }

        [EventSink(EventNames.PerformTranslation)]
        public void InitDefaults()
        {
            ProxyAddress = Translator.Translate("TXT_DEFINE_PROXYSERVERADDRESS");
            ProxyUser = Translator.Translate("TXT_DEFINE_PROXYSERVERUSERNAME");
            ProxyPassword = string.Empty;
        }

        ~ProxySettings()
        {
            EventDispatch.UnregisterHandler(this);
        }
    }

    public static class AppConfig
    {
        public const int VerWin2000 =  50;
        public const int VerWinXP =    51;
        
        public const int VerWinVista = 60;
        public const int VerWin7 =     61;
        
        public const int VerWin8 =     62;
        public const int VerWin8_1 =   63;
        public const int VerWin10 =   100;

        static Dictionary<string, CultureInfo> _cultures = new Dictionary<string, CultureInfo>();

        static string _skinType = string.Empty;
        static string _languageId = string.Empty;

        const string DefaultDownloadUriBase = "http://ocpa.ro/protone/current";
        const string DefaultHelpUriBase = "http://ocpa.ro/protone/protone-suite-docs/#VERSION#/";

        static AppConfig()
        {
            if (string.Compare(Constants.PersistenceServiceShortName,
                ApplicationInfo.ApplicationName, true) != 0)
            {
                _cultures.Add("en", new CultureInfo("en"));
                _cultures.Add("de", new CultureInfo("de"));
                _cultures.Add("fr", new CultureInfo("fr"));
                _cultures.Add("ro", new CultureInfo("ro"));

                _skinType = PersistenceProxy.ReadObject("SkinType", "Black");

                string defLangId = Regedit.InstallLanguageID;
                _languageId = PersistenceProxy.ReadObject("LanguageID", defLangId);
            }
        }

        public static void OnSettingsChanged(ChangeType changeType, string persistenceId, string persistenceContext, string objectContent)
        {
            if (changeType != ChangeType.Saved)
                return;

            if (DetectSettingsChanges)
            {
                lock (_settingsChangesLock)
                {
                    try
                    {
                        switch(persistenceId)
                        {
                            case "LanguageID":
                                {
                                    string langId = objectContent;
                                    if (langId != _languageId)
                                    {
                                        _languageId = langId;
                                        MainThread.Post((c) => Translator.SetInterfaceLanguage(_languageId));
                                    }
                                }
                                break;

                            case "SkinType":
                                {
                                    string skinType = objectContent;
                                    if (skinType != _skinType)
                                    {
                                        _skinType = skinType;
                                        MainThread.Post((c) => EventDispatch.DispatchEvent(EventNames.ThemeUpdated));
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
        }

        #region Generic Purpose API (Level 0 settings)

        private static bool _allowGUISetup = true;
        private static object _allowGUISetupLock = new object();
        public static bool AllowRealtimeGUISetup
        {
            get
            {
                lock (_allowGUISetupLock)
                {
                    return _allowGUISetup;
                }
            }
            set
            {
                lock (_allowGUISetupLock)
                {
                    _allowGUISetup = value;
                    DetectSettingsChanges = value;
                }
            }
        }

        private static bool _detectSettingsChanges = true;
        private static object _settingsChangesLock = new object();
        public static bool DetectSettingsChanges
        { 
            get 
            {
                lock (_settingsChangesLock)
                {
                    return _detectSettingsChanges;
                }
            } 
            set 
            {
                lock (_settingsChangesLock)
                {
                    _detectSettingsChanges = value;
                }
            } 
        }

        public static uint OSVersion
        {
            get
            {
                uint winVer = 0;
                
                winVer += (uint)Environment.OSVersion.Version.Major * 10;
                winVer += (uint)Environment.OSVersion.Version.Minor;

                return winVer;
            }
        }

        public static bool CurrentUserIsAdministrator
        {
            get
            {
                try
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                        return true;

                    if (OSVersion < VerWinVista)
                        // Operating system does not support UAC; skipping elevation check.
                        return false;

                    int tokenInfLength = Marshal.SizeOf(typeof(int));
                    IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);

                    try
                    {
                        var token = identity.Token;
                        var result = Advapi32.GetTokenInformation(token,
                            Advapi32.TokenInformationClass.TokenElevationType, tokenInformation, tokenInfLength, out tokenInfLength);

                        if (!result)
                        {
                            var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                            throw new InvalidOperationException("Couldn't get token information", exception);
                        }

                        var elevationType = (Advapi32.TokenElevationType)Marshal.ReadInt32(tokenInformation);

                        switch (elevationType)
                        {
                            case Advapi32.TokenElevationType.TokenElevationTypeDefault:
                                // TokenElevationTypeDefault - User is not using a split token, so they cannot elevate.
                                return false;

                            case Advapi32.TokenElevationType.TokenElevationTypeFull:
                                // TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
                                return true;

                            case Advapi32.TokenElevationType.TokenElevationTypeLimited:
                                // TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
                                return true;

                            default:
                                // Unknown token elevation type.
                                return false;
                        }
                    }
                    finally
                    {
                        if (tokenInformation != IntPtr.Zero) 
                            Marshal.FreeHGlobal(tokenInformation);
                    }
                }
                catch
                {
                }

                return false;
            }
        }

        public static string InstallationPath
        {
            get
            {
                string retVal = string.Empty;
                try
                {
                    if (ApplicationInfo.IsSuiteApplication)
                        retVal = Regedit.InstallPathOverride;

                    if (string.IsNullOrEmpty(retVal))
                    {
                        Assembly asm = Assembly.GetAssembly(typeof(AppConfig));
                        if (asm != null)
                        {
                            retVal = Path.GetDirectoryName(asm.Location);
                        }
                    }
                }
                catch
                {
                    retVal = string.Empty;
                }

                return retVal;
            }
        }

        public static CultureInfo[] SupportedCultures
        {
            get
            {
                CultureInfo[] retVal = new CultureInfo[_cultures.Count];
                _cultures.Values.CopyTo(retVal, 0);
                return retVal;
            }
        }

        public static string SkinType
        {
            get
            {
                return _skinType;
            }

            set
            {
                if (value != _skinType)
                {
                    _skinType = value;
                    PersistenceProxy.SaveObject("SkinType", value);
                    EventDispatch.DispatchEvent(EventNames.ThemeUpdated);
                }
            }
        }

        public static string LanguageID
        {
            get
            {
                return _languageId;
            }

            set
            {
                if (value != _languageId)
                {
                    _languageId = value;
                    PersistenceProxy.SaveObject("LanguageID", value);
                    Translator.SetInterfaceLanguage(_languageId);
                }
            }
        }

        public static string HelpUriBase
        {
            get
            {
                try
                {
                    if (UseOnlineDocumentation)
                    {
                        string val = DefaultHelpUriBase;
                        if (!string.IsNullOrEmpty(val))
                        {
                            Version ver = new Version(SuiteVersion.Version);
                            val = val.Replace("#VERSION#", $"{ver.Major}.{ver.Minor}");
                            return val;
                        }
                    }
                }
                catch { }

                return $"file:///{AppConfig.InstallationPath}/docs";
            }
        }

        public static bool UseOnlineDocumentation
        {
            get
            {
                return PersistenceProxy.ReadObject("UseOnlineDocumentation", true);
            }
        }

        public static string DownloadUriBase
        {
            get
            {
                return DefaultDownloadUriBase;
            }
        }

        public static bool AllowAutomaticUpdates
        {
            get
            {
                return PersistenceProxy.ReadObject("AllowAutomaticUpdates", false);
            }

            set
            {
                PersistenceProxy.SaveObject("AllowAutomaticUpdates", value);
            }
        }

        #endregion

        #region Level 1 settings using Settings File (Combined per-app and per-user settings)

        #region Network preferences
       

        public static IWebProxy GetWebProxy()
        {
            IWebProxy wp = null;
            ProxySettings ps = ProxySettings;

            if (ps == null || ps.ProxyType == ProxyType.NoProxy)
            {
                wp = new WebProxy();
            }
            else if (ps.ProxyType != ProxyType.InternetExplorerProxy)
            {
                wp = new WebProxy(ps.ProxyAddress, ps.ProxyPort);
                wp.Credentials = new NetworkCredential(ps.ProxyUser, ps.ProxyPassword);
                (wp as WebProxy).BypassProxyOnLocal = true;
            }

            return wp;
        }
                
        public static ProxySettings ProxySettings
        {
            get
            {
                ProxySettings ps = ProxySettings.Empty;
                ps.ProxyAddress = ProxyAddress;
                ps.ProxyPassword = ProxyPassword;
                ps.ProxyPort = ProxyPort;
                ps.ProxyType = ProxyType;
                ps.ProxyUser = ProxyUser;

                return ps;
            }

            set
            {
                ProxyAddress = value.ProxyAddress;
                ProxyPassword = value.ProxyPassword;
                ProxyPort = value.ProxyPort;
                ProxyType = value.ProxyType;
                ProxyUser = value.ProxyUser;
            }
        }

        public static ProxyType ProxyType
        {
            get
            {
                return (ProxyType)PersistenceProxy.ReadObject("ProxyType", (int)ProxyType.NoProxy);
            }
            set
            {
                PersistenceProxy.SaveObject("ProxyType", (int)value);
            }
        }

        public static string ProxyAddress
        {
            get
            {
                return PersistenceProxy.ReadObject("ProxyAddress", "your.proxy.address");
            }
            set
            {
                PersistenceProxy.SaveObject("ProxyAddress", value);
            }
        }

        public static int ProxyPort
        {
            get
            {
                return PersistenceProxy.ReadObject("ProxyPort", 8080);
            }
            set
            {
                PersistenceProxy.SaveObject("ProxyPort", value);
            }
        }

        public static string ProxyUser
        {
            get
            {
                return PersistenceProxy.ReadObject("ProxyUser", "user.name");
            }
            set
            {
                PersistenceProxy.SaveObject("ProxyUser", value);
            }
        }

        public static string ProxyPassword
        {
            get
            {
                return PersistenceProxy.ReadObject("ProxyPassword", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject("ProxyPassword", value);
            }
        }
        #endregion

        #region User interface persistence

        public static Point WindowLocation
        {
            get
            {
                Point point = new Point(100, 100);
                if (ApplicationInfo.IsSuiteApplication)
                {
                    point = new Point(Screen.PrimaryScreen.Bounds.Width / 6, Screen.PrimaryScreen.Bounds.Height / 6);
                }

                try
                {
                    string str = PersistenceProxy.ReadObject(true, "WindowLocation", string.Empty);
                    if (!string.IsNullOrEmpty(str))
                    {
                        point = (Point)new PointConverter().ConvertFromInvariantString(str);
                    }
                }
                catch
                {
                }

                return point;
            }
            set
            {
                if ((value.X >= 0) && (value.Y >= 0))
                {
                    PersistenceProxy.SaveObject(true, "WindowLocation", new PointConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static Size WindowSize
        {
            get
            {
                Size size = new Size(800, 600);
                if (ApplicationInfo.IsSuiteApplication)
                {
                    size = new Size(2 * Screen.PrimaryScreen.Bounds.Width / 3, 2 * Screen.PrimaryScreen.Bounds.Height / 3);
                }
                    
                try
                {
                    string str = PersistenceProxy.ReadObject(true, "WindowSize", string.Empty);
                    if (!string.IsNullOrEmpty(str))
                    {
                        size = (Size)new SizeConverter().ConvertFromInvariantString(str);
                    }
                }
                catch
                {
                }
                return size;
            }
            set
            {
                if ((value.Width >= 0) && (value.Height >= 0))
                {
                    PersistenceProxy.SaveObject(true, "WindowSize", new SizeConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static FormWindowState WindowState
        {
            get
            {
                try
                {
                    return (FormWindowState)PersistenceProxy.ReadObject(true, "WindowState", (int)FormWindowState.Normal);
                }
                catch
                {
                }

                return FormWindowState.Normal;
            }
            set
            {
                PersistenceProxy.SaveObject(true, "WindowState", (int)value);
            }
        }

        public static bool MimimizedToTray
        {
            get
            {
                return (PersistenceProxy.ReadObject(true, "MimimizedToTray", false) && CanSendToTray);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "MimimizedToTray", value && CanSendToTray);
            }
        }

        public static bool CanSendToTray
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "CanSendToTray", false);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "CanSendToTray", value);
            }
        }
        
        #endregion

        #region Application state persistence
        public static string LastExploredFolder
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastExploredFolder", PathUtils.CurrentDir);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastExploredFolder", value);
            }
        }
        #endregion

        #endregion

    }
}
