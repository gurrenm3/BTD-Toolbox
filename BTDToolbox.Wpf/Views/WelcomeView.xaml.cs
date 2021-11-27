using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using BTDToolbox.Wpf.Windows;
using BTDToolbox.Lib.Persistance;

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

        private void openProject_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Toolbox Files | *.toolbox";
            openFileDialog.Title = "Browse for project";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog().HasValue)
            {
                if (string.IsNullOrEmpty(openFileDialog.FileName))
                    return;

                var project = ToolboxProject.LoadFromFile(openFileDialog.FileName);
                MainWindow mainWindow = new MainWindow(project);
                mainWindow.Show();
                Window.GetWindow(this).Close();
            }
        }

        private void continueWithoutCode_Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            Window.GetWindow(this).Close();
        }

        private void cloneProject_Button_Click(object sender, RoutedEventArgs e)
        {
            Popup.Show("This feature hasn't been implimented yet.");
        }

        private void SetWindowView<T>() where T : IViewModel, new() => Window.GetWindow(this).ChangeView<T>();
    }
}
