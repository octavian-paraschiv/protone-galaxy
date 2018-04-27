using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

// Ported from original header file deezer-track.h

namespace OPMedia.DeezerInterop.PlayerApi
{
    /// <summary>
    /// Deezer track qualities
    /// </summary>
    public enum dz_track_quality_t
    {
        /// <summary>
        /// Track quality has not been set yet, not a valid value
        /// </summary>
        DZ_TRACK_QUALITY_UNKNOWN,
        /// <summary>
        /// Medium quality compressed audio
        /// </summary>
        DZ_TRACK_QUALITY_STANDARD,
        /// <summary>
        /// High quality compressed audio
        /// </summary>
        DZ_TRACK_QUALITY_HIGHQUALITY,
        /// <summary>
        /// Lossless two channel 44,1KHz 16bits
        /// </summary>
        DZ_TRACK_QUALITY_CDQUALITY,
        /// <summary>
        /// Try using smaller file formats
        /// </summary>
        DZ_TRACK_QUALITY_DATA_EFFICIENT,

        /// <summary>
        /// NOT a valid value, just for internal purpose of array sizing, keep it at the last position
        /// </summary>
        DZ_TRACK_QUALITY_LAST_ENTRY
    }

    /// <summary>
    /// Track metadata types
    /// </summary>
    public enum dz_track_metadata_t
    {
        /// <summary>
        /// Track metadata has not been set yet, not a valid value
        /// </summary>
        DZ_TRACK_METADATA_UNKNOWN,
        /// <summary>
        /// Track header metadata type
        /// </summary>
        DZ_TRACK_METADATA_FORMAT_HEADER,
        /// <summary>
        /// Track duration metadata type
        /// </summary>
        DZ_TRACK_METADATA_DURATION_MS,
    };

    /// <summary>
    /// Media format of the audio track
    /// </summary>
    public enum dz_media_format_t
    {
        /// <summary>
        /// Media format has not been set yet, not a valid value
        /// </summary>
        DZ_MEDIA_FORMAT_UNKNOWN,   
        /// <summary>
        /// Audio format is PCM
        /// </summary>
        DZ_MEDIA_FORMAT_AUDIO_PCM, 
        /// <summary>
        /// Audio format is MPEG
        /// </summary>
        DZ_MEDIA_FORMAT_AUDIO_MPEG,
        /// <summary>
        /// Audio format is FLAC
        /// </summary>
        DZ_MEDIA_FORMAT_AUDIO_FLAC,
    };

    public struct audio_mpeg_t
    {
        public int  layer;    /**< Equals to 3 for MP3 */
        public float version; /**< Equals to 1.0, 2.0 or 2.5 for layer 3 */
    }

    public struct audio_flac_t
    {
        public int dummy;     /**< Dummy field to avoid MSVC complaints */
    }

    public struct samples_t
    {
        public UInt32 format; /* Sample size. */
        public Int32 sample_rate; /* Sample rate (in Hz). */
        public UInt32 channels; /* Number of channels. */
        public UInt32 channel_position_mask; /* Channel position @see dz_channel_position_t. */
    }

    /* Audio information. */
    public struct audio_t
    {
        public samples_t samples; /* Samples information. */
        public UInt64 total_samples; /* Number of samples. */
    }

    public struct video_t
    {
        public int dummy0; /* Dummy field to avoid MSVC complaints */
    }

    public struct subtitle_t
    {
        public int dummy1; /* Dummy field to avoid MSVC complaints */
    }

    /// <summary>
    /// Media track detailed informations
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct dz_media_track_detailed_infos_t
    {
        [FieldOffset(0)]
        public dz_media_format_t format; /**< @see dz_media_format_t */

        #region union(audio_mpeg, audio_flac)
        [FieldOffset(4)]
        public audio_mpeg_t audio_mpeg;

        [FieldOffset(4)]
        public audio_flac_t audio_flac;
        #endregion union

        [FieldOffset(12)]
        public bool is_cbr;           /**< Is the media encoded at a constant bitrate */

        [FieldOffset(16)]
        public float average_bitrate; /**< Average bitrate */
    
        #region union(audio, video, subtitle)

        [FieldOffset(20)]
        public audio_t audio;

        [FieldOffset(20)]
        public video_t video;

        [FieldOffset(20)]
        public subtitle_t subtitle;

        #endregion
    }


    public partial class DeezerApi
    {
        /*
        dz_track_metadata_t dz_track_metadata_get_type(IntPtr self);
        IntPtr  dz_track_metadata_get_format_header(IntPtr self);
        UInt32 dz_track_metadata_get_duration(IntPtr self);
         */ 
    }
}
