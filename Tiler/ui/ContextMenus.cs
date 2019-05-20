using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui
{
	/// <summary>
	/// 
	/// </summary>
	internal static class ContextMenus
	{
		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		public static ContextMenuStrip Create()
		{
			// Add the default menu options.
			var menu = new ContextMenuStrip();

			// Auto Arrange.
			menu.Items.Add("Auto Arrange Now", null, AutoArrange_Click);
			
			// Window Placement.
			menu.Items.Add("Configure Window Placements", null, WindowPlacementConfig_Click);
			
			// About.
			menu.Items.Add("About", null, About_Click);

			// Separator.
			menu.Items.Add(new ToolStripSeparator());

			// Exit.
			menu.Items.Add("Exit", SystemIcons.WinLogo.ToBitmap(), Exit_Click);

			return menu;
		}

		/// <summary>
		/// Handles the Click event of Auto Arrange.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void AutoArrange_Click(object sender, EventArgs e)
		{
			TilerTasks.ReArrangeWindows();
		}

		private static void WindowPlacementConfig_Click(object sender, EventArgs e)
		{
			new WindowPlacementConfig().ShowDialog();
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
	}
}