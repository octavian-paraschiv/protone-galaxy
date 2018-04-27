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
using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Runtime.ProTONE.ExtendedInfo;
#endregion

namespace OPMedia.Runtime.ProTONE.Rendering.Base
{
    public abstract class StreamRenderer : IDisposable
    {
        #region Members
        protected Control renderRegion = null;
        protected Control messageDrain = null;
        protected string renderMediaName = string.Empty;
        protected List<string> supportedMediaTypes = null;

        protected MediaFileInfo renderMediaInfo = MediaFileInfo.Empty;

        protected object _sync = new object();
        #endregion

        protected object _vuLock = new object();
        protected object _waveformLock = new object();
        protected object _spectrogramLock = new object();
        protected AudioSampleData _vuMeterData = null;
        protected double[][] _waveformData = null;
        protected double[] _spectrogramData = null;

        protected int _waveformWindowSize = 512;
        protected int _vuMeterWindowSize = 64;
        protected int _fftWindowSize = 2048;
        protected double _maxLevel = short.MaxValue;
        protected double _maxLogLevel = Math.Log((double)short.MaxValue);

        protected System.Timers.Timer _tmrInternalClock = null;
        private Int64 _elapsedSeconds = 0;
        protected object _syncElapsedSeconds = new object();


        #region Properties

        public Int64 ElapsedSeconds
        {
            get
            {
                lock (_syncElapsedSeconds)
                {
                    return _elapsedSeconds;
                }
            }
        }

        internal Control RenderRegion
        { get { return renderRegion; } set { renderRegion = value; } }

        internal bool MediaSeekable
        { 
            get 
            {
                return IsMediaSeekable(); 
            } 
        }

        internal double DurationScaleFactor
        {
            get
            {
                return GetDurationScaleFactor();
            }
        }


        internal double MediaLength
        { 
            get 
            {
                return GetMediaLength(); 
            } 
        }

        internal double MediaPosition
        { 
            get 
            {
                return DoGetMediaPosition();
                //return GetMediaPosition(); 
            } 
            set 
            {
                SetMediaPosition(value); 
            } 
        }

        protected virtual double DoGetMediaPosition()
        {
            return _elapsedSeconds;
        }

        internal bool AudioMediaAvailable
        { 
            get 
            {
                return IsAudioMediaAvailable(); 
            } 
        }

        internal bool VideoMediaAvailable
        {
            get
            {
                return IsVideoMediaAvailable();
            }
        }

        internal OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState FilterState
        { 
            get 
            {
                return GetFilterState(); 
            } 
        }

        internal int AudioVolume
        {
            get 
            {
                return GetAudioVolume(); 
            } 
            set 
            {
                int scaledVolume = GetScaledVolume(value);
                SetAudioVolume(scaledVolume); 
            } 
        }

        internal int AudioBalance
        {
            get
            {
                return GetAudioBalance();
            }
            set
            {
                SetAudioBalance(value);
            }
        }

        internal Control MessageDrain
        {
            get { return messageDrain; }
            set { messageDrain = value; }
        }

        internal bool ShowCursor
        {
            get { return IsCursorVisible(); }
            set { DoShowCursor(value); }
        }

        internal bool FullScreen
        {
            get { return IsFullScreen(); }
            set { DoSetFullScreen(value); }
        }

        internal object GraphFilter
        { get { return DoGetGraphFilter(); } }

        public WaveFormatEx ActualAudioFormat
        {
            get
            {
                return DoGetActualAudioFormat();
            }
        }

        public AudioSampleData VuMeterData
        {
            get
            {
                lock (_vuLock)
                {
                    return _vuMeterData;
                }
            }
        }

        public double[][] WaveformData
        {
            get
            {
                lock (_waveformLock)
                {
                    return _waveformData;
                }
            }
        }

        public double[] SpectrogramData
        {
            get
            {
                lock (_spectrogramLock)
                {
                    return _spectrogramData;
                }
            }
        }

        public double MaxLevel
        {
            get
            {
                return _maxLevel;
            }
        }

        public double FFTWindowSize
        {
            get
            {
                return _fftWindowSize;
            }
        }

        #endregion

        #region Methods
        internal string RenderMediaName
        {
            get
            {
                return renderMediaName;
            }

            set
            {
                renderMediaName = value;
            }
        }


        internal MediaFileInfo RenderMediaInfo
        {
            get
            {
                return renderMediaInfo;
            }
        }

        internal void StartRendererWithHint(RenderingStartHint startHint)
        {
            int hintPos = 0;
            if (startHint is BookmarkStartHint)
                hintPos = (int)(startHint as BookmarkStartHint).Bookmark.PlaybackTimeInSeconds;
                
            CreateAndStartInternalClock(hintPos); 
            DoStartRendererWithHint(startHint);
        }

        internal void StartRenderer()
        {
            CreateAndStartInternalClock();
            DoStartRenderer();
        }

        internal void PauseRenderer()
        {
            PauseInternalClock();
            DoPauseRenderer();
        }

        internal void StopRenderer()
        {
            StopInternalClock();
            DoStopRenderer();
        }

        internal void ResumeRenderer(double fromPosition)
        {
            ResumeInternalClock((int)fromPosition);
            DoResumeRenderer(fromPosition);
        }

        private void CreateAndStartInternalClock(int elapsedSeconds = 0)
        {
            lock (_syncElapsedSeconds)
            {
                _elapsedSeconds = elapsedSeconds;
            }

            if (_tmrInternalClock == null)
            {
                _tmrInternalClock = new System.Timers.Timer();
                _tmrInternalClock.AutoReset = true;
                _tmrInternalClock.Interval = 1000;
                _tmrInternalClock.Elapsed += new System.Timers.ElapsedEventHandler(OnInternalClock);
            }

            _tmrInternalClock.Stop();
            _tmrInternalClock.Start();
        }

        void OnInternalClock(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_syncElapsedSeconds)
            {
                _elapsedSeconds++;
            }
        }

        private void ResumeInternalClock(int newPosition)
        {
            if (_tmrInternalClock != null)
                _tmrInternalClock.Start();

            lock (_syncElapsedSeconds)
            {
                _elapsedSeconds = newPosition;
            }
        }

        private void PauseInternalClock()
        {
            if (_tmrInternalClock != null)
                _tmrInternalClock.Stop();
        }

        private void StopInternalClock()
        {
            if (_tmrInternalClock != null)
                _tmrInternalClock.Stop(); 

            lock (_syncElapsedSeconds)
            {
                _elapsedSeconds = 0;
            }
        }

        internal void AdjustVideoSize(VideoSizeAdjustmentDirection direction, VideoSizeAdjustmentAction action)
        {
            DoAdjustVideoSize(direction, action);
        }

        internal bool EndOfMedia
        {
            get
            {
                bool ret = IsEndOfMedia();
                
                if (ret)
                    StopInternalClock();
                
                return ret;
            }
        }

        internal int GetSubtitleStream()
        {
            return DoGetSubtitleStream();
        }

        internal void SetSubtitleStream(int sid)
        {
            DoSetSubtitleStream(sid);
        }

        #endregion

        #region Construction
        public StreamRenderer()
        {
            //ResetVolumeLevels();
        }
        #endregion

        #region Implementation

        //protected void ResetVolumeLevels()
        //{
        //    VolumeAvgL = 0;
        //    VolumeAvgR = 0;
        //    VolumePeakL = 0;
        //    VolumePeakR = 0;
        //}

        #region IDisposable Members
        public void Dispose()
        {
            DoDispose();
        }

        protected virtual void DoDispose()
        {
            DoStopRenderer();
        }

        #endregion

        protected abstract void DoStartRenderer();
        protected abstract void DoStartRendererWithHint(RenderingStartHint startHint);

        protected abstract void DoPauseRenderer();
        protected abstract void DoStopRenderer();
        protected abstract void DoResumeRenderer(double fromPosition);

        protected abstract double GetMediaLength();
        protected abstract double GetMediaPosition();
        protected abstract double GetDurationScaleFactor();

        protected abstract void SetMediaPosition(double pos);

        protected abstract int GetAudioVolume();
        protected abstract void SetAudioVolume(int b);

        protected abstract int GetAudioBalance();
        protected abstract void SetAudioBalance(int b);

        protected abstract bool IsAudioMediaAvailable();
        protected abstract bool IsVideoMediaAvailable();
        protected abstract bool IsMediaSeekable();

        protected abstract OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses.FilterState GetFilterState();

        protected abstract bool IsCursorVisible();
        protected abstract void DoShowCursor(bool show);

        protected abstract void DoAdjustVideoSize(VideoSizeAdjustmentDirection direction, VideoSizeAdjustmentAction action);

        protected abstract bool IsEndOfMedia();

        protected abstract int DoGetSubtitleStream();
        protected abstract void DoSetSubtitleStream(int sid);

        protected abstract bool IsFullScreen();
        protected abstract void DoSetFullScreen(bool fullScreen);

        protected abstract object DoGetGraphFilter();

        protected abstract WaveFormatEx DoGetActualAudioFormat();

        #endregion

        protected virtual int GetScaledVolume(int rawVolume)
        {
            return rawVolume;
        }
    }
}

