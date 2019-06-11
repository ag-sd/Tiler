using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class ApplicationEditor : Panel
    {
        private ProcessListItem _currentApplication;
        public ApplicationEditor()
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

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill };
            layout.SuspendLayout();
            layout.Controls.Add(_lblCaption, 0, 0);
            layout.Controls.Add(_desktopMap, 0, 3);
            layout.Controls.Add(new FieldLabelPanel(_cmbPlacement, "Region"){Dock = DockStyle.Fill}, 0, 1);
            layout.Controls.Add(new FieldLabelPanel(_cmbDesktop, "Desktop"){Dock = DockStyle.Fill}, 0, 2);

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
            if ((Placement) _cmbPlacement.SelectedItem == _currentApplication.Placement) return;
            
            var placement = (Placement) _cmbPlacement.SelectedItem;
            _currentApplication.Placement = placement;
            _currentApplication.Desktop = _cmbDesktop.SelectedItem.ToString();
            _desktopMap.Placement = placement;
        }
    }
}