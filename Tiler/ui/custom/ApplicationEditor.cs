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
            _monitorMap.Placement = applicationItem.Placement;
            foreach (var screen in Screen.AllScreens)
            {
                if (!screen.DeviceName.Equals(applicationItem.Monitor)) continue;
                _monitorMap.Screen = screen;
                break;
            }
            _cmbPlacement.SetItem(applicationItem.Placement);
            _cmbMonitor.SetItem(applicationItem.Monitor);
        }

        private void InitUi()
        {
            SuspendLayout();

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill };
            layout.SuspendLayout();
            layout.Controls.Add(_lblCaption, 0, 0);
            layout.Controls.Add(_monitorMap, 0, 3);
            layout.Controls.Add(new FieldLabelPanel(_cmbPlacement, "Region"){Dock = DockStyle.Fill}, 0, 1);
            layout.Controls.Add(new FieldLabelPanel(_cmbMonitor, "Monitor"){Dock = DockStyle.Fill}, 0, 2);

            _cmbPlacement.SelectedIndexChanged += CmbPlacement_SelectionChanged;
            _cmbMonitor.SelectedIndexChanged += CmbMonitorOnSelectedIndexChanged;

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
        private readonly MonitorMap _monitorMap = new MonitorMap {Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly MonitorComboBox _cmbMonitor = new MonitorComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly PlacementComboBox _cmbPlacement = new PlacementComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        
        private void CmbPlacement_SelectionChanged(object sender, EventArgs  e)
        {
            var placement = (Placement) _cmbPlacement.SelectedItem;
            if (_cmbPlacement.SelectedItem == null || _currentApplication == null) return;
            if (placement == _currentApplication.Placement) return;

            _currentApplication.Placement = placement;
//            _currentApplication.Desktop = _cmbDesktop.SelectedItem.ToString();
            _monitorMap.Placement = placement;
        }
        
        private void CmbMonitorOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var monitor = (ScreenItem) _cmbMonitor.SelectedItem;
            if(monitor.Text.Equals(_currentApplication.Monitor) && monitor.Text.Equals(_monitorMap.Screen.DeviceName)) return;

            _currentApplication.Monitor = monitor.ToString();
            _monitorMap.Screen = monitor.Screen;
        }
    }
}