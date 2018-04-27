using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IO = System.IO;
using OPMedia.Core;
using OPMedia.Core.Utilities;
using NAudio.Wave;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using OPMedia.Core.Logging;
using System.Threading;

namespace OPMedia.Runtime.ProTONE.MediaProcessing
{
    public class DecoderToWav
    {
        static int _calls = 0;
        static object _decoderLock = new object();
        string _inFile = null;
        string _outFile = null;

        public byte[] Decode(string audioType, byte[] input)
        {
            byte[] output = null;
            _calls++;

            lock (_decoderLock)
            {
                string fileType = "";
                switch (audioType)
                {
                    case "audio/mpg":
                    case "audio/mpeg":
                        fileType = "mp3";
                        break;

                    case "audio/aac":
                    case "audio/aacp":
                        fileType = "aac";
                        break;
                }

                try
                {
                    string _inFile = PathUtils.GetTempFilePath(string.Format("opm-dec-in-{1}.{0}", fileType,
                        _calls % 10));
                    string _outFile = PathUtils.GetTempFilePath(string.Format("opm-dec-out.wav"));

                    DeleteFile(_outFile);
                    DeleteFile(_inFile);

                    IO.File.WriteAllBytes(_inFile, input);

                    WaveFormat wf = new WaveFormat(WaveFormatEx.Cdda.nSamplesPerSec, 2);

                    using (MediaFoundationReader mfr = new MediaFoundationReader(_inFile))
                    using (ResamplerDmoStream res = new ResamplerDmoStream(mfr, wf))
                    using (WaveFileWriter wav = new WaveFileWriter(_outFile, wf))
                    {
                        res.CopyTo(wav);

                        wav.Close();
                        res.Close();
                        mfr.Close();
                    }

                    if (IO.File.Exists(_outFile))
                    {
                        byte[] outWav = IO.File.ReadAllBytes(_outFile);
                        if (outWav.Length > 44)
                        {
                            output = new byte[outWav.Length];
                            Array.Copy(outWav, 44, output, 0, outWav.Length - 44);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            return output;
        }

        public void Dispose()
        {
        }

        private void DeleteFile(string p)
        {
            try
            {
                if (IO.File.Exists(p))
                {
                    var attr = IO.File.GetAttributes(p);
                    IO.File.SetAttributes(p, attr ^ attr);

                    IO.File.Delete(p);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
