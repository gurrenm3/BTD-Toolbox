using BTDToolbox.Lib.Json;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BTDToolbox.Wpf
{
    public class JsonEditor : TextEditor
    {
        public FoldingManager FoldingMgr { get; private set; }

        public JsonEditor() : base()
        {
            FontSize = 16;
            ShowLineNumbers = true;
            Background = FindResource("backgroundColor") as SolidColorBrush;
            Foreground = FindResource("foregroundColor") as SolidColorBrush;

            Options.EnableRectangularSelection = true;
            Options.AllowScrollBelowDocument = true;
            Options.CutCopyWholeLine = true;
            Options.EnableHyperlinks = true;
            Options.HighlightCurrentLine = true;
        }

        public void InstallFoldingMgr()
        {
            FoldingMgr = FoldingManager.Install(TextArea);
        }

        public async Task UpdateFoldingsAsync()
        {
            FoldingMgr.Clear();
            BracketPairFinder pairFinder = new BracketPairFinder(Text, '[', ']');
            var pairs = await pairFinder.GetBracketPairsAsync();
            for (int i = 0; i < pairs.Count; i++)
            {
                var pair = pairs.ElementAt(i);
                FoldingMgr.CreateFolding(pair.Key, pair.Value + 1);
            }

            pairFinder = new BracketPairFinder(Text, '{', '}');
            pairs = await pairFinder.GetBracketPairsAsync();
            for (int i = 0; i < pairs.Count; i++)
            {
                var pair = pairs.ElementAt(i);
                FoldingMgr.CreateFolding(pair.Key, pair.Value + 1);
            }
        }

        public void UninstallFoldingMgr()
        {
            FoldingManager.Uninstall(FoldingMgr);
            FoldingMgr = null;
        }
    }
}
