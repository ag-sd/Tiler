using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;

namespace Tiler.runtime
{
    /// <summary>
    /// An event to indicate that windows were resized.
    /// </summary>
    /// <param name="source">The calling class</param>
    /// <param name="e">Default Event Args</param>
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

        public void ReArrangeWindows(ISet<int> processIds = null)
        {
            // Get current applications
            var dictionary = WindowsShell.GetWindowsByProcessId();
            var scrRects = Screen.AllScreens.ToDictionary(screen => screen.DeviceName, screen => screen.WorkingArea);
            Debug.WriteLine("ALL Working Areas = " + scrRects);

            foreach (var processId in dictionary.Keys)
            {
                if (processIds != null && !processIds.Contains(processId)) continue;

                var process = Process.GetProcessById(processId);
                // Check if there is a saved placement
                var (placement, desktop) = INISettings.GetAppPlacement(process.ProcessName);

                if (!scrRects.ContainsKey(desktop))
                {
                    log.Warn($"Could not find the monitor {desktop} configured for {process.ProcessName}. Skipping");
                    continue;
                }
                var scrRect = scrRects[desktop];

                var monitorInfo = WindowsShell.GetMonitorCoordinates();
                if(!monitorInfo.ContainsKey(desktop))
                {
                    log.Warn($"Could not find the virtual coordinates for monitor {desktop} configured for {process.ProcessName}. Skipping");
                    continue;
                }
                var virtualTop = monitorInfo[desktop];

                if (Placement.None.Equals(placement))
                {
                    log.Warn($"{process.ProcessName} is configured with a `None` placement. Skipping");
                    continue;
                }
                
                AdjustWindows(scrRect, virtualTop, placement, dictionary[process.Id], process.ProcessName);
            }
            
            WindowResizedEvent?.Invoke(this, new EventArgs());
        }

        private void ProcessMonitorOnProcessesAddedEvent(object source, ProcessChangedEventArgs e)
        {
            if (ActiveMode) ReArrangeWindows(e.Processes);
        }

        private static void AdjustWindows(Rectangle screenRect, Point virtualTop, Placement placement, IEnumerable<IntPtr> windowHandles, string appName)
        {
            log.Info($"Will adjust window position of {appName}");
            var newLocation = placement.ResolveLocation(screenRect.Width, screenRect.Height);
            log.Info($"{appName}: Screen coordinates resolved to {newLocation}");
            newLocation.Offset(virtualTop.X, virtualTop.Y);
            log.Info($"{appName}: Virtual coordinates resolved to {newLocation}");
            var newSize = placement.ResolveSize(screenRect.Width, screenRect.Height);
            
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

                var oldPlacement = windowPlacement.NormalPosition;
                windowPlacement.NormalPosition = offsetPlacement;
                ret = WindowsShell.ChangeWindowPlacement(handle, ref windowPlacement);
                log.Info($"{appName}: Window was moved to compensate for invisible frame from " +
                         $"{oldPlacement} to {offsetPlacement} with result code {ret}");
            }
        }
    }
}