using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class ProcessListView : ListView
    {

        private readonly object _config;

        private readonly ImageList _lvwImages;

        public ProcessListView(object config)
        {
            _config = config;
            
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
                var lvi = new ListViewItem
                {
                    ImageKey = process.ProcessName,
                    Text = process.ProcessName
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