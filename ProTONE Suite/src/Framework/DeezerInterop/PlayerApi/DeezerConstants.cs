using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPMedia.DeezerInterop.PlayerApi
{
    public static class Constants
    {
        // deezer-track
        public const UInt32 dz_sample_fmt_float32          = 0x00000001; /* Float 32 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_int32            = 0x00000002; /* Int   32 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_int24            = 0x00000004; /* Int   24 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_int16            = 0x00000008; /* Int   16 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_int8             = 0x00000010; /* Int    8 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_uint8            = 0x00000020; /* Unsigned Int 8 bits packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_other            = 0x00008000; /* Other packed format @see dz_audiosample_format_t. */
        public const UInt32 dz_sample_fmt_non_interleaved  = 0x80000000; /* Non interleaved information @see dz_audiosample_format_t. */

        // deezer-track
        public const UInt32 dz_channel_position_front_left   = 0x00000001; /* Front left   channel. */
        public const UInt32 dz_channel_position_front_right  = 0x00000002; /* Front right  channel. */
        public const UInt32 dz_channel_position_front_center = 0x00000004; /* Front center channel. */
        public const UInt32 dz_channel_position_lfe          = 0x00000008; /* Low Frequency Effects channel. */
        public const UInt32 dz_channel_position_back_left    = 0x00000010; /* Back left  also surround left  for 5.1 or 6.1 mapping. */
        public const UInt32 dz_channel_position_back_right   = 0x00000020; /* Back right also surround right for 5.1 or 6.1 mapping.  */
        public const UInt32 dz_channel_position_side_left    = 0x00000040; /* Side left  channel. */
        public const UInt32 dz_channel_position_side_right   = 0x00000080; /* Side right channel. */

        // deezer-player
        public const Int32 DZ_INDEX_IN_QUEUELIST_INVALID  = -1; /* Index in queuelist of tracks has not been set yet, not a valid value. */
        public const Int32 DZ_INDEX_IN_QUEUELIST_PREVIOUS = -2; /* Play the previous track in the queuelist of tracks. */
        public const Int32 DZ_INDEX_IN_QUEUELIST_CURRENT  = -3; /* Play the current track in the queuelist of tracks. */
        public const Int32 DZ_INDEX_IN_QUEUELIST_NEXT     = -4; /* Play the next track in the queuelist of tracks. */
    }
}
