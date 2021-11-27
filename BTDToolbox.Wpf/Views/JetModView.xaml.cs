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

namespace BTDToolbox.Wpf.Views
{
    /// <summary>
    /// Interaction logic for JetModView.xaml
    /// </summary>
    public partial class JetModView : UserControl
    {
        public ToolboxProject Project { get; set; }
        public JsonTab SelectedTab { get; private set; }
        public JetFile Jet { get; private set; }

        public JetModView()
        {
            InitializeComponent();
        }

        public JetModView(ToolboxProject project) : this()
        {
            Project = project;

            Jet = new JetFile(@"F:\Program Files (x86)\Steam\steamapps\common\BloonsTD5\Assets\BTD5.jet");
            Jet.Password = project.JetProject.LastJetPassword;

            var jetView = new JetView(Jet);
            jetView.OnItemSelected = new Action<JetViewItem>(JetItemSelected);
            fileTree.Items.AddRange(jetView.Items);
        }

        public void OpenFile(string filePath)
        {
            if (SelectTab(filePath))
                return;

            var tab = new JsonTab(filePath);
            tab.ModView = this;
            jsonTabControl.Items.Add(tab);
            SelectTab(filePath);
        }

        public void OpenFile(ZipEntry entry)
        {
            string path = entry.Name.TrimEnd('/');
            if (SelectTab(path))
                return;

            var tab = new JsonTab();
            tab.ModView = this;
            tab.OpenFile(entry);
            jsonTabControl.Items.Add(tab);
            SelectTab(path);
        }

        public bool SelectTab(string filePath)
        {
            for (int i = 0; i < jsonTabControl.Items.Count; i++)
            {
                var tab = (JsonTab)jsonTabControl.Items[i];
                if (tab.FilePath != filePath)
                    continue;

                jsonTabControl.SelectedIndex = i;
                SelectedTab = tab;
                return true;
            }
            return false;
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

        private void JetItemSelected(JetViewItem item)
        {
            var entry = Jet.FirstOrDefault(entry => entry.Name.TrimEnd('/') == item.FilePath);
            OpenFile(entry);
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (SelectedTab == null)
                    return;

                e.Handled = true;
                int negativeModifier = e.Delta < 0 ? -1 : 1;
                SelectedTab.Editor.FontSize += 1 * negativeModifier;
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
    }
}
