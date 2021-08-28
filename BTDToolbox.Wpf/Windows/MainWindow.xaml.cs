using System;
using System.Windows;

namespace BTDToolbox.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string versionNumber = "0.0.0";
        public static MainWindow Instance { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);
    }
}
