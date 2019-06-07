using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;

namespace Tiler.ui
{
    public class MainIcon : IDisposable
    {
        private readonly NotifyIcon _ni;

        public MainIcon()
        {
            _ni = new NotifyIcon();
        }

        public void Display()
        {
            _ni.MouseClick += ni_MouseClick;
            _ni.Text = Application.ProductName;
            _ni.Icon = (Icon) Resources.ResourceManager.GetObject("app_16");
            _ni.Visible = true;
            
            // Attach a context menu.
            _ni.ContextMenuStrip = ContextMenus.Create();
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
            _ni.Dispose();
        }
    }
}