using BTDToolbox.Extensions;
using BTDToolbox.Lib;
using BTDToolbox.Lib.Json;
using BTDToolbox.Lib.Persistance;
using BTDToolbox.Wpf.Views;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    public class JetView : TreeView
    {
        public List<JetViewItem> AllJetItems { get; protected set; } = new List<JetViewItem>();
        public Action<JetViewItem> OnItemSelected { get; set; }
        public ToolboxProject Project { get => parent?.Project; }

        public readonly JetModView parent;

        public JetView(JetModView parent)
        {
            this.parent = parent;
        }

        public TreeViewItem GetParentFolderByPath(string fullPath, string basePath, TreeViewItem parentFolder = null)
        {
            string itemName = fullPath.Replace(basePath, "").Trim('/').Trim('\\');
            string[] split = itemName.Split('\\');

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

            return parentFolder;
        }


        protected TreeViewItem GetTreeItem(string name) => GetTreeItem(this, name);

        protected TreeViewItem GetTreeItem(ItemsControl parent, string name)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                var treeItem = (TreeViewItem)parent.Items[i];
                if (treeItem.Header.ToString() == name)
                    return treeItem;
            }

            return null;
        }        

        protected void AddItems(FileSystemInfo[] itemsToAdd, string basePath)
        {
            foreach (var item in itemsToAdd)
            {
                AddItem(item, basePath);
            }
        }

        protected JetViewItem AddItem(FileSystemInfo itemInfo, string basePath, TreeViewItem parentFolder = null)
        {
            string itemName = itemInfo.FullName.Replace(basePath, "").Trim('/').Trim('\\');
            string[] split = itemName.Split('\\');

            TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = split[split.Length - 1];
            treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;

            JetViewItem jetItem = new JetViewItem();
            jetItem.FilePath = itemInfo.FullName;
            jetItem.TreeItem = treeItem;
            jetItem.isDirectory = string.IsNullOrEmpty(itemInfo.Extension);
            AllJetItems.Add(jetItem);

            if (parentFolder == null)
                parentFolder = GetParentFolderByPath(itemName, basePath);

            var itemToAddTo = parentFolder != null ? parentFolder.Items : Items;
            itemToAddTo.Add(treeItem);
            return jetItem;
        }

        protected JetViewItem AddItem(ZipEntry entry, string basePath, TreeViewItem parentFolder = null)
        {
            string itemName = entry.Name.Replace(basePath, "").Trim('/').Trim('\\');
            string[] split = itemName.Split('\\');

            TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = split[split.Length - 1];
            treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;

            JetViewItem jetItem = new JetViewItem();
            jetItem.FilePath = entry.Name;
            jetItem.TreeItem = treeItem;
            jetItem.isDirectory = entry.IsDirectory;
            jetItem.Entry = entry;
            AllJetItems.Add(jetItem);

            if (parentFolder == null)
                parentFolder = GetParentFolderByPath(itemName, basePath);

            var itemToAddTo = parentFolder != null ? parentFolder.Items : Items;
            itemToAddTo.Add(treeItem);
            return jetItem;
        }

        protected JetViewItem AddItem(string header, bool isDirectory, string fullPath, TreeViewItem parentFolder)
        {
            TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = header;
            treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;

            JetViewItem jetItem = new JetViewItem();
            jetItem.FilePath = fullPath;
            jetItem.TreeItem = treeItem;
            jetItem.isDirectory = isDirectory;
            AllJetItems.Add(jetItem);

            var itemToAddTo = parentFolder != null ? parentFolder.Items : Items;
            itemToAddTo.Add(treeItem);
            return jetItem;
        }

        public virtual bool AddDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                string message = $"Tried adding the directory: \"{dirPath}\" to the {nameof(JetView)} but it doesn't exist!";
                Logger.WriteLine(message);
                Popup.Show(message, "Error!");
                return false;
            }

            var dirInfo = new DirectoryInfo(dirPath);

            // add directories first because it looks nicer
            var allDirectories = dirInfo.GetDirectories("*", SearchOption.AllDirectories);
            AddItems(allDirectories, dirPath);

            var allFiles = dirInfo.GetFiles("*", SearchOption.AllDirectories).ToList();
            allFiles.RemoveAll(f => f.Extension == ".jet");
            AddItems(allFiles.ToArray(), dirPath);
            return true;
        }

        protected void TreeItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeItem = (TreeViewItem)sender;
            var jetItem = AllJetItems.FirstOrDefault(item => item.TreeItem == treeItem);
            
            if (jetItem != null)
                OnItemSelected?.Invoke(jetItem);
        }
    }
}
