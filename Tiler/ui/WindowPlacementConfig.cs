using System.Windows.Forms;
using Tiler.ui.custom;

namespace Tiler.ui
{
    public partial class WindowPlacementConfig : Form
    {
        private readonly ProcessListView _lvw;
        private readonly AppPlacementEditor _editor;
        
        public WindowPlacementConfig()
        {
            InitializeComponent();
            _lvw = new ProcessListView();
            _editor = new AppPlacementEditor();
            InitUi();
        }
        private void InitUi()
        {
            split.Panel1.Controls.Add(_lvw);
            _lvw.Dock = DockStyle.Fill;
            _lvw.ItemSelectionChanged += (sender, args) =>
            {
                if (args.IsSelected && args.Item != null)
                {
                    var item = (ProcessListItem) args.Item;
                    _editor.SetApplication(item);
                }
            };
            split.Panel2.Controls.Add(_editor);
            _editor.Dock = DockStyle.Fill;
        }
    }
}