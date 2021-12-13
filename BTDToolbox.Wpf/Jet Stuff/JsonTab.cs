using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Wpf.Views;
using BTDToolbox.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;
using System;


namespace BTDToolbox.Wpf
{
    /// <summary>
    /// A Tab Item that represents a JSON file.
    /// </summary>
    public class JsonTab : TabItem
    {
        /// <summary>
        /// The Header of this tab.
        /// </summary>
        public HeaderedContentControl TabHeader { get => (HeaderedContentControl)Header; }

        /// <summary>
        /// The Project associated with the file in this tab.
        /// </summary>
        public ToolboxProject Project { get => parent.Project; }

        /// <summary>
        /// Used to determine if there are any unsaved changes in the text editor.
        /// </summary>
        public bool HasUnsavedChanges { get; private set; }

        /// <summary>
        /// The parent of this json tab.
        /// </summary>
        public readonly JetModView parent;

        /// <summary>
        /// The currently opened file.
        /// </summary>
        public readonly JetViewItem currentFile;

        /// <summary>
        /// The Editor that holds the JSON file's contents.
        /// </summary>
        public readonly JsonEditor editor;

        /// <summary>
        /// The text that was last saved.
        /// </summary>
        private string _lastSavedText;


        /// <summary>
        /// Creates a <see cref="JsonTab"/> and initializes the <see cref="JsonEditor"/> which will hold the JSON text.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="file"></param>
        /// <exception cref="NullReferenceException">Throws an exception if either parameter is null.</exception>
        public JsonTab(JetModView parent, JetViewItem file) : base()
        {
            this.parent = parent;
            currentFile = file;
            if (this.parent == null || currentFile == null)
                throw new NullReferenceException("The parent or file that was opened was null");

            Loaded += JsonTab_Loaded;

            // create json editor
            editor = new JsonEditor();
            editor.TextChanged += Editor_TextChanged;
            editor.InstallFoldingMgr();    

            // get highlighting definition from file.
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(Properties.Resources.BJson);
            writer.Flush();
            writer.BaseStream.Position = 0;

            using var xmlReader = new System.Xml.XmlTextReader(stream);
            editor.SyntaxHighlighting = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);

            Content = editor;
        }

        /// <summary>
        /// Opens <see cref="currentFile"/> and populates the JsonEditor with the contents of the file, regardless of whether or not
        /// it is a local file or within a zip file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception if for some reason it's unable to open any file.</exception>
        private async Task OpenFileAsync()
        {
            Header = new HeaderedContentControl();
            TabHeader.MouseDown += HeaderControl_MouseDown;

            string text = "";

            // this is inside of a zip file.
            if (currentFile.ContainingJet != null && currentFile.Entry != null)
            {
                TabHeader.Content = Path.GetFileName(currentFile.Entry.Name);
                TabHeader.ToolTip = currentFile.Entry.Name;
                
                if (BinEncryption.IsEncrypted(currentFile.ContainingJet, currentFile.Entry))
                {
                    text = BinEncryption.DecryptFile(currentFile.ContainingJet, currentFile.Entry);
                }
                else
                {
                    using var stream = currentFile.ContainingJet.GetInputStream(currentFile.Entry);
                    using var streamReader = new StreamReader(stream);
                    text = await streamReader.ReadToEndAsync();
                }
            }

            // this is a local file
            else if (!string.IsNullOrEmpty(currentFile.FilePath) && File.Exists(currentFile.FilePath))
            {
                TabHeader.Content = Path.GetFileName(currentFile.FilePath);
                TabHeader.ToolTip = currentFile.FilePath;

                if (BinEncryption.IsEncrypted(currentFile.FilePath))
                {
                    text = BinEncryption.DecryptFile(currentFile.FilePath);
                }
                else
                {
                    using var stream = new FileStream(currentFile.FilePath, FileMode.Open);
                    using var streamReader = new StreamReader(stream);
                    text = await streamReader.ReadToEndAsync();
                }
            }
            else
            {
                throw new Exception("An unknown error occured and Toolbox failed to read the file");
            }

            editor.Text = JValue.Parse(text).ToString(Formatting.Indented); // will attempt to autoformat the text
            _lastSavedText = editor.Text;
            UpdateUnsavedChanges();
        }


        /// <summary>
        /// Called when the user presses a mouse button on the Header.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle) // close with middle mouse button
            {
                CloseTab();
            }
        }

        /// <summary>
        /// Called whenever the JsonEditor's text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Editor_TextChanged(object sender, System.EventArgs e)
        {
            UpdateUnsavedChanges();
            try
            {
                await editor.UpdateFoldingsAsync();
            }
            catch
            {
                await Popup.ShowError("Unable to open this file because it's most likely not a text file.");
                CloseTab();
            }
        }

        /// <summary>
        /// Updates whether or not there is unsaved text.
        /// </summary>
        private void UpdateUnsavedChanges()
        {
            HasUnsavedChanges = _lastSavedText != editor.Text;
            if (HasUnsavedChanges)
            {
                if (!TabHeader.Content.ToString().EndsWith(" *"))
                    TabHeader.Content += " *";
            }
            else
            {
                TabHeader.Content = TabHeader.Content.ToString().TrimEnd('*').Trim();
            }
        }

        /// <summary>
        /// Saves the currently opened file to the Toolbox Project.
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// Instructs the Parent to close this tab.
        /// </summary>
        public void CloseTab() => parent.TryCloseTab(this);

        /// <summary>
        /// Called when the JsonTab has finished loading. Used to asyncronously open the json file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void JsonTab_Loaded(object sender, System.Windows.RoutedEventArgs e) => await OpenFileAsync();
    }
}