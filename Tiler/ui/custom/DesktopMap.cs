using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class DesktopMap : Panel
    {
        
        private Placement _placement;
        private Screen _screen;
        private int _sWIdth;
        private int _sHeight;
        
        public Placement Placement
        {
            set  {
                _placement = value;
                Refresh();
            }
        }

        public Screen Screen
        {
            set  {
                _screen = value;
                ConfigureWidthAndHeight();
                Refresh();
            } 
        }

        public DesktopMap()
        {
            BackColor = SystemColors.Window;
            _screen = Screen.PrimaryScreen;
            ResizeRedraw = true;
            Resize += OnResize;
            ConfigureWidthAndHeight();
        }

        private void ConfigureWidthAndHeight()
        {
            var scrSize = _screen.WorkingArea;
            var screenRatio = (float)scrSize.Width / (float)scrSize.Height;
            var canvasRatio = (float)Width / (float)Height;
            if (screenRatio > canvasRatio)
            {
                _sWIdth = Width;
                _sHeight = scrSize.Height * Width / scrSize.Width;
            }
            else
            {
                _sHeight = Height;
                _sWIdth = scrSize.Width * Height / scrSize.Height;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.  
            base.OnPaint(e);

            // Draw the Placement object
            DrawMap(e.Graphics, _placement, _sWIdth, _sHeight, _screen.DeviceName);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            ConfigureWidthAndHeight();
        }

        public static void DrawMap(Graphics graphics, Placement placement, int width, int height, string text = "")
        {
            if (placement != null)
            {
                var location = placement.ResolveLocation(width, height);
                var size = placement.ResolveSize(width, height);
                // Draw the Placement object
                using (var myPen = new SolidBrush(Color.Salmon))
                {
                    graphics.FillRectangle(myPen, new Rectangle(location, size));
                }
            }
            
            // Draw the desktop border  
            var rect = new Rectangle(2, 2, width -5, height - 5);
            using (var screenBorderPen = new Pen(Color.Black, 5))  
            {
                graphics.DrawRectangle(screenBorderPen, rect);  
            }
            
            if(string.IsNullOrEmpty(text)) return;
            
            var sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center
            };
            
            graphics.DrawString(text, DefaultFont, SystemBrushes.WindowText, rect, sf);
        }
    }
}