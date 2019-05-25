using System.Drawing;
using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class FieldLabelPanel : TableLayoutPanel
    {
        public FieldLabelPanel(Control control, string text)
        {
            AutoSize = true;
            RowStyles.Add(new RowStyle());
            RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(label, 0, 0);
            Controls.Add(control, 0, 1);
        }
    }
}