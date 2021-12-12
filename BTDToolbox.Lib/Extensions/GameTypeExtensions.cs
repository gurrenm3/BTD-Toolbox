using BTDToolbox.Lib;

namespace BTDToolbox.Extensions
{
    public static class GameTypeExtensions
    {
        public static bool HasJetFile(this GameType gameType)
        {
            return gameType == GameType.BloonsTDB || gameType == GameType.BloonsMC || gameType == GameType.BloonsTD5;
        }
    }
}
