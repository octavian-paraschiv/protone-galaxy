using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OPMedia.Core.Utilities;
using OPMedia.Core.Logging;

namespace OPMedia.Core
{
    public static class PathUtils
    {
        static char[] _dirSeps = new char[] { Path.DirectorySeparatorChar };
        static string _dirSep = new string(_dirSeps);

        static char[] _curDirs = new char[] { '.' };
        static string _curDir = new string(_curDirs);

        static string _parentDir = "..";

        static string _networkPathStart = $"{DirectorySeparator}{DirectorySeparator}";

        public static string DirectorySeparator
        {
            get
            {
                return _dirSep;
            }
        }

        public static char[] DirectorySeparatorChars
        {
            get
            {
                return _dirSeps;
            }
        }

        public static string CurrentDir
        {
            get
            {
                return _curDir;
            }
        }

        public static char[] CurrentDirChars
        {
            get
            {
                return _curDirs;
            }
        }

        public static string ParentDir
        {
            get
            {
                return _parentDir;
            }
        }

        public static string NetworkPathStart
        {
            get
            {
                return _networkPathStart;
            }
        }

        public static bool Exists(string path)
        {
            return (File.Exists(path) || Directory.Exists(path));
        }

        public static string GetDirectoryTitle(string path)
        {
            string parentDir = Path.GetDirectoryName(path);
            if (parentDir == null)
                return path;

            return path.Replace(parentDir, string.Empty).Trim('\\');
        }

        public static string GetExtension(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;

                // Is this a VPath ?? (Catalog explorer item)
                if (path.EndsWith(":"))
                    return ":";

                string ext = Path.GetExtension(path);
                return ext.Trim('.').ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetFileName(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;

                // Is this a VPath ?? (Catalog explorer item)
                if (path.EndsWith(":"))
                    return "";

                string ext = Path.GetFileName(path);
                return ext.Trim('.');
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool ObjectHasAttribute(string path, FileAttributes fa)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return ((attr & fa) == fa);
            }
            catch { }

            return false;
        }

        public static bool PathHasChildFolder(string path, string childName)
        {
            return Directory.Exists(Path.Combine(path, childName));
        }


        public static bool IsRootPath(string path)
        {
            string strRootSpec = Path.GetPathRoot(path).TrimEnd(new char[] { Path.DirectorySeparatorChar });
            string strDirSpec = path.TrimEnd(new char[] { Path.DirectorySeparatorChar });

            // Paths under Windows are case-insensitive
            return (strRootSpec.ToLowerInvariant() == strDirSpec.ToLowerInvariant());
        }

        public static bool PathsAreOnSameRoot(string path1, string path2)
        {
            try
            {
                return (string.Compare(Path.GetPathRoot(path1), Path.GetPathRoot(path2), true) == 0);
            }
            catch { }

            return false;
        }

        public delegate void FileSystemObjectDeleted(string path, bool isFolder);

        public static void DeleteFolderTree(string folder, FileSystemObjectDeleted deletedCB = null)
        {
            if (folder != CurrentDir)
            {
                List<string> subdirs = EnumDirectories(folder);
                foreach (string subdir in subdirs)
                {
                    DeleteFolderTree(subdir, deletedCB);
                }

                List<string> files = EnumFiles(folder);
                foreach (string file in files)
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);

                        if (deletedCB != null)
                            deletedCB(file, false);
                    }
                    catch
                    {
                    }
                }

                try
                {
                    File.SetAttributes(folder, FileAttributes.Directory);
                    Directory.Delete(folder);

                    if (deletedCB != null)
                        deletedCB(folder, true);
                }
                catch
                {
                }
            }
        }

        #region EnumerateDirectories

        public static List<string> EnumDirectoriesUsingMultiFilter(ManualResetEvent abortEvent, string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (string.IsNullOrEmpty(multiFilter))
                return EnumDirectories(abortEvent, sourceFolder, "*", searchOption);

            if (multiFilter.Contains(";") == false)
                return EnumDirectories(abortEvent, sourceFolder, multiFilter, searchOption);

            List<string> subfolders = new List<string>();

            string[] allFilters = multiFilter.Replace(" ;", ";").Replace("; ", ";").Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string filter in allFilters)
            {
                if (abortEvent != null && abortEvent.WaitOne(0))
                    break;

                subfolders.AddRange(EnumDirectories(abortEvent, sourceFolder, filter, searchOption));
            }

            subfolders.Sort();
            return subfolders;
        }

        public static List<string> EnumDirectoriesUsingMultiFilter(string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumDirectoriesUsingMultiFilter(null, sourceFolder, multiFilter, searchOption);
        }


        public static List<string> EnumDirectories(string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return EnumDirectories(null, dir, searchPattern, searchOptions);
        }

        public static List<string> EnumDirectories(ManualResetEvent abortEvent, string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            List<string> dirList = new List<string>();

            try
            {
                if (abortEvent == null || abortEvent.WaitOne(0) == false)
                {
                    Application.DoEvents();
                    InternalEnumDirectories(abortEvent, dir, searchPattern, searchOptions, ref dirList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            dirList.Sort();
            return dirList;
        }

        private static void InternalEnumDirectories(ManualResetEvent abortEvent, string dir, string searchPattern, SearchOption searchOptions,
            ref List<string> dirList)
        {
            try
            {
                if (abortEvent != null && abortEvent.WaitOne(0))
                    return;

                Application.DoEvents();

                List<string> subdirs = InternalEnumSubFolders(dir, searchPattern);
                dirList.AddRange(subdirs);

                if (searchOptions == SearchOption.AllDirectories)
                {
                    foreach (string subdir in Directory.EnumerateDirectories(dir))
                    {
                        if (abortEvent != null && abortEvent.WaitOne(0))
                            return;

                        Application.DoEvents();

                        try
                        {
                            InternalEnumDirectories(abortEvent, subdir, searchPattern, searchOptions, ref dirList);
                        }
                        catch (Exception ex)
                        {
                            string s = ex.Message;
                        }

                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private static List<string> InternalEnumSubFolders(string dir, string searchPattern)
        {
            List<string> subfolders = new List<string>();

            try
            {
                IEnumerable<string> dirList = Directory.EnumerateDirectories(dir, searchPattern, SearchOption.TopDirectoryOnly);
                if (dirList != null)
                {
                    // This is done to exclude the matches on the 8.3 DOS-like file names.
                    // This app is not 16-bit and it does not target 16-bit OS-es.
                    // So we're not interested in the 8.3 DOS-like file names.
                    // We need to keep only the long file names that match the specified pattern.
                    var x = from dd in dirList
                            where StringUtils.StringMatchesPattern(GetDirectoryTitle(dd), searchPattern)
                            select dd;

                    if (x != null)
                        subfolders.AddRange(x.ToList());
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return subfolders;
        }
        #endregion

        #region EnumerateFiles

        public static List<string> EnumFilesUsingMultiFilter(ManualResetEvent abortEvent, string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (string.IsNullOrEmpty(multiFilter))
                return EnumFiles(abortEvent, sourceFolder, "*", searchOption);

            if (multiFilter.Contains(";") == false)
                return EnumFiles(abortEvent, sourceFolder, multiFilter, searchOption);

            List<string> files = new List<string>();

            string[] allFilters = multiFilter.Replace(" ;", ";").Replace("; ", ";").Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string filter in allFilters)
            {
                if (abortEvent != null && abortEvent.WaitOne(0))
                    break;

                files.AddRange(EnumFiles(abortEvent, sourceFolder, filter, searchOption));
            }

            files.Sort();
            return files;
        }

        public static List<string> EnumFilesUsingMultiFilter(string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumFilesUsingMultiFilter(null, sourceFolder, multiFilter, searchOption);
        }

        public static List<string> EnumFiles(string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return EnumFiles(null, dir, searchPattern, searchOptions);
        }

        public static List<string> EnumFiles(ManualResetEvent abortEvent, string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            List<string> fileList = new List<string>();

            try
            {
                if (abortEvent == null || abortEvent.WaitOne(0) == false)
                {
                    Application.DoEvents();
                    InternalEnumFiles(abortEvent, dir, searchPattern, searchOptions, ref fileList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            fileList.Sort();
            return fileList;
        }

        private static void InternalEnumFiles(ManualResetEvent abortEvent, string dir, string searchPattern, SearchOption searchOptions,
            ref List<string> fileList)
        {
            if (abortEvent != null && abortEvent.WaitOne(0))
                return;

            Application.DoEvents();

            List<string> filesInThisFolder = InternalEnumFilesInFolder(dir, searchPattern);
            fileList.AddRange(filesInThisFolder);

            if (searchOptions == SearchOption.AllDirectories)
            {
                var dd = Directory.EnumerateDirectories(dir);
                if (dd != null)
                {
                    List<string> subfolders = dd.ToList();
                    foreach (string subdir in subfolders)
                    {
                        if (abortEvent != null && abortEvent.WaitOne(0))
                            return;

                        Application.DoEvents();

                        try
                        {
                            InternalEnumFiles(abortEvent, subdir, searchPattern, searchOptions, ref fileList);
                        }
                        catch (Exception ex)
                        {
                            string s = ex.Message;
                        }

                        Application.DoEvents();
                    }
                }
            }
        }

        private static List<string> InternalEnumFilesInFolder(string dir, string searchPattern)
        {
            List<string> files = new List<string>();

            try
            {
                IEnumerable<string> fiList = Directory.EnumerateFiles(dir, searchPattern, SearchOption.TopDirectoryOnly);
                if (fiList != null)
                {
                    // This is done to exclude the matches on the 8.3 DOS-like file names.
                    // This app is not 16-bit and it does not target 16-bit OS-es.
                    // So we're not interested in the 8.3 DOS-like file names.
                    // We need to keep only the long file names that match the specified pattern.
                    var x = from fi in fiList
                            where StringUtils.StringMatchesPattern(Path.GetFileName(fi), searchPattern)
                            select fi;

                    if (x != null)
                        files.AddRange(x.ToList());
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return files;
        }

        #endregion

        #region EnumFileSystemEntries
        public static List<string> EnumFileSystemEntriesUsingMultiFilter(ManualResetEvent abortEvent, string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (string.IsNullOrEmpty(multiFilter))
                return EnumFileSystemEntries(abortEvent, sourceFolder, "*", searchOption);

            if (multiFilter.Contains(";") == false)
                return EnumFileSystemEntries(abortEvent, sourceFolder, multiFilter, searchOption);

            List<string> fsiList = new List<string>();

            string[] allFilters = multiFilter.Replace(" ;", ";").Replace("; ", ";").Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string filter in allFilters)
            {
                if (abortEvent != null && abortEvent.WaitOne(0))
                    break;

                fsiList.AddRange(EnumFileSystemEntries(abortEvent, sourceFolder, filter, searchOption));
            }

            return fsiList;
        }

        public static List<string> EnumFileSystemEntriesUsingMultiFilter(string sourceFolder, string multiFilter,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return EnumFileSystemEntriesUsingMultiFilter(null, sourceFolder, multiFilter, searchOption);
        }

        public static List<string> EnumFileSystemEntries(string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return EnumFileSystemEntries(null, dir, searchPattern, searchOptions);
        }

        public static List<string> EnumFileSystemEntries(ManualResetEvent abortEvent, string dir,
            string searchPattern = "*", SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            List<string> fsiList = new List<string>();

            try
            {
                if (abortEvent == null || abortEvent.WaitOne(0) == false)
                {
                    Application.DoEvents();
                    InternalEnumFileSystemEntries(abortEvent, dir, searchPattern, searchOptions, ref fsiList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return fsiList;
        }

        private static void InternalEnumFileSystemEntries(ManualResetEvent abortEvent, string dir, string searchPattern,
            SearchOption searchOptions, ref List<string> fsiList)
        {
            InternalEnumDirectories(abortEvent, dir, searchPattern, searchOptions, ref fsiList);
            InternalEnumFiles(abortEvent, dir, searchPattern, searchOptions, ref fsiList);
        }
        #endregion

        #region IsEmptyFolder

        public static bool IsEmptyFolder(string dir)
        {
            return IsEmptyFolder(null, dir);
        }

        public static bool IsEmptyFolder(ManualResetEvent abortEvent, string dir)
        {
            List<string> fsiList = new List<string>();

            try
            {
                if (abortEvent == null || abortEvent.WaitOne(0) == false)
                {
                    Application.DoEvents();
                    InternalEnumFileSystemEntries(abortEvent, dir, "*", SearchOption.TopDirectoryOnly, ref fsiList);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return (fsiList.Count < 1);
        }
        #endregion

       

        public static bool IsSpecialFolder(string dir1)
        {
            bool ret = false;

            try
            {
                foreach(Environment.SpecialFolder sf in Enum.GetValues(typeof(Environment.SpecialFolder)))
                {
                    string dir2 = Environment.GetFolderPath(sf);

                    DirectoryInfo di1 = new DirectoryInfo(dir1);
                    DirectoryInfo di2 = new DirectoryInfo(dir2);

                    if (string.Compare(di1.FullName, di2.FullName, false) == 0)
                    {
                        ret = true;
                        break;
                    }
                }

                
            }
            catch
            {
                ret = false;
            }

            return ret;
        }


    }
}
