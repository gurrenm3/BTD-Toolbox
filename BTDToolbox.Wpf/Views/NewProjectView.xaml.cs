using BTDToolbox.Extensions;
using BTDToolbox.Lib;
using BTDToolbox.Lib.Enums;
using BTDToolbox.Wpf.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BTDToolbox.Lib.Persistance;
using System.IO;
using BTDToolbox.Lib.UI;
using BTDToolbox.Wpf.Windows;
using System.Diagnostics;

namespace BTDToolbox.Wpf.Views
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        private GameInfo gameInfo;
        private List<NewProjectItem> projectTypes;
        private GameType selectedGame = GameType.None;

        public NewProjectView()
        {
            InitializeComponent();
            projectTypes = GetPossibleProjectTypes();
            LoadSupportedGames();
            gameTypesLB.SelectedIndex = 0;
        }

        private void LoadSupportedGames()
        {
            gameTypesLB.Items.Clear();
            foreach (GameType game in Enum.GetValues(typeof(GameType)))
            {
                if (game == GameType.None)
                    continue;

                if (!IsGameSupported(game))
                    continue;

                ListBoxItem item = new ListBoxItem();
                item.Content = game.ToString();
                gameTypesLB.Items.Add(item);
            }
        }

        private bool IsGameSupported(GameType game)
        {
            return projectTypes.Any(type => type.Games.Contains(game));
        }

        private List<NewProjectItem> GetPossibleProjectTypes()
        {
            List<NewProjectItem> projectItems = new List<NewProjectItem>();
            projectItems.Add(new NewProjectItem("Map Mod", GameType.BloonsTD6, ModType.Map));

            List<GameType> jetModGames = new List<GameType>() { GameType.BloonsTD5, GameType.BloonsTDB, GameType.BloonsMC, GameType.BloonsTDB2 };
            projectItems.Add(new NewProjectItem("Jet Mod", jetModGames, ModType.Jet));

            return projectItems;
        }

        private void gameTypesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gameTypesLB.SelectedIndex < 0)
                return;

            selectedGame = GetSelectedGame();
            if (selectedGame == GameType.None)
                return;

            gameInfo = Settings.Instance.GetGameInfo(selectedGame);
            projPass_Grid.Visibility = selectedGame.HasJetFile() ? Visibility.Visible : Visibility.Hidden;

            bool isPassKnown = selectedGame == GameType.BloonsMC || selectedGame == GameType.BloonsTD5;
            jetPass_TextBox.IsReadOnly = isPassKnown ? true : false;
            jetPass_TextBox.Text = isPassKnown ? "Q%_{6#Px]]" : "";

            projLocation_TextBox.Text = gameInfo.GetDefaultProjectDir();
            projLocation_TextBox.Select(projLocation_TextBox.Text.Length, 0);
            projLocation_TextBox.CaretIndex = projLocation_TextBox.Text.Length;
            projLocation_TextBox.Focus();

            LoadAvailableModProjects();
        }

        private void LoadAvailableModProjects()
        {
            projTypesLB.Items.Clear();
            foreach (var projType in projectTypes)
            {
                if (!projType.Games.Contains(selectedGame))
                    continue;

                ListBoxItem item = new ListBoxItem();
                item.Content = projType.ItemName;
                projTypesLB.Items.Add(item);
            }
        }

        private GameType GetSelectedGame()
        {
            if (gameTypesLB.SelectedIndex < 0)
                return GameType.None;

            var listboxItem = (ListBoxItem)gameTypesLB.SelectedItem;
            var selected = Enum.Parse(typeof(GameType), listboxItem?.Content?.ToString());
            return selected == null ? GameType.None : (GameType)selected;
        }

        private ModType GetSelectedMod()
        {
            if (projTypesLB.SelectedIndex < 0)
                return ModType.None;

            var listboxItem = (ListBoxItem)projTypesLB.SelectedItem;
            bool found = Enum.TryParse(listboxItem?.Content?.ToString().Replace("Mod", "").Replace(" ", ""), out ModType selectedMod);
            return !found ? ModType.None : selectedMod;
        }

        private bool DoesGameSupportMod(GameType game, ModType mod)
        {
            if (game == GameType.None || mod == ModType.None)
                return false;

            return projectTypes.Any(type => type.ModType == mod && type.Games.Contains(game));
        }

        private void back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).ChangeView<WelcomeViewModel>();
        }

        private async void create_Button_Click(object sender, RoutedEventArgs e)
        {
            if (GetSelectedGame() == GameType.None)
            {
                Popup.ShowError("You need to select a game to mod!");
            }
            else if (GetSelectedMod() == ModType.None)
            {
                Popup.ShowError("You need to select a mod!");
            }
            else if (!DoesGameSupportMod(GetSelectedGame(), GetSelectedMod()))
            {
                Popup.ShowError("The game you selected doesn't support that mod!");
            }
            else if (projLocation_TextBox.Text == "")
            {
                Popup.ShowError("You need to select a project location!");
            }
            else if (projName_TextBox.Text == "")
            {
                Popup.ShowError("You need to give your project a name!");
            }
            else if (!gameInfo.IsGameDirValid())
            {
                await Popup.Show($"The path for {selectedGame} has not been set. Please set the path to continue...", $"Select Path for {selectedGame}", async () =>
                {
                    string path = gameInfo.BrowseForGamePath();
                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    {
                        await Popup.Show($"Did not get the path for {selectedGame}.", "No Path Selected.");
                    }
                    else
                    {
                        gameInfo.GamePath = new FileInfo(path).DirectoryName;
                        Settings.Instance.Save();
                        await Popup.Show($"The path for {selectedGame} was successfully set. You can now create your project.", "Game Path Set!");
                    }
                });
            }
            else
            {
                string filePath = projLocation_TextBox.Text + "\\" + projName_TextBox.Text;
                filePath += !filePath.EndsWith(ToolboxProject.fileExtension) ? ToolboxProject.fileExtension : "";

                if (File.Exists(filePath))
                {
                    var popupAction = new PopupAction(() => { OpenProject(filePath); }, () =>  {  return; });
                    await Popup.Show("A project with that name already exists. Do you want to replace it?", "Project already exists", popupAction);
                }
                else
                {
                    OpenProject(filePath);
                }
            }
        }

        private void OpenProject(string filePath)
        {
            ToolboxProject project = CreateToolboxProject(filePath);
            project.SaveToFile();

            new MainWindow(project).Show();
            Window.GetWindow(this).Close();
        }

        private ToolboxProject CreateToolboxProject(string filePath)
        {
            ToolboxProject project = new ToolboxProject(GetSelectedGame(), filePath);
            
            if (GetSelectedMod() == ModType.Jet)
            {
                JetMod mod = new JetMod();
                mod.LastJetPassword = jetPass_TextBox.Text;
                project.JetProject = mod;
            }

            return project;
        }

        private void browseLocation_Button_Click(object sender, RoutedEventArgs e)
        {
            string defaultDir = $"{Environment.CurrentDirectory}\\Toolbox Projects";
            Directory.CreateDirectory(defaultDir);
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.Title = "Select save location";
            folderPicker.InputPath = defaultDir;
            if (folderPicker.ShowDialog().HasValue)
            {
                projLocation_TextBox.Text = folderPicker.ResultPath;
            }
        }
    }
}
