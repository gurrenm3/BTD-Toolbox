using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Wpf.Windows;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System;

namespace BTDToolbox.Wpf.Views;

/// <summary>
/// Interaction logic for WelcomeView.xaml
/// </summary>
public partial class WelcomeView : UserControl
{
    public WelcomeView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the user clicks the "New Project" button. Used to create new Toolbox Projects.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void newProject_Button_Click(object sender, RoutedEventArgs e) => SetWindowView<NewProjectViewModel>();

    /// <summary>
    /// Used to change the View that is currently being displayed on screen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void SetWindowView<T>() where T : IViewModel, new() => Window.GetWindow(this).ChangeView<T>();

    /// <summary>
    /// Called when the user clicks the "Clone from Github" button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void cloneProject_Button_Click(object sender, RoutedEventArgs e)
    {
        await Popup.Show("This feature hasn't been implimented yet.", "Not Implemented");
    }

    /// <summary>
    /// Called when the user clicks the "Open Project" button. Used to browse for an existing Toolbox Project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void openProject_Button_Click(object sender, RoutedEventArgs e)
    {
        string defaultDir = $"{Environment.CurrentDirectory}\\Toolbox Projects";
        Directory.CreateDirectory(defaultDir);

        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Toolbox Files | *.toolbox";
        ofd.Title = "Browse for project";
        ofd.Multiselect = false;
        ofd.InitialDirectory = defaultDir;

        if (ofd.ShowDialog().HasValue)
        {
            if (string.IsNullOrEmpty(ofd.FileName) || !File.Exists(ofd.FileName))
                return;

            var loadedProject = ToolboxProject.LoadFromFile(ofd.FileName);
            new MainWindow(loadedProject).Show();
            Window.GetWindow(this).Close();
        }
    }
}