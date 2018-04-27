using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OPMedia.Core.Utilities;

namespace OPMedia.Core
{
    public class Ole32
    {
        const string OLE32 = "ole32.dll";
        const string OLEPRO32 = "olepro32.dll";

        [ComVisible(false)]
        [Flags]
        public enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

#if HAVE_MONO
        public static void CoUninitialize() { }

        public static void ReleaseStgMedium(ref STGMEDIUM pmedium) { }

        public static int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
			ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved
            )
        {
            return -1;
        }

        public static int CreateBindCtx(int reserved, out IBindCtx ppbc)
        {
            ppbc = null;
            return -1;
        }

        public static int MkParseDisplayName(IBindCtx pbc, string szUserName, ref int pchEaten, out IMoniker ppmk)
        {
            ppmk = null;
            return -1;
        }

        public static int CoCreateInstance(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IntPtr rReturnedComObject
            )
        {
            rReturnedComObject = IntPtr.Zero;
            return -1;
        }
#else  
        [DllImport(OLE32)]
        public static extern void CoUninitialize();

        [DllImport(OLE32)]
        public static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

        /// <summary>
        /// COM function helper for displaying properties dialog
        /// </summary>
        [DllImport(OLEPRO32)]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
			ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved
            );

        [DllImport(OLE32)]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport(OLE32, CharSet = CharSet.Unicode)]
        public static extern int MkParseDisplayName(IBindCtx pbc, string szUserName, ref int pchEaten, out IMoniker ppmk);

        [DllImport(OLE32)]
        public static extern int CoCreateInstance(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IntPtr rReturnedComObject
            );
#endif
    }

    public class Quartz
    {
        const string QUARTZ = "quartz.dll";

#if HAVE_MONO
        public static int AMGetErrorText(int hr, StringBuilder buf, int max)
        {
            return -1;
        }
#else
        [DllImport(QUARTZ, CharSet = CharSet.Auto)]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);
#endif
    }


    public static class WindowHelper
    {
        class find_arg
        {
            public List<IntPtr> _out_array { get; set; }
            public string _in_className { get; set; }
        }

        public static IntPtr FindWindow(string className = null)
        {
            find_arg fa = new find_arg();
            fa._in_className = className;
            fa._out_array = new List<IntPtr>();
            GCHandle gch = GCHandle.Alloc(fa);

            int res = User32.EnumWindows(new User32.EnumWindowProc(EnumWindowCallBack), (IntPtr)gch);

            if (res > 0 && fa._out_array != null && fa._out_array.Count > 0)
            {
                return fa._out_array[0];
            }

            return IntPtr.Zero;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, IntPtr lParam)
        {
            GCHandle gch = (GCHandle)lParam;
            find_arg fa = gch.Target as find_arg;

            if (fa != null)
            {
                StringBuilder sbc = new StringBuilder(256);
                User32.GetClassName(hwnd, sbc, sbc.Capacity);

                if (sbc.Length > 0)
                {
                    if (sbc.ToString().Contains(fa._in_className))
                        fa._out_array.Add(hwnd);
                }

                return true;
            }

            return false;
        }
    }

    public static class Advapi32
    {
        const string ADVAPI32 = "advapi32.dll";

        /// <summary>
        /// Passed to <see cref="GetTokenInformation"/> to specify what
        /// information about the token to return.
        /// </summary>
        public enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUiAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        /// <summary>
        /// The elevation type for a user token.
        /// </summary>
        public enum TokenElevationType
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        [DllImport(ADVAPI32, SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr tokenHandle, TokenInformationClass tokenInformationClass, IntPtr tokenInformation, int tokenInformationLength, out int returnLength);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    };
}
