using BTDToolbox.Lib;
using System.Collections.Generic;

namespace BTDToolbox.Wpf
{
    public static class ToolboxData
    {
        /// <summary>
        /// Current version of Toolbox.
        /// </summary>
        public static readonly string versionNumber = "0.0.1";

        /// <summary>
        /// Contains games that support jet mods that can be made within Toolbox.
        /// </summary>
        public static List<GameType> GamesWithJetMods { get; } = new List<GameType>()
        {
            GameType.BloonsMC,
            GameType.BloonsTD5,
            GameType.BloonsTDB,
            GameType.BloonsTDB2
        };

        /// <summary>
        /// Contains games that support custom maps that can be made within Toolbox.
        /// </summary>
        public static List<GameType> GamesWithMapMods { get; } = new List<GameType>()
        {
            GameType.BloonsTD6
        };
    }
}
