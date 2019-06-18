using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class MonitorMap : Panel
    {
        
        private Placement _placement;
        private Screen _screen;
        private int _sWidth;
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
            get => _screen;
        }

        public MonitorMap()
        {
            _screen = Screen.PrimaryScreen;
            Resize += OnResize;
            ConfigureWidthAndHeight();
            InitUi();
        }

        private void InitUi()
        {
            BackColor = Color.LightGray;
            BorderStyle = BorderStyle.FixedSingle;
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        private void ConfigureWidthAndHeight()
        {
            var scrSize = _screen.WorkingArea;
            var screenRatio = (float)scrSize.Width / (float)scrSize.Height;
            var canvasRatio = (float)Width / (float)Height;
            if (screenRatio > canvasRatio)
            {
                _sWidth = (int)(Width - Width * 0.05f);
                _sHeight = scrSize.Height * _sWidth / scrSize.Width;
            }
            else
            {
                _sHeight = (int)(Height - Height * 0.05f);
                _sWidth = scrSize.Width * _sHeight / scrSize.Height;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.  
            base.OnPaint(e);
            
            // Draw the Placement object
            DrawMap(e.Graphics, _placement, _sWidth, _sHeight, Width, Height, _screen.DeviceName);
        }

        private void OnResize(object sender, EventArgs e)
        {
            ConfigureWidthAndHeight();
        }

        public static void DrawMap(Graphics graphics, Placement placement, int sWidth, int sHeight, int gWidth, int gHeight, string text = "")
        {
            var x = (gWidth - sWidth) / 2;
            var y = (gHeight - sHeight) / 2;
            
            if (placement != null)
            {
                var location = placement.ResolveLocation(sWidth, sHeight);
                location.Offset(x, y);
                var size = placement.ResolveSize(sWidth, sHeight);
                // Draw the Placement object
                using ( var pen = new Pen(Color.Red) )
                using ( var brush = new SolidBrush(Color.Salmon) )
                {
                    var rectangle = new Rectangle(location, size);
                    graphics.FillRectangle(brush, rectangle);
                    graphics.DrawRectangle(pen, rectangle);
                }
            }
            
            // Draw the monitor border 
            var scrBorderLocation = new Point(x, y);
            var scrBorderSize = new Size(sWidth, sHeight);
            var rect = new Rectangle(scrBorderLocation, scrBorderSize);
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