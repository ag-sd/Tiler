using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class CustomPlacementEditor : Panel
    {
        public event PlacementChangedEvent PlacementChangedEvent;
        
        public CustomPlacementEditor()
        {
            InitUi();
        }
        
        public void SetPlacement(Placement placement)
        {
            _lblCaption.Text = placement.Name;
            _monitorMap.Placement = placement;
            _placementEntryPanel.Placement = placement;
        }
        
        private void InitUi()
        {
            SuspendLayout();

            var layout = new TableLayoutPanel {Dock = DockStyle.Fill};
            layout.SuspendLayout();
            
            layout.Controls.Add(_lblCaption, 0, 0);
            layout.Controls.Add(_placementEntryPanel, 0, 1);
            layout.Controls.Add(_monitorMap, 0, 2);

            layout.ResumeLayout();
            
            Controls.Add(layout);
            
            ResumeLayout();

            _placementEntryPanel.PlacementChangedEvent += (source, args) => { _monitorMap.Placement = args.Placement; };
            _placementEntryPanel.PlacementChangedEvent += (source, args) => { PlacementChangedEvent?.Invoke(this, args); };
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

        private readonly PlacementEntryPanel _placementEntryPanel = new PlacementEntryPanel {Dock = DockStyle.Fill};
    }
}