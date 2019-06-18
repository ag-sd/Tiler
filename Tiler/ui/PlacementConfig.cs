using System;
using System.Windows.Forms;
using Tiler.runtime;
using Tiler.ui.custom;

namespace Tiler.ui
{
    public partial class PlacementConfig : Form
    {
        public PlacementConfig()
        {
            InitializeComponent();
            
            InitUi();
        }

        private void InitUi()
        {
            SuspendLayout();
            Margin = new Padding(10);
            Padding = new Padding(10);
            
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4
            };
            layout.SuspendLayout();
            
            layout.Controls.Add(_desktopMap, 2, 0);
            layout.SetRowSpan(_desktopMap, 6);
            layout.SetColumnSpan(_desktopMap, 2);
            
//            var cmbPlacementField = new FieldLabelPanel(_cmbPlacement, "Saved Placements"){Dock = DockStyle.Fill};
//            layout.Controls.Add(cmbPlacementField, 0, 0);
//            layout.SetColumnSpan(cmbPlacementField, 2);
//            _cmbPlacement.SelectedValueChanged += CmbPlacement_SelectionChanged;

            var txtNameField = new FieldLabelPanel(_txtName, "Name"){Dock = DockStyle.Fill};
            layout.Controls.Add(txtNameField, 0, 1);  
            layout.SetColumnSpan(txtNameField, 2);
//            
//            layout.Controls.Add(new FieldLabelPanel(new TextBox(), "Left"), 0, 2);   
//            layout.Controls.Add(new FieldLabelPanel(new TextBox(), "Top"){Dock = DockStyle.Fill}, 1, 2);   
//            layout.Controls.Add(new FieldLabelPanel(new TextBox(), "Width"){Dock = DockStyle.Fill}, 0, 3);   
//            layout.Controls.Add(new FieldLabelPanel(new TextBox(), "height"){Dock = DockStyle.Fill}, 1, 3);
//
//            var separator = new Label{AutoSize = false, Dock = DockStyle.Bottom, Height = 2, BorderStyle = BorderStyle.Fixed3D};
//            layout.Controls.Add(separator, 0, 4);
//            layout.SetColumnSpan(separator, 2);
//            
//            layout.Controls.Add(new Button(), 0, 5);
//            layout.Controls.Add(new Button(), 1, 5);
//            
            layout.ResumeLayout();
            
            Controls.Add(layout);
            
            ResumeLayout();
        }

        private void CmbPlacement_SelectionChanged(object sender, EventArgs e)
        {
            var placement = (Placement) _cmbPlacement.SelectedItem;
            _txtName.Text = placement.Name;
        }
        
        private readonly PlacementComboBox _cmbPlacement = new PlacementComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly DesktopMap _desktopMap = new DesktopMap{Dock = DockStyle.Fill};
        private readonly TextBox _txtName = new TextBox();

//        private readonly DesktopMap _
//        private readonly DesktopMap _
//        private readonly DesktopMap _
    }
}