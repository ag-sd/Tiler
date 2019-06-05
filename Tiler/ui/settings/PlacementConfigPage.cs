using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;
using Tiler.ui.custom;

namespace Tiler.ui.settings
{
    public class PlacementConfigPage : UserControl
    {
        private class PlacementListItem : ListViewItem
        {
            public PlacementListItem(Placement placement)
            {
                Placement = placement;
                Text = Placement.Name;
                SubItems.Add(new ListViewSubItem(this, StringVal(Placement.LeftPercent)));
                SubItems.Add(new ListViewSubItem(this, StringVal(Placement.TopPercent)));
                SubItems.Add(new ListViewSubItem(this, StringVal(Placement.WidthPercent)));
                SubItems.Add(new ListViewSubItem(this, StringVal(Placement.HeightPercent)));
            }

            public Placement Placement { get; }

            private static string StringVal(float value) => $"{value * 100}%";
        }
        
        private readonly ListView _lvw;
        private readonly SplitContainer _split;
        private readonly CustomPlacementEditor _editor;

        public PlacementConfigPage()
        {
            _lvw = new ListView
            {
                View = View.Details,
                LabelEdit = false,
                AllowColumnReorder = false,
                FullRowSelect = true,
                GridLines = false,
                Sorting = SortOrder.Descending,
                Dock = DockStyle.Fill
            };
            _lvw.Columns.Add("Name", -1, HorizontalAlignment.Left);
            _lvw.Columns.Add("X", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Y", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Width", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Height", -2, HorizontalAlignment.Left);
            _lvw.ItemSelectionChanged += (sender, args) =>
            {
                if (!args.IsSelected || args.Item == null) return;
                var item = (PlacementListItem) args.Item;
                _editor.SetPlacement(item.Placement);
            };

            _editor = new CustomPlacementEditor {Dock = DockStyle.Fill};
            foreach (var placement in Placement.Values)
            {
                _lvw.Items.Add(new PlacementListItem(placement));
            }
            
            _split = new SplitContainer();
            InitUi();
        }

        private void InitUi()
        {
            _split.SuspendLayout();
            SuspendLayout();

            _split.Dock = DockStyle.Fill;
            _split.Size = new Size(500, 273);
            _split.SplitterDistance = 275;
            // This splitter moves in 10-pixel increments.
           
            _split.SplitterWidth = 4;
            _split.FixedPanel = FixedPanel.Panel1;
            
            _split.Panel1.Controls.Add(_lvw);
            _split.Panel2.Controls.Add(_editor);

            Controls.Add(_split);
            _split.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}