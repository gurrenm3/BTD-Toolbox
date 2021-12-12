using BTDToolbox.Lib.Web;
using BTDToolbox.Wpf.Windows;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BTDToolbox.Wpf.Web
{
    class UpdateHandler
    {
        public string DownloadUrl { get; private set; }
        public string ReleaseApiUrl { get; private set; }

        public UpdateHandler(string releaseApiUrl, string downloadUrl)
        {
            ReleaseApiUrl = releaseApiUrl;
            DownloadUrl = downloadUrl;
        }

        public async Task HandleUpdates()
        {
            UpdateChecker updateChecker = new UpdateChecker(ReleaseApiUrl);

            try
            {
                var releaseInfo = await updateChecker.GetReleaseInfoAsync();
                var latestRelease = releaseInfo[0];
                bool isUpdate = updateChecker.IsUpdate(MainWindow.versionNumber, latestRelease);
                if (!isUpdate)
                    return;

                Action onYesClicked = new Action(() => {
                    try { Process.Start(new ProcessStartInfo(DownloadUrl) { UseShellExecute = true }); }
                    catch (Exception)
                    {
                        Popup.ShowError("Unexpected error: Failed to load downloads page. You can" +
                            $" manually download the update here: {DownloadUrl}");
                    }
                });

                Popup.Show("An update is available for BTD Toolbox! Do you want to download it?", "An Update is Available!", onYesClicked, null);
            }
            catch (Exception)
            {
                Popup.ShowError("Failed to check for updates");
            }
        }
    }
}
