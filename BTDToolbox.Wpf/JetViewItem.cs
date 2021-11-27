using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    public class JetViewItem
    {
        public string FilePath { get; set; }
        public ZipEntry Entry { get; set; }
        public TreeViewItem TreeItem { get; set; }

        public JetViewItem()
        {

        }
    }
}
