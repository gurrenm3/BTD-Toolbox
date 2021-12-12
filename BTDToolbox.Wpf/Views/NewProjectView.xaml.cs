using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Extensions;
using BTDToolbox.Lib.Enums;
using BTDToolbox.Lib;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Wpf.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;


namespace BTDToolbox.Wpf.Views;

/// <summary>
/// Interaction logic for NewProjectView.xaml
/// </summary>
public partial class NewProjectView : UserControl
{
    private List<NewProjectItem> supportedProjectTypes;
    private GameType selectedGame = GameType.None;
    private GameInfo selectedGameInfo;

    /// <summary>
    /// Creates the NewProjectView and initializes the supported games/project types.
    /// </summary>
    public NewProjectView()
    {
        InitializeComponent();

        // Get all project types that toolbox can currently handle.
        supportedProjectTypes = new List<NewProjectItem>()
        {
            new NewProjectItem("Map Mod", ToolboxData.GamesWithMapMods, ModType.Map),
            new NewProjectItem("Jet Mod", ToolboxData.GamesWithJetMods, ModType.Jet)
        };

        // Populate project types in this window.
        gameTypesLB.Items.Clear();
        Enum.GetValues<GameType>().Where(game => game != GameType.None && IsGameSupported(game)).ForEach(game =>
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = game.ToString();
            gameTypesLB.Items.Add(item);
        });
    }

    /// <summary>
    /// Called whenever the user selects a different game for their mod.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void gameTypesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedGame = GetSelectedGame();
        if (selectedGame == GameType.None)
            return;

        selectedGameInfo = Settings.Instance.GetGameInfo(selectedGame);

        // Show/Hide jet password stuff depending on if the game has a jet with a password.
        bool hasJetFile = selectedGame.HasJetFile();
        projPass_Grid.Visibility = (hasJetFile && selectedGame != GameType.BloonsTDB2) ? Visibility.Visible : Visibility.Hidden;
        if (hasJetFile)
        {
            bool isPassKnown = selectedGame == GameType.BloonsMC || selectedGame == GameType.BloonsTD5;
            jetPass_TextBox.IsReadOnly = isPassKnown ? true : false;
            jetPass_TextBox.Text = isPassKnown ? "Q%_{6#Px]]" : "";
        }

        // Set project location to default path and move cursor to end.
        projLocation_TextBox.Text = selectedGameInfo.GetDefaultProjectDir();
        projLocation_TextBox.Select(projLocation_TextBox.Text.Length, 0);
        projLocation_TextBox.CaretIndex = projLocation_TextBox.Text.Length;
        projLocation_TextBox.Focus();

        // Populate available mod types
        projTypesLB.Items.Clear();
        supportedProjectTypes.Where(proj => proj.Games.Contains(selectedGame)).ForEach(proj =>
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = proj.ItemName;
            projTypesLB.Items.Add(item);
        });
    }

    /// <summary>
    /// Used to set the location for this Toolbox project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void browseLocation_Button_Click(object sender, RoutedEventArgs e)
    {
        string defaultDir = $"{Environment.CurrentDirectory}\\Toolbox Projects";
        Directory.CreateDirectory(defaultDir);

        FolderPicker folderPicker = new FolderPicker();
        folderPicker.Title = "Select Save Location";
        folderPicker.InputPath = defaultDir;
        if (folderPicker.ShowDialog().HasValue)
        {
            projLocation_TextBox.Text = folderPicker.ResultPath;
        }
    }

    /// <summary>
    /// Returns which game the user has currently selected.
    /// </summary>
    /// <returns></returns>
    private GameType GetSelectedGame()
    {
        if (gameTypesLB.SelectedIndex < 0)
            return GameType.None;

        var listboxItem = (ListBoxItem)gameTypesLB.SelectedItem;
        var selectedGame = Enum.Parse<GameType>(listboxItem?.Content?.ToString());
        return selectedGame;
    }

    /// <summary>
    /// Returns which mod type the user currently has selected.
    /// </summary>
    /// <returns></returns>
    private ModType GetSelectedMod()
    {
        if (projTypesLB.SelectedIndex < 0)
            return ModType.None;

        var listboxItem = (ListBoxItem)projTypesLB.SelectedItem;
        var selectedMod = Enum.Parse<ModType>(listboxItem?.Content?.ToString().Replace("Mod", "").Replace(" ", ""));
        return selectedMod;
    }

    /// <summary>
    /// Returns whether or not this game supports the provided mod type.
    /// </summary>
    /// <param name="gameToCheck"></param>
    /// <param name="mod"></param>
    /// <returns></returns>
    private bool DoesGameSupportMod(GameType gameToCheck, ModType mod)
    {
        if (gameToCheck == GameType.None || mod == ModType.None)
            return false;

        return supportedProjectTypes.Any(type => type.ModType == mod && type.Games.Contains(gameToCheck));
    }

    /// <summary>
    /// Called when the user clicks the "Create" button. Used to actually create the project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void create_Button_Click(object sender, RoutedEventArgs e)
    {
        if (await IsAllInfoProvided() == false)
            return;

        string filePath = Path.Combine(projLocation_TextBox.Text, projName_TextBox.Text);
        filePath += !filePath.EndsWith(ToolboxProject.fileExtension) ? ToolboxProject.fileExtension : "";

        if (File.Exists(filePath))
        {
            bool replaceFile = false;
            await Popup.Show("A project with that name already exists. Do you want to replace it?",
                "Replace Existing Project?", () => replaceFile = true, null);
            if (!replaceFile) return;
        }

        var project = CreateProject(filePath);
        new MainWindow(project).Show();
        Window.GetWindow(this).Close();
    }

    /// <summary>
    /// Called when the user clicks "Create" button. Checks the UI to see if all required info was provided to make a project.
    /// Shows popup error with missing info if there is any.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> IsAllInfoProvided()
    {
        string error = "";
        var selectedMod = GetSelectedMod();

        if (selectedGame == GameType.None)
        {
            error += "• You need to select a game to mod!\n";
        }
        if (selectedMod == ModType.None)
        {
            error += "• You need to select a mod!\n";
        }
        if ((selectedGame != GameType.None && selectedMod != ModType.None) && !DoesGameSupportMod(selectedGame, selectedMod))
        {
            error += "• The game you selected doesn't support that mod!\n";
        }
        if (projName_TextBox.Text == "")
        {
            error += "• You need to give your project a name!\n";
        }
        if (projLocation_TextBox.Text == "")
        {
            error += "• You need to select a project location!\n";
        }

        bool isInfoGood = string.IsNullOrEmpty(error);
        if (!isInfoGood)
            await Popup.Show(error, "You are Missing Info");

        return isInfoGood;
    }

    /// <summary>
    /// Creates the Toolbox project and saves it to file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private ToolboxProject CreateProject(string filePath)
    {
        ToolboxProject project = new ToolboxProject(selectedGame, filePath);

        if (GetSelectedMod() == ModType.Jet)
        {
            JetMod mod = new JetMod();
            mod.LastJetPassword = jetPass_TextBox.Text;
            project.JetProject = mod;
        }

        project.SaveToFile();
        return project;
    }

    /// <summary>
    /// Returns whether or not Toolbox currently supports modding this game.
    /// </summary>
    /// <param name="game">Game to check.</param>
    /// <returns>True if game can be modded at all using Toolbox, otherwise false.</returns>
    private bool IsGameSupported(GameType game) => supportedProjectTypes.Any(type => type.Games.Contains(game));

    /// Take user back to Welcome screen
    private void back_Button_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).ChangeView<WelcomeViewModel>();
}
