using BTDToolbox.Lib.Enums;
using BTDToolbox.Lib.Persistance;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    internal class ToolboxTabItem : TabItem
    {
        public ModType ModKind { get; private set; }
        public ToolboxMod Mod { get; private set; }

        public ToolboxTabItem()
        {

        }

        public ToolboxTabItem(ToolboxMod mod) : this()
        {
            SetMod(mod);
        }

        public void SetMod(ToolboxMod mod)
        {
            ModKind = mod.ModKind;
            Mod = mod;
        }
    }
}
