using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.runtime;

namespace Tiler.ui
{
    public class MainIcon : IDisposable
    {
        private readonly NotifyIcon _ni;
        private readonly SettingsDialog _settingsDialog;
        private readonly WindowResizeManager _resizeManager;

        public MainIcon()
        {
            _ni = new NotifyIcon();
            _settingsDialog = new SettingsDialog();
            _resizeManager = new WindowResizeManager();
            _resizeManager.WindowResizedEvent += resizeManager_OnWindowResizedEvent;
        }

        public void Display()
        {
            _ni.MouseClick += ni_MouseClick;
            _ni.Text = Application.ProductName;
            _ni.Icon = (Icon) Resources.ResourceManager.GetObject("app_tiler");
            _ni.Visible = true;
            
            // Attach a context menu.
            _ni.ContextMenuStrip = createContextMenu();
        }

        private void ni_MouseClick(object sender, MouseEventArgs e)
        {
            // Handle mouse button clicks.
            if (e.Button == MouseButtons.Left && !_settingsDialog.Visible)
            {
                // Start Windows Explorer.
                _settingsDialog.ShowDialog();
            }
        }
        
        private void resizeManager_OnWindowResizedEvent(object source, EventArgs e)
        {
            // Animate Icon
        }

        private ContextMenuStrip createContextMenu()
        {
            // Add the default menu options.
            var menu = new ContextMenuStrip();

            // Auto Arrange.
            menu.Items.Add("Auto Arrange Now", null, AutoArrange_Click);
            // Separator.
            menu.Items.Add(new ToolStripSeparator());
            // Arrangement Mode
            menu.Items.Add("Active Arrangement", null, ActiveArrangement_Click);
            // Separator.
            menu.Items.Add(new ToolStripSeparator());
            // Window Placement.
            menu.Items.Add("Settings", null, Settings_Click);
            // About.
            menu.Items.Add("About", null, About_Click);
            // Separator.
            menu.Items.Add(new ToolStripSeparator());
            // Exit.
            menu.Items.Add("Exit", SystemIcons.WinLogo.ToBitmap(), Exit_Click);

            return menu;
        }

        private void ActiveArrangement_Click(object sender, EventArgs e)
        {
            var btn = (ToolStripMenuItem) sender;
            btn.Checked = !btn.Checked;
            _resizeManager.ActiveMode = btn.Checked;
        }

        /// <summary>
        /// Handles the Click event of Auto Arrange.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AutoArrange_Click(object sender, EventArgs e)
        {
            _resizeManager.ReArrangeWindows();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            if (!_settingsDialog.Visible) _settingsDialog.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void About_Click(object sender, EventArgs e)
        {
            Console.WriteLine("About!");
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }

        public void Dispose()
        {
            _ni.Dispose();
        }
    }
}