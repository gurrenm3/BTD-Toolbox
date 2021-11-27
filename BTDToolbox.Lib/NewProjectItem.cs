using BTDToolbox.Lib.Enums;
using System.Collections.Generic;

namespace BTDToolbox.Lib
{
    public class NewProjectItem
    {
        public string ItemName { get; set; }
        public List<GameType> Games { get; set; }
        public ModType ModType { get; set; }

        public NewProjectItem()
        {

        }

        public NewProjectItem(string name, GameType game, ModType type)
        {
            ItemName = name;
            Games = new List<GameType>() { game };
            ModType = type;
        }

        public NewProjectItem(string name, List<GameType> games, ModType type)
        {
            ItemName = name;
            Games = games;
            ModType = type;
        }
    }
}
