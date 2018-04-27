#region Copyright © 2006 OPMedia Research
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written permission of the copyright owner.

// File: 	MediaRenderer.cs
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using OPMedia.Runtime.ProTONE.Rendering.Base;
using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.Core.Logging;
using OPMedia.Core.TranslationSupport;

#if HAVE_DSHOW
    using OPMedia.Runtime.ProTONE.Rendering.DS;
#else
    using OPMedia.Runtime.ProTONE.Rendering.Mono;
#endif

using OPMedia.Core;
using System.IO;

using OPMedia.Core.Utilities;
using OPMedia.Runtime.ProTONE.SubtitleDownload;
using OPMedia.Runtime.ProTONE.FfdShowApi;
using OPMedia.Core.Configuration;
using OPMedia.UI.Generic;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Runtime.ProTONE.Utilities;
using OPMedia.Runtime.ProTONE.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;
using OPMedia.Runtime.ProTONE.Rendering.SHOUTCast;

#endregion

namespace OPMedia.Runtime.ProTONE.Rendering
{
    public enum VideoSizeAdjustmentDirection
    {
        None = 0,
        Horizontal,
        Vertical
    }
    
    public enum VideoSizeAdjustmentAction
    {
        None = 0,
        Shrink,
        Expand
    }

    public delegate void MediaRendererEventHandler();
    public delegate void FilterStateChangedHandler(OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState oldState, string oldMedia, 
        OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState newState, string newMedia);

    public delegate void MediaRenderingExceptionHandler(RenderingExceptionEventArgs args);
    
    public delegate void RenderedStreamPropertyChangedHandler(Dictionary<string, string> newData);

    [DataContract]
    public class AudioSampleData
    {
        [DataMember]
        public double LVOL { get; private set; }

        [DataMember]
        public double RVOL { get; private set; }

        public AudioSampleData(double lVol, double rVol)
        {
            LVOL = lVol;
            RVOL = rVol;
        }
    }

    [DataContract]
    public class SignalAnalisysData
    {
        [DataMember]
        public AudioSampleData MomentarySampleData { get; set; }

        [DataMember]
        public double[] SpectrogramData { get; set; }

        public override string ToString()
        {
            return string.Format("MomSample: L={0},R={1}, Spectrogram: len={2}, data[0]={3:0.00}",
                (MomentarySampleData != null) ? MomentarySampleData.LVOL : 0, 
                (MomentarySampleData != null) ? MomentarySampleData.RVOL : 0, 
                (SpectrogramData != null) ? SpectrogramData.Length : 0, 
                (SpectrogramData != null) ? SpectrogramData[0] : 0);
        }
    }

    [ServiceContract]
    public interface ISignalAnalisys
    {
        [OperationContract]
        SignalAnalisysData GetSignalAnalisysData();
    }


    public sealed class MediaRenderer : IDisposable
    {
        public const int VolumeFull = 0;
        public const int VolumeSilence = -10000;

        #region Supported file types
        static List<string> __supportedAudioMediaTypes = new List<string>(new string[] 
            { 
                // 17 supported audio file types
                "au",   
                "aif", 
                "aiff", 
                "cda", // Audio CD track
                "flac", 
                "mid", "midi", 
                
                "mod", // audio "module" file type

                "mp1", "mp2",  "mp3", "mpa",  
                "raw", 
                "rmi",  
                "snd",  
                "wav",  
                "wma",
            });

        static List<string> __supportedVideoMediaTypes = new List<string>(new string[] 
            {
                // 14 supported video file types

                "avi", "divx", "qt",  "m1v", "m2v", 
                
                "mod", // video format for use in digital tapeless camcorders (JVC / Panasonic / Canon)

                "mov",  "mpg", "mpeg", "vob", 
                "wm", "wmv", 
                
                "mkv", "mp4", 
            });
        static List<string> __supportedHDVideoMediaTypes = new List<string>(new string[] 
            {
                "mkv", "mp4", 
            });

        static List<string> __supportedPlaylists = new List<string>(new string[] 
            {
                "m3u", "pls", "asx", "wpl"
            });

        static List<string> __supportedSubtitles = new List<string>(new string[] 
            {
                // MicroDVD
                "sub", 
                
                // SubRip
                "srt", 
                
                // Universal Subtitle Format
                "usf", 
                
                // SubStation Alpha
                "ass", "ssa", 

                //"utf", "idx", "smi", "rt", "aqt", "mpl", 
            });

        #endregion

        private int _hash = DateTime.Now.GetHashCode();
        private double _position = 0;

        public event RenderedStreamPropertyChangedHandler RenderedStreamPropertyChanged = null;

        public class SupportedFileProvider : ISupportedFileProvider
        {
            public List<string> SupportedAudioTypes { get; internal set; }
            public List<string> SupportedHDVideoTypes { get; internal set; }
            public List<string> SupportedVideoTypes { get; internal set; }
            public List<string> SupportedPlaylists { get; internal set; }
            public List<string> SupportedSubtitles { get; internal set; }
            public List<string> AllMediaTypes { get; internal set; }
        }

        #region Members
        private static MediaRenderer __defaultInstance = new MediaRenderer(true);

        private StreamRenderer streamRenderer = null;
        private Timer timerCheckState = null;

        private OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState oldState = 
            OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped;

        private string oldMedia = string.Empty;

        Control renderPanel = null;
        Control messageDrain = null;

        volatile bool playlistAtEnd = false;
        object _syncPlaylist = new object();

        #endregion

        #region Properties
        
        public static MediaRenderer DefaultInstance
        {
            get
            {
                return __defaultInstance;
            }
        }

        public static MediaRenderer NewInstance()
        {
            return new MediaRenderer(false);
        }

        

        internal object GraphFilter 
        { get { return (streamRenderer != null) ? streamRenderer.GraphFilter : null; } }

        public WaveFormatEx ActualAudioFormat
        { get { return (streamRenderer != null) ? streamRenderer.ActualAudioFormat : null; } }


        public double[] EqFrequencies
        {
            get
            {
                double[] freqs = new double[10];

                using (FfdShowLib ff = FfdShowInstance())
                {
                    for (FFDShowConstants.FFDShowDataId i = FFDShowConstants.FFDShowDataId.IDFF_filterEQ; i < FFDShowConstants.FFDShowDataId.IDFF_filterWinamp2; i++)
                    {
                        int iVal = ff.getIntParam(i);
                        string sVal = ff.getStringParam(i);
                    }

                }

                return freqs;
            }

            set
            {
            }
        }


        public int[] EqLevels
        {
            get
            {
                int[] levels = new int[10];

                return levels;
            }

            set
            {
            }
        }

        public Control RenderPanel
        {
            get { return renderPanel; }
            set { renderPanel = value; }
        }

        public Control MessageDrain
        {
            get { return messageDrain; }
            set { messageDrain = value; }
        }

        public bool PlaylistAtEnd
        {
            get 
            {
                lock (_syncPlaylist)
                {
                    return playlistAtEnd;
                }
            }
            
            set 
            {
                lock (_syncPlaylist)
                {
                    playlistAtEnd = value;
                }
            }
        }

        private bool IsEndOfMedia
        {
            get
            {
                if (streamRenderer == null)
                    return false;

                return streamRenderer.EndOfMedia;
            }
        }

        public double MediaPosition
        {
            get { return (streamRenderer == null) ? 0 : streamRenderer.MediaPosition; }
            set { if (streamRenderer != null) { streamRenderer.MediaPosition = value; } }
        }

        public int AudioVolume
        {
            get { return (streamRenderer == null) ? (int)VolumeRange.Minimum : streamRenderer.AudioVolume; }
            set { if (streamRenderer != null) { streamRenderer.AudioVolume = value; } }
        }

        public int AudioBalance
        {
            get { return (streamRenderer == null) ? 0 : streamRenderer.AudioBalance; }
            set { if (streamRenderer != null) { streamRenderer.AudioBalance = value; } }
        }

        public int SubtitleStream
        {
            get
            {
                return (streamRenderer == null &&
                    FilterState != OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped) ?
                    -1 : streamRenderer.GetSubtitleStream();
            }

            set
            {
                if (streamRenderer != null &&
                    FilterState != OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped)
                {
                    streamRenderer.SetSubtitleStream(value);
                }
            }
        }

        public bool CanSeekMedia
        { get { return (streamRenderer != null && streamRenderer.MediaSeekable && streamRenderer.MediaLength > 0); } }

        public static List<string> SupportedAudioTypes
        { get { return __supportedAudioMediaTypes; } }

        public static List<string> SupportedHDVideoTypes
        { get { return __supportedHDVideoMediaTypes; } }

        public static List<string> SupportedVideoTypes
        { get { return __supportedVideoMediaTypes; } }

        public static List<string> SupportedPlaylists
        { get { return __supportedPlaylists; } }

        public double DurationScaleFactor
        { get { return (streamRenderer == null) ? 0 : streamRenderer.DurationScaleFactor; } }

        public double MediaLength
        { get { return (streamRenderer == null) ? 0 : streamRenderer.MediaLength; } }

        public string RenderMediaName
        {
            get
            {
                if (streamRenderer == null)
                    return string.Empty;

                return streamRenderer.RenderMediaName;
            }
        }

        public MediaFileInfo RenderedMediaInfo
        {
            get
            {
                if (streamRenderer == null)
                    return null;

                return streamRenderer.RenderMediaInfo;
            }
        }

        public MediaTypes RenderedMediaType
        { 
            get 
            {
                MediaTypes mediaType = MediaTypes.None;

                if (streamRenderer != null)
                {
                    if (streamRenderer.AudioMediaAvailable)
                    {
                        mediaType = (streamRenderer.VideoMediaAvailable) ?
                            MediaTypes.Both : MediaTypes.Audio;
                    }
                    else if (streamRenderer.VideoMediaAvailable)
                    {
                        mediaType = MediaTypes.Video;
                    }
                }

                return mediaType;
            } 
        }

        public OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState FilterState
         { 
            get 
            { 
                return (streamRenderer == null) ?
                    OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped : 
                    streamRenderer.FilterState; 
            } 
        }

        public string TranslatedFilterState
        { get { return Translator.Translate("TXT_" + FilterState.ToString().ToUpperInvariant()); } }

        //public bool ShowCursor
        //{
        //    get { return streamRenderer.ShowCursor; }
        //    set { streamRenderer.ShowCursor = value; }
        //}

        //public bool FullScreen
        //{
        //    get { return streamRenderer.FullScreen; }
        //    set { streamRenderer.FullScreen = value; }
        //}

        public static string AllMediaTypesMultiFilter
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string type in AllMediaTypes)
                    sb.Append(string.Format("{0};", type));

                return sb.ToString().ToLowerInvariant().Trim(';');
            }
        }

        public static List<string> AllMediaTypes
        {
            get
            {
                List<string> allTypes = new List<string>();
                allTypes.AddRange(__supportedAudioMediaTypes);
                allTypes.AddRange(__supportedVideoMediaTypes);
                allTypes.AddRange(__supportedPlaylists); // supported playlists
                return allTypes;
            }
        }

       public AudioSampleData VuMeterData
        {
            get
            {
                return (streamRenderer != null) ?
                    streamRenderer.VuMeterData : null;
            }
        }

        public double[][] WaveformData
        {
            get
            {
                return (streamRenderer != null) ?
                    streamRenderer.WaveformData : null;
            }
        }

        public double[] SpectrogramData
        {
            get
            {
                return (streamRenderer != null) ?
                    streamRenderer.SpectrogramData : null;
            }
        }

         public double MaxLevel
        {
            get
            {
                return (streamRenderer != null) ?
                    streamRenderer.MaxLevel : 0;
            }
        }

        public double FFTWindowSize
        {
            get
            {
                return (streamRenderer != null) ?
                    streamRenderer.FFTWindowSize : 0;
            }
        }
        public double MaxFFTLevel
        {
            get
            {
                return MaxLevel * FFTWindowSize;
            }
        }

        bool _hasRenderingErrors = false;
        public bool HasRenderingErrors
        {
            get
            {
                return _hasRenderingErrors;
            }
        }

        #endregion

        #region Methods

        public bool IsStreamedMedia { get { return (streamRenderer is DSShoutcastRenderer); } }

        public Dictionary<string, string> StreamData { get; private set; }

        internal void FireStreamPropertyChanged(Dictionary<string, string> newData)
        {
            if (IsStreamedMedia && RenderedStreamPropertyChanged != null)
            {
                if (StreamData == null)
                    StreamData = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in newData)
                {
                    if (StreamData.ContainsKey(kvp.Key))
                    {
                        if (String.IsNullOrEmpty(kvp.Value))
                            StreamData.Remove(kvp.Key);
                        else
                            StreamData[kvp.Key] = kvp.Value;
                    }
                    else if (String.IsNullOrEmpty(kvp.Value) == false)
                        StreamData.Add(kvp.Key, kvp.Value);
                }

                RenderedStreamPropertyChanged(newData);
            }
        }

        public static SupportedFileProvider GetSupportedFileProvider()
        {
            SupportedFileProvider retVal = new SupportedFileProvider();
            retVal.SupportedAudioTypes = __supportedAudioMediaTypes;
            retVal.SupportedVideoTypes = __supportedVideoMediaTypes;
            retVal.SupportedHDVideoTypes = __supportedHDVideoMediaTypes;
            retVal.SupportedPlaylists = __supportedPlaylists;
            retVal.SupportedSubtitles = __supportedSubtitles;

            retVal.AllMediaTypes = MediaRenderer.AllMediaTypes;

            return retVal;
        }

        public string GetRenderFile()
        {
            string retVal = string.Empty;
            try
            {
                if (streamRenderer == null)
                    return string.Empty;

                return streamRenderer.RenderMediaName;
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
            return retVal;
        }

        public void SetRenderFile(string file)
        {
            try
            {
                _hasRenderingErrors = false;

                // Select the proper renderer for the specified media
                Uri uri = null;
                try
                {
                    uri = new Uri(file, UriKind.Absolute);
                }
                catch
                {
                    uri = null;
                }

                if (uri != null && !uri.IsFile)
                {
                    if (uri.OriginalString.StartsWith("dzmedia:///track/"))
                    {
                        CreateNewRenderer<DeezerRenderer>();
                    }
                    else
                    {
                        using (ShoutcastStream ss = new ShoutcastStream(file, 4000, false))
                        {
                            if (ss.Connected)
                                CreateNewRenderer<DSShoutcastRenderer>();
                            else
                            {
                                // Media file exposed as URL
                                CreateNewRenderer<DSFileRenderer>();
                            }
                        }

                    }
                }
                else
                {
                    if (DvdMedia.FromPath(file) != null)
                    {
                        CreateNewRenderer<DSDvdRenderer>();
                    }
                    else
                    {
                        string streamType = PathUtils.GetExtension(file).ToLowerInvariant();
                        if (streamType == "cda")
                        {
                            CreateNewRenderer<DSAudioCDRenderer>();
                        }
                        else
                        {
                            CreateNewRenderer<DSFileRenderer>();
                        }
                    }
                }

                Logger.LogTrace("Now playing media: {0}", file);

                if (streamRenderer != null)
                { 
                    streamRenderer.RenderRegion = renderPanel;

                    if (this.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped)
                    {
                        streamRenderer.RenderMediaName = file;
                    }
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
        }

        private void CreateNewRenderer<T>() where T : StreamRenderer
        {
            if (streamRenderer as T == null)
            {
                // Cleanup old renderer
                if (streamRenderer != null)
                {
                    streamRenderer.Dispose();
                    streamRenderer = null;
                }

                // Set up new one
                streamRenderer = Activator.CreateInstance(typeof(T)) as StreamRenderer;
            }
        }

        public void StartRendererWithHint(RenderingStartHint startHint)
        {
            try
            {
                _hasRenderingErrors = false;
                _position = 0;
                StreamData = new Dictionary<string, string>();

                if (this.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused)
                {
                    double position = this.MediaPosition;
                    this.ResumeRenderer(position);
                }
                else if (streamRenderer != null)
                {
                    streamRenderer.StartRendererWithHint(startHint);
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
        }

        public void StartRenderer()
        {
            try
            {
                _hasRenderingErrors = false;
                _position = 0;
                StreamData = new Dictionary<string, string>();

                Logger.LogTrace("Media will be rendered using {0}", streamRenderer.GetType().Name);

                if (streamRenderer != null)
                { 
                    if (this.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped)
                    {
                        streamRenderer.StartRenderer();
                    }
                    else if (streamRenderer.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused)
                    {
                        double position = streamRenderer.MediaPosition;
                        streamRenderer.ResumeRenderer(position);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
        }

        public void PauseRenderer()
        {
            try
            {
                _hasRenderingErrors = false;

                if (streamRenderer != null &&
                    streamRenderer.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running)
                {
                    streamRenderer.PauseRenderer();
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
        }

        public void ResumeRenderer(double fromPosition)
        {
            try
            {
                _hasRenderingErrors = false;

                if (streamRenderer != null &&
                    streamRenderer.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused)
                {
                    _position = fromPosition;
                    streamRenderer.ResumeRenderer(fromPosition);
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }

        }

        bool _isStopFromGui = false;

        public void StopRenderer(bool isStopFromGui)
        {
            try
            {
                _hasRenderingErrors = false;
                _isStopFromGui = isStopFromGui;

                _position = 0;

                if (streamRenderer != null &&
                    (streamRenderer.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running ||
                    streamRenderer.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused))
                {
                    streamRenderer.StopRenderer();
                }
            }
            catch (Exception ex)
            {
                ReportRenderingException(ex);
            }
        }

        public string AvailableFileTypesFilter
        {
            get
            {
                return string.Format("{0}{1}{2}{3}",
                    AudioFilesFilter, VideoFilesFilter, VideoHDFilesFilter, PlaylistsFilter);
            }
        }

        public string AudioFilesFilter
        { get { return ConstructFilter("TXT_AUDIO_FILES", SupportedAudioTypes); } }

        public string VideoHDFilesFilter
        { get { return ConstructFilter("TXT_VIDEO_HD_FILES", SupportedHDVideoTypes); } }

        public string VideoFilesFilter
        { get { return ConstructFilter("TXT_VIDEO_FILES", SupportedVideoTypes); } }

        public string PlaylistsFilter
        { get { return ConstructFilter("TXT_PLAYLISTS", SupportedPlaylists); } }

        private string ConstructFilter(string tag, List<string> fileTypes)
        {
            string filterFormat = tag + " ({0})|{1}|";
            string filterPart1 = "";
            string filterPart2 = "";

            foreach (string fileType in fileTypes)
            {
                filterPart1 += "*." + fileType;
                filterPart1 += ",";

                filterPart2 += "*." + fileType;
                filterPart2 += ";";
            }

            filterPart1 = filterPart1.TrimEnd(new char[] { ',' });
            filterPart2 = filterPart2.TrimEnd(new char[] { ';' });
            
            return string.Format(filterFormat, filterPart1, filterPart2);
        }

        public VideoFileInfo QueryVideoMediaInfo(string path)
        {
            VideoFileInfo vfi = null;

            DvdMedia dvdDrive = DvdMedia.FromPath(path);
            if (dvdDrive != null)
            {
                vfi = dvdDrive.VideoDvdInformation;
            }
            else
            {
                vfi = new VideoFileInfo(path, false);

                try
                {
                    if (vfi != null && vfi.IsValid)
                    {
                        Guid filterGraphGuid = ProTONEConfig.FilterGraphGuid;
                        Type mediaControlType = Type.GetTypeFromCLSID(filterGraphGuid, true);

                        IMediaControl mediaControl = Activator.CreateInstance(mediaControlType) as IMediaControl;
                        IBasicAudio basicAudio = mediaControl as IBasicAudio;
                        IBasicVideo basicVideo = mediaControl as IBasicVideo;
                        IMediaPosition mediaPosition = mediaControl as IMediaPosition;

                        mediaControl.RenderFile(path);

                        double val = 0;
                        DsError.ThrowExceptionForHR(mediaPosition.get_Duration(out val));
                        vfi.Duration = TimeSpan.FromSeconds(val);

                        DsError.ThrowExceptionForHR(basicVideo.get_AvgTimePerFrame(out val));
                        vfi.FrameRate = new FrameRate(1f / val);

                        int h = 0, w = 0;
                        DsError.ThrowExceptionForHR(basicVideo.get_VideoHeight(out h));
                        DsError.ThrowExceptionForHR(basicVideo.get_VideoWidth(out w));
                        vfi.VideoSize = new VSize(w, h);

                        mediaControl.Stop();
                        mediaControl = null;
                        mediaPosition = null;
                        basicVideo = null;
                        basicAudio = null;

                        GC.Collect();
                    }
                }
                catch
                {
                }
            }

            return vfi;
        }

        public string GetStateDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("[DUR] : {0}\r\n", this.MediaLength);
            sb.AppendFormat("[POS] : {0}\r\n", this.MediaPosition);
            sb.AppendFormat("[MST] : {0}\r\n", this.FilterState);
            sb.AppendFormat("[FIL] : {0}\r\n", this.GetRenderFile());

            return sb.ToString();
        }

        public void AdjustVideoSize(VideoSizeAdjustmentDirection direction, VideoSizeAdjustmentAction action)
        {

            if (streamRenderer != null &&
                (this.RenderedMediaType == MediaTypes.Video ||
                this.RenderedMediaType == MediaTypes.Both) &&
                this.FilterState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running)
            {
                streamRenderer.AdjustVideoSize(direction, action);
            }
        }
        #endregion

        #region Construction
        private MediaRenderer(bool isDefaultInstance)
        {
            streamRenderer = null;

            SuiteRegistrationSupport.Init(GetSupportedFileProvider());

            timerCheckState = new Timer();
            timerCheckState.Enabled = true;
            timerCheckState.Interval = 500;
            timerCheckState.Start();
            timerCheckState.Tick += new EventHandler(timerCheckState_Tick);

            if (isDefaultInstance && 
                ProTONEConfig.IsPlayer && 
                ProTONEConfig.SignalAnalisysFunctionActive(SignalAnalisysFunction.WCFInterface))
            {
                InternalInitSignalAnalisysWCF();
            }

        }

        ~MediaRenderer()
        {
            if (timerCheckState != null)
            {
                timerCheckState.Dispose();
            }

            if (this == __defaultInstance)
            {
                if (ProTONEConfig.IsPlayer)
                {
                    SystemScheduler.Stop();
                    CleanupSignalAnalisysWCF();
                }

                __defaultInstance = null;
            }

            streamRenderer = null;
        }

        double oldMediaPosition = 0;
        int nofPasses = 0;

        double _prevTime = 0;
        void timerCheckState_Tick(object sender, EventArgs e)
        {
            double newMediaPosition = this.MediaPosition;

            double nowTime = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
            double diff = (nowTime - _prevTime);
            _prevTime = nowTime;

            FireMediaRendererClock();

            OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState newState = OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened;

            try
            {
                newState = this.FilterState;
                string newMedia = this.RenderMediaName;

                if (newState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running && oldMediaPosition == newMediaPosition)
                {
                    nofPasses++;
                    Logger.LogTrace("Media position did not change in the last {0} iterations ... old={1}, new={2}", 
                        nofPasses, oldMediaPosition, newMediaPosition);
                }
                else
                {
                    nofPasses = 0;
                    Logger.LogTrace("Media position changed ... old={0}, new={1}",
                        oldMediaPosition, newMediaPosition);

                    oldMediaPosition = newMediaPosition;
                }

                if (this.IsEndOfMedia || (!IsStreamedMedia && nofPasses > 10))
                {
                    if (newState != OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped)
                    {
                        Logger.LogTrace("timerCheckState_Tick ... IsEndOfMedia={0}, IsStreamedMedia={1}, nofPasses={2}, newState={3}", 
                            IsEndOfMedia, IsStreamedMedia, nofPasses, newState);

                        this.StopRenderer(false);
                        newState = OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened;
                    }
                    else
                    {
                        newState = OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped;
                    }
                    
                }
                else if (oldState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened && newState == OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped)
                {
                    newState = OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened;
                }

                if (newState != oldState || newMedia != oldMedia)
                {
                    FireFilterStateChanged(oldState, oldMedia, newState, newMedia);
                }

                switch (newState)
                {
                    case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running:
                        switch (oldState)
                        {
                            case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Running:
                                _position += diff;
                                break;

                            case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened:
                            case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped:
                                //break;

                            case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused:
                                _position = streamRenderer.MediaPosition;
                                break;
                        }
                        break;

                    case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped:
                    case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened:
                        _position = 0;
                        break;

                    case OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Paused:
                        break;
                }

                if (newState != OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.Stopped &&
                    newState != OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState.NotOpened)
                {
                    FireMediaRendererHeartbeat();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                oldState = newState;
                oldMedia = this.RenderMediaName;
            }
        }

        void ReportRenderingException(Exception ex)
        {
             _hasRenderingErrors = true;

            try
            {
                RenderingException rex = RenderingException.FromException(ex);
                rex.RenderedFile = this.GetRenderFile();

                RenderingExceptionEventArgs args = new RenderingExceptionEventArgs(rex);
                FireMediaRenderingException(args);

                if (args.Handled)
                    return;
            }
            catch
            {
            }

            throw ex;
        }

        #endregion

        public override bool Equals(object obj)
        {
            MediaRenderer mr = (obj as MediaRenderer);
            if (mr != null)
            {
                return (this._hash == mr._hash);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public static bool IsSupportedPlaylist(string path)
        {
            string ext = PathUtils.GetExtension(path);
            return MediaRenderer.SupportedPlaylists.Contains(ext);
        }


        public static bool IsSupportedMedia(string path)
        {
            try
            {
                if (PathUtils.IsRootPath(path))
                {
                    DvdMedia dvdMedia = DvdMedia.FromPath(path);
                    if (dvdMedia != null)
                        return true; // DVD's are supported media
                }

                if (Directory.Exists(path))
                    return FolderContainsMediaFiles(path);
            }
            catch
            {
            }

            string ext = PathUtils.GetExtension(path);
            return MediaRenderer.AllMediaTypes.Contains(ext);
        }

        public static bool FolderContainsMediaFiles(string path)
        {
            return FolderContainsMediaFiles(path, 0);
        }

        const int MaxRecursionLevel = 3;

        public static bool FolderContainsMediaFiles(string path, int level)
        {
            List<string> files = PathUtils.EnumFiles(path);
            foreach (string file in files)
            {
                if (IsSupportedMedia(file))
                {
                    return true;
                }
            }

            List<string> subfolders = PathUtils.EnumDirectories(path);
            foreach (string subfolder in subfolders)
            {
                if (level < (MaxRecursionLevel - 1) &&
                    FolderContainsMediaFiles(subfolder, level + 1))
                {
                    return true;
                }
            }

            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (streamRenderer != null)
            {
                streamRenderer.Dispose();
                streamRenderer = null;
            }
        }

        #endregion

        #region Published events
        public event MediaRendererEventHandler MediaRendererClock = null;
        private void FireMediaRendererClock()
        {
            if (MediaRendererClock != null)
            {
                MediaRendererClock();
            }
        }

        public event MediaRendererEventHandler MediaRendererHeartbeat = null;
        private void FireMediaRendererHeartbeat()
        {
            if (MediaRendererHeartbeat != null)
            {
                MediaRendererHeartbeat();
            }
        }

        public event FilterStateChangedHandler FilterStateChanged = null;
        private void FireFilterStateChanged(OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState oldState, string oldMedia,
            OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState newState, string newMedia)
        {
            if (FilterStateChanged != null)
            {
                FilterStateChanged(oldState, oldMedia, newState, newMedia);
            }
        }
        
        public event MediaRenderingExceptionHandler MediaRenderingException = null;
        private void FireMediaRenderingException(RenderingExceptionEventArgs args)
        {
            if (MediaRenderingException != null)
            {
                MediaRenderingException(args);
            }
        }
        
        #endregion

        #region FFdShow subtitle and OSD

        IntPtr _oldFfdShowHandle = IntPtr.Zero;

        public string CurrentSubtitleFile
        {
            get
            {
                using (FfdShowLib i = FfdShowInstance())
                {
                    return i.CurrentSubtitleFile;
                }
            }

            set
            {
                using (FfdShowLib i = FfdShowInstance())
                {
                    i.CurrentSubtitleFile = value;
                }
            }
        }

        public void DisplayOsdMessage(string msg)
        {
            if (ProTONEConfig.OsdEnabled)
            {
                using (FfdShowLib i = FfdShowInstance())
                {
                    int osdPersistTimer = ProTONEConfig.OsdPersistTimer;
                    float frameRate = i.getFrameRate();
                    int persistFrames = (int)(osdPersistTimer * frameRate / 1000);

                    // OsdPersistTimer is given in msec
                    i.clearOsd();
                    i.setOsdDuration(persistFrames);
                    i.displayOSDMessage(msg, true);
                }
            }
        }

        public void ReloadFfdShowSettings()
        {
            using (FfdShowLib i = FfdShowInstance())
            {
                EnforceSettings(i);
            }
        }

        private void EnforceSettings(FfdShowLib ffdShowLib)
        {
            // Subtitles
            ffdShowLib.DoShowSubtitles = ProTONEConfig.SubEnabled;

            if (ProTONEConfig.SubEnabled)
            {
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontColor,
                    ColorHelper.BGR(ProTONEConfig.SubColor));
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontSizeA,
                    ProTONEConfig.SubFont.Height);
                ffdShowLib.setStringParam(FFDShowConstants.FFDShowDataId.IDFF_fontName,
                    ProTONEConfig.SubFont.OriginalFontName);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontCharset,
                  ProTONEConfig.SubFont.GdiCharSet);

                LOGFONT lf = new LOGFONT();
                ProTONEConfig.SubFont.ToLogFont(lf);

                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontWeight,
                    lf.lfWeight);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontItalic,
                   lf.lfItalic);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_fontUnderline,
                   lf.lfUnderline);
            }

            if (ProTONEConfig.OsdEnabled)
            {
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontColor,
                    ColorHelper.BGR(ProTONEConfig.OsdColor));
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontSize,
                    ProTONEConfig.OsdFont.Height);
                ffdShowLib.setStringParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontName,
                    ProTONEConfig.OsdFont.OriginalFontName);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontCharset,
                    ProTONEConfig.OsdFont.GdiCharSet);

                LOGFONT lf = new LOGFONT();
                ProTONEConfig.OsdFont.ToLogFont(lf);

                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontWeight,
                    lf.lfWeight);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontItalic,
                   lf.lfItalic);
                ffdShowLib.setIntParam(FFDShowConstants.FFDShowDataId.IDFF_OSDfontUnderline,
                   lf.lfUnderline);
            }
        }

        private FfdShowLib FfdShowInstance()
        {
            string renderFile = this.GetRenderFile();
            FfdShowLib ffdShowLib = new FfdShowLib(renderFile);

            if (ffdShowLib.checkFFDShowActive())
            {
                if (ffdShowLib.FFDShowInstanceHandle != _oldFfdShowHandle)
                {
                    // API re-created so enforce parameters
                    EnforceSettings(ffdShowLib);

                    _oldFfdShowHandle = ffdShowLib.FFDShowInstanceHandle;
                }
            }

            return ffdShowLib;
        }

        #endregion

        #region Signal Analisys WCF Interface
        ServiceHost _wcfHost = null;

        public void InitSignalAnalisysWCF()
        {
            if (__defaultInstance != this)
                throw new InvalidOperationException("InitSignalAnalisysWCF can only be done on MediaRenderer default instance !");

            InternalInitSignalAnalisysWCF();
        }

        public void CleanupSignalAnalisysWCF()
        {
            if (__defaultInstance != this)
                throw new InvalidOperationException("CleanupSignalAnalisysWCF can only be done on MediaRenderer default instance !");

            InternalCleanupSignalAnalisysWCF();
        }

        private void InternalInitSignalAnalisysWCF()
        {
            try
            {
                Logger.LogInfo("Opening Signal Analisys WCF Interface ...");
                if (_wcfHost == null)
                {
                    string address = "http://localhost/ProTONESignalAnalisys.svc";

                    var binding = new WSHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

                    _wcfHost = new ServiceHost(typeof(ProTONESignalAnalisys));
                    _wcfHost.AddServiceEndpoint(typeof(ISignalAnalisys), binding, address);

                    _wcfHost.Open();

                    Logger.LogInfo("Signal Analisys WCF Interface opened succesfully.");
                }
                else
                {
                    Logger.LogInfo("Signal Analisys WCF Interface was already open.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void InternalCleanupSignalAnalisysWCF()
        {
            try
            {
                Logger.LogInfo("Closing Signal Analisys WCF Interface ...");

                if (_wcfHost != null)
                {
                    _wcfHost.Close();
                    _wcfHost = null;

                    Logger.LogInfo("Signal Analisys WCF Interface closed succesfully.");
                }
                else
                {
                    Logger.LogInfo("Signal Analisys WCF Interface was already closed.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

        }

        #endregion

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
        public class ProTONESignalAnalisys : ISignalAnalisys
        {
            public SignalAnalisysData GetSignalAnalisysData()
            {
                SignalAnalisysData data = new SignalAnalisysData();
                data.MomentarySampleData = MediaRenderer.DefaultInstance.VuMeterData;
                data.SpectrogramData = MediaRenderer.DefaultInstance.SpectrogramData;
                return data;
            }
        }
    }

   

    public abstract class RenderingStartHint
    {
        public abstract bool IsSubtitleHint { get; }
    }
}

#region ChangeLog
#region Date: 01.08.2006			Author: octavian
// File created.
#endregion
#endregion