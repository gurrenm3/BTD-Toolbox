using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace BTDToolbox.Lib
{
    public static class ZipEntryExtensions
    {
        /// <summary>
        /// Get the directory of this entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string GetDirectory(this ZipEntry entry)
        {
            return Path.GetDirectoryName(entry.Name);
        }

        /// <summary>
        /// Returns just the name of this entry without it's path included.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string GetFileName(this ZipEntry entry)
        {
            return Path.GetFileName(entry.Name);
        }
    }
}
