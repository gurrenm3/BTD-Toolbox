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

namespace BTDToolbox.Wpf.Views
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
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

            List<GameType> jetModGames = new List<GameType>() { GameType.BloonsTD5, GameType.BloonsTDB, GameType.BloonsMC };
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

            projPass_Grid.Visibility = selectedGame.HasJetFile() ? Visibility.Visible : Visibility.Hidden;

            bool isPassKnown = selectedGame == GameType.BloonsMC || selectedGame == GameType.BloonsTD5;
            jetPass_TextBox.IsReadOnly = isPassKnown ? true : false;
            jetPass_TextBox.Text = isPassKnown ? "Q%_{6#Px]]" : "";

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
                item.Foreground = Brushes.Black;
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

        private void create_Button_Click(object sender, RoutedEventArgs e)
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
            else
            {
                string filePath = projLocation_TextBox.Text + "\\" + projName_TextBox.Text;
                filePath += !filePath.EndsWith(ToolboxProject.fileExtension) ? ToolboxProject.fileExtension : "";

                if (File.Exists(filePath))
                {
                    var popupAction = new PopupAction(() => { OpenProject(filePath); }, () =>  {  return; });
                    Popup.Show("A project with that name already exists. Do you want to replace it?", popupAction);
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

            MainWindow mainWindow = new MainWindow(project);
            mainWindow.Show();
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
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.Title = "Select save location";
            folderPicker.InputPath = Environment.CurrentDirectory;
            if (folderPicker.ShowDialog().HasValue)
            {
                projLocation_TextBox.Text = folderPicker.ResultPath;
            }
        }
    }
}
