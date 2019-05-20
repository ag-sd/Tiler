using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiler.ui;

namespace Tiler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

//            using (MainIcon mi = new MainIcon())
//            {
//                mi.Display();
//                
//                Application.Run();
//            }
//            new WindowPlacementConfig().Show();
            Application.Run(new WindowPlacementConfig());
        }
    }
}