using BTDToolbox.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BTDToolbox.Lib
{
    public class GameBackup
    {
        public GameType Game { get; private set; }
        public GameInfo GameData { get; private set; }

        public GameBackup(GameType game)
        {
            Game = game;
            GameData = Settings.Instance.GetGameInfo(game);
        }

        public bool CheckForBackup() // probably need a lot more logic here
        {
            bool hasBackup = GameData.HasBackup();
            bool isBackupOld = GameData.IsBackupOutOfDate();
            if (!hasBackup || isBackupOld)
            {
                return false;
            }

            return true;
        }

        public async Task CreateBackup()
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
