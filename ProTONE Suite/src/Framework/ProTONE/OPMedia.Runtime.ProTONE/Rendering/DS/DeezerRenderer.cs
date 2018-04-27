using Newtonsoft.Json.Linq;
using OPMedia.Core;
using OPMedia.Core.Configuration;
using OPMedia.DeezerInterop.PlayerApi;
using OPMedia.Runtime.ProTONE.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Core.Logging;
using System.Diagnostics;
using OPMedia.Runtime.Shortcuts;

namespace OPMedia.Runtime.ProTONE.Rendering.DS
{
    public class DeezerRenderer : DsCustomRenderer
    {
        dz_connect_configuration _dzConfig = null;

        IntPtr _dzConnect = IntPtr.Zero;
        IntPtr _dzPlayer = IntPtr.Zero;

        dz_player_onrenderprogress_cb _dzRenderProgressCB = null;
        dz_player_onevent_cb _dzPlayerEventCB = null;
        dz_player_onrendererevent_cb _dzRendererEventCB = null;

        dz_activity_operation_callback _dzPlayerDeactivatedCB = null;
        dz_activity_operation_callback _dzConnectDeactivatedCB = null;
        
        bool _connected = false;

        const string USER_CACHE_PATH = "c:\\dzr\\dzrcache_NDK_SAMPLE";
        
        const int DZ_SESSION_TIMEOUT = 60000;
        const int DZ_OPERATION_TIMEOUT = 10000;

        protected override void DoDispose()
        {
            Logger.LogTrace("DeezerRenderer::~DoDispose => Cleanup ...");
            DoStopRenderer();

            _connected = false;

            CleanupAppContext();
            CleanupConfig();
        }
        
        private void CheckIfInitialized()
        {
            if (_connected == false)
            {
                Logger.LogTrace("DeezerRenderer::CheckIfInitialized => Initializing ...");
                SetupConfig();
                SetupAppContext();
            }
        }

        private void SetupConfig()
        {
            if (_dzConfig == null)
            {
                Logger.LogTrace("DeezerRenderer::SetupConfig => Creating config object ...");

                _dzConfig = new dz_connect_configuration();
                _dzConfig.app_id = ProTONEConfig.DeezerApplicationId;
                _dzConfig.product_id = ApplicationInfo.ApplicationName;
                _dzConfig.product_build_id = SuiteVersion.Version;
                _dzConfig.user_profile_path = USER_CACHE_PATH;          // TODO SET THE USER CACHE PATH
                _dzConfig.connect_event_cb = new dz_connect_onevent_cb(OnApplicationConnectEvent);
                _dzConfig.app_has_crashed_delegate = new dz_connect_crash_reporting_delegate(OnAppCrashed);
            }
        }

        private bool OnAppCrashed()
        {
            Logger.LogTrace("DeezerRenderer::OnAppCrashed called ...");
            return false;
        }

        private void SetupAppContext()
        {
            if (_connected == false)
            {
                _evtAppUserOfflineAvailable.Reset();
                _evtAppUserLoginOK.Reset();

                Logger.LogTrace("DeezerRenderer::SetupConfig => Creating app context ...");

                dz_error_t err;

                _dzRenderProgressCB = new dz_player_onrenderprogress_cb(OnRenderProgress);
                _dzPlayerEventCB = new dz_player_onevent_cb(OnPlayerEvent);
                _dzRendererEventCB = new dz_player_onrendererevent_cb(OnRendererEvent);

                _dzConnect = DeezerApi.dz_connect_new(_dzConfig);
                if (_dzConnect == IntPtr.Zero)
                    DeezerApi.HandleDzErrorCode("dz_connect_new", dz_error_t.DZ_ERROR_CLASS_INSTANTIATION);

                //err = DeezerApi.dz_connect_debug_log_disable(_dzConnect);
                //DeezerApi.HandleDzErrorCode("dz_connect_debug_log_disable", err);

                err = DeezerApi.dz_connect_activate(_dzConnect, IntPtr.Zero);
                DeezerApi.HandleDzErrorCode("dz_connect_activate", err);

                err = DeezerApi.dz_connect_cache_path_set(_dzConnect, null, IntPtr.Zero, USER_CACHE_PATH);
                DeezerApi.HandleDzErrorCode("dz_connect_cache_path_set", err);

                _dzPlayer = DeezerApi.dz_player_new(_dzConnect);
                if (_dzPlayer == IntPtr.Zero)
                    DeezerApi.HandleDzErrorCode("dz_player_new", dz_error_t.DZ_ERROR_CLASS_INSTANTIATION);

                err = DeezerApi.dz_player_activate(_dzPlayer, IntPtr.Zero);
                DeezerApi.HandleDzErrorCode("dz_player_activate", err);

                err = DeezerApi.dz_player_set_event_cb(_dzPlayer, _dzPlayerEventCB);
                DeezerApi.HandleDzErrorCode("dz_player_set_event_cb", err);

                err = DeezerApi.dz_player_set_render_progress_cb(_dzPlayer, _dzRenderProgressCB, (UInt64)5e5);
                DeezerApi.HandleDzErrorCode("dz_player_set_render_progress_cb", err);

                err = DeezerApi.dz_player_set_renderer_event_cb(_dzPlayer, _dzRendererEventCB);
                DeezerApi.HandleDzErrorCode("dz_player_set_renderer_event_cb", err);

                string token = ProTONEConfig.DeezerUserAccessToken;
                err = DeezerApi.dz_connect_set_access_token(_dzConnect, null, IntPtr.Zero, token);
                DeezerApi.HandleDzErrorCode("dz_connect_set_access_token", err);

                err = DeezerApi.dz_connect_offline_mode(_dzConnect, null, IntPtr.Zero, false);
                DeezerApi.HandleDzErrorCode("dz_connect_offline_mode", err);

                if (_evtAppUserLoginOK.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                    DeezerApi.HandleDzErrorCode("DeezerRenderer::SetupConfig", dz_error_t.DZ_ERROR_CONNECT_SESSION_LOGIN_FAILED);

                _connected = true;
            }
        }

        private void CleanupAppContext()
        {
            Logger.LogTrace("DeezerRenderer::CleanupAppContext => Cleaning up app context ...");

            dz_error_t err;

            if (_dzPlayer != IntPtr.Zero)
            {
                //_evtPlayerDeactivated.Reset();
                //_dzPlayerDeactivatedCB = new dz_activity_operation_callback(OnPlayerDeactivated);

                err = DeezerApi.dz_player_deactivate(_dzPlayer, _dzPlayerDeactivatedCB, IntPtr.Zero);

                DeezerApi.HandleDzErrorCode("dz_player_deactivate", err, false);

                //_evtPlayerDeactivated.WaitOne(DZ_OPERATION_TIMEOUT);
                Thread.Sleep(2000);

                Logger.LogTrace("dz_player_deactivate => Assuming Success");

                DeezerApi.dz_object_release(_dzPlayer);
                _dzPlayer = IntPtr.Zero;
            }

            if (_dzConnect != IntPtr.Zero)
            {
                //_evtConnectDeactivated.Reset();
                //_dzConnectDeactivatedCB = new dz_activity_operation_callback(OnConnectDeactivated);

                err = DeezerApi.dz_connect_deactivate(_dzConnect, _dzConnectDeactivatedCB, IntPtr.Zero);

                DeezerApi.HandleDzErrorCode("dz_connect_deactivate", err, false);

                //_evtConnectDeactivated.WaitOne(DZ_OPERATION_TIMEOUT);
                Thread.Sleep(2000);

                Logger.LogTrace("dz_connect_deactivate => Assuming Success");

                DeezerApi.dz_object_release(_dzConnect);
                _dzConnect = IntPtr.Zero;
            }
        }

        private void CleanupConfig()
        {
            if (_dzConfig != null)
            {
                Logger.LogTrace("DeezerRenderer::CleanupConfig => Cleaning up config ...");
                _dzConfig = null;
            }
        }

        public void OnConnectDeactivated(IntPtr userData, IntPtr operation_userdata, dz_error_t status, IntPtr result)
        {
            Logger.LogTrace("DeezerRenderer::OnConnectDeactivated ...");
            _evtConnectDeactivated.Set();
        }

        public void OnPlayerDeactivated(IntPtr userData, IntPtr operation_userdata, dz_error_t status, IntPtr result)
        {
            Logger.LogTrace("DeezerRenderer::OnPlayerDeactivated ...");
            _evtPlayerDeactivated.Set();
        }


        protected override void DoStartRendererWithHint(RenderingStartHint startHint)
        {
            dz_error_t err;

            _needNaturalNext = false;

            Logger.LogTrace("DeezerRenderer::DoStartRendererWithHint startHint={0}", startHint);

            if (renderMediaName == null || renderMediaName.Length <= 0)
                return;

            CheckIfInitialized();

            if (_dzConnect == IntPtr.Zero || _dzPlayer == IntPtr.Zero)
                DeezerApi.HandleDzErrorCode("DeezerRenderer::DoStartRendererWithHint", dz_error_t.DZ_ERROR_CLASS_INSTANTIATION);

            // --------------------------------------------------------------------
            _evtQueueListLoaded.Reset();
            err = DeezerApi.dz_player_load(_dzPlayer, null, IntPtr.Zero, renderMediaName);
            DeezerApi.HandleDzErrorCode("dz_player_load", err);

            if (_evtQueueListLoaded.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                DeezerApi.HandleDzErrorCode("dz_player_load", dz_error_t.DZ_ERROR_PLAYER_LOAD_TIMEOUT);
            
            Logger.LogTrace("dz_player_load => Success");
                       
            // --------------------------------------------------------------------

            // --------------------------------------------------------------------
            // Start playback using dz_player_play
            // This will trigger DZ_PLAYER_EVENT_RENDER_TRACK_START
            // Upon completion, _evtPlayerPlaybackStarted will be set.
            _evtPlayerPlaybackStarted.Reset();

            err = DeezerApi.dz_player_play(_dzPlayer, null, IntPtr.Zero,
                       dz_player_play_command_t.DZ_PLAYER_PLAY_CMD_START_TRACKLIST,
                       DeezerInterop.PlayerApi.Constants.DZ_INDEX_IN_QUEUELIST_CURRENT);

            DeezerApi.HandleDzErrorCode("dz_player_play", err);

            if (_evtPlayerPlaybackStarted.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                DeezerApi.HandleDzErrorCode("dz_player_play", dz_error_t.DZ_ERROR_PLAYER_PLAY_TIMEOUT);
            
            Logger.LogTrace("dz_player_play => Success");
            // --------------------------------------------------------------------
           
        }

        ManualResetEvent _evtAppUserOfflineAvailable = new ManualResetEvent(false);
        ManualResetEvent _evtAppUserLoginOK = new ManualResetEvent(false);

        ManualResetEvent _evtQueueListLoaded = new ManualResetEvent(false);
        ManualResetEvent _evtPlayerPaused = new ManualResetEvent(false);
        ManualResetEvent _evtPlayerStreamReadyAfterSeek = new ManualResetEvent(false);

        ManualResetEvent _evtPlayerPlaybackStarted = new ManualResetEvent(false);
        ManualResetEvent _evtPlayerPlaybackStopped = new ManualResetEvent(false);

        ManualResetEvent _evtPlayerDeactivated = new ManualResetEvent(false);
        ManualResetEvent _evtConnectDeactivated = new ManualResetEvent(false);

        //private void ResetAllEvents()
        //{
        //    _evtAppUserOfflineAvailable.Reset();
        //    _evtAppUserLoginOK.Reset();
        //    _evtPlayerPaused.Reset();
        //    _evtPlayerStreamReadyAfterSeek.Reset();
        //    _evtQueueListLoaded.Reset();
        //    _evtPlayerPlaybackStarted.Reset();
        //}

        protected override void DoStopRenderer()
        {
            StackTrace st = new StackTrace();
            Logger.LogTrace("DeezerRenderer::DoStopRenderer call Stack = {0}", st.ToString());

            if (FilterState != FilterState.Stopped)
            {
                if (_dzPlayer != IntPtr.Zero)
                {
                    dz_error_t err;

                    _evtPlayerPlaybackStopped.Reset();

                    err = DeezerApi.dz_player_stop(_dzPlayer, null, IntPtr.Zero);
                    DeezerApi.HandleDzErrorCode("dz_player_stop", err);

                    if (_evtPlayerPlaybackStopped.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                        DeezerApi.HandleDzErrorCode("dz_player_pause", dz_error_t.DZ_ERROR_PLAYER_STOP_TIMEOUT);

                    RenderPosition = 0;

                    Logger.LogTrace("dz_player_stop => Success");
                }
            }
        }

        protected override void DoPauseRenderer()
        {
            Logger.LogTrace("DeezerRenderer::DoPauseRenderer");

            if (FilterState == FilterState.Running)
            {
                if (_dzPlayer != IntPtr.Zero)
                {
                    dz_error_t err;

                    _evtPlayerPaused.Reset();

                    err = DeezerApi.dz_player_pause(_dzPlayer, null, IntPtr.Zero);
                    DeezerApi.HandleDzErrorCode("dz_player_pause", err);

                    if (_evtPlayerPaused.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                        DeezerApi.HandleDzErrorCode("dz_player_pause", dz_error_t.DZ_ERROR_PLAYER_PAUSE_TIMEOUT);
                    
                    Logger.LogTrace("dz_player_pause => Success");
                }
            }
        }

        protected override void DoResumeRenderer(double fromPosition)
        {
            Logger.LogTrace("DeezerRenderer::DoResumeRenderer fromPosition={0}", fromPosition);

            dz_error_t err;

            if (_evtPlayerPaused.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                DeezerApi.HandleDzErrorCode("DeezerRenderer::DoResumeRenderer", dz_error_t.DZ_ERROR_PLAYER_PAUSE_NOT_STARTED);
            
            Logger.LogTrace("DeezerRenderer::DoResumeRenderer player is now paused.");

            if (FilterState == FilterState.Paused)
            {
                if (_dzPlayer != IntPtr.Zero)
                {
                    int resumePos = (int)fromPosition;
                    if (resumePos != RenderPosition)
                    {
                        // dz_player_seek will trigger DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY_AFTER_SEEK
                        // Upon completion, _evtPlayerStreamReadyAfterSeek will be set.
                        _evtPlayerStreamReadyAfterSeek.Reset();
                        err = DeezerApi.dz_player_seek(_dzPlayer, null, IntPtr.Zero, (UInt64)(resumePos * 1e6));
                        DeezerApi.HandleDzErrorCode("dz_player_seek", err);

                        if (_evtPlayerStreamReadyAfterSeek.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                            DeezerApi.HandleDzErrorCode("dz_player_seek", dz_error_t.DZ_ERROR_PLAYER_SEEK_TIMEOUT);
                        
                        Logger.LogTrace("dz_player_seek => DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY_AFTER_SEEK");
                    }

                    err = DeezerApi.dz_player_resume(_dzPlayer, null, IntPtr.Zero);
                    DeezerApi.HandleDzErrorCode("dz_player_resume", err);
                }
            }
        }

        protected override void SetMediaPosition(double pos)
        {
            Logger.LogTrace("DeezerRenderer::SetMediaPosition pos={0}", pos);

            if (FilterState != FilterState.Stopped)
            {
                if (_dzPlayer != IntPtr.Zero)
                {
                    dz_error_t err;

                    int resumePos = (int)pos;
                    if (resumePos != RenderPosition)
                    {
                        err = DeezerApi.dz_player_seek(_dzPlayer, null, IntPtr.Zero, (UInt64)(resumePos * 1e6));
                        DeezerApi.HandleDzErrorCode("dz_player_seek", err);
                    }

                    if (_evtPlayerPaused.WaitOne(DZ_OPERATION_TIMEOUT) == false)
                        DeezerApi.HandleDzErrorCode("[Event toggled by dz_player_seek]", dz_error_t.DZ_ERROR_PLAYER_SEEK_TIMEOUT);
                    
                    Logger.LogTrace("[Event toggled by dz_player_seek] => DZ_PLAYER_EVENT_RENDER_TRACK_PAUSED");
                }
            }
        }

        protected override bool IsMediaSeekable()
        {
            return true;
        }

        int _volume = -5000;


        protected override int GetAudioVolume()
        {
            return _volume;
        }

        protected override void SetAudioVolume(int vol)
        {
            if (_dzPlayer != IntPtr.Zero)
            {
                dz_error_t err;

                Logger.LogTrace("DeezerRenderer::SetAudioVolume vol={0}", vol);

                err = DeezerApi.dz_player_set_output_volume(_dzPlayer, null, IntPtr.Zero, vol);

                DeezerApi.HandleDzErrorCode("dz_player_set_output_volume", err);
            }
        }


        private bool OnApplicationError()
        {
            return true;
        }

        private void OnApplicationConnectEvent(IntPtr handle, IntPtr evtHandle, IntPtr userData)
        {
            dz_connect_event_t evtType = DeezerApi.dz_connect_event_get_type(evtHandle);
            Logger.LogTrace("DeezerRenderer::OnApplicationConnectEvent evtType={0}", evtType);

            switch (evtType)
            {
                case dz_connect_event_t.DZ_CONNECT_EVENT_USER_OFFLINE_AVAILABLE:
                    _evtAppUserOfflineAvailable.Set();
                    break;

                case dz_connect_event_t.DZ_CONNECT_EVENT_USER_LOGIN_OK:
                    _evtAppUserLoginOK.Set();
                    break;
            }
        }

        #region Duration and related

        int _duration = 0;
        bool _needNaturalNext = false;

        private void OnPlayerEvent(IntPtr handle, IntPtr evtHandle, IntPtr userdata)
        {
            dz_player_event_t evtType = DeezerApi.dz_player_event_get_type(evtHandle);
            Logger.LogTrace("DeezerRenderer::OnPlayerEvent evtType={0}", evtType);

            switch (evtType)
            {
                case dz_player_event_t.DZ_PLAYER_EVENT_QUEUELIST_TRACK_SELECTED:
                    {
                        string selectedInfo = DeezerApi.dz_player_event_track_selected_dzapiinfo(evtHandle);
                        dynamic obj2 = JObject.Parse(selectedInfo);
                        _duration = obj2.duration;
                    }
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_RENDER_TRACK_START:
                    _evtPlayerPlaybackStarted.Set();
                    FilterState = FilterState.Running;
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_RENDER_TRACK_RESUMED:
                    FilterState = FilterState.Running;
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_RENDER_TRACK_PAUSED:
                    _evtPlayerPaused.Set();
                    FilterState = FilterState.Paused;
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_RENDER_TRACK_END:
                case dz_player_event_t.DZ_PLAYER_EVENT_RENDER_TRACK_REMOVED:
                    if (_needNaturalNext)
                    {
                        // HACK: The Deezer Native SDK told us that we should advance 
                        // to the next item by using a "natural next" (WTF is that anyways ??)
                        _needNaturalNext = false;
                        // So this is exactly what we'll give it ...
                        OPMShortcutEventArgs args = new OPMShortcutEventArgs(OPMShortcut.CmdNext);
                        EventDispatch.DispatchEvent(EventNames.ExecuteShortcut, args);
                    }
                    else
                    {
                        _evtPlayerPlaybackStopped.Set();
                        FilterState = FilterState.Stopped;
                        RenderPosition = 0;
                    }
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY_AFTER_SEEK:
                    _evtPlayerStreamReadyAfterSeek.Set();
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_QUEUELIST_NEED_NATURAL_NEXT:
                    _needNaturalNext = true;
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_QUEUELIST_LOADED:
                    _evtQueueListLoaded.Set();
                    break;

                case dz_player_event_t.DZ_PLAYER_EVENT_QUEUELIST_NO_RIGHT:
                    _evtQueueListLoaded.Reset();
                    break;
            }
        }

        protected override double GetMediaLength()
        {
            return _duration;
        }

        protected override bool IsEndOfMedia()
        {
            long nMediaPos = (long)GetMediaPosition();
            long nMediaLen = (long)GetMediaLength();
            long nElapsed = base.ElapsedSeconds;

            return (nElapsed >= nMediaLen || nMediaPos >= nMediaLen);
        }
        #endregion

        #region Media position and related

        #region RenderPosition
        int _renderPosition = 0;
        object _renderPosLock = new object();

        private int RenderPosition
        {
            get
            {
                lock (_renderPosLock)
                {
                    return _renderPosition;
                }
            }

            set
            {
                lock (_renderPosLock)
                {
                    _renderPosition = value;
                };
            }
        }
        #endregion

        #region FilterState

        FilterState _filterState = FilterState.Stopped;
        object _fsLock = new object();

        private FilterState FilterState
        {
            get
            {
                lock (_fsLock)
                {
                    return _filterState;
                }
            }

            set
            {
                lock (_fsLock)
                {
                    _filterState = value;
                };
            }
        }
        #endregion

        private void OnRendererEvent(IntPtr handle, IntPtr evtHandle, IntPtr userdata)
        {
            string s = DeezerApi.dz_renderer_event_get_infos(evtHandle);
            Logger.LogTrace("DeezerRenderer::OnRendererEvent info={0}", s ?? "<null>");
        }

        private void OnRenderProgress(IntPtr handle, UInt64 progress, IntPtr userdata)
        {
            int curRenderPos = (int)(progress / 1e6);

            if (curRenderPos != RenderPosition && curRenderPos > 0)
            {
                Logger.LogTrace("DeezerRenderer::OnRenderProgress curRenderPos={0}", curRenderPos);
                RenderPosition = curRenderPos;
                FilterState = FilterState.Running;
            }
        }

        protected override double GetMediaPosition()
        {
            return RenderPosition;
        }

        protected override double DoGetMediaPosition()
        {
            return RenderPosition;
        }

        #endregion

        protected override bool IsAudioMediaAvailable()
        {
            return true;
        }

        protected override FilterState GetFilterState()
        {
            return FilterState;
        }

        protected override int GetScaledVolume(int rawVolume)
        {
            int dz_vol = (int)(0.01 * rawVolume);
            return dz_vol;
        }
    }
}
