using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.TranslationSupport;

using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;

namespace OPMedia.Addons.Builtin.Shared.Compression
{
    public enum BitrateMode
    {
        CBR = 0,
        ABR,
        VBR,
        Preset
    }

    public enum ChannelMode
    {
        Stereo = 0,
        JointStereo,
        DualChannel,
        SingleChannel,
    }

    public enum Preset
    {
        Normal_Quality = 0,
        Low_Quality = 1,
        High_Quality = 2,
        Very_High_Quality = 5,
        Standard = 6,
        Fast_Standard = 7,
        Extreme = 8,
        Fast_Extreme = 9,
        Insane = 10,
        Medium = 13,
        Fast_Medium = 14,
    }

    public class Mp3ConversionOptions
    {
        public BitrateMode BitrateMode { get; set; }
        public ChannelMode ChannelMode { get; set; }

        public int Bitrate { get; set; }

        public Preset VBRPreset { get; set; }

        public int VBRQuality { get; set; }
            
        public WaveFormatEx WaveFormat { get; set; }

        public int ResampleFrequency { get; set; }

        public Mp3ConversionOptions()
        {
            this.BitrateMode = BitrateMode.CBR;
            this.ChannelMode = ChannelMode.Stereo;
            this.Bitrate = 192;
            this.VBRPreset = Preset.Standard;
            this.VBRQuality = 7;
            this.WaveFormat = WaveFormatEx.Cdda;
            this.ResampleFrequency = WaveFormatEx.Cdda.nSamplesPerSec;
        }

        public LameEncConfig GetConfig(ref string summary)
        {
            LameEncConfig cfg = new LameEncConfig(this.WaveFormat, (uint)this.Bitrate);
            cfg.format.nMode = (MpegMode)this.ChannelMode;
            cfg.format.dwReSampleRate = (uint)this.ResampleFrequency;
            cfg.format.bStrictIso = 1; // Audacity does the same
            cfg.format.nPreset = LAME_QUALITY_PRESET.LQP_NOPRESET;
            cfg.format.bWriteVBRHeader = cfg.format.bEnableVBR = 1;
            cfg.format.dwBitrate = cfg.format.dwMaxBitrate = 0;
                                
            switch (BitrateMode)
            {
                case BitrateMode.CBR:
                cfg.format.bWriteVBRHeader = cfg.format.bEnableVBR = 0;
                cfg.format.dwBitrate = (uint)this.Bitrate;
                cfg.format.nVbrMethod = VBRMETHOD.VBR_METHOD_NONE;
                summary = Translator.Translate("TXT_CBR_SUMMARY", this.ChannelMode, this.Bitrate, this.ResampleFrequency);
                break;

                case BitrateMode.ABR:
                // ABR == VBR bitrate-based
                // Caution: LAME DLL calculates avg bitrate as (dwVbrAbr_bps + 500) / 1000
                cfg.format.dwVbrAbr_bps = (uint)this.Bitrate * 1000 - 500;
                cfg.format.nVBRQuality = 4;  // Audacity does the same
                cfg.format.nVbrMethod = VBRMETHOD.VBR_METHOD_ABR;
                summary = Translator.Translate("TXT_ABR_SUMMARY", this.ChannelMode, this.Bitrate, this.ResampleFrequency);
                break;

                case BitrateMode.VBR:
                // VBR quality-based
                int q = 9 - this.VBRQuality;
                cfg.format.nVBRQuality = (int)Math.Max(0, Math.Min(9, q));
                cfg.format.nVbrMethod = VBRMETHOD.VBR_METHOD_NEW;
                summary = Translator.Translate("TXT_VBR_SUMMARY", this.ChannelMode, cfg.format.nVBRQuality, this.ResampleFrequency);
                break;

                case BitrateMode.Preset:
                // Preset mode is VBR or CBR/320 in case that VBRPreset == INSANE
                cfg.format.nPreset = (LAME_QUALITY_PRESET)this.VBRPreset;
                if (this.VBRPreset != Preset.Insane)
                    summary = Translator.Translate("TXT_PRESET_SUMMARY", this.ChannelMode, this.VBRPreset, this.ResampleFrequency);
                else
                    summary = Translator.Translate("TXT_PRESET_INSANE", this.ChannelMode, this.VBRPreset, this.ResampleFrequency);
                break;
            }

            return cfg;
        }

    }

}
