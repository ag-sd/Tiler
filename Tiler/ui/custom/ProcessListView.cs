using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class ProcessListItem : ListViewItem
    {
        private class SavedPlacement
        {
            public Placement Placement { get; }
            public string Desktop { get; }

            public SavedPlacement(Placement placement, string desktop)
            {
                Placement = placement;
                Desktop = desktop;
            }
        }
        
        private Placement _placement = Placement.None;
        private string _desktop;
        
        public ProcessListItem(string name, string caption)
        {
            Text = name;
            Caption = caption;
            // Check for a saved placement
            if (Properties.Settings.Default == null)
//            {
//                Properties.Settings.Default.Properties.Add(new SettingsProperty());
//                    //["placement"] = new Dictionary<string, SavedPlacement>()
//            }
            var props = (Dictionary<string, SavedPlacement>) Properties.Settings.Default["placement"];
            if (props?[Text] == null)
            {
                SubItems.Add(new ListViewSubItem(this, string.Empty));
                SubItems.Add(new ListViewSubItem(this, string.Empty));
            }
            else
            {
                var savedPlacement = props[Text];
                _placement = savedPlacement.Placement;
                SubItems.Add(new ListViewSubItem(this, savedPlacement.Placement.Name));
                _desktop = savedPlacement.Desktop;
                SubItems.Add(new ListViewSubItem(this, _desktop));
            }
        }

        public void SavePlacement()
        {
            var dict = (Dictionary<string, SavedPlacement>) Properties.Settings.Default["placements"] ?? 
                new Dictionary<string, SavedPlacement>();
            dict[Text] = new SavedPlacement(_placement, _desktop);
            Properties.Settings.Default["placements"] = dict;
            Properties.Settings.Default.Save();
        }
        
        public string Caption { get; }

        public Placement Placement
        {
            get => _placement;
            set
            {
                _placement = value;
                SubItems[0].Name = _placement.Name;
            }
        }
        
        public string Desktop
        {
            get => _desktop;
            set
            {
                _desktop = value;
                SubItems[1].Name = value;
            }
        }
    }
    
    public class ProcessListView : ListView
    {
        private readonly ImageList _lvwImages;

        public ProcessListView()
        {
            _lvwImages = new ImageList();

            InitUi();
            
            ShowProcesses();
        }

        private void InitUi()
        {
            View = View.Details;
            LabelEdit = false;
            AllowColumnReorder = false;
            FullRowSelect = true;
            GridLines = false;
            Sorting = SortOrder.Descending;
            Columns.Add("Process", -2, HorizontalAlignment.Left);
            Columns.Add("Placement", -2, HorizontalAlignment.Left);
            Columns.Add("Desktop", -2, HorizontalAlignment.Left);
            SmallImageList = _lvwImages;
            Font = new Font("Segoe UI", 12f);
            
            _lvwImages.ColorDepth = ColorDepth.Depth32Bit;
            _lvwImages.ImageSize = new Size(32, 32);
            _lvwImages.TransparentColor = Color.White;
        }

        private void ShowProcesses()
        {
            var allProcesses = Process.GetProcesses();

            foreach (var process in allProcesses)
            {
                if (process.MainWindowHandle == IntPtr.Zero || string.IsNullOrEmpty(process.MainWindowTitle)) continue;

                _lvwImages.Images.Add(process.ProcessName, getProcessIcon(process));
                var lvi = new ProcessListItem(process.ProcessName, process.MainWindowTitle)
                {
                    ImageKey = process.ProcessName
                };
                Items.Add(lvi);
            }
        }

        private static Icon getProcessIcon(Process process)
        {
            try
            {
                if (process.MainModule != null) return Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while extracting application icon process=" 
                                + process + " \n Exception is" + ex);
            }

            return SystemIcons.Application;
        }

    }
}