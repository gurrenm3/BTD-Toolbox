using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Wpf.Views;
using BTDToolbox.Extensions;
using System.Threading.Tasks;
using System.Windows;
using BTDToolbox.Lib;
using System.IO;
using System;
using System.Windows.Media.Converters;
using System.Diagnostics;

namespace BTDToolbox.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance of the currently opened Main Window.
        /// </summary>
        public static MainWindow Instance { get; private set; }

        /// <summary>
        /// The Toolbox Project that is currently opened.
        /// </summary>
        public ToolboxProject Project { get; private set; }
        

        /// <summary>
        /// Creates the Main Window, assigning <see cref="Instance"/> and <see cref="Project"/>.
        /// </summary>
        /// <param name="project"></param>
        public MainWindow(ToolboxProject project)
        {
            InitializeComponent();
            Instance = this;
            Project = project;
        }


        /// <summary>
        /// Opens a toolbox mod in the ModsTabControl and initializes the necessary code/UI elements.
        /// </summary>
        /// <param name="mod">Mod to initialize.</param>
        public void OpenMod(ToolboxMod mod)
        {
            var tab = new ToolboxTabItem(mod);
            tab.Header = mod.ModKind + " Mod";

            switch (mod.ModKind)
            {
                case Lib.Enums.ModType.Jet:
                    tab.Content = new JetModView(Project);
                    break;
                case Lib.Enums.ModType.Map: // not setup yet
                    break;
                default:
                    break;
            }

            modsTabControl.SelectedIndex = modsTabControl.Items.Add(tab);
        }

        /// <summary>
        /// Called when the Window is loaded. Used to verify Toolbox data and load project if all data is good.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool isEverythingOkay = await RunToolboxChecks();
            if (!isEverythingOkay)
            {
                await Popup.Show("Toolbox was unable to get some information that is crucial to running properly." +
                    " As a result you will be returned to the Welcome Screen...", "Failed To Load Properly");

                ReturnToMainMenu();
                return;
            }

            OpenMod(Project.JetProject);
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// If this is called then one of the Toolbox Checks failed. This is used to return the user to the main menu
        /// because some critical info was invalid.
        /// </summary>
        private void ReturnToMainMenu()
        {
            new StartupWindow().Show();
            Close();
        }

        /// <summary>
        /// Performs important checks about Toolbox and the current opened project to make sure all of the
        /// critical data is valid. <br/>If not the user should be returned to the main menu to prevent unknown errors.
        /// </summary>
        /// <returns>True if all checks pass and all data is valid. Will return false if even one check fails, 
        /// in which case the user should be returned to the main menu.</returns>
        private async Task<bool> RunToolboxChecks()
        {
            // Toolbox project is bad.
            if (Project == null)
            {
                await Popup.Show("Toolbox project was null/invalid when trying to load into the program.", 
                    "Project was null");
                return false;
            }

            // Game info is bad.
            var gameInfo = Settings.Instance?.GetGameInfo(Project.Game);
            if (gameInfo == null)
            {
                await Popup.Show($"Toolbox encountered an unknown error. Failed to get game info for {Project.Game}.",
                    "Unknown error occured.");
                return false;
            }

            // Game directory not set.
            if (!gameInfo.IsGameDirValid())
            {
                await Popup.Show($"You have not set the path for {gameInfo.Game}. Toolbox needs to know the game path to run properly." +
               $" Please select the path for {gameInfo.Game}'s EXE file. It's in the same folder that you installed the game to.", 
               $"Browse for {gameInfo.Game}");

                if (!await BrowseForGamePath(gameInfo))
                {
                    bool success = false;
                    while (true)
                    {
                        bool tryAgain = true;
                        await Popup.Show("Would you like to try setting the path again?", "Try Again?", null, () => { tryAgain = false; });
                        if (!tryAgain) break;

                        if (await BrowseForGamePath(gameInfo))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (!success) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Used to actually browse for the game's path. Also shows Popup errors if there are issues with the path selected.
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <returns></returns>
        private async Task<bool> BrowseForGamePath(GameInfo gameInfo)
        {
            string path = gameInfo.BrowseForGamePath();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                await Popup.Show("You did not select the game path or it was invalid. " +
                    "Toolbox can't work properly without having the game's directory", "Bad Path Selected");
                return false;
            }

            if (!path.TrimEnd('/').TrimEnd('\\').EndsWith(GameInfo.gameExeNames[gameInfo.Game]))
            {
                await Popup.Show($"You did not select the correct EXE file for {gameInfo.Game}. " +
                    $"Please select an EXE file named \"{GameInfo.gameExeNames[gameInfo.Game]}\"", "Wrong EXE Selected");
                return false;
            }

            gameInfo.GamePath = new FileInfo(path).DirectoryName;
            Settings.Instance.Save();
            await Popup.Show($"{gameInfo.Game} path was successfully set to \"{gameInfo.GamePath}\"", "Success");
            return true;
        }

        /// <summary>
        /// Run's an action on the UI thread.
        /// </summary>
        /// <param name="action">codeto run on the UI thread.</param>
        public static void RunOnUIThread(Action action) => Instance.Dispatcher.Invoke(action);

        /// <summary>
        /// Changes the View that is currently displayed in the Main Window. Will replace whatever was previously displayed.
        /// </summary>
        /// <typeparam name="T">The ViewModel to show.</typeparam>
        public static void ChangeView<T>() where T : IViewModel, new() => Instance.ChangeView<T>();
    }
}
