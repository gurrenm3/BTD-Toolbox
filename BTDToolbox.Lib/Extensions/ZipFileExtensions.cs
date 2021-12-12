using BTDToolbox.Lib;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace BTDToolbox.Extensions
{
    public static class ZipFileExtensions
    {

        public static HashSet<string> GetAllDirectories(this ZipFile zipFile)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (ZipEntry entry in zipFile)
                result.Add(entry.GetDirectory());

            return result;
        }

        public static void ForEach(this ZipFile zipFile, Action<ZipEntry> action)
        {
            for (int i = 0; i < zipFile.Count; i++)
            {
                action.Invoke(zipFile[i]);
            }
        }

        public static bool Any(this ZipFile zipFile, Func<ZipEntry, bool> match)
        {
            for (int i = 0; i < zipFile.Count; i++)
            {
                if (match.Invoke(zipFile[i]))
                    return true;
            }
            return false;
        }

        public static List<ZipEntry> FindAll(this ZipFile zipFile, Func<ZipEntry, bool> match)
        {
            List<ZipEntry> entries = new List<ZipEntry>();
            for (int i = 0; i < zipFile.Count; i++)
            {
                if (match.Invoke(zipFile[i]))
                    entries.Add(zipFile[i]);
            }
            return entries;
        }

        public static ZipEntry FirstOrDefault(this ZipFile zipFile, Func<ZipEntry, bool> match)
        {
            for (int i = 0; i < zipFile.Count; i++)
            {
                if (match.Invoke(zipFile[i]))
                    return zipFile[i];
            }
            return null;
        }
    }
}
