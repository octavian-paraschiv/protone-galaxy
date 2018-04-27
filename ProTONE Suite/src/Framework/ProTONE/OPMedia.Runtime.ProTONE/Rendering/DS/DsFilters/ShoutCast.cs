using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Runtime.ProTONE.Rendering.Cdda;
using OPMedia.Core;
using System.IO;
using System.Runtime.InteropServices;
using OPMedia.Runtime.ProTONE.Rendering.SHOUTCast;
using OPMedia.Core.Logging;
using NAudio.Wave;
using OPMedia.Runtime.ProTONE.Compression;

namespace OPMedia.Runtime.ProTONE.Rendering.DS.DsFilters
{
    public class ShoutcastStreamParser : FileParser
    {
        ShoutcastStream _stream = null;

        public ShoutcastStream ShoutcastStream { get { return _stream; } }

        public ShoutcastStreamParser()
            : base(false)
        {
        }

        protected override HRESULT CheckFile()
        {
            _stream = new ShoutcastStream(m_sFileName, 4000, false);
            if (_stream != null && _stream.Connected)
                return S_OK;

            return S_FALSE;
        }

        protected override HRESULT LoadTracks()
        {
            string contentType = _stream.ContentType;
            Logger.LogTrace("ShoutCast::LoadTracks contentType={0}", contentType);

            AMMediaType mt = new AMMediaType();

            switch (contentType)
            {
                case "audio/mpg":
                case "audio/mpeg":
                    {
                        BaseClasses.Mp3WaveFormat wfex = new BaseClasses.Mp3WaveFormat();
                        wfex.cbSize = 12; // MPEGLAYER3_WFX_EXTRA_BYTES
                        wfex.fdwFlags = 0; // MPEGLAYER3_FLAG_PADDING_ISO

                        wfex.nAvgBytesPerSec = (_stream.Bitrate * 1024 / 8);

                        wfex.nBlockAlign = 1; // must be 1 for streamed MP3
                        wfex.nBlockSize = 522; // MP3_BLOCK_SIZE magic number
                        wfex.nChannels = 2; // Stereo
                        wfex.nCodecDelay = 0; // must be 0
                        wfex.nFramesPerBlock = 1; // must be 1

                        wfex.nSamplesPerSec = _stream.SampleRate;

                        wfex.wBitsPerSample = 0; // must be 0
                        wfex.wFormatTag = 0x0055; // WAVE_FORMAT_MPEGLAYER3
                        wfex.wID = 1; // MPEGLAYER3_ID_MPEG

                        mt.majorType = MediaType.Audio;
                        mt.subType = MediaSubType.MPEG1Audio;
                        mt.sampleSize = 0;
                        mt.fixedSizeSamples = false;
                        mt.SetFormat(wfex);
                    }
                    break;

                //case "audio/aac":
                //case "audio/aacp":
                //    {
                //        WaveFormatEx wfex = WaveFormatEx.Cdda;

                //        mt.majorType = MediaType.Audio;
                //        mt.subType = MediaSubType.PCM;
                //        mt.sampleSize = wfex.nBlockAlign;
                //        mt.fixedSizeSamples = true;
                //        mt.SetFormat(wfex);
                //    }
                //    break;
            }

            m_Tracks.Add(new ShoutcastStreamTrack(this, mt));

            return S_OK;
        }
    }

    public class ShoutcastStreamTrack : DemuxTrack
    {
        #region Variables

        protected AMMediaType m_mt = null;
        protected long m_rtMediaPosition = 0;

        #endregion

        #region Constructor

        public ShoutcastStreamTrack(ShoutcastStreamParser _parser, AMMediaType mt)
            : base(_parser, TrackType.Audio)
        {
            m_mt = mt;
        }

        #endregion

        #region Overridden Methods

        public override HRESULT SetMediaType(AMMediaType pmt)
        {
            if (pmt.majorType != m_mt.majorType)
                return VFW_E_TYPE_NOT_ACCEPTED;

            if (pmt.formatPtr == IntPtr.Zero)
                return VFW_E_INVALIDMEDIATYPE;

            if (pmt.subType != m_mt.subType)
                return VFW_E_TYPE_NOT_ACCEPTED;

            if (pmt.formatType != m_mt.formatType)
                return VFW_E_TYPE_NOT_ACCEPTED;

            return NOERROR;
        }

        public override HRESULT GetMediaType(int iPosition, ref AMMediaType pmt)
        {
            if (iPosition < 0) return E_INVALIDARG;
            if (iPosition > 0) return VFW_S_NO_MORE_ITEMS;
            pmt.Set(m_mt);
            return NOERROR;
        }

        public override HRESULT SeekTrack(long _time)
        {
            m_rtMediaPosition = _time;
            return base.SeekTrack(_time);
        }

        public override PacketData GetNextPacket()
        {
            ShoutcastStream ss = (m_pParser as ShoutcastStreamParser).ShoutcastStream;
            string contentType = ss.ContentType;

            Logger.LogTrace("ShoutcastStreamTrack::GetNextPacket contentType={0}", ss.ContentType);

            int slice = ss.Bitrate * 1024 / 8;

            PacketData dataToReturn = null;

            PacketData dataToRead = new PacketData();
            dataToRead.Buffer = new byte[slice];

            int bytesRead = ss.Read(dataToRead.Buffer, 0, slice);
            if (bytesRead > 0)
            {
                Logger.LogTrace("ShoutcastStreamTrack::GetNextPacket contentType={0} bytesRead={1}", ss.ContentType, bytesRead);

                switch (contentType)
                {
                    case "audio/mpg":
                    case "audio/mpeg":
                        {
                            // The data that was read has mp3 format
                            // Since we initialized the media subtype as MPEG1Audio, it's safe to send this data directly
                            // to the output renderer.
                            dataToRead.Position = 0;
                            dataToRead.Size = (int)bytesRead;
                            dataToRead.SyncPoint = true;
                            dataToRead.Start = m_rtMediaPosition;
                            dataToRead.Stop = dataToRead.Start + UNITS / 2;
                            m_rtMediaPosition = dataToRead.Stop;
                            dataToReturn = dataToRead;
                        }
                        break;

                    //case "audio/aac":
                    //case "audio/aacp":
                    //    {
                    //        try
                    //        {
                    //            // The data that was read has ac3 format
                    //            // Since we initialized the media subtype as PCM, we'll need to convert the AC3 data
                    //            // to WAV before sending to the output renderer.
                    //            string aacFile = PathUtils.GetTempFilePath("shoutcast_in.aac");
                    //            string wavFile = PathUtils.GetTempFilePath("shoutcast_out.wav");

                    //            File.WriteAllBytes(aacFile, dataToRead.Buffer);

                    //            string aacFileUrl = string.Format(@"file:///{0}", aacFile.Replace("\\", "/"));

                                

                    //            using (var reader = new MediaFoundationReader(aacFileUrl))
                    //                WaveFileWriter.CreateWaveFile(wavFile, reader);

                    //            WaveFormatEx wfex = WaveFormatEx.Cdda;
                    //            byte[] buff = WaveFile.ReadWaveData(wavFile, ref wfex);

                    //            dataToRead.Position = 0;
                    //            dataToRead.Size = buff.Length;
                    //            dataToRead.SyncPoint = true;
                    //            dataToRead.Start = m_rtMediaPosition;
                    //            dataToRead.Stop = dataToRead.Start + UNITS;
                    //            m_rtMediaPosition = dataToRead.Stop;

                    //            dataToReturn = new PacketData();
                    //            dataToReturn.Buffer = new byte[buff.Length];
                    //            Array.Copy(buff, dataToReturn.Buffer, buff.Length);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Logger.LogTrace("ShoutcastStreamTrack: " + ex.ToString());
                    //        }
                    //    }
                    //    break;
                }
            }

            Logger.LogTrace("ShoutcastStreamTrack::GetNextPacket contentType={0} returning {1}null", ss.ContentType, 
                (dataToReturn == null) ? string.Empty : "non-");
            return dataToReturn;
        }

        #endregion
    }

    public class ShoutcastStreamSourceFilter : BaseSourceFilterTemplate<ShoutcastStreamParser>
    {
        public ShoutcastStreamSourceFilter()
            : base("OPM Shoutcast Stream Source Filter")
        {
        }
    }
}
