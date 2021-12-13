using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Lib.Json;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using BTDToolbox.Extensions;
using System;
using BTDToolbox.Lib.Enums;
using BTDToolbox.Wpf.Windows;
using BTDToolbox.Wpf.ViewModels;
using BTDToolbox.Lib.UI;
using BTDToolbox.Lib;

namespace BTDToolbox.Wpf.Views
{
    /// <summary>
    /// Interaction logic for JetModView.xaml
    /// </summary>
    public partial class JetModView : UserControl
    {
        public ToolboxProject Project { get; private set; }
        public JsonTab SelectedTab { get => (JsonTab)jsonTabControl.SelectedItem; }
        public JetFile Jet { get; private set; }

        public JetModView()
        {
            InitializeComponent();

            //GameBackup backup = new GameBackup(Project.Game);
        }

        public JetModView(ToolboxProject project) : this()
        {
            Project = project;

            var jetView = CreateJetView();
            if (jetView == null)
                return;


            jetView.OnItemSelected = new Action<JetViewItem>(JetItemSelected);
            fileTree.Items.AddRange(jetView.Items);
        }

        public void OpenFile(JetViewItem itemToOpen)
        {
            if (SelectTab(itemToOpen))
                return;

            var tab = new JsonTab(this, itemToOpen);
            jsonTabControl.SelectedIndex = jsonTabControl.Items.Add(tab);
        }

        public bool SelectTab(JetViewItem itemToSelect)
        {
            var foundTab = jsonTabControl.Items.FirstOrDefault<JsonTab>(item => item.currentFile.Equals(itemToSelect));
            if (foundTab == null)
                return false;

            jsonTabControl.SelectedItem = foundTab;
            return true;
        }

        public List<JsonTab> GetAllTabs()
        {
            if (jsonTabControl.Items.Count == 0)
                return null;

            List<JsonTab> tabs = new List<JsonTab>();
            for (int i = 0; i < jsonTabControl.Items.Count; i++)
                tabs.Add((JsonTab)jsonTabControl.Items[i]);

            return tabs;
        }

        private JetView CreateJetView()
        {
            string gameDir = Settings.Instance.GetGameInfo(Project.Game)?.GamePath;
            if (string.IsNullOrEmpty(gameDir))
                return null;

            if (Project.Game != GameType.BloonsTDB2)
            {
                Popup.Show($"{Project.Game} is not currently supported. You will now be returned to the Main Menu", "Notice",
                    new PopupAction(() =>
                    {
                        new StartupWindow().Show();
                        MainWindow.Instance.Close();
                    }));

                return null;

                // previous code
                /*Jet = new JetFile(@"F:\Program Files (x86)\Steam\steamapps\common\BloonsTD5\Assets\BTD5.jet");
                Jet.Password = Project.JetProject.LastJetPassword;
                return new JetView(Jet);*/
            }

            
            if (Project.Game == GameType.BloonsTDB2)
            {
                string dirToAdd = $"{gameDir}\\game_data";
                Battles2JetView jetView = new Battles2JetView(this);
                jetView.AddDirectory(dirToAdd);
                return jetView;
            }

            return null;
        }

        private void JetItemSelected(JetViewItem item)
        {
            if (item == null || item.isDirectory)
                return;

            OpenFile(item);

            /*if (Project.Game == GameType.BloonsTDB2)
            {
                if (item.ContainingJet != null && item.Entry != null)
                {
                    OpenFile(item.Entry, item.ContainingJet);
                }
                else
                {
                    if (BinEncryption.IsEncrypted(item.FilePath))
                        BinEncryption.DecryptFile(item.FilePath, true);

                    OpenFile(item.FilePath);
                }
            }
            else
            {
                // worry about this later when adding other games.
                *//*if (item.Entry != null)
                    OpenFile(item.Entry);*//*
            }*/
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (SelectedTab == null)
                    return;

                e.Handled = true;
                int negativeModifier = e.Delta < 0 ? -1 : 1;
                SelectedTab.editor.FontSize += 1 * negativeModifier;
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S) && !Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (SelectedTab != null)
                    SelectedTab.Save();
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyDown(Key.LeftShift))
            {
                GetAllTabs().ForEach(tab => tab.Save());
            }
        }

        public void TryCloseTab(JsonTab jsonTab)
        {
            if (!jsonTab.HasUnsavedChanges)
            {
                CloseTab(jsonTab);
            }
            else
            {
                Popup.Show("There are unsaved changes to this tab. Are you sure you want to close?", "Are you sure?", () => CloseTab(jsonTab), null);
            }
        }

        private void CloseTab(JsonTab jsonTab)
        {
            jsonTabControl.Items.Remove(jsonTab);
        }
    }
}
