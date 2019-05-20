
using System.Windows.Forms;
using Tiler.ui.custom;

namespace Tiler.ui
{
    public partial class WindowPlacementConfig : Form
    {
        public WindowPlacementConfig()
        {
            InitializeComponent();

            var panel = new AppPlacementEditor();
            panel.Dock = DockStyle.Fill;
            Controls.Add(panel);

        }
    }
}