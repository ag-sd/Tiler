
using System;
using System.Windows.Forms;
using Tiler.ui.custom;

namespace Tiler.ui
{
    public partial class WindowPlacementConfig : Form
    {
        private readonly ProcessListView _processListView;
        private readonly AppPlacementEditor _appPlacementEditor;
        
        public WindowPlacementConfig()
        {
            InitializeComponent();

            _appPlacementEditor = new AppPlacementEditor {Dock = DockStyle.Fill};
            split.Panel2.Controls.Add(_appPlacementEditor);
            
            _processListView = new ProcessListView{Dock = DockStyle.Fill};
            _processListView.ItemSelectionChanged += ProcessListView_ItemSelectionChanged;
            split.Panel1.Controls.Add(_processListView);
        }

        private void ProcessListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                _appPlacementEditor.SetApplication((ProcessListItem) e.Item);
            }
        }
        
        
    }
}