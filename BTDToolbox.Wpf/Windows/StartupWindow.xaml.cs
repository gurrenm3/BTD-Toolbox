using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using BTDToolbox.Wpf.Web;
using System;
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
            ChangeView<WelcomeViewModel>();
        }

        private async void Window_ContentRendered(object sender, System.EventArgs e)
        {
            //await HandleUpdates();
        }

        private async Task HandleUpdates()
        {
            string downloadUrl = "https://github.com/gurrenm3/BTD-Toolbox/releases";
            const string updateUrl = "https://api.github.com/repos/gurrenm3/BTD-Toolbox/releases";

            UpdateHandler updateHandler = new UpdateHandler(updateUrl, downloadUrl);
            await updateHandler.HandleUpdates();
        }

        public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);

        public static void ChangeView<T>() where T : IViewModel, new() => Instance.ChangeView<T>();
    }
}
