using ICSharpCode.SharpZipLib.Zip;

namespace BTDToolbox.Lib.Json
{
    public class JetEntry
    {
        public JetFile ContainingJet { get; set; }

        public ZipEntry Entry { get; set; }
    }
}
