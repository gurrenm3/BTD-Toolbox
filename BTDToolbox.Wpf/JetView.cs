using BTDToolbox.Extensions;
using BTDToolbox.Lib.Json;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    public class JetView : TreeView
    {
        public Action<JetViewItem> OnItemSelected { get; set; }
        private Dictionary<TreeViewItem, JetViewItem> _items = new Dictionary<TreeViewItem, JetViewItem>();

        public JetView()
        {

        }

        public JetView(JetFile jetFile)
        {
            SetItems(jetFile, true);
            SetItems(jetFile, false);
        }

        private void SetItems(JetFile jetFile, bool ignoresFiles, bool clearItems = false)
        {
            if (clearItems)
                Items.Clear();

            foreach (ZipEntry entry in jetFile)
            {
                if (ignoresFiles && entry.IsFile)
                    continue;

                if (!ignoresFiles && entry.IsDirectory)
                    continue;

                string entryName = entry.Name.TrimEnd('/');
                string[] split = entryName.Split('/');

                TreeViewItem parentFolder = null;
                for (int j = 0; j < split.Length - 1; j++)
                {
                    if (j == split.Length - 1)
                        break;

                    if (parentFolder != null)
                    {
                        parentFolder = GetTreeItem(parentFolder, split[j]);
                        continue;
                    }
                    parentFolder = GetTreeItem(split[j]);
                }

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = split[split.Length - 1];
                treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;

                JetViewItem jetItem = new JetViewItem();
                jetItem.FilePath = entryName;
                jetItem.Entry = entry;

                _items.Add(treeItem, jetItem);

                if (parentFolder != null)
                    parentFolder.Items.Add(treeItem);
                else
                    Items.Add(treeItem);
            }
        }

        private TreeViewItem GetTreeItem(string name) => GetTreeItem(this, name);

        private TreeViewItem GetTreeItem(ItemsControl parent, string name)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                var treeItem = (TreeViewItem)parent.Items[i];
                if (treeItem.Header.ToString() == name)
                    return treeItem;
            }

            return null;
        }

        private void TreeItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeItem = (TreeViewItem)sender;
            bool success = _items.TryGetValue(treeItem, out var jetItem);

            if (jetItem.Entry.IsFile)
                OnItemSelected?.Invoke(jetItem);
        }
    }
}
