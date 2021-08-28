using BTDToolbox.Lib.Web;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace BTDToolbox.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public static StartupWindow Instance { get; set; }

        public StartupWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private async void Window_ContentRendered(object sender, System.EventArgs e)
        {
            await HandleUpdates();
        }

        private async Task HandleUpdates()
        {
            string downloadUrl = "https://github.com/gurrenm3/BTD-Toolbox/releases";
            const string updateUrl = "https://api.github.com/repos/gurrenm3/BTD-Toolbox/releases";
            UpdateChecker updateChecker = new UpdateChecker(updateUrl);
            var releaseInfo = await updateChecker.GetReleaseInfoAsync(updateUrl);
            var latestRelease = releaseInfo[0];
            bool isUpdate = updateChecker.IsUpdate(MainWindow.versionNumber, latestRelease);
            //bool isUpdate = true;
            if (isUpdate)
            {
                Action onYesClicked = new Action(() => {
                    try
                    {
                        Process.Start("google.com");
                        Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });
                    }
                    catch (Exception) 
                    {
                        Popup.ShowError("Unexpected error: Failed to load downloads page. You can" +
                            $" manually download the update here: {downloadUrl}");
                    }
                });

                Popup.Show("An update is available for BTD Toolbox! Do you want to download it?", 
                    onYesClicked, null, "An Update is Available!");
            }
        }

        public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);
    }
}
