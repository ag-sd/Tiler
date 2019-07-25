using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using log4net;
using Microsoft.Win32;

namespace Tiler.runtime
{
    public delegate void ProcessesAddedEvent(object source, ProcessChangedEventArgs e);
    
    public delegate void ProcessesRemovedEvent(object source, ProcessChangedEventArgs e);
    
    public class ProcessChangedEventArgs : EventArgs
    {
        public ISet<int> Processes { get; }

        public ProcessChangedEventArgs(ISet<int> processes)
        {
            Processes = processes;
        }
    }
    
    public class ProcessMonitor : IDisposable
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly Timer _systemMonitorTimer;
        private ISet<int> _processSet;

        public event ProcessesAddedEvent ProcessesAddedEvent;
        public event ProcessesRemovedEvent ProcessesRemovedEvent;

        public ProcessMonitor()
        {
            _processSet = new HashSet<int>();
            _systemMonitorTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };
            _systemMonitorTimer.Elapsed += SystemMonitorTimerOnElapsed;
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
        }

        private void SystemEventsOnDisplaySettingsChanged(object sender, EventArgs e)
        {
            log.Info("Desktop settings changed. Triggering a hard update...");
            Stop();
            // Snapshot all process
            var allApps = new ProcessChangedEventArgs(new HashSet<int>(_processSet));
            // Trigger a notification to remove all of them
            ProcessesRemovedEvent?.Invoke(this, allApps);
            _processSet.Clear();
            // Restart the timer to force a re-trigger of all placements
            Start();
        }

        private void SystemMonitorTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            ManageProcesses();
        }

        private void ManageProcesses()
        {
            var windowedProcesses = WindowsShell.GetWindowsByProcessId();
            
            //log.Debug("Processes found were " + string.Join(", ", windowedProcesses.Keys));
            var removedProcessKeys = _processSet.Except(windowedProcesses.Keys).ToHashSet();
            var addedProcessKeys = windowedProcesses.Keys.Except(_processSet).ToHashSet();

            var addedEventArgs = addedProcessKeys.Count > 0 ? new ProcessChangedEventArgs(addedProcessKeys) : null;
            var removedEventArgs = removedProcessKeys.Count > 0 ? new ProcessChangedEventArgs(removedProcessKeys) : null;
            
            _processSet = windowedProcesses.Keys.ToHashSet();

            if (addedEventArgs != null)
            {
                log.Info("System detected that processes were added. Raising event...");
                ProcessesAddedEvent?.Invoke(this, addedEventArgs);
            }

            if (removedEventArgs != null)
            {
                log.Info("System detected that processes were removed. Raising event...");
                ProcessesRemovedEvent?.Invoke(this, removedEventArgs);
            }
        }

        public void Start()
        {
            ManageProcesses();
            _systemMonitorTimer.Enabled = true;
        }

        public void Stop() => _systemMonitorTimer.Enabled = false;
        
        public IEnumerable<int> GetProcessIds() => new HashSet<int>(_processSet);
        
        public void Dispose()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;
        }
    }
}