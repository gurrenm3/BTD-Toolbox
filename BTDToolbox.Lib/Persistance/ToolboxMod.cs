using BTDToolbox.Lib.Enums;

namespace BTDToolbox.Lib.Persistance
{
    public class ToolboxMod
    {
        public ModType ModKind { get; set; } = ModType.None;

        public ToolboxMod()
        {

        }

        public ToolboxMod(ModType mod)
        {
            ModKind = mod;
        }
    }
}
