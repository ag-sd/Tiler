using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class AppPlacementEditor : Panel
    {
        private ProcessListItem _currentApplication;
        public AppPlacementEditor()
        {
            InitUi();
        }

        public void SetApplication(ProcessListItem applicationItem)
        {
            _currentApplication = applicationItem;
            _lblCaption.Text = applicationItem.Caption;
            _desktopMap.Placement = applicationItem.Placement;
            _cmbPlacement.SetItem(applicationItem.Placement);
            _cmbDesktop.SetItem(applicationItem.Desktop);
        }

        private void InitUi()
        {
            SuspendLayout();
            Margin = new Padding(10);
            Padding = new Padding(10);
            
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnStyles = {new ColumnStyle(SizeType.Percent, 100)},
                RowStyles = { new RowStyle(SizeType.Percent, 10), new RowStyle(SizeType.Percent, 60)}
            };
            layout.SuspendLayout();
            layout.Controls.Add(_lblCaption, 0, 0);
            layout.Controls.Add(_desktopMap, 0, 1);
            layout.Controls.Add(new FieldLabelPanel(_cmbPlacement, "Placement"){Dock = DockStyle.Fill}, 0, 2);
            layout.Controls.Add(new FieldLabelPanel(_cmbDesktop, "Desktop"){Dock = DockStyle.Fill}, 0, 3);

            _cmbPlacement.SelectedIndexChanged += CmbPlacement_SelectionChanged;

            layout.ResumeLayout();
            
            Controls.Add(layout);
            
            ResumeLayout();
        }

        private readonly Label _lblCaption = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter, 
            Anchor = AnchorStyles.Top, 
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoEllipsis = true
        };
        private readonly DesktopMap _desktopMap = new DesktopMap {Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly DesktopComboBox _cmbDesktop = new DesktopComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly PlacementComboBox _cmbPlacement = new PlacementComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        
        private void CmbPlacement_SelectionChanged(object sender, EventArgs  e)
        {
            if (_cmbPlacement.SelectedItem == null || _currentApplication == null) return;
            if (_cmbPlacement.SelectedItem == _currentApplication.Placement) return;
            
            var placement = (Placement) _cmbPlacement.SelectedItem;
            _currentApplication.Placement = placement;
            _currentApplication.Desktop = _cmbDesktop.SelectedItem.ToString();
            _desktopMap.Placement = placement;
        }
    }
}