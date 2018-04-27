using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

// Ported from original header file deezer-player.h

namespace OPMedia.DeezerInterop.PlayerApi
{
    #region Enums
    // Player events.
    public enum dz_player_event_t
    {
        DZ_PLAYER_EVENT_UNKNOWN,                             /* Player event has not been set yet, not a valid value. */

        // Data access related event.
        DZ_PLAYER_EVENT_LIMITATION_FORCED_PAUSE,             /* Another deezer player session was created elsewhere, the player has entered pause mode. */

        // Track selection related event.
        DZ_PLAYER_EVENT_QUEUELIST_LOADED,                    /* Content has been loaded. */
        DZ_PLAYER_EVENT_QUEUELIST_NO_RIGHT,                  /* You don't have the right to play this content: track, album or playlist */
        DZ_PLAYER_EVENT_QUEUELIST_TRACK_NOT_AVAILABLE_OFFLINE,/* You're offline, the track is not available. */
        DZ_PLAYER_EVENT_QUEUELIST_TRACK_RIGHTS_AFTER_AUDIOADS,/* You have right to play it, but you should render an ads first :
                                                                  - Use dz_player_event_get_advertisement_infos_json().
                                                                  - Play an ad with dz_player_play_audioads().
                                                                  - Wait for #DZ_PLAYER_EVENT_RENDER_TRACK_END.
                                                                  - Use dz_player_play() with previous track or DZ_PLAYER_PLAY_CMD_RESUMED_AFTER_ADS (to be done even on mixes for now).
                                                              */
        DZ_PLAYER_EVENT_QUEUELIST_SKIP_NO_RIGHT,              /* You're on a mix, and you had no right to do skip. */

        DZ_PLAYER_EVENT_QUEUELIST_TRACK_SELECTED,             /* A track is selected among the ones available on the server, and will be fetched and read. */

        DZ_PLAYER_EVENT_QUEUELIST_NEED_NATURAL_NEXT,          /* We need a new natural_next action. */

        // Data loading related event.
        DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY,              /* Data is ready to be introduced into audio output (first data after a play). */
        DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY_AFTER_SEEK,   /* Data is ready to be introduced into audio output (first data after a seek). */

        // Play (audio rendering on output) related event.
        DZ_PLAYER_EVENT_RENDER_TRACK_START_FAILURE,       /* Error, track is unable to play. */
        DZ_PLAYER_EVENT_RENDER_TRACK_START,               /* A track has started to play. */
        DZ_PLAYER_EVENT_RENDER_TRACK_END,                 /* A track has stopped because the stream has ended. */
        DZ_PLAYER_EVENT_RENDER_TRACK_PAUSED,              /* Currently on paused. */
        DZ_PLAYER_EVENT_RENDER_TRACK_SEEKING,             /* Waiting for new data on seek. */
        DZ_PLAYER_EVENT_RENDER_TRACK_UNDERFLOW,           /* Underflow happened whilst playing a track. */
        DZ_PLAYER_EVENT_RENDER_TRACK_RESUMED,             /* Player resumed play after a underflow or a pause. */
        DZ_PLAYER_EVENT_RENDER_TRACK_REMOVED,             /* Player stopped playing a track. */
}

    // Add an hint about why a new play is done.
    public enum dz_player_play_command_t
    {
        DZ_PLAYER_PLAY_CMD_UNKNOWN,           /* Player command has not been set yet, not a valid value. */
        DZ_PLAYER_PLAY_CMD_START_TRACKLIST,   /* A new tracklist was loaded and a track played. */
        DZ_PLAYER_PLAY_CMD_JUMP_IN_TRACKLIST, /* The user jump into a new song in the current tracklist. */
        DZ_PLAYER_PLAY_CMD_NEXT,              /* Next button. */
        DZ_PLAYER_PLAY_CMD_PREV,              /* Prev button. */
        DZ_PLAYER_PLAY_CMD_DISLIKE,           /* Dislike button. Dislike and skip the current track (Only available for radios/mixes) */
        DZ_PLAYER_PLAY_CMD_NATURAL_END,       /* Natural end. */
        DZ_PLAYER_PLAY_CMD_RESUMED_AFTER_ADS, /* Reload after playing an ads. */
    }


    // Player tracklist repeat modes.
    // Defines the behavior of the player when it finishes playing a track.
    public enum dz_queuelist_repeat_mode_t
    {
        DZ_QUEUELIST_REPEAT_MODE_OFF,          /* Play the loaded content starting from the given track index in the queuelist. */
        DZ_QUEUELIST_REPEAT_MODE_ONE,          /* Automatically play the current track forever. */
        DZ_QUEUELIST_REPEAT_MODE_ALL,          /* Automatically play the entire queuelist forever with a natural order. */
    }
    #endregion

    #region Delegates
    /**
     // Prototype of the render progress callback.
     * @param handle             Deezer player handle.
     * @param progress           The playback progress information in microseconds.
     * @param userdata           A reference to the user’s data.
     */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void dz_player_onrenderprogress_cb(IntPtr handle, UInt64 progress, IntPtr userdata);

    /**
     // Prototype of the on event callback.
     * @param handle             Deezer player handle.
     * @param event              The event information.
     * @param userdata           A reference to the user’s data.
     */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void dz_player_onevent_cb(IntPtr handle, IntPtr evtHandle, IntPtr userdata);

        /**
     // Prototype of the on renderer event callback
     *
     * @param self     Deezer player handle.
     * @param event    The handle of the renderer event that has just occured.
     * @param userdata A reference to the user’s data.
     */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void dz_player_onrendererevent_cb(IntPtr handle, IntPtr evtHandle, IntPtr userdata);
    #endregion

    public partial class DeezerApi
    {
        /**
         * @brief Get the event type of a player event instance.
         * @param self A player event instance.
         *
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_player_event_t dz_player_event_get_type(IntPtr self);

        /**
         * @brief Get the Deezer API information for the player event instance.
         * @note Only available for #DZ_PLAYER_EVENT_QUEUELIST_TRACK_SELECTED event.
         * @param self A player event instance.
         *
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaller))]
        public static extern string dz_player_event_track_selected_dzapiinfo(IntPtr self);

        /**
         * @brief Constructor for Deezer player.
         *
         * @param connect A deezer connect handle.
         * @returns A player handle.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dz_player_new(IntPtr connect);

        /**
         * @brief Register the event callback.
         *
         * Only one callback can be registered per player instance.
         * Should be done after dz_player_new() and before dz_player_load().
         *
         * @param self Deezer player handle
         * @param cb   The dz_player_onerror_cb() callback.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_set_event_cb(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_player_onevent_cb cb);

        /**
         * @brief Register a callback that will be called when there is more audio
         * data rendered (played on an audio output).
         *
         * Only one callback can be registered per player instance.
         * Should be done after dz_player_new and before dz_player_load
         *
         * @param self       Deezer player handle.
         * @param cb         The dz_player_onprogress_cb() callback.
         * @param refresh_us Approximate refresh time self callback is called.
         * @return DZ_ERROR_NO_ERROR if OK
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_set_render_progress_cb(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_player_onrenderprogress_cb cb,

            UInt64 refresh_us);


        /**
         * @brief Activate the player.
         *
         * @note Activate the player after the callback has been set.
         *
         * @param self       Deezer player handle you want to activate
         * @param supervisor Application context that will be returned when events occur.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_activate(IntPtr self, IntPtr supervisor);


        /**
        * @brief Deactivate the player.
        *
        * @param self               Deezer player handle you want to deactivate.
        * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
        * @param operation_userdata A reference to the user’s data.
        */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_deactivate(IntPtr self,
            
            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,
            
            IntPtr operation_userdata);


        /**
         * @brief Set the tracks/radio to play.
         *
         * @note A #DZ_PLAYER_EVENT_QUEUELIST_LOADED is sent when the content is ready to be played
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         * @param tracklist_data:
         *        - On demand: a deezer url, such as dzmedia:///track/[track_id] for single track content.
         *        - On demand: a deezer url, such as dzmedia:///album/[album_id] or dzmedia:///playlist/[playlist_id] for an album or a playlist.
         *        - (Not available) On demand: a json with a list of deezer url
         *        - Radio: a theme/artist/user/playlist radio dzradio:///[radio_id]
         *          "radio_id" is like "radio-???", "artist-???", "user-???", "playlist-???" respectively
         *          for a radio, a mix based on an artist, a user or a playlist.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_load(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,
            
            IntPtr operation_userdata,
            
            string tracklist_data);

        /**
         * @brief Begin playing the already loaded tracklist.
         *        The player gets data and renders it.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         * @param command            See function comments.
         * @param idx                Index of the track in the queuelist to play.\n
         *                           * Radios (aka mixes) also support #Int32_CURRENT or #Int32_NEXT.\n
         *                           * Albums & Playlists support #Int32_PREVIOUS, #Int32_CURRENT
         *                           or #Int32_NEXT.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_play(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata,
            
            dz_player_play_command_t command,
            
            Int32 idx);

        /**
         * @brief Put the player in stop state.
         *
         * Cached buffers have been dropped, and the data callback has stopped being triggered by the player.
         * The player can start streaming again from the begin if you call dz_player_play() again.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_stop(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata);

        /**
         * @brief Put the player on pause.
         *
         * No cache buffer is dropped, and the data callback stops being triggered by the player.
         * @note You need to call dz_player_resume() to start data callback again.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_pause(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata);

        /**
         * @brief Resumes playback after a pause.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_resume(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata);

        /**
         * @brief Seek at the specified time position.
         *
         * Cached buffers have been dropped, and the player will prepare data
         * from the closest position.
         *
         * If the player is in stop state, it goes into the pause state.
         * Otherwise the player stays in play / pause state.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data..
         * @param pos_usecond        Position in microseconds from the begin of track.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_seek(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,
            
            IntPtr operation_userdata,

            UInt64 pos_usecond);


                /**
         * @brief Set the player output volume.
         *
         * Changing this volume won't impact the volume of the other applications
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         * @param volume             Volume to set in percent (0 to 100).
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_set_output_volume(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata,

            Int32 volume);

        /**
         * @brief Get the information for the renderer event.
         * @param self The event from which the information will be extracted.
         *
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaller))]
        public static extern string dz_renderer_event_get_infos(IntPtr self);

       /**
         * @brief Register a on renderer event callback.
         *
         * Only one callback can be registered per player instance.
         * Should be done after dz_player_new() and before dz_player_load().
         *
         * @param self Deezer player handle.
         * @param cb   The dz_player_onerror_cb() callback.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_set_renderer_event_cb(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_player_onrendererevent_cb cb);

        /**
         * @brief Change/add/remove a renderer to the playback.
         *
         * @param self               Deezer player handle.
         * @param cb                 The dz_activity_operation_callback() callback called when the result is available.
         * @param operation_userdata A reference to the user’s data.
         * @param renderer_id        The renderer id impacted.
         * @param volume             The volume to be applied to the renderer.
         */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern dz_error_t dz_player_renderer_set_volume(IntPtr self,

            [MarshalAs(UnmanagedType.FunctionPtr)]
            dz_activity_operation_callback cb,

            IntPtr operation_userdata,
            
            string renderer_id,
            
            Int32 volume);

    }
}
