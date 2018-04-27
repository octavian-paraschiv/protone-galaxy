﻿using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using OPMedia.Core.Win32;

namespace OPMedia.Core.NetworkAccess
{
    #region Share

    /// <summary>
    /// Information about a local share
    /// </summary>
    public class Share
    {
        #region Private data

        private string _server;
        private string _netName;
        private string _path;
        private ShareType _shareType;
        private string _remark;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="shi"></param>
        public Share(string server, string netName, string path, ShareType shareType, string remark)
        {
            if (ShareType.Special == shareType && "IPC$" == netName)
            {
                shareType |= ShareType.IPC;
            }

            _server = server;
            _netName = netName;
            _path = path;
            _shareType = shareType;
            _remark = remark;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the computer that this share belongs to
        /// </summary>
        public string Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Share name
        /// </summary>
        public string NetName
        {
            get { return _netName; }
        }

        /// <summary>
        /// Local path
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Share type
        /// </summary>
        public ShareType ShareType
        {
            get { return _shareType; }
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string Remark
        {
            get { return _remark; }
        }

        /// <summary>
        /// Returns true if this is a file system share
        /// </summary>
        public bool IsFileSystem
        {
            get
            {
                // Shared device
                if (0 != (_shareType & ShareType.Device)) return false;
                // IPC share
                if (0 != (_shareType & ShareType.IPC)) return false;
                // Shared printer
                if (0 != (_shareType & ShareType.Printer)) return false;

                // Standard disk share
                if (0 == (_shareType & ShareType.Special)) return true;

                // Special disk share (e.g. C$)
                if (ShareType.Special == _shareType && null != _netName && 0 != _netName.Length)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Get the root of a disk-based share
        /// </summary>
        public DirectoryInfo Root
        {
            get
            {
                if (IsFileSystem)
                {
                    if (null == _server || 0 == _server.Length)
                        if (null == _path || 0 == _path.Length)
                            return new DirectoryInfo(ToString());
                        else
                            return new DirectoryInfo(_path);
                    else
                        return new DirectoryInfo(ToString());
                }
                else
                    return null;
            }
        }

        #endregion

        /// <summary>
        /// Returns the path to this share
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (null == _server || 0 == _server.Length)
            {
                return $"\\\\{Environment.MachineName}\\{_netName}";
            }
            else
                return $"\\\\{_server}\\{_netName}";
        }

        /// <summary>
        /// Returns true if this share matches the local path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool MatchesPath(string path)
        {
            if (!IsFileSystem) return false;
            if (null == path || 0 == path.Length) return true;

            return path.ToLower().StartsWith(_path.ToLower());
        }
    }

    #endregion

    #region Share Collection

    /// <summary>
    /// A collection of shares
    /// </summary>
    public class ShareCollection : ReadOnlyCollectionBase
    {
        #region Platform

        /// <summary>
        /// Is this an NT platform?
        /// </summary>
        protected static bool IsNT
        {
            get { return (PlatformID.Win32NT == Environment.OSVersion.Platform); }
        }

        /// <summary>
        /// Returns true if this is Windows 2000 or higher
        /// </summary>
        protected static bool IsW2KUp
        {
            get
            {
                OperatingSystem os = Environment.OSVersion;
                if (PlatformID.Win32NT == os.Platform && os.Version.Major >= 5)
                    return true;
                else
                    return false;
            }
        }

        #endregion


        #region Enumerate shares

        /// <summary>
        /// Enumerates the shares on Windows NT
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The ShareCollection</param>
        protected static void EnumerateSharesNT(string server, ShareCollection shares)
        {
            int level = 2;
            int entriesRead, totalEntries, nRet, hResume = 0;
            IntPtr pBuffer = IntPtr.Zero;

            try
            {
                nRet = UncUtilities.NetShareEnum(server, level, out pBuffer, -1,
                    out entriesRead, out totalEntries, ref hResume);

                if (UncUtilities.ERROR_ACCESS_DENIED == nRet)
                {
                    //Need admin for level 2, drop to level 1
                    level = 1;
                    nRet = UncUtilities.NetShareEnum(server, level, out pBuffer, -1,
                        out entriesRead, out totalEntries, ref hResume);
                }

                if (UncUtilities.NO_ERROR == nRet && entriesRead > 0)
                {
                    Type t = (2 == level) ? typeof(SHARE_INFO_2) : typeof(SHARE_INFO_1);
                    int offset = Marshal.SizeOf(t);

                    for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += offset)
                    {
                        IntPtr pItem = new IntPtr(lpItem);
                        if (1 == level)
                        {
                            SHARE_INFO_1 si = (SHARE_INFO_1)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                        }
                        else
                        {
                            SHARE_INFO_2 si = (SHARE_INFO_2)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                        }
                    }
                }

            }
            finally
            {
                // Clean up buffer allocated by system
                if (IntPtr.Zero != pBuffer)
                    UncUtilities.NetApiBufferFree(pBuffer);
            }
        }

        /// <summary>
        /// Enumerates the shares on Windows 9x
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The ShareCollection</param>
        protected static void EnumerateShares9x(string server, ShareCollection shares)
        {
            int level = 50;
            int nRet = 0;
            ushort entriesRead, totalEntries;

            Type t = typeof(SHARE_INFO_50);
            int size = Marshal.SizeOf(t);
            ushort cbBuffer = (ushort)(UncUtilities.MAX_SI50_ENTRIES * size);
            //On Win9x, must allocate buffer before calling API
            IntPtr pBuffer = Marshal.AllocHGlobal(cbBuffer);

            try
            {
                nRet = UncUtilities.NetShareEnum(server, level, pBuffer, cbBuffer,
                    out entriesRead, out totalEntries);

                if (UncUtilities.ERROR_WRONG_LEVEL == nRet)
                {
                    level = 1;
                    t = typeof(SHARE_INFO_1_9x);
                    size = Marshal.SizeOf(t);

                    nRet = UncUtilities.NetShareEnum(server, level, pBuffer, cbBuffer,
                        out entriesRead, out totalEntries);
                }

                if (UncUtilities.NO_ERROR == nRet || UncUtilities.ERROR_MORE_DATA == nRet)
                {
                    for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += size)
                    {
                        IntPtr pItem = new IntPtr(lpItem);

                        if (1 == level)
                        {
                            SHARE_INFO_1_9x si = (SHARE_INFO_1_9x)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                        }
                        else
                        {
                            SHARE_INFO_50 si = (SHARE_INFO_50)Marshal.PtrToStructure(pItem, t);
                            shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                        }
                    }
                }
                else
                    Console.WriteLine(nRet);

            }
            finally
            {
                //Clean up buffer
                Marshal.FreeHGlobal(pBuffer);
            }
        }

        /// <summary>
        /// Enumerates the shares
        /// </summary>
        /// <param name="server">The server name</param>
        /// <param name="shares">The ShareCollection</param>
        protected static void EnumerateShares(string server, ShareCollection shares)
        {
            if (null != server && 0 != server.Length && !IsW2KUp)
            {
                server = server.ToUpper();

                // On NT4, 9x and Me, server has to start with "\\"
                if (!('\\' == server[0] && '\\' == server[1]))
                    server = @"\\" + server;
            }

            if (IsNT)
                EnumerateSharesNT(server, shares);
            else
                EnumerateShares9x(server, shares);
        }

        #endregion

        #endregion

        #region Static methods

        /// <summary>
        /// Returns true if fileName is a valid local file-name of the form:
        /// X:\, where X is a drive letter from A-Z
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns></returns>
        public static bool IsValidFilePath(string fileName)
        {
            if (null == fileName || 0 == fileName.Length) return false;

            char drive = char.ToUpper(fileName[0]);
            if ('A' > drive || drive > 'Z')
                return false;

            else if (Path.VolumeSeparatorChar != fileName[1])
                return false;
            else if (Path.DirectorySeparatorChar != fileName[2])
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns the UNC path for a mapped drive or local share.
        /// </summary>
        /// <param name="fileName">The path to map</param>
        /// <returns>The UNC path (if available)</returns>
        public static string PathToUnc(string fileName)
        {
            if (null == fileName || 0 == fileName.Length) return string.Empty;

            fileName = Path.GetFullPath(fileName);
            if (!IsValidFilePath(fileName)) return fileName;

            int nRet = 0;
            UNIVERSAL_NAME_INFO rni = new UNIVERSAL_NAME_INFO();
            int bufferSize = Marshal.SizeOf(rni);

            nRet = UncUtilities.WNetGetUniversalName(
                fileName, UncUtilities.UNIVERSAL_NAME_INFO_LEVEL,
                ref rni, ref bufferSize);

            if (UncUtilities.ERROR_MORE_DATA == nRet)
            {
                IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize); ;
                try
                {
                    nRet = UncUtilities.WNetGetUniversalName(
                        fileName, UncUtilities.UNIVERSAL_NAME_INFO_LEVEL,
                        pBuffer, ref bufferSize);

                    if (UncUtilities.NO_ERROR == nRet)
                    {
                        rni = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer,
                            typeof(UNIVERSAL_NAME_INFO));
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pBuffer);
                }
            }

            switch (nRet)
            {
                case UncUtilities.NO_ERROR:
                    return rni.lpUniversalName;

                case UncUtilities.ERROR_NOT_CONNECTED:
                    //Local file-name
                    ShareCollection shi = LocalShares;
                    if (null != shi)
                    {
                        Share share = shi[fileName];
                        if (null != share)
                        {
                            string path = share.Path;
                            if (null != path && 0 != path.Length)
                            {
                                int index = path.Length;
                                if (Path.DirectorySeparatorChar != path[path.Length - 1])
                                    index++;

                                if (index < fileName.Length)
                                    fileName = fileName.Substring(index);
                                else
                                    fileName = string.Empty;

                                fileName = Path.Combine(share.ToString(), fileName);
                            }
                        }
                    }

                    return fileName;

                default:
                    Console.WriteLine("Unknown return value: {0}", nRet);
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns the local <see cref="Share"/> object with the best match
        /// to the specified path.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Share PathToShare(string fileName)
        {
            if (null == fileName || 0 == fileName.Length) return null;

            fileName = Path.GetFullPath(fileName);
            if (!IsValidFilePath(fileName)) return null;

            ShareCollection shi = LocalShares;
            if (null == shi)
                return null;
            else
                return shi[fileName];
        }

        #endregion

        #region Local shares

        /// <summary>The local shares</summary>
        private static ShareCollection _local = null;

        /// <summary>
        /// Return the local shares
        /// </summary>
        public static ShareCollection LocalShares
        {
            get
            {
                if (null == _local)
                    _local = new ShareCollection();

                return _local;
            }
        }

        /// <summary>
        /// Return the shares for a specified machine
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static ShareCollection GetShares(string server)
        {
            return new ShareCollection(server);
        }

        #endregion

        #region Private Data

        /// <summary>The name of the server this collection represents</summary>
        private string _server;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor - local machine
        /// </summary>
        public ShareCollection()
        {
            _server = string.Empty;
            EnumerateShares(_server, this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Server"></param>
        public ShareCollection(string server)
        {
            _server = server;
            EnumerateShares(_server, this);
        }

        #endregion

        #region Add

        protected void Add(Share share)
        {
            InnerList.Add(share);
        }

        protected void Add(string netName, string path, ShareType shareType, string remark)
        {
            InnerList.Add(new Share(_server, netName, path, shareType, remark));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the name of the server this collection represents
        /// </summary>
        public string Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Returns the <see cref="Share"/> at the specified index.
        /// </summary>
        public Share this[int index]
        {
            get { return (Share)InnerList[index]; }
        }

        /// <summary>
        /// Returns the <see cref="Share"/> which matches a given local path
        /// </summary>
        /// <param name="path">The path to match</param>
        public Share this[string path]
        {
            get
            {
                if (null == path || 0 == path.Length) return null;

                path = Path.GetFullPath(path);
                if (!IsValidFilePath(path)) return null;

                Share match = null;

                for (int i = 0; i < InnerList.Count; i++)
                {
                    Share s = (Share)InnerList[i];

                    if (s.IsFileSystem && s.MatchesPath(path))
                    {
                        //Store first match
                        if (null == match)
                            match = s;

                        // If this has a longer path,
                        // and this is a disk share or match is a special share, 
                        // then this is a better match
                        else if (match.Path.Length < s.Path.Length)
                        {
                            if (ShareType.Disk == s.ShareType || ShareType.Disk != match.ShareType)
                                match = s;
                        }
                    }
                }

                return match;
            }
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Share[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        #endregion
    }
}