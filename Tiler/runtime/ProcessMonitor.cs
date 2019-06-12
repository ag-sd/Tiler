using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using log4net;

namespace Tiler.runtime
{
    public delegate void ProcessesAddedEvent(object source, ProcessChangedEventArgs e);
    
    public delegate void ProcessesRemovedEvent(object source, ProcessChangedEventArgs e);
    
    public class ProcessChangedEventArgs : EventArgs
    {
        public Dictionary<string, Process> Processes { get; }

        public ProcessChangedEventArgs(Dictionary<string, Process> processes)
        {
            Processes = processes;
        }
    }
    
    public class ProcessMonitor
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly Timer _systemMonitorTimer;
        private Dictionary<string, Process> _processList;

        public event ProcessesAddedEvent ProcessesAddedEvent;
        public event ProcessesRemovedEvent ProcessesRemovedEvent;

        public ProcessMonitor()
        {
            _processList = new Dictionary<string, Process>();
            _systemMonitorTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };
            _systemMonitorTimer.Elapsed += SystemMonitorTimerOnElapsed;
        }

        private void SystemMonitorTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            ManageProcesses();
        }

        private void ManageProcesses()
        {
            var allProcesses = Process.GetProcesses();
            var windowedProcesses = 
                allProcesses.Where(process => process.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(process.MainWindowTitle))
                    .ToDictionary(process => process.ProcessName);

            var removedProcessKeys = _processList.Keys.Except(windowedProcesses.Keys).ToList();
            var addedProcessKeys = windowedProcesses.Keys.Except(_processList.Keys).ToList();

            var addedEventArgs = addedProcessKeys.Count > 0 ? createArgs(addedProcessKeys, windowedProcesses) : null;
            var removedEventArgs = removedProcessKeys.Count > 0 ? createArgs(removedProcessKeys, _processList) : null;
            
            _processList = windowedProcesses;

            if (addedEventArgs != null)
            {
                log.Info("Detected processes were added. Raising event...");
                ProcessesAddedEvent?.Invoke(this, addedEventArgs);
            }

            if (removedEventArgs != null)
            {
                log.Info("Detected processes were removed. Raising event...");
                ProcessesRemovedEvent?.Invoke(this, removedEventArgs);
            }
        }
        
        private static ProcessChangedEventArgs createArgs(IEnumerable<string> keys, IReadOnlyDictionary<string, Process> refDict)
        {
            var dict = keys.ToDictionary(key => key, key => refDict[key]);
            return new ProcessChangedEventArgs(dict);
        }

        public void Start()
        {
            ManageProcesses();
            _systemMonitorTimer.Enabled = true;
        }

        public void Stop() => _systemMonitorTimer.Enabled = false;
        
        public IEnumerable<Process> GetProcesses() => new HashSet<Process>(_processList.Values);
    }
}