using BTDToolbox.Wpf.Views;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    public class JsonTab : TabItem
    {
        public JetModView ModView { get; set; }
        public string FilePath { get; private set; }
        public JsonEditor Editor { get; private set; }
        public bool HasUnsavedChanges { get; private set; }
        public HeaderedContentControl TabHeader { get => (HeaderedContentControl)Header; }

        private string lastSavedText;

        public JsonTab()
        {

        }

        public JsonTab(string filePath) : this()
        {
            OpenFile(filePath);
        }

        public JsonTab(ZipEntry entry) : this()
        {
            OpenFile(entry);
        }

        public void OpenFile(string filePath)
        {
            FilePath = filePath;

            FileInfo fileInfo = new FileInfo(FilePath);
            var headerControl = new HeaderedContentControl();
            headerControl.Content = fileInfo.Name;
            headerControl.ToolTip = fileInfo.FullName;
            Header = headerControl;

            Editor = CreateEditor();
            Editor.TextChanged += Editor_TextChanged;

            string json = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
            Editor.Text = json;
            lastSavedText = json;
            Content = Editor;
            SetUnsavedChanges();
        }

        public void OpenFile(ZipEntry entry)
        {
            FilePath = entry.Name.TrimEnd('/');

            var headerControl = new HeaderedContentControl();
            headerControl.Content = FilePath.Split('/').Last();
            headerControl.ToolTip = FilePath;
            Header = headerControl;

            Editor = CreateEditor();
            Editor.TextChanged += Editor_TextChanged;

            var stream = ModView.Jet.GetInputStream(entry);
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            Editor.Text = json;
            lastSavedText = json;
            Content = Editor;
            SetUnsavedChanges();
        }

        private JsonEditor CreateEditor()
        {
            JsonEditor editor = new JsonEditor();
            editor.ShowLineNumbers = true;
            editor.FontSize = 16;
            editor.Options.EnableRectangularSelection = true;
            editor.Options.AllowScrollBelowDocument = true;
            editor.Options.CutCopyWholeLine = true;
            editor.Options.EnableHyperlinks = true;
            editor.Options.HighlightCurrentLine = true;

            editor.InstallFoldingMgr();            
            editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Json");

            return editor;
        }

        private async void Editor_TextChanged(object sender, System.EventArgs e)
        {
            SetUnsavedChanges();
            await Editor.UpdateFoldingsAsync();
        }

        private void SetUnsavedChanges()
        {
            HasUnsavedChanges = lastSavedText != Editor.Text;
            if (HasUnsavedChanges)
            {
                TabHeader.Content += " *";
            }
            else
                TabHeader.Content = TabHeader.Content.ToString().TrimEnd('*').Trim();
        }

        public void Save()
        {
            File.WriteAllBytes(FilePath, Encoding.UTF8.GetBytes(Editor.Text));
            lastSavedText = Editor.Text;
            SetUnsavedChanges();
        }
    }
}
