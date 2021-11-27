using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;

namespace BTDToolbox.Extensions
{
    public static class ZipFileExtensions
    {


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
