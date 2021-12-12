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
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
                return ofd.FileName;

            return null;
        }
    }
}
