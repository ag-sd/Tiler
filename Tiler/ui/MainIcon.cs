using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Tiler.ui
{
    public class MainIcon : IDisposable
    {
        readonly NotifyIcon ni;

        public MainIcon()
        {
            ni = new NotifyIcon();
        }

        public void Display()
        {
            ni.MouseClick += ni_MouseClick;
            ni.Text = Application.ProductName;
            ni.Icon = SystemIcons.Application;
            ni.Visible = true;
            
            // Attach a context menu.
            ni.ContextMenuStrip = ContextMenus.Create();
            
        }

        private static void ni_MouseClick(object sender, MouseEventArgs e)
        {
            // Handle mouse button clicks.
            if (e.Button == MouseButtons.Left)
            {
                // Start Windows Explorer.
                new WindowPlacementConfig().ShowDialog();
            }
        }

        public void Dispose()
        {
            ni.Dispose();
        }
    }
}