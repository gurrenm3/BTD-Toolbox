using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using System;
using System.Windows;
using BTDToolbox.Lib.Persistance;
using System.Windows.Controls;
using BTDToolbox.Wpf.Views;

namespace BTDToolbox.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string versionNumber = "0.0.1";
        public static MainWindow Instance { get; set; }
        public ToolboxProject Project { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        public MainWindow(ToolboxProject project) : this()
        {
            Project = project;
            OpenMod(project.JetProject);
        }

        public void OpenMod(ToolboxMod mod)
        {
            var tab = new ToolboxTabItem(mod);
            tab.Header = mod.ModKind + " Mod";

            switch (mod.ModKind)
            {
                case Lib.Enums.ModType.Jet:
                    tab.Content = new JetModView(Project);
                    break;
                case Lib.Enums.ModType.Map:
                    break;
                default:
                    break;
            }

            modsTabControl.Items.Add(tab);
        }

        public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);

        public static void ChangeView<T>() where T : IViewModel, new() => Instance.ChangeView<T>();
    }
}
