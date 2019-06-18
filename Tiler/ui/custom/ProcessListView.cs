using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using Tiler.Properties;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class ProcessListItem : ListViewItem
    {
        private Placement _placement;
        private string _monitor;
        private readonly int id;
        
        public ProcessListItem(int id, string name, string caption)
        {
            this.id = id;
            Text = name;
            Caption = caption;
            // Check for a saved placement
            (_placement, _monitor) = INISettings.GetAppPlacement(name);
            SubItems.Add(new ListViewSubItem(this, _placement.Name));
            SubItems.Add(new ListViewSubItem(this, _monitor));
        }

        private void SavePlacement()
        {
            INISettings.SaveAppPlacement(Text, _placement, _monitor);
        }

        public string Caption { get; }

        public int Id => id;

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

        public string Monitor
        {
            get => _monitor;
            set
            {
                _monitor = value;
                SubItems[2].Text = value;
                SavePlacement();
            }
        }
    }

    public class ProcessListView : ListView
    {
        private static readonly ILog log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
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
            //Sorting = SortOrder.Descending;
            Columns.Add("Process", -2, HorizontalAlignment.Left);
            Columns.Add("Region", -2, HorizontalAlignment.Left);
            Columns.Add("Monitor", -2, HorizontalAlignment.Left);
            SmallImageList = _lvwImages;

            _lvwImages.ColorDepth = ColorDepth.Depth32Bit;
            _lvwImages.ImageSize = new Size(16, 16);
            _lvwImages.TransparentColor = Color.Black;
            
            Program.ProcessMonitor.ProcessesAddedEvent += ProcessMonitorOnProcessesAddedEvent;
            Program.ProcessMonitor.ProcessesRemovedEvent += ProcessMonitorOnProcessesRemovedEvent;
        }

        private void ProcessMonitorOnProcessesRemovedEvent(object source, ProcessChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke( ( MethodInvoker ) delegate { RemoveItems(e.Processes); });
            }
            else
            {
                RemoveItems(e.Processes);
            }
        }

        private void ProcessMonitorOnProcessesAddedEvent(object source, ProcessChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke( ( MethodInvoker ) delegate { AddItems(e.Processes); });
            }
            else
            {
                AddItems(e.Processes);
            }
        }

        private void AddItems(IEnumerable<int> processIds)
        {
            BeginUpdate();
            foreach (var id in processIds)
            {
                AddListViewItem(id);
            }
            EndUpdate();
        }

        private void RemoveItems(ICollection<int> processIds)
        {
            BeginUpdate();
            foreach (ProcessListItem item in Items)
            {
                if(!processIds.Contains(item.Id)) continue;
                log.Info($"Removing {item.Text} from list");
                Items.Remove(item);
            }
            EndUpdate();
        }

        private void AddListViewItem(int processId)
        {
            var process = Process.GetProcessById(processId);
            log.Info($"Adding {process.ProcessName} to list");
            _lvwImages.Images.Add(process.ProcessName, GetProcessIcon(process));
            var lvi = new ProcessListItem(processId, process.ProcessName, process.MainWindowTitle)
            {
                ImageKey = process.ProcessName
            };
            Items.Insert(Items.Count, lvi);
        }

        private void ShowProcesses()
        {
            foreach (var id in Program.ProcessMonitor.GetProcessIds())
            {
                AddListViewItem(id);
            }
        }

        private static Icon GetProcessIcon(Process process)
        {
            try
            {
                if (process.MainModule != null) return Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            }
            catch (Exception)
            {
                log.Error($"Could not extract the icon for process {process.ProcessName}. A default will be used");
            }

            return (Icon) Resources.ResourceManager.GetObject("app_unknown");
        }

    }
}