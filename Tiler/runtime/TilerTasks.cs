using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tiler.runtime
{
    internal static class TilerTasks
    {
        public static void ReArrangeWindows()
        {
            // Get current applications
            var dictionary = WindowsShell.GetWindowsByProcessId();
            var allProcesses = Process.GetProcesses();
            // Force to primary display for now
            var scrRects = Screen.AllScreens.ToDictionary(screen => screen.DeviceName, screen => screen.WorkingArea);
            Debug.WriteLine("ALL Working Areas = " + scrRects);

            foreach (var process in allProcesses)
            {
                if (!dictionary.ContainsKey(process.Id)) continue;
                // Check if there is a saved placement
                var (placement, desktop) = INISettings.GetPlacement(process.ProcessName);

                if (!scrRects.ContainsKey(desktop)) 
                {
                    MessageBox.Show(
                        "Could not find desktop" + desktop + 
                        "\nIf you have ported your settings from another computer or changed your monitor configuration, " +
                        "you can reconfigure the desktops in the Window Placement Editor" +
                        Application.ProductName + " will now stop rearranging windows until this issue is resolved", 
                        desktop + " Was not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    // Exit method to prevent any further popups
                    return;
                }

                var scrRect = scrRects[desktop];
                if(Placement.None.Equals(placement)) continue;
                
                AdjustWindows(scrRect, placement, dictionary[process.Id]);
            }
        }

        private static void AdjustWindows(Rectangle screenRect, Placement placement, IEnumerable<IntPtr> windowHandles)
        {
            var newLocation = placement.ResolveLocation(screenRect.Width, screenRect.Height);
            var newSize = placement.ResolveSize(screenRect.Width, screenRect.Height);
            
            foreach (var handle in windowHandles)
            {
                // First set with window placement
                var windowPlacement = WindowsShell.GetPlacement(handle);
                windowPlacement.NormalPosition = new Rectangle(newLocation, newSize);
                windowPlacement.ShowCmd = 1;
                Debug.WriteLine("Moving to {0}", windowPlacement);
                
                var ret = WindowsShell.ChangeWindowPlacement(handle, ref windowPlacement);
                Debug.WriteLine(ret);
                    
                // If Windows 10, then find the window rect and frame rect because of the invisible border
                var frame = WindowsShell.GetWindowFrame(handle);
                if (!frame.hasBorder()) continue;
                    
                var offsetLocation = new Point(newLocation.X + frame.WindowRect.X - frame.DwmExtendedWindowRect.X, 
                    newLocation.Y + frame.WindowRect.Y - frame.DwmExtendedWindowRect.Y);
                var offsetSize = new Size(newSize.Width + frame.WindowRect.Width - frame.DwmExtendedWindowRect.Width,
                    newSize.Height + frame.WindowRect.Height - frame.DwmExtendedWindowRect.Height);
                var offsetPlacement = new Rectangle(offsetLocation, offsetSize);
                Debug.WriteLine("Window needed to be updated to compensate for invisible frame from {0} to {1}", 
                    windowPlacement.NormalPosition, offsetPlacement);
                windowPlacement.NormalPosition = offsetPlacement;
                ret = WindowsShell.ChangeWindowPlacement(handle, ref windowPlacement);
                Debug.WriteLine(ret);
            }
        }
    }
}