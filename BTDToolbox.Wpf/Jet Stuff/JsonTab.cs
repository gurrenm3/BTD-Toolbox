using BTDToolbox.Lib;
using BTDToolbox.Wpf.Views;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

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

        public JsonTab(ZipEntry entry, ZipFile containingZip) : this()
        {
            OpenFile(entry, containingZip);
        }

        public void OpenFile(string filePath)
        {
            FilePath = filePath;

            FileInfo fileInfo = new FileInfo(FilePath);
            var headerControl = new HeaderedContentControl();
            headerControl.Content = fileInfo.Name;
            headerControl.ToolTip = fileInfo.FullName;
            headerControl.MouseDown += HeaderControl_MouseDown;
            Header = headerControl;

            Editor = CreateEditor();
            Editor.TextChanged += Editor_TextChanged;

            string json = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
            json = JValue.Parse(json).ToString(Formatting.Indented);
            Editor.Text = json;
            lastSavedText = json;
            Content = Editor;
            SetUnsavedChanges();
        }

        private void HeaderControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                CloseTab();
            }
        }

        public void OpenFile(ZipEntry entry, ZipFile containingZip)
        {
            FilePath = entry.Name.TrimEnd('/');

            var headerControl = new HeaderedContentControl();
            headerControl.Content = FilePath.Split('/').Last();
            headerControl.ToolTip = FilePath;
            headerControl.MouseDown += HeaderControl_MouseDown;
            Header = headerControl;

            Editor = CreateEditor();
            Editor.TextChanged += Editor_TextChanged;

            string json = "";
            if (BinEncryption.IsEncrypted(containingZip, entry))
            {
                json = BinEncryption.DecryptFile(containingZip, entry);
            }
            else
            {
                var stream = containingZip.GetInputStream(entry);
                using var reader = new StreamReader(stream);
                json = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(json))
                return;

            // this line auto formats json
            try { json = JValue.Parse(json).ToString(Formatting.Indented); }
            catch {  }

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

            
            using var xmlReader = new System.Xml.XmlTextReader(GetHighlightDefStream());
            editor.SyntaxHighlighting = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);

            editor.Background = FindResource("backgroundColor") as SolidColorBrush;
            editor.Foreground = FindResource("foregroundColor") as SolidColorBrush;

            return editor;
        }

        private MemoryStream GetHighlightDefStream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(Properties.Resources.BJson);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async void Editor_TextChanged(object sender, System.EventArgs e)
        {
            SetUnsavedChanges();
            try
            {
                await Editor.UpdateFoldingsAsync();
            }
            catch (System.Exception)
            {
                Popup.ShowError("Can't open this file!");
                CloseTab();
            }
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

        public void CloseTab()
        {
            ModView.TryCloseTab(this);            
        }

        public void Save()
        {
            File.WriteAllBytes(FilePath, Encoding.UTF8.GetBytes(Editor.Text));
            lastSavedText = Editor.Text;
            SetUnsavedChanges();
        }
    }
}