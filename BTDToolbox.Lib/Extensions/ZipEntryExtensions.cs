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
    }
}
