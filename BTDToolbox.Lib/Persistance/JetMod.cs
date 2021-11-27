using System.Collections.Generic;

namespace BTDToolbox.Lib.Persistance
{
    public class JetMod : ToolboxMod
    {
        public string LastJetPassword { get; set; }
        public List<string> LastOpenedFiles { get; set; }

        public JetMod()
        {
            ModKind = Enums.ModType.Jet;
        }
    }
}
