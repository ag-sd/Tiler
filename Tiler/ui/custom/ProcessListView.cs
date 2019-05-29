using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class ProcessListItem : ListViewItem
    {
        private Placement _placement;
        private string _desktop;
        
        public ProcessListItem(string name, string caption)
        {
            Text = name;
            Caption = caption;
            // Check for a saved placement
            (_placement, _desktop) = INISettings.GetPlacement(name);
            SubItems.Add(new ListViewSubItem(this, _placement.Name));
            SubItems.Add(new ListViewSubItem(this, _desktop));
        }

        private void SavePlacement()
        {
            INISettings.SavePlacement(Text, _placement, _desktop);
        }

        public string Caption { get; }

        public Placement Placement
        {
            get => _placement;
            set
            {
                _placement = value;
                SubItems[1].Text = _placement.Name;
                SavePlacement();
            }
        }

        public string Desktop
        {
            get => _desktop;
            set
            {
                _desktop = value;
                SubItems[2].Text = value;
                SavePlacement();
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
            
            BorderStyle = BorderStyle.None;

            _lvwImages.ColorDepth = ColorDepth.Depth32Bit;
            _lvwImages.ImageSize = new Size(16, 16);
            _lvwImages.TransparentColor = Color.Black;
        }

        private void ShowProcesses()
        {
            var allProcesses = Process.GetProcesses();

            foreach (var process in allProcesses)
            {
                if (process.MainWindowHandle == IntPtr.Zero || string.IsNullOrEmpty(process.MainWindowTitle)) continue;

                _lvwImages.Images.Add(process.ProcessName, GetProcessIcon(process));
                var lvi = new ProcessListItem(process.ProcessName, process.MainWindowTitle)
                {
                    ImageKey = process.ProcessName
                };
                Items.Add(lvi);
            }
        }

        private static Icon GetProcessIcon(Process process)
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

            return (Icon) Resources.ResourceManager.GetObject("app_unknown");
        }

    }
}