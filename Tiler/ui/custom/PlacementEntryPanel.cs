using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public delegate void PlacementChangedEvent(object source, PlacementChangedEventArgs e);

    public enum PlacementChangeType
    {
        None, Added, Updated, Deleted
    }

    public class PlacementChangedEventArgs : EventArgs
    {
        public Placement Placement { get; }
        
        public PlacementChangeType ChangeType { get; }
       
        public PlacementChangedEventArgs(Placement placement, PlacementChangeType changeType)
        {
            Placement = placement;
            ChangeType = changeType;
        }
    }

        
    public class PlacementEntryPanel : Panel
    {
        private Placement _placement;

        public event PlacementChangedEvent PlacementChangedEvent;

        public PlacementEntryPanel()
        {
            _nameField = new FieldLabelPanel(_name, "Name", false);
            InitUi();
        }
        
        public Placement Placement
        {
            set  {
                _placement = value;
                UpdateFields();
                UpdateToolButtons(_placement);
            }
        }

        private void InitUi()
        {
            SuspendLayout();

            var layout = new TableLayoutPanel {Dock = DockStyle.Fill};
            
            layout.SuspendLayout();
            
            layout.Controls.Add(_nameField, 0, 0);
            layout.SetColumnSpan(_nameField, 2);
            
            layout.Controls.Add(_tb, 3, 0);
            
            layout.Controls.Add(new FieldLabelPanel(_txtX, "X", false), 0, 1);
            layout.Controls.Add(new FieldLabelPanel(_txtY, "Y", false), 0, 2);
            layout.Controls.Add(new FieldLabelPanel(_txtW, " Width", false), 1, 1);
            layout.Controls.Add(new FieldLabelPanel(_txtH, "Height", false), 1, 2);
            
            _tb.Items.Add(_tsbSave);
            _tb.Items.Add(new ToolStripSeparator{Alignment = ToolStripItemAlignment.Right});
            _tb.Items.Add(_tsbDelete);
            _tb.Items.Add(new ToolStripSeparator{Alignment = ToolStripItemAlignment.Right});
            _tb.Items.Add(_tsbAdd);
            _tb.Items.Add(new ToolStripSeparator{Alignment = ToolStripItemAlignment.Right});

            Controls.Add(layout);

            layout.ResumeLayout();
            ResumeLayout();
            
            _txtH.ValueChanged += PlacementValueChanged;
            _txtW.ValueChanged += PlacementValueChanged;
            _txtX.ValueChanged += PlacementValueChanged;
            _txtY.ValueChanged += PlacementValueChanged;
            _name.TextChanged += PlacementValueChanged;

            _tsbSave.Click += ToolStripButtonClick;
            _tsbAdd.Click += ToolStripButtonClick;
            _tsbDelete.Click += ToolStripButtonClick;
        }

        private void ToolStripButtonClick(object sender, EventArgs e)
        {
            var newPlacement = GetPlacement();
            var changeType = (PlacementChangeType) ((ToolStripButton) sender).Tag;
            switch (changeType)
            {
                case PlacementChangeType.Added:
                case PlacementChangeType.Updated:
                    INISettings.SavePlacement(newPlacement);
                    break;
                case PlacementChangeType.Deleted:
                    INISettings.DeletePlacement(newPlacement);
                    break;
                case PlacementChangeType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            PlacementChangedEvent?.Invoke(this, new PlacementChangedEventArgs(newPlacement, changeType));
        }
        
        private void PlacementValueChanged(object sender, EventArgs e)
        {
            var placement = GetPlacement();
            UpdateToolButtons(placement);
            PlacementChangedEvent?.Invoke(this, new PlacementChangedEventArgs(placement, PlacementChangeType.None));
        }

        private void UpdateFields()
        {
            _txtH.Value = (decimal) _placement.HeightPercent * 100;
            _txtW.Value = (decimal) _placement.WidthPercent * 100;
            _txtX.Value = (decimal) _placement.LeftPercent * 100;
            _txtY.Value = (decimal) _placement.TopPercent * 100;
            _name.Text = _placement.Name;
        }

        private void UpdateToolButtons(Placement newPlacement)
        {
            _tsbDelete.Enabled = _placement.IsCustom;
            _tsbAdd.Enabled = !_placement.Name.Equals(_name.Text);
            _tsbSave.Enabled = !_tsbAdd.Enabled && _tsbDelete.Enabled && !_placement.SizeMatches(newPlacement);
        }

        private Placement GetPlacement()
        {
            return new Placement(_name.Text, 
                (float) _txtX.Value / 100, 
                (float)_txtY.Value / 100, 
                (float)_txtW.Value / 100,
                (float)_txtH.Value / 100,
                true);
        }

        private readonly TextBox _name = new TextBox();
        private readonly FieldLabelPanel _nameField;
        
        private readonly ToolStripButton _tsbDelete = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_cancel"))
            {Alignment = ToolStripItemAlignment.Right, Enabled = false, 
                ToolTipText = "Delete", Tag = PlacementChangeType.Deleted};
        
        private readonly ToolStripButton _tsbSave = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_save"))
            {Alignment = ToolStripItemAlignment.Right, Enabled = false, 
                ToolTipText = "Save", Tag = PlacementChangeType.Updated};
            
        private readonly ToolStripButton _tsbAdd = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_add"))
            {Alignment = ToolStripItemAlignment.Right, Enabled = false, 
                ToolTipText = "Add", Tag = PlacementChangeType.Added};

        private readonly ToolStrip _tb = new ToolStrip
        {
            GripStyle = ToolStripGripStyle.Hidden,
            ImageScalingSize = new Size(16,16),
            BackColor = SystemColors.Control
        };
        private readonly NumericUpDown _txtX = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Width = 43
        };
        private readonly NumericUpDown _txtY = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Width = 43
        };
        private readonly NumericUpDown _txtW = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Width = 43
        };

        private readonly NumericUpDown _txtH = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            Width = 43
        };
    }
}