using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Runtime.ProTONE.Rendering.DS.BaseClasses;
using System.IO;
using System.Runtime.InteropServices;

namespace OPMedia.Runtime.ProTONE.Compression
{
    public static class WaveFile
    {
        public const uint RIFF_TAG = 0x46464952;
        public const uint CDDA_TAG = 0x41444443;
        public const uint WAVE_TAG = 0x45564157;
        public const uint FMT__TAG = 0x20746D66;
        public const uint DATA_TAG = 0x61746164;
        public const uint WAVE_FORMAT_PCM = 0x01;

        public const uint WaveHeaderSize = 38;
        public const uint WaveFormatSize = 18;

        public static byte[] ReadWaveData(string inputFile, ref WaveFormatEx wfex)
        {
            try
            {
                using (FileStream fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    br.ReadUInt32(); // RIFF_TAG
                    uint rawSize = br.ReadUInt32(); // WaveHeaderSize + buff.Length
                    uint buffSize = rawSize - WaveHeaderSize - sizeof(uint);

                    br.ReadUInt32(); // WAVE_TAG
                    br.ReadUInt32(); // FMT__TAG
                    br.ReadUInt32(); // WaveFormatSize

                    byte[] bytesWfex = br.ReadBytes(Marshal.SizeOf(wfex));

                    var pinnedRawData = GCHandle.Alloc(bytesWfex, GCHandleType.Pinned);
                    try
                    {
                        // Get the address of the data array
                        var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();

                        // overlay the data type on top of the raw data
                        wfex = (WaveFormatEx)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(WaveFormatEx));
                    }
                    finally
                    {
                        // must explicitly release
                        pinnedRawData.Free();
                    }

                    br.ReadUInt32(); // DATA_TAG

                    return br.ReadBytes((int)buffSize);
                }
            }
            catch { }

            return null;
        }

        public static void WriteWaveData(byte[] buff, WaveFormatEx wfex, string destFile)
        {
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(RIFF_TAG);
                bw.Write((uint)(WaveHeaderSize + buff.Length));
                bw.Write(WAVE_TAG);
                bw.Write(FMT__TAG);
                bw.Write(WaveFormatSize);

                int size = Marshal.SizeOf(wfex);
                byte[] arr = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(wfex, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);

                bw.Write(arr);
                bw.Write(DATA_TAG);
                bw.Write(buff.Length);
                bw.Write(buff);
            }
        }
    }
}
