using BTDToolbox.Lib;
using Microsoft.Win32;

namespace BTDToolbox.Wpf
{
    public static class GameInfoExtensions
    {
        public static string BrowseForGamePath(this GameInfo gameInfo)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Exe Files|*.exe";
            ofd.Title = "Browse for game exe";
            ofd.ShowDialog();
            return ofd.FileName;
        }
    }
}
