using System;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using Tiler.runtime;
using Tiler.ui.custom;

namespace Tiler.ui.settings
{
    public class PlacementConfigPage : UserControl
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
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
                SubItems.Add(new ListViewSubItem(this, Placement.IsCustom ? "Custom" : "Preset"));
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
                Dock = DockStyle.Fill,
            };
            _lvw.Columns.Add("Name", -1, HorizontalAlignment.Left);
            _lvw.Columns.Add("X", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Y", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Width", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Height", -2, HorizontalAlignment.Left);
            _lvw.Columns.Add("Type", -2, HorizontalAlignment.Left);
            _lvw.ItemSelectionChanged += (sender, args) =>
            {
                if (!args.IsSelected || args.Item == null) return;
                var item = (PlacementListItem) args.Item;
                log.Info("Placement selected " + item.Placement);
                _editor.SetPlacement(item.Placement);
            };

            _editor = new CustomPlacementEditor {Dock = DockStyle.Fill};
            foreach (var placement in Placement.Values)
            {
                _lvw.Items.Add(new PlacementListItem(placement));
            }

            _lvw.Items[0].Selected = true;

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

            _split.SplitterWidth = 4;
            _split.FixedPanel = FixedPanel.Panel1;
            
            _split.Panel1.Controls.Add(_lvw);
            _split.Panel2.Controls.Add(_editor);

            Controls.Add(_split);
            _split.ResumeLayout(false);
            ResumeLayout(false);

            _editor.PlacementChangedEvent += (source, args) =>
            {
                if(!PlacementChangeType.None.Equals(args.ChangeType)) log.Info(
                    $"Placement Editing completed. Action was {args.ChangeType}. Placement affected was {args.Placement}");
                
                switch (args.ChangeType)
                {
                    case PlacementChangeType.None:
                        break;
                    case PlacementChangeType.Added:
                        _lvw.Items.Add(new PlacementListItem(args.Placement));
                        break;
                    case PlacementChangeType.Updated:
                        DeletePlacementFromListView(args.Placement);
                        _lvw.Items.Add(new PlacementListItem(args.Placement));
                        break;
                    case PlacementChangeType.Deleted:
                        DeletePlacementFromListView(args.Placement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private void DeletePlacementFromListView(Placement placement)
        {
            foreach (ListViewItem listViewItem in _lvw.Items)
            {
                if (!listViewItem.Text.Equals(placement.Name)) continue;
                _lvw.Items.Remove(listViewItem);
                return;
            }
        }
    }
}