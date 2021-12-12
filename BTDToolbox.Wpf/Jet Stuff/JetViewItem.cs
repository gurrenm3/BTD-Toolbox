using BTDToolbox.Lib.Json;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    public class JetViewItem
    {
        public string FilePath { get; set; }

        public bool isDirectory;

        /// <summary>
        /// The ZipEntry that this <see cref="JetViewItem"/> is associated with. 
        /// <br/><br/>Note: Will be null if this file was not in a jet file.
        /// </summary>
        public ZipEntry Entry { get; set; }

        public JetFile ContainingJet { get; set; }

        public TreeViewItem TreeItem { get; set; }


        public JetViewItem()
        {

        }
    }
}
