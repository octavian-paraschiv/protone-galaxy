using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using OPMedia.Addons.Builtin.Navigation.FileExplorer.CdRipperWizard.Tasks;
using OPMedia.Addons.Builtin.Shared.EncoderOptions;
using OPMedia.Core;
using OPMedia.Core.Logging;

using OPMedia.Runtime.ProTONE.FileInformation;
using OPMedia.Runtime.ProTONE.Rendering.DS;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.UI;
using OPMedia.Addons.Builtin.Shared.Compression;
using OPMedia.Runtime.ProTONE.Compression;

namespace OPMedia.Addons.Builtin.Property.TaggedFileProp.TaggingWizard.Helpers
{
    public class Transcoder
    {
        static List<Transcoding> _supportedTranscodings = new List<Transcoding>();

        [Browsable(false)]
        public EncoderSettings EncoderSettings { get; set; }

        private Transcoding _transcoding = null;

        public void RequestCancel()
        {
            if (_transcoding != null)
                _transcoding.RequestCancel();
        }

        public bool MustCancel()
        {
            if (_transcoding != null)
                return _transcoding.MustCancel();

            return false;
        }

        static Transcoder()
        {
            _supportedTranscodings.Add(new Transcoding 
                {
                    // MP3 to MP3 is supported
                    InputFormat = AudioMediaFormatType.MP3,
                    OutputFormat = AudioMediaFormatType.MP3
                });

            _supportedTranscodings.Add(new Transcoding
                {
                    // MP3 to WAV is supported
                    InputFormat = AudioMediaFormatType.MP3,
                    OutputFormat = AudioMediaFormatType.WAV
                });

            _supportedTranscodings.Add(new Transcoding
                {
                    // WAV to MP3 is supported
                    InputFormat = AudioMediaFormatType.WAV,
                    OutputFormat = AudioMediaFormatType.MP3
                });
        }

        public Transcoder()
        {
        }

        public void ChangeEncoding(string file)
        {
            string inputFileType = PathUtils.GetExtension(file).ToUpperInvariant();

            AudioMediaFormatType inputFormat = AudioMediaFormatType.WAV;
            AudioMediaFormatType outputFormat = EncoderSettings.FormatType;

            if (Enum.TryParse<AudioMediaFormatType>(inputFileType, out inputFormat) == false)
                throw new NotSupportedException(string.Format("TXT_UNSUPPORTED_INPUT_FORMAT: {0}", inputFileType));

            InternalChangeEncoding(inputFormat, outputFormat, file);
        }

        private void InternalChangeEncoding(AudioMediaFormatType inputFormat, AudioMediaFormatType outputFormat, string file)
        {
            _transcoding = new Transcoding
            {
                InputFormat = inputFormat,
                OutputFormat = outputFormat
            };

            if (_supportedTranscodings.Contains(_transcoding) == false)
                throw new NotSupportedException(string.Format("TXT_UNSUPPORTED_TRANSCODING: {0}", _transcoding));

            _transcoding.DoTranscoding(EncoderSettings, file);
        }
    }

    public class Transcoding
    {
        public AudioMediaFormatType InputFormat { get; set; }
        public AudioMediaFormatType OutputFormat { get; set; }

        private CdRipper _grabber = null;

        public void RequestCancel()
        {
            if (_grabber != null)
                _grabber.RequestCancel();
        }

        public bool MustCancel()
        {
            if (_grabber != null)
                return _grabber.MustCancel();

            return false;
        }

        public override bool Equals(object obj)
        {
            Transcoding t = obj as Transcoding;
            if (t != null)
            {
                return (t.InputFormat == this.InputFormat) &&
                    (t.OutputFormat == this.OutputFormat);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", InputFormat, OutputFormat);
        }

        public void DoTranscoding(EncoderSettings encoderSettings, string inputFile)
        {
            _grabber = CdRipper.CreateGrabber(OutputFormat);
            if (_grabber == null)
                throw new NotSupportedException(string.Format("TXT_UNSUPPORTED_OUTPUT_FORMAT: {0}", InputFormat));

            switch (InputFormat)
            {
                case AudioMediaFormatType.WAV:
                    switch (OutputFormat)
                    {
                        case AudioMediaFormatType.MP3:
                            {
                                // Transcode WAV => MP3 i.o.w encode the wav
                                WaveFormatEx wfex = WaveFormatEx.Cdda;
                                byte[] buff = WaveFile.ReadWaveData(inputFile, ref wfex);
                                GrabberToMP3 grabber = (_grabber as GrabberToMP3);
                                grabber.Options = (encoderSettings as Mp3EncoderSettings).Options;

                                
                                // Resample is not supported at this time.
                                // Specify the same settings as the input WAV file, otherwise we'll be failing.
                                grabber.Options.WaveFormat = wfex;

                                grabber.EncodeBuffer(buff,
                                    Path.ChangeExtension(inputFile, "MP3"),
                                    false, null);
                                
                                return;
                            }
                    }
                    break;

                case AudioMediaFormatType.MP3:
                    switch (OutputFormat)
                    {
                        case AudioMediaFormatType.WAV:
                            // Transcode MP3 => WAV i.o.w decode the MP3
                            string outputFile = Path.ChangeExtension(inputFile, "WAV");
                            if (DecodeMP3ToWAV(inputFile, outputFile) == false)
                                throw new Exception("TXT_FAILED_CONVERSION_MP3_WAV");

                            return;

                        case AudioMediaFormatType.MP3:
                            {
                                // Transcode MP3 => MP3 i.o.w adjust MP3 encoding
                                string tempWavFile = Path.GetTempFileName();
                                if (DecodeMP3ToWAV(inputFile, tempWavFile) == false)
                                    throw new Exception("TXT_FAILED_CONVERSION_MP3_TEMP_WAV");

                                WaveFormatEx wfex = WaveFormatEx.Cdda;
                                byte[] buff = WaveFile.ReadWaveData(tempWavFile, ref wfex);

                                GrabberToMP3 grabber = (_grabber as GrabberToMP3);
                                grabber.Options = (encoderSettings as Mp3EncoderSettings).Options;

                                ID3FileInfoSlim ifiSlim = 
                                    new ID3FileInfoSlim(MediaFileInfo.FromPath(inputFile, false));

                                grabber.EncodeBuffer(buff,
                                    Path.ChangeExtension(inputFile, "REENC.MP3"),
                                    (encoderSettings as Mp3EncoderSettings).CopyInputFileMetadata,
                                    ifiSlim);

                                if (File.Exists(tempWavFile))
                                    File.Delete(tempWavFile);

                                return;
                            }
                    }
                    break;
            }

            throw new NotSupportedException(string.Format("TXT_UNSUPPORTED_TRANSCODING: {0}", this));
        }

        private bool DecodeMP3ToWAV(string inputFile, string outputFile)
        {
            try
            {
                using (Mp3FileReader reader = new Mp3FileReader(inputFile))
                    WaveFileWriter.CreateWaveFile(outputFile, reader);

                return true;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }

            return false;
        }
    }
}
