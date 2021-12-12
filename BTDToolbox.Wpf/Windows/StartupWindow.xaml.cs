using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using BTDToolbox.Wpf.Web;
using BTDToolbox.Lib;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace BTDToolbox.Wpf.Windows;

/// <summary>
/// Interaction logic for StartupWindow.xaml
/// </summary>
public partial class StartupWindow : Window
{
    /// <summary>
    /// Instance of the currently opened StartUp Window.
    /// </summary>
    public static StartupWindow Instance { get; private set; }
    private static bool _checkedForUpdates = false;

    public StartupWindow()
    {
        InitializeComponent();
        Instance = this;
        ChangeView<WelcomeViewModel>();
    }

    /// <summary>
    /// Called after literally everything has finished loading. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Window_ContentRendered(object sender, System.EventArgs e)
    {
        Settings.Load();
        //await HandleUpdates();
    }

    /// <summary>
    /// Check for updates.
    /// </summary>
    /// <returns></returns>
    private async Task HandleUpdates()
    {
        if (_checkedForUpdates) return;

        string downloadUrl = "https://github.com/gurrenm3/BTD-Toolbox/releases";
        const string updateUrl = "https://api.github.com/repos/gurrenm3/BTD-Toolbox/releases";

        UpdateHandler updateHandler = new UpdateHandler(updateUrl, downloadUrl);
        await updateHandler.HandleUpdates();
        _checkedForUpdates = true;
    }


    /// <summary>
    /// Run an Action on the UI thread.
    /// </summary>
    /// <param name="action">Code to run.</param>
    public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);


    /// <summary>
    /// Change the view that's currently being displayed on the window.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void ChangeView<T>() where T : IViewModel, new() => Instance.ChangeView<T>();
}