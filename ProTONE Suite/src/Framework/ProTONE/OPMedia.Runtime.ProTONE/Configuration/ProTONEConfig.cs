using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Configuration;
using System.ServiceProcess;
using OPMedia.Core;
using System.IO;
using OPMedia.Core.Utilities;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using OPMedia.Runtime.ProTONE.Rendering.DS;

namespace OPMedia.Runtime.ProTONE.Configuration
{
    public enum CddaInfoSource
    {
        // Don't read audio cd info
        None = 0,
        // Read CD-text only
        CdText,
        // Read CDDB only
        Cddb,
        // Try CD-text first then CDDB [Default]
        CdText_Cddb,
        // Try CDDB first then CD-text
        Cddb_CdText
    }

    [Flags]
    public enum MediaScreen
    {
        None = 0x00,

        Playlist = 0x01,
        TrackInfo = 0x02,
        SignalAnalisys = 0x04,
        BookmarkInfo = 0x08,

        All = 0xFF
    }

    [Flags]
    public enum SignalAnalisysFunction
    {
        None = 0x00,

        VUMeter = 0x01,
        Waveform = 0x02,
        Spectrogram = 0x04,

        WCFInterface = 0x10,

        All = 0xFF
    }

    public static class ProTONEConfig
    {
        const string _Fallback_DefaultSubtitleURIs = 
            @"BSP_V1;http://api.bsplayer-subtitles.com/v1.php;1\Osdb;http://api.opensubtitles.org/xml-rpc;1\NuSoap;http://api.getsubtitle.com/server.php;0";

        const string _Fallback_DefaultLinkedFiles =
            @"AU;AIF;AIFF;CDA;FLAC;MID;MIDI;MP1;MP2;MP3;MPA;RAW;RMI;SND;WAV;WMA/BMK\AVI;DIVX;QT;M1V;M2V;MOD;MOV;MPG;MPEG;VOB;WM;WMV;MKV;MP4/SUB;SRT;USF;ASS;SSA;BMK";


        #region Calculated Level 2 settings

        public static bool IsPlayer
        {
            get
            {
                return (string.Compare(ApplicationInfo.ApplicationName, ProTONEConstants.PlayerName) == 0);
            }
        }

        public static bool IsMediaLibrary
        {
            get
            {
                return (string.Compare(ApplicationInfo.ApplicationName, ProTONEConstants.LibraryName) == 0);
            }
        }

        public static string PlayerInstallationPath
        {
            get
            {
                return Path.Combine(AppConfig.InstallationPath, ProTONEConstants.PlayerBinary);
            }
        }

        public static string LibraryInstallationPath
        {
            get
            {
                return Path.Combine(AppConfig.InstallationPath, ProTONEConstants.LibraryBinary);
            }
        }

        public static bool IsRCCServiceInstalled
        {
            get
            {
                try
                {
                    ServiceController sc = new ServiceController(ProTONEConstants.RCCServiceShortName);
                    ServiceControllerStatus scs = sc.Status;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static string RCCManagerInstallationPath
        {
            get
            {
                return Path.Combine(AppConfig.InstallationPath, ProTONEConstants.RCCManagerBinary);
            }
        }

        public static string RCCServiceInstallationPath
        {
            get
            {
                return Path.Combine(AppConfig.InstallationPath, ProTONEConstants.RCCServiceBinary);
            }
        }
        #endregion

        #region Level 2 Settings using Persistence Service (Per-suite settings)

        public static int OsdbKeepAliveInterval
        {
            get
            {
                return PersistenceProxy.ReadObject("OsdbKeepAliveInterval", 5 * 60 * 1000);
            }
            set
            {
                PersistenceProxy.SaveObject("OsdbKeepAliveInterval", value);
            }
        }


        public static bool UseMetadata
        {
            get { return PersistenceProxy.ReadObject("UseMetadata", true); }
            set { PersistenceProxy.SaveObject("UseMetadata", value); }
        }

        public static bool UseFileNameFormat
        {
            get { return PersistenceProxy.ReadObject("UseFileNameFormat", true); }
            set { PersistenceProxy.SaveObject("UseFileNameFormat", value); }
        }

        public static string PlaylistEntryFormat
        {
            get { return PersistenceProxy.ReadObject("PlaylistEntryFormat", "<A> - <T>"); }
            set { PersistenceProxy.SaveObject("PlaylistEntryFormat", value); }
        }

        public static string FileNameFormat
        {
            get { return PersistenceProxy.ReadObject("FileNameFormat", "<A> - <T>"); }
            set { PersistenceProxy.SaveObject("FileNameFormat", value); }
        }

        public static string CustomPlaylistEntryFormats
        {
            get { return PersistenceProxy.ReadObject("CustomPlaylistEntryFormats", string.Empty); }
            set { PersistenceProxy.SaveObject("CustomPlaylistEntryFormats", value); }
        }

        public static string CustomFileNameFormats
        {
            get { return PersistenceProxy.ReadObject("CustomFileNameFormats", string.Empty); }
            set { PersistenceProxy.SaveObject("CustomFileNameFormats", value); }
        }


        public static bool DisableDVDMenu
        {
            get { return PersistenceProxy.ReadObject(true, "DisableDVDMenu", false); }
            set { PersistenceProxy.SaveObject(true, "DisableDVDMenu", value); }
        }

        public static int PrefferedSubtitleLang
        {
            get { return PersistenceProxy.ReadObject("PrefferedSubtitleLang", 1033); }
            set { PersistenceProxy.SaveObject("PrefferedSubtitleLang", value); }
        }

        public static bool SubEnabled
        {
            get { return PersistenceProxy.ReadObject("SubEnabled", false); }
            set { PersistenceProxy.SaveObject("SubEnabled", value); }
        }

        public static bool OsdEnabled
        {
            get { return PersistenceProxy.ReadObject("OsdEnabled", false); }
            set { PersistenceProxy.SaveObject("OsdEnabled", value); }
        }

        public static Color OsdColor
        {
            get
            {
                int argb = PersistenceProxy.ReadObject("OsdColor", Color.White.ToArgb());
                return Color.FromArgb(argb);
            }

            set
            {
                PersistenceProxy.SaveObject("OsdColor", value.ToArgb());
            }
        }

        public static Color SubColor
        {
            get
            {
                int argb = PersistenceProxy.ReadObject("SubColor", Color.White.ToArgb());
                return Color.FromArgb(argb);
            }

            set
            {
                PersistenceProxy.SaveObject("SubColor", value.ToArgb());
            }
        }

        static Font DefSubAndOsdFont = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Point);

        public static Font OsdFont
        {
            get
            {
                string _f = PersistenceProxy.ReadObject("OsdFont", new FontConverter().ConvertToInvariantString(DefSubAndOsdFont));
                Font f = (Font)new FontConverter().ConvertFromInvariantString(_f);
                byte charSet = (byte)PersistenceProxy.ReadObject("OsdFontCharSet", DefSubAndOsdFont.GdiCharSet);
                return new Font(f.FontFamily, f.Size, f.Style, f.Unit, charSet);
            }

            set
            {
                PersistenceProxy.SaveObject("OsdFont", new FontConverter().ConvertToInvariantString(value));
                PersistenceProxy.SaveObject("OsdFontCharSet", value.GdiCharSet);
            }
        }

        public static Font SubFont
        {
            get
            {
                string _f = PersistenceProxy.ReadObject("SubFont", new FontConverter().ConvertToInvariantString(DefSubAndOsdFont));
                Font f = (Font)new FontConverter().ConvertFromInvariantString(_f);
                byte charSet = (byte)PersistenceProxy.ReadObject("SubFontCharSet", DefSubAndOsdFont.GdiCharSet);
                return new Font(f.FontFamily, f.Size, f.Style, f.Unit, charSet);
            }

            set
            {
                PersistenceProxy.SaveObject("SubFont", new FontConverter().ConvertToInvariantString(value));
                PersistenceProxy.SaveObject("SubFontCharSet", value.GdiCharSet);
            }
        }

        public static int OsdPersistTimer
        {
            get
            {
                return PersistenceProxy.ReadObject("OsdPersistTimer", 4000);
            }

            set
            {
                PersistenceProxy.SaveObject("OsdPersistTimer", value);
            }
        }

        public static bool SubtitleDownloadEnabled
        {
            get
            {
                return PersistenceProxy.ReadObject("SubtitleDownloadEnabled", true);
            }

            set
            {
                PersistenceProxy.SaveObject("SubtitleDownloadEnabled", value);
            }
        }

        public static int SubtitleMinimumMovieDuration
        {
            get
            {
                return PersistenceProxy.ReadObject("SubtitleMinimumMovieDuration", 20 /* 20 minutes */);
            }

            set
            {
                PersistenceProxy.SaveObject("SubtitleMinimumMovieDuration", value);
            }
        }

        public static string DefaultSubtitleURIs
        {
            get { return PersistenceProxy.ReadObject("DefaultSubtitleURIs", _Fallback_DefaultSubtitleURIs, false); }
        }

        public static string DefaultLinkedFiles
        {
            get { return PersistenceProxy.ReadObject("DefaultLinkedFiles", _Fallback_DefaultLinkedFiles, false); }
        }


        public static CddaInfoSource AudioCdInfoSource
        {
            get { return PersistenceProxy.ReadObject("AudioCdInfoSource", CddaInfoSource.CdText_Cddb, false); }
            set { PersistenceProxy.SaveObject("AudioCdInfoSource", value, false); }
        }

        public static string CddbServerName
        {
            get { return PersistenceProxy.ReadObject("CddbServerName", "freedb.freedb.org", false); }
            set { PersistenceProxy.SaveObject("CddbServerName", value, false); }
        }

        public static int CddbServerPort
        {
            get { return PersistenceProxy.ReadObject("CddbServerPort", 8880, false); }
            set { PersistenceProxy.SaveObject("CddbServerPort", value, false); }
        }

        #endregion

        #region Level 2 Settings using Persistence Service (Per-user settings)

        public static string SubtitleDownloadURIs
        {
            get { return PersistenceProxy.ReadObject("SubtitleDownloadURIs", DefaultSubtitleURIs); }
            set { PersistenceProxy.SaveObject("SubtitleDownloadURIs", value); }
        }

        public static List<string> GetFavoriteFolders(string favFoldersHiveName)
        {
            List<string> favoriteFolders = new List<string>();

            string str = PersistenceProxy.ReadObject(favFoldersHiveName, string.Empty);
            if (!string.IsNullOrEmpty(str))
            {
                string[] favFolders = StringUtils.ToStringArray(str, '?');
                favoriteFolders.AddRange(favFolders);
            }

            return favoriteFolders;
        }

        public static void SetFavoriteFolders(List<string> folders, string favFoldersHiveName)
        {
            List<string> favoriteFolders = new List<string>();
            favoriteFolders.AddRange(folders);

            string favFolders = StringUtils.FromStringArray(favoriteFolders.ToArray(), '?');
            if (favFolders == null)
                favFolders = string.Empty;

            PersistenceProxy.SaveObject(favFoldersHiveName, favFolders);
        }

        public static bool AddToFavoriteFolders(string path)
        {
            List<string> favorites = new List<string>(ProTONEConfig.GetFavoriteFolders("FavoriteFolders"));
            if (favorites.Contains(path))
                return false;

            favorites.Add(path);
            ProTONEConfig.SetFavoriteFolders(favorites, "FavoriteFolders");
            return true;
        }

        static bool? _useLinkedFiles = null;

        public static bool UseLinkedFiles
        {
            get 
            {
                if (_useLinkedFiles.HasValue)
                    return _useLinkedFiles.Value;

                _useLinkedFiles = (PersistenceProxy.ReadObject("UseLinkedFiles", 1) != 0);

                return _useLinkedFiles.Value; 
            }
            set 
            {
                _useLinkedFiles = value;
                PersistenceProxy.SaveObject("UseLinkedFiles", value ? 1 : 0); 
            }
        }

        static Dictionary<string, string> _table = null;
        public static Dictionary<string, string> LinkedFilesTable
        {
            get
            {
                if (_table == null)
                {
                    _table = new Dictionary<string, string>();

                    try
                    {
                        string st = PersistenceProxy.ReadObject("LinkedFiles", DefaultLinkedFiles);
                        string[] pairs = StringUtils.ToStringArray(st, '\\');
                        if (pairs != null && pairs.Length > 0)
                        {
                            foreach (string pair in pairs)
                            {
                                string[] nameValue = StringUtils.ToStringArray(pair, '/');
                                if (nameValue != null && nameValue.Length > 0)
                                {
                                    string name = nameValue[0];
                                    string value = nameValue.Length > 1 ? nameValue[1] : string.Empty;

                                    try
                                    {
                                        _table.Add(name, value);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                return _table;
            }

            set
            {
                try
                {
                    if (value != null)
                    {
                        string str = string.Empty;
                        foreach (KeyValuePair<string, string> kvp in value)
                        {
                            str += kvp.Key;
                            str += "/";
                            str += kvp.Value;
                            str += "\\";
                        }

                        str = str.Trim('\\').Trim('/');

                        PersistenceProxy.SaveObject("LinkedFiles", str);
                    }

                    _table = value;
                }
                catch
                {
                }
            }
        }

        public static string[] GetChildFileTypes(string fileType)
        {
            if (_table == null)
                _table = LinkedFilesTable;

            foreach (KeyValuePair<string, string> kvp in _table)
            {
                List<string> types = new List<string>(StringUtils.ToStringArray(kvp.Key, ';'));
                if (types.Contains(fileType.ToUpperInvariant()))
                {
                    return StringUtils.ToStringArray(kvp.Value, ';');
                }
            }

            return null;
        }

        public static string[] GetParentFileTypes(string fileType)
        {
            foreach (KeyValuePair<string, string> kvp in _table)
            {
                List<string> types = new List<string>(StringUtils.ToStringArray(kvp.Value, ';'));
                if (types.Contains(fileType.ToUpperInvariant()))
                {
                    return StringUtils.ToStringArray(kvp.Key, ';');
                }
            }

            return null;
        }
        #endregion

        #region Level 2 Settings using Settings File (Combined per-app and per-user settings)

        public static MediaScreen ShowMediaScreens
        {
            get { return (MediaScreen)PersistenceProxy.ReadObject(true, "ShowMediaScreens", (int)MediaScreen.All); }
            set { PersistenceProxy.SaveObject(true, "ShowMediaScreens", (int)value); }
        }

        public static bool MediaScreenActive(MediaScreen mediaScreen)
        {
            return ((ProTONEConfig.ShowMediaScreens & mediaScreen) == mediaScreen);
        }

        private static SignalAnalisysFunction? _signalAnalisysFunctions = null;
        private static object _signalAnalisysFunctionsLock = new object();

        public static SignalAnalisysFunction SignalAnalisysFunctions
        {
            get 
            {
                lock (_signalAnalisysFunctionsLock)
                {
                    if (_signalAnalisysFunctions == null)
                        _signalAnalisysFunctions = (SignalAnalisysFunction)PersistenceProxy.ReadObject(true, "SignalAnalisysFunctions", 
                            (int)SignalAnalisysFunction.All);

                    if (_signalAnalisysFunctions == null)
                        return SignalAnalisysFunction.All;

                    return _signalAnalisysFunctions.Value;
                }
            }
            
            set 
            {
                lock (_signalAnalisysFunctionsLock)
                {
                    if (_signalAnalisysFunctions == null || _signalAnalisysFunctions.Value != value)
                    {
                        _signalAnalisysFunctions = value;
                        PersistenceProxy.SaveObject(true, "SignalAnalisysFunctions", (int)value);
                    }
                }
            }
        }

        public static bool SignalAnalisysFunctionActive(SignalAnalisysFunction function)
        {
            return ((ProTONEConfig.SignalAnalisysFunctions & function) == function);
        }

        public static bool IsSignalAnalisysActive()
        {
            return (ProTONEConfig.SignalAnalisysFunctions != SignalAnalisysFunction.None);
        }

        
        public static string ExplorerLaunchType
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "ExplorerLaunchType", "EnqueueFiles");
            }

            set
            {
                PersistenceProxy.SaveObject(true, "ExplorerLaunchType", value);
            }
        }

        public static int LastBalance
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastBalance", 0);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastBalance", value);
            }
        }

        public static int LastVolume
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastVolume", 5000);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastVolume", value);
            }
        }

        public static int LastFilterIndex
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastFilterIndex", 0);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastFilterIndex", value);
            }
        }

        public static string LastOpenedFolder
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LastOpenedFolder", PathUtils.CurrentDir);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LastOpenedFolder", value);
            }
        }

        public static int PL_LastFilterIndex
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "PL_LastFilterIndex", 0);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "PL_LastFilterIndex", value);
            }
        }

        public static string PL_LastOpenedFolder
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "PL_LastOpenedFolder", PathUtils.CurrentDir);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "PL_LastOpenedFolder", value);
            }
        }



        public static bool LoopPlay
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "LoopPlay", false);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "LoopPlay", value);
            }
        }

        public static bool ShufflePlaylist
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "ShufflePlaylist", false);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "ShufflePlaylist", value);
            }
        }

        public static int PlaylistEventHandler
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "PlaylistEventHandler", 0);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "PlaylistEventHandler", value);
            }
        }

        public static string PlaylistEventData
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "PlaylistEventData", string.Empty);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "PlaylistEventData", value);
            }
        }


        public static int ScheduledEventHandler
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "ScheduledEventHandler", 0);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "ScheduledEventHandler", value);
            }
        }

        public static TimeSpan ScheduledEventTime
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "ScheduledEventTime", new TimeSpan(0, 0, 0));
            }
            set
            {
                PersistenceProxy.SaveObject(true, "ScheduledEventTime", value);
            }
        }

        public static int ScheduledEventDays
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "ScheduledEventDays", 0);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "ScheduledEventDays", value);
            }
        }

        public static bool EnableScheduledEvent
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "EnableScheduledEvent", false);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "EnableScheduledEvent", value);
            }
        }

        public static int SchedulerWaitTimerProceed
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SchedulerWaitTimerProceed", 2);
            }
            set
            {
                PersistenceProxy.SaveObject(true, "SchedulerWaitTimerProceed", value);
            }
        }



        public static bool MediaStateNotificationsEnabled
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "MediaStateNotificationsEnabled", true);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "MediaStateNotificationsEnabled", value);
            }
        }

        public static bool SubDownloadedNotificationsEnabled
        {
            get
            {
                return PersistenceProxy.ReadObject(true, "SubDownloadedNotificationsEnabled", true);
            }

            set
            {
                PersistenceProxy.SaveObject(true, "SubDownloadedNotificationsEnabled", value);
            }
        }

        
        public static bool FullScreenOn
        {
            get
            {
                if (IsPlayer)
                    return PersistenceProxy.ReadObject(true, "FullScreenOn", false);

                return false;
            }

            set
            {
                if (IsPlayer)
                    PersistenceProxy.SaveObject(true, "FullScreenOn", value);
            }
        }

        public static Point OnlineContentBrowser_WindowLocation
        {
            get
            {
                try
                {
                    string str = PersistenceProxy.ReadObject(true, "OnlineContentBrowser_WindowLocation", string.Empty);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return (Point)new PointConverter().ConvertFromInvariantString(str);
                    }
                }
                catch
                {
                }

                Point ptFallback = new Point(100, 100);

                PersistenceProxy.SaveObject(true, "OnlineContentBrowser_WindowLocation", new PointConverter().ConvertToInvariantString(ptFallback));

                return ptFallback;
            }
            set
            {
                if ((value.X >= 0) && (value.Y >= 0))
                {
                    PersistenceProxy.SaveObject(true, "OnlineContentBrowser_WindowLocation", new PointConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static Size OnlineContentBrowser_WindowSize
        {
            get
            {
                Size size = new Size(800, 600);
                try
                {
                    string str = PersistenceProxy.ReadObject(true, "OnlineContentBrowser_WindowSize", string.Empty);
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
                    PersistenceProxy.SaveObject(true, "OnlineContentBrowser_WindowSize", new SizeConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static Point DetachedWindowLocation
        {
            get
            {
                try
                {
                    string str = PersistenceProxy.ReadObject(true, "DetachedWindowLocation", string.Empty);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return (Point)new PointConverter().ConvertFromInvariantString(str);
                    }
                }
                catch
                {
                }

                Point ptFallback = new Point(100, 100);

                PersistenceProxy.SaveObject(true, "DetachedWindowLocation", new PointConverter().ConvertToInvariantString(ptFallback));

                return ptFallback;
            }
            set
            {
                if ((value.X >= 0) && (value.Y >= 0))
                {
                    PersistenceProxy.SaveObject(true, "DetachedWindowLocation", new PointConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static Size DetachedWindowSize
        {
            get
            {
                Size size = new Size(800, 600);
                try
                {
                    string str = PersistenceProxy.ReadObject(true, "DetachedWindowSize", string.Empty);
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
                    PersistenceProxy.SaveObject(true, "DetachedWindowSize", new SizeConverter().ConvertToInvariantString(value));
                }
            }
        }

        public static FormWindowState DetachedWindowState
        {
            get
            {
                FormWindowState normal = FormWindowState.Normal;
                try
                {
                    normal = (FormWindowState)PersistenceProxy.ReadObject(true, "DetachedWindowState", 0);
                }
                catch
                {
                }
                return normal;
            }
            set
            {
                PersistenceProxy.SaveObject(true, "DetachedWindowState", (int)value);
            }
        }

        #endregion

        #region SHoutCast settings

        public static string ShoutCastApiDevID
        {
            get { return PersistenceProxy.ReadObject("ShoutCastApiDevID", string.Empty); }
            set { PersistenceProxy.SaveObject("ShoutCastApiDevID", value); }
        }

        public static string ShoutCastSearchBaseURL
        {
            get { return PersistenceProxy.ReadObject("ShoutCastSearchBaseURL", "http://api.shoutcast.com/station"); }
            set { PersistenceProxy.SaveObject("ShoutCastSearchBaseURL", value); }
        }

        public static string ShoutCastTuneInBaseURL
        {
            get { return PersistenceProxy.ReadObject("ShoutCastTuneInBaseURL", "http://yp.shoutcast.com"); }
            set { PersistenceProxy.SaveObject("ShoutCastTuneInBaseURL", value); }
        }

        #endregion

        #region Deezer settings

        public static string DeezerUserAccessToken
        {
            get { return PersistenceProxy.ReadObject("DeezerUserAccessToken", string.Empty); }
            set { PersistenceProxy.SaveObject("DeezerUserAccessToken", value); }
        }

        public static string DeezerApplicationId
        {
            get { return PersistenceProxy.ReadObject("DeezerApplicationId", string.Empty); }
            set { PersistenceProxy.SaveObject("DeezerApplicationId", value); }
        }

        public static string DeezerApiEndpoint
        {
            get { return PersistenceProxy.ReadObject("DeezerApiEndpoint", "http://api.deezer.com/"); }
            set { PersistenceProxy.SaveObject("DeezerApiEndpoint", value); }
        }

        #endregion

        #region Media Browser History
        public static List<string> Media_Browser_History_Local
        {
            get
            {
                string str = PersistenceProxy.ReadObject(true, "Media_Browser_History_Local", string.Empty);
                return StringUtils.ToStringList(str, '|');
            }

            set
            {
                string str = StringUtils.FromStringList(value, '|');
                PersistenceProxy.SaveObject(true, "Media_Browser_History_Local", str);
            }
        }

        public static List<string> Media_Browser_History_Shoutcast
        {
            get
            {
                string str = PersistenceProxy.ReadObject(true, "Media_Browser_History_Shoutcast", string.Empty);
                return StringUtils.ToStringList(str, '|');
            }

            set
            {
                string str = StringUtils.FromStringList(value, '|');
                PersistenceProxy.SaveObject(true, "Media_Browser_History_Shoutcast", str);
            }
        }

        public static List<string> Media_Browser_History_Deezer
        {
            get
            {
                string str = PersistenceProxy.ReadObject(true, "Media_Browser_History_Deezer", string.Empty);
                return StringUtils.ToStringList(str, '|');
            }

            set
            {
                string str = StringUtils.FromStringList(value, '|');
                PersistenceProxy.SaveObject(true, "Media_Browser_History_Deezer", str);
            }
        }
        #endregion

        static Guid _filterGraphGuid = Guid.Empty;
        static object _filterGraphGuidLock = new object();

        public static Guid FilterGraphGuid
        {
            get
            {
                try
                {
                    if (_filterGraphGuid == Guid.Empty)
                    {
                        lock (_filterGraphGuidLock)
                        {
                            if (_filterGraphGuid == Guid.Empty)
                            {
                                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\OPMedia Research\ProTONE Suite");
                                if (key != null)
                                {
                                    string str = key.GetValue("SelectedFilterGraphGuid", Filters.FilterGraphNoThread.ToString()) as string;
                                    _filterGraphGuid = Guid.Parse(str);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    _filterGraphGuid = Filters.FilterGraphNoThread;
                }

                return _filterGraphGuid;
            }
        }

    }
}
