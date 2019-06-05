using System.Drawing;
using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class FieldLabelPanel : TableLayoutPanel
    {
        public FieldLabelPanel(Control control, string text, bool topDown = true)
        {
            AutoSize = true;
            if (topDown)
                InitTopDown(control, text);
            else
                InitOnSide(control, text);
        }

        private void InitOnSide(Control control, string text)
        {
            ColumnStyles.Add(new ColumnStyle());
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            Controls.Add(label, 0, 0);
            Controls.Add(control, 1, 0);
            
        }
        
        private void InitTopDown(Control control, string text)
        {
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