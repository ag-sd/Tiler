using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class PlacementEntryPanel : Panel
    {
        private Placement _placement;
        
        public PlacementEntryPanel()
        {
            InitUi();
        }
        
        public Placement Placement
        {
            set  {
                _placement = value;
                _tsbDelete.Enabled = _placement.IsCustom;
                UpdateFields();
            }
        }

        private void InitUi()
        {
            SuspendLayout();

            var layout = new TableLayoutPanel {Dock = DockStyle.Fill};
            
            layout.SuspendLayout();
            
            var x = new FieldLabelPanel(_name, "Name", false);
            layout.Controls.Add(x, 0, 0);
            layout.SetColumnSpan(x, 2);
            
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
        }

        private void UpdateFields()
        {
            _txtH.Value = (decimal) _placement.HeightPercent * 100;
            _txtW.Value = (decimal) _placement.WidthPercent * 100;
            _txtX.Value = (decimal) _placement.LeftPercent * 100;
            _txtY.Value = (decimal) _placement.TopPercent * 100;
            _name.Text = _placement.Name;
        }
        
        private readonly TextBox _name = new TextBox();
        
        private readonly ToolStripButton _tsbDelete = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_cancel"), 
                (sender, args) => {  })
            {Alignment = ToolStripItemAlignment.Right};
        
        private readonly ToolStripButton _tsbSave = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_save"), 
                (sender, args) => {  })
            {Alignment = ToolStripItemAlignment.Right};
            
        private readonly ToolStripButton _tsbAdd = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("lvw_add"), 
                (sender, args) => {  })
            {Alignment = ToolStripItemAlignment.Right};

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