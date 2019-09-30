using System;
using System.Windows.Forms;
using log4net;
using Tiler.runtime;
using Tiler.ui;

namespace Tiler
{
    internal static class Program
    {
        public static readonly ProcessMonitor ProcessMonitor = new ProcessMonitor();
        public static readonly Settings Settings = INISettings.ReadSettings();
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            log.Info("Starting Application...");

            ProcessMonitor.Start();
            using (var mi = new AppIcon())
            {
                mi.Display();
                
                Application.Run();
            }

//            var form = new SettingsDialog();
//            Application.Run(form);
        }
    }
}