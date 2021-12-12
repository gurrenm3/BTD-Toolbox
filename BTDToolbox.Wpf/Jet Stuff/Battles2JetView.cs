using BTDToolbox.Extensions;
using BTDToolbox.Lib;
using BTDToolbox.Lib.Json;
using BTDToolbox.Wpf.Views;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace BTDToolbox.Wpf;

public class Battles2JetView : JetView
{
    public Battles2JetView(JetModView parent) : base(parent)
    {
    }

    public override bool AddDirectory(string dirPath)
    {
        if (!base.AddDirectory(dirPath))
            return false;

        var assetTree = AllJetItems.FirstOrDefault(item => item.TreeItem.Header.ToString() == "asset_bundles");
        if (assetTree == null)
            return false;

        var jetFiles = new DirectoryInfo(dirPath).GetFiles("*.jet", SearchOption.AllDirectories);

        // Get all directories without duplicates
        HashSet<string> allJetDirectories = new HashSet<string>();
        foreach (var file in jetFiles)
        {
            JetFile jet = new JetFile(file.FullName);
            foreach (var dir in jet.GetAllDirectories())
                allJetDirectories.Add(dir);
        }

        AddAllJetDirectories(allJetDirectories, assetTree.TreeItem);

        List<JetEntry> jetEntries = new List<JetEntry>();
        foreach (var file in jetFiles)
        {
            JetFile jet = new JetFile(file.FullName);
            foreach (ZipEntry entry in jet)
            {
                var jetEntry = new JetEntry();
                jetEntry.ContainingJet = jet;
                jetEntry.Entry = entry;
                jetEntries.Add(jetEntry);
            }    
        }

        AddAllJetFiles(jetEntries, assetTree.TreeItem);

        return true;
    }

    public bool AddAllJetDirectories(HashSet<string> allJetDirectories, TreeViewItem assetTree)
    {       

        // add directories to jet view
        foreach (var dir in allJetDirectories)
        {
            string path = dir.Replace("\\", "/");
            if (path.StartsWith("game_data/game_project/assets"))
                path = path.Remove(0, 29);

            path = path.Trim('/');
            TreeViewItem parentFolder = assetTree;

            string[] split = path.Split('/');
            for (int index = 0; index < split.Length - 1; index++)
            {
                TreeViewItem previousItem = parentFolder;

                // try finding the folder with this name
                foreach (TreeViewItem item in parentFolder.Items)
                {
                    if (item.Header.ToString() == split[index])
                        parentFolder = item;
                }

                // Parent is still the header so the folder doesn't exist yet
                if (parentFolder == previousItem)
                {
                    var itemToAddTo = parentFolder.Items;

                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = split[index];
                    treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;
                    

                    JetViewItem jetItem = new JetViewItem();
                    jetItem.FilePath = dir;
                    jetItem.TreeItem = treeItem;
                    jetItem.isDirectory = true;
                    AllJetItems.Add(jetItem);

                    itemToAddTo.Add(treeItem);
                    parentFolder = treeItem;
                }
            }
        }

        return true;
    }


    public bool AddAllJetFiles(List<JetEntry> allJetFiles, TreeViewItem assetTree)
    {
        // add files to jet view
        foreach (var entry in allJetFiles)
        {
            string path = entry.Entry.Name.Replace("\\", "/");
            if (path.StartsWith("game_data/game_project/assets"))
                path = path.Remove(0, 29);

            path = path.Trim('/');
            TreeViewItem parentFolder = assetTree;

            string[] split = path.Split('/');
            for (int index = 0; index < split.Length; index++)
            {
                TreeViewItem previousItem = parentFolder;

                // try finding the folder with this 
                parentFolder.Items.ForEach<TreeViewItem>(item =>
                {
                    if (item.Header.ToString() == split[index])
                        parentFolder = item;
                });

                // Parent is still the header so the folder doesn't exist yet
                if (parentFolder == previousItem)
                {
                    var itemToAddTo = parentFolder.Items;

                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = split[index];
                    treeItem.PreviewMouseDoubleClick += TreeItem_MouseDown;

                    JetViewItem jetItem = new JetViewItem();
                    jetItem.Entry = entry.Entry;
                    jetItem.ContainingJet = entry.ContainingJet;
                    jetItem.TreeItem = treeItem;
                    jetItem.isDirectory = false;
                    AllJetItems.Add(jetItem);

                    itemToAddTo.Add(treeItem);
                    parentFolder = treeItem;
                }
            }
        }

        return true;
    }




    public StackPanel CustomTreeViewItem(string itemText, System.Drawing.Bitmap img)
    {
        var newImg = new System.Windows.Controls.Image();
        newImg.Source = img.ToBitmapSource();
        newImg.Width = 16;
        newImg.Height = 16;

        // Create TextBlock
        TextBlock lbl = new TextBlock();
        lbl.Text = "  " + itemText;

        // Add to stack
        StackPanel stkPanel = new StackPanel();
        stkPanel.Orientation = Orientation.Horizontal;

        stkPanel.Children.Add(newImg);
        stkPanel.Children.Add(lbl);


        return stkPanel;
    }
}
