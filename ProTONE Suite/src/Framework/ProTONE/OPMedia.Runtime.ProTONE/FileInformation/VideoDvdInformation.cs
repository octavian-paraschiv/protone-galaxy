using System;
using System.Collections.Generic;
using System.Text;
using OPMedia.Core.Logging;
using System.Runtime.InteropServices;
using OPMedia.Runtime.ProTONE.Rendering.Base;

#if HAVE_DSHOW
using OPMedia.Runtime.ProTONE.Rendering.DS;
using OPMedia.Core;
using System.IO;
using OPMedia.Core.Configuration;
#endif

namespace OPMedia.Runtime.ProTONE.FileInformation
{
    public class VideoDvdInformation : VideoFileInfo
    {
        public const string UnsupportedWindows = "Sorry, ProTONE does not support playing video DVD's in the current Windows v.({0}).";
        public const string ErrDvdVolume = "An invalid DVD volume was specified";

        string _dvdPath = string.Empty;
        string _label = string.Empty;
        string _defaultLabel = string.Empty;

        List<int> _chaptersPerTitle = new List<int>();
        List<int> _durationPerTitle = new List<int>();
        List<DvdSubpictureAttributes> _subtitles = new List<DvdSubpictureAttributes>();

        DvdMenuAttributes _dma;

        public string Label 
        { 
            get 
            { 
                return string.IsNullOrEmpty(_label) ? _defaultLabel : _label;
            } 
        }
        
        public override string MediaType
        {
            get
            {
                return "DVD";
            }
        }
        
        public new string Path
        { get { return _dvdPath; } }

        public new string Name
        { get { return Label; } }

        public List<int> ChaptersPerTitle
        { get { return _chaptersPerTitle; } }

        public List<DvdSubpictureAttributes> AvailableSubtitles
        { get { return _subtitles; } }

        public override int GetHashCode()
        {
            return _dvdPath.GetHashCode() + Label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            VideoDvdInformation drv = obj as VideoDvdInformation;
            if (drv == null) return false;

            return (_dvdPath == drv._dvdPath && Label == drv._label);
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", _dvdPath, Label);
        }

        public int GetSubtitle(int lcid)
        {
            if (_subtitles.Count > 0)
            {
                for (int i = 0; i < _subtitles.Count; i++)
                {
                    if (_subtitles[i].Language == lcid)
                        return i;
                }
            }

            return -1;
        }

        public VideoDvdInformation(string path)
        {
            _dvdPath = path;

            DriveInfo di = new DriveInfo(System.IO.Path.GetPathRoot(path));
            _defaultLabel = di.VolumeLabel;

            string volumePath = string.Empty;
            if (path.ToUpperInvariant().EndsWith("VIDEO_TS"))
            {
                volumePath = path;
            }
            else
            {
                volumePath = System.IO.Path.Combine(path, "VIDEO_TS");
            }

            FetchDVDInformation_DS(volumePath);
        }

        private void FetchDVDInformation_DS(string volumePath)
        {
            IDvdGraphBuilder dvdGraphBuilder =
                        Activator.CreateInstance(Type.GetTypeFromCLSID(Filters.DvdGraphBuilder, true))
                        as IDvdGraphBuilder;

            AMDvdRenderStatus status;

            dvdGraphBuilder.TryRenderDVD(volumePath, out status);

            if (status.bDvdVolInvalid)
                throw new COMException(ErrDvdVolume, -1);

            object comobj = null;

            dvdGraphBuilder.GetDvdInterface(typeof(IDvdInfo2).GUID, out comobj);

            IDvdInfo2 dvdInfo = comobj as IDvdInfo2;
            IDvdControl2 dvdControl = comobj as IDvdControl2;

            dvdControl.SetOption(DvdOptionFlag.HMSFTimeCodeEvents, true);	// use new HMSF timecode format
            dvdControl.SetOption(DvdOptionFlag.ResetOnStop, false);

            // Try getting the frame rate and the video size
            dvdInfo.GetVMGAttributes(out _dma);

            this.FrameRate = new FrameRate(_dma.VideoAttributes.frameRate);
            this.VideoSize = new VSize(_dma.VideoAttributes.sourceResolutionX, _dma.VideoAttributes.sourceResolutionY);

            // Try getting the DVD volume name.
            // Stage 1: Get the number of available languages.
            int numLangs = 0;
            dvdInfo.GetDVDTextNumberOfLanguages(out numLangs);

            if (numLangs > 0)
            {
                // Stage 2: Get string count for the first language.
                int numStrings = 0;
                int langId = 0;
                DvdTextCharSet charSet = DvdTextCharSet.CharSet_Unicode;

                dvdInfo.GetDVDTextLanguageInfo(0, out numStrings, out langId, out charSet);

                // Stage 3: Iterate through the string collection and identify the volume name
                for (int i = 0; i < numStrings; i++)
                {
                    int maxSize = 4096;

                    StringBuilder sb = new StringBuilder(maxSize);
                    int txtSize = 0;
                    DvdTextStringType textType;

                    dvdInfo.GetDVDTextStringAsUnicode(0, i, sb, maxSize, out txtSize, out textType);

                    // Is this the volume name ?
                    if (textType == DvdTextStringType.DVD_General_Name)
                    {
                        // Volume name was found, so exit iteration.
                        _label = sb.ToString();
                        break;
                    }
                }
            }

            // Try getting the titles, chapters and overall duration info
            int numVolumes = 0, volumeNumber = 0, numTitles = 0;
            DvdDiscSide sideInfo;
            dvdInfo.GetDVDVolumeInfo(out numVolumes, out volumeNumber, out sideInfo, out numTitles);

            for (int i = 1; i <= numTitles; i++)
            {
                int numChapters = 0;
                try
                {
                    DvdMenuAttributes menuAttr;
                    DvdTitleAttributes titleAttr = new DvdTitleAttributes();

                    if (i == 1)
                    {
                        dvdInfo.GetTitleAttributes(i, out menuAttr, titleAttr);

                        for (int j = 0; j < titleAttr.ulNumberOfSubpictureStreams; j++)
                        {
                            _subtitles.Add(titleAttr.SubpictureAttributes[j]);
                        }
                    }

                    dvdInfo.GetNumberOfChapters(i, out numChapters);
                    _chaptersPerTitle.Add(numChapters);
                }
                catch { }
            }
        }
    }

    public static class VideoDVDHelpers
    {
        public static bool IsOSSupported
        {
            get
            {
                bool retVal = false;

                var ver = AppConfig.OSVersion;

                switch (ver)
                {
                    case AppConfig.VerWin10:
                    case AppConfig.VerWin7:
                        retVal = true;
                        break;

                    default:
                        break;

                }

                Logger.LogTrace("[Windows {0}.{1}] - VideoDVDHelpers::IsOSSupported={2}",
                    ver / 10, ver % 10, retVal);

                return retVal;
            }
        }

        private static AMDvdGraphFlags DvdRenderingFlags
        {
            get
            {
                AMDvdGraphFlags retVal = AMDvdGraphFlags.None;
                var ver = AppConfig.OSVersion;

                switch (ver)
                {
                    case AppConfig.VerWin10:
                    case AppConfig.VerWin8:
                    case AppConfig.VerWin8_1:
                        retVal = AMDvdGraphFlags.SWDecPrefer;
                        break;

                    default:
                        retVal = AMDvdGraphFlags.VMR9Only;
                        break;
                }

                Logger.LogTrace("[Windows {0}.{1}] Using {2} to render DVD media.",
                    ver / 10, ver % 10, retVal);

                return retVal;
            }
        }

        public static void TryRenderDVD(this IDvdGraphBuilder dvdGraphBuilder, string volumePath, out AMDvdRenderStatus latestStatus)
        {
            // Is the OS supported for this operation ?
            if (IsOSSupported == false)
            {
                var ver = AppConfig.OSVersion;
                throw new COMException(string.Format(VideoDvdInformation.UnsupportedWindows, 
                    string.Format("{0}.{1}", ver / 10, ver % 10)), -1);
            }

            // First we try with the default flags per current OS
            AMDvdGraphFlags defaultFlags = DvdRenderingFlags;
            dvdGraphBuilder.RenderDvdVideoVolume(volumePath, defaultFlags, out latestStatus);
            if (latestStatus.bDvdVolInvalid)
                throw new COMException(VideoDvdInformation.ErrDvdVolume, -1);

            if (latestStatus.iNumStreamsFailed == 0)
            {
                Logger.LogTrace("VideoDVDHelpers::TryRenderDVD: Using default flags {0} => SUCCESS !", defaultFlags);
                return;
            }

            Logger.LogTrace("VideoDVDHelpers::TryRenderDVD: Using default flags  {0} => failed to open DVD streams: {1}.",
                defaultFlags, latestStatus.dwFailedStreamsFlag);

            // If no success, we iterate through all possible rendering flags
            foreach (AMDvdGraphFlags flags in Enum.GetValues(typeof(AMDvdGraphFlags)))
            {
                if (latestStatus.iNumStreamsFailed == 0)
                {
                    Logger.LogTrace("VideoDVDHelpers::TryRenderDVD: Using flags {0} => SUCCESS !", flags);
                    return;
                }

                Logger.LogTrace("VideoDVDHelpers::TryRenderDVD: Using flags {0} => failed to open DVD streams: {1}", 
                    flags, latestStatus.dwFailedStreamsFlag);
            }

            
        }
    }
}
