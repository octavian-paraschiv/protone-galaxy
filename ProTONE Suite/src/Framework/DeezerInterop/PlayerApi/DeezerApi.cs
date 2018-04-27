using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OPMedia.Core.Logging;

// Ported from original header file deezer-api.h

namespace OPMedia.DeezerInterop.PlayerApi
{
    public class DeezerPlayerException : Exception
    {
        public dz_error_t DzErrorCode { get; private set; }

        public static DeezerPlayerException FromDzErrorCode(dz_error_t err)
        {
            DeezerPlayerException ex = new DeezerPlayerException();
            ex.DzErrorCode = err;

            throw ex;
        }

        public override string ToString()
        {
            return $"Error raised by the Deezer Player Runtime: {DzErrorCode}";
        }

        private DeezerPlayerException() 
            : base()
        {
        }
    }

    public partial class DeezerApi
    {
        const string DLL_NAME_X86 = "libdeezer.x86.dll";
        const string DLL_NAME_X64 = "libdeezer.x64.dll";

        /**
       * @brief Destructor for Deezer objects.
       *
       * @param obj A Deezer object handle.
       */
        [DllImport(DLL_NAME_X86, CallingConvention = CallingConvention.Cdecl)]
        public static extern void dz_object_release(IntPtr obj);

        public static bool IsError(dz_error_t err)
        {
            switch (err)
            {
                case dz_error_t.DZ_ERROR_NO_ERROR:
                case dz_error_t.DZ_ERROR_NO_ERROR_ASYNC:
                    return false;

                default:
                    return true;
            }
        }

        public static void HandleDzErrorCode(string operation, dz_error_t err, bool throwException = true)
        {
            if (IsError(err))
            {
                Logger.LogTrace("{0} => {1}", operation, err);

                if (throwException)
                    throw DeezerPlayerException.FromDzErrorCode(err);
            }
        }
    }
}
