using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class LogDisplayListView : ListView
    {

        public LogDisplayListView()
        {
            View = View.Details;
            LabelEdit = false;
            AllowColumnReorder = false;
            FullRowSelect = true;
            GridLines = true;
            
            Columns.Add($"{Application.ProductName} Log Messages", -2, HorizontalAlignment.Left);

            InitUI();
        }

        private void InitUI()
        {
            ContextMenuStrip = CreateContextMenu();

        }

        private ContextMenuStrip CreateContextMenu()
        {
            // Add the default menu options.
            var menu = new ContextMenuStrip();

            // Refresh.
            menu.Items.Add("Refresh", null, Refresh_Click);
            // Clear
            menu.Items.Add("Clear", null, (sender, args) => Items.Clear());

            return menu;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            var logPath = Path.GetDirectoryName(Application.ExecutablePath) +
                          Path.DirectorySeparatorChar + 
                          "logs" + Path.DirectorySeparatorChar +
                          Application.ProductName + ".log";
            using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string logLine;
                var counter = 0;
                while ((logLine = sr.ReadLine()) != null)
                {
                    if(counter ++ < Items.Count) continue;
                    Items.Add(logLine);
                }
                sr.Close();
                fs.Close();
            }
        }
    }
}