using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;

namespace Tiler.runtime
{
    public delegate void WindowResizedEvent(object source, EventArgs e);

    
    public class WindowResizeManager
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public event WindowResizedEvent WindowResizedEvent;
        private bool _activeMode;

        public WindowResizeManager()
        {
            Program.ProcessMonitor.ProcessesAddedEvent += ProcessMonitorOnProcessesAddedEvent;
        }

        public bool ActiveMode
        {
            get => _activeMode;
            set
            {
                _activeMode = value;
                if (value) ReArrangeWindows();
            }
        }

        public void ReArrangeWindows(HashSet<string> processNames = null)
        {
            // Get current applications
            var dictionary = WindowsShell.GetWindowsByProcessId();
            var allProcesses = Program.ProcessMonitor.GetProcesses();
            // Force to primary display for now
            var scrRects = Screen.AllScreens.ToDictionary(screen => screen.DeviceName, screen => screen.WorkingArea);
            Debug.WriteLine("ALL Working Areas = " + scrRects);

            foreach (var process in allProcesses)
            {
                if (!dictionary.ContainsKey(process.Id)) continue;
                if (processNames != null && !processNames.Contains(process.ProcessName)) continue;
                
                // Check if there is a saved placement
                var (placement, desktop) = INISettings.GetAppPlacement(process.ProcessName);

                if (!scrRects.ContainsKey(desktop)) continue;

                var scrRect = scrRects[desktop];
                if(Placement.None.Equals(placement)) continue;
                
                AdjustWindows(scrRect, placement, dictionary[process.Id], process.ProcessName);
            }
            
            WindowResizedEvent?.Invoke(this, new EventArgs());
        }
        
        private void ProcessMonitorOnProcessesAddedEvent(object source, ProcessChangedEventArgs e)
        {
            ReArrangeWindows(e.Processes.Keys.ToHashSet());
        }

        private static void AdjustWindows(Rectangle screenRect, Placement placement, IEnumerable<IntPtr> windowHandles, string appName)
        {
            var newLocation = placement.ResolveLocation(screenRect.Width, screenRect.Height);
            var newSize = placement.ResolveSize(screenRect.Width, screenRect.Height);
            log.Info($"Will adjust window position of {appName}");
            
            foreach (var handle in windowHandles)
            {
                // First set with window placement
                var windowPlacement = WindowsShell.GetPlacement(handle);
                windowPlacement.NormalPosition = new Rectangle(newLocation, newSize);
                windowPlacement.ShowCmd = 1;

                var ret = WindowsShell.ChangeWindowPlacement(handle, ref windowPlacement);
                log.Info($"{appName}: Moving to {windowPlacement} with result code {ret}");

                // If Windows 10, then find the window rect and frame rect because of the invisible border
                var frame = WindowsShell.GetWindowFrame(handle);
                if (!frame.hasBorder()) continue;
                    
                var offsetLocation = new Point(newLocation.X + frame.WindowRect.X - frame.DwmExtendedWindowRect.X, 
                    newLocation.Y + frame.WindowRect.Y - frame.DwmExtendedWindowRect.Y);
                var offsetSize = new Size(newSize.Width + frame.WindowRect.Width - frame.DwmExtendedWindowRect.Width,
                    newSize.Height + frame.WindowRect.Height - frame.DwmExtendedWindowRect.Height);
                var offsetPlacement = new Rectangle(offsetLocation, offsetSize);
                
                windowPlacement.NormalPosition = offsetPlacement;
                ret = WindowsShell.ChangeWindowPlacement(handle, ref windowPlacement);
                log.Info($"{appName}: Window was moved to compensate for invisible frame from " +
                         $"{windowPlacement.NormalPosition} to {offsetPlacement} with result code {ret}");
            }
        }
    }
}