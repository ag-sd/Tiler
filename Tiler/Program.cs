using System;
using System.Windows.Forms;
using Tiler.runtime;
using Tiler.ui;

namespace Tiler
{
    internal static class Program
    {
        public static readonly ProcessMonitor ProcessMonitor = new ProcessMonitor();
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            ProcessMonitor.Start();
            using (var mi = new MainIcon())
            {
                mi.Display();
                
                Application.Run();
            }
            

//            var form = new SettingsDialog();
//            Application.Run(form);
        }
    }
}