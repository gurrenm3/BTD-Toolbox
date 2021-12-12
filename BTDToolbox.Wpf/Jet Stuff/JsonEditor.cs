using BTDToolbox.Lib.Json;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using System.Linq;
using System.Threading.Tasks;

namespace BTDToolbox.Wpf
{
    public class JsonEditor : TextEditor
    {
        public FoldingManager FoldingMgr { get; private set; }

        public JsonEditor()
        {
            
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
