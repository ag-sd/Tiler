using System;
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
//            using (var mi = new MainIcon())
//            {
//                mi.Display();
//                
//                Application.Run();
//            }

//            var form = new Form();
//
//            form.Controls.Add(new AppPlacementEditor{Dock = DockStyle.Fill});

            Application.Run(new PlacementConfig());
        }
    }
}