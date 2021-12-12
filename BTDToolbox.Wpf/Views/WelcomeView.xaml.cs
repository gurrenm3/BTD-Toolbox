using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using BTDToolbox.Wpf.Windows;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Lib;
using System.Diagnostics;
using System;
using System.IO;

namespace BTDToolbox.Wpf.Views
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void newProject_Button_Click(object sender, RoutedEventArgs e)
        {
            SetWindowView<NewProjectViewModel>();            
        }

        private async void openProject_Button_Click(object sender, RoutedEventArgs e)
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
                if (string.IsNullOrEmpty(ofd.FileName))
                    return;

                var project = ToolboxProject.LoadFromFile(ofd.FileName);
                if (project == null)
                    return;

                new MainWindow(project).Show();
                Window.GetWindow(this).Close();
            }
        }

        private async void cloneProject_Button_Click(object sender, RoutedEventArgs e)
        {
            await Popup.Show("This feature hasn't been implimented yet.", "Not Implemented");
        }

        private void SetWindowView<T>() where T : IViewModel, new() => Window.GetWindow(this).ChangeView<T>();
    }
}
