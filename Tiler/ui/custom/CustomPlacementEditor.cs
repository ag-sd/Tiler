using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class CustomPlacementEditor : Panel
    {
        
//        private Placement _currentPlacement;
        public CustomPlacementEditor()
        {
            InitUi();
        }
        
        public void SetPlacement(Placement placement)
        {
//            _currentPlacement = placement;
            _lblCaption.Text = placement.Name;
            _desktopMap.Placement = placement;
            _placementEntryPanel.Placement = placement;
        }
        
        private void InitUi()
        {
            SuspendLayout();

            var layout = new TableLayoutPanel {Dock = DockStyle.Fill};
            layout.SuspendLayout();
            
            layout.Controls.Add(_lblCaption, 0, 0);
            layout.Controls.Add(_placementEntryPanel, 0, 1);
            layout.Controls.Add(_desktopMap, 0, 2);

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
            AutoEllipsis = true,
            BackColor = Color.LightSlateGray
        };
        
        private readonly DesktopMap _desktopMap = new DesktopMap {Anchor = AnchorStyles.None, Dock = DockStyle.Fill};

        private readonly PlacementEntryPanel _placementEntryPanel = new PlacementEntryPanel {Dock = DockStyle.Fill};



    }
}