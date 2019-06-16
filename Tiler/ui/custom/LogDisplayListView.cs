using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class LogDisplayListView : ListView
    {
        private static readonly string[] SPLITTER = { " - " };
        private const string Error = "ERROR";
        private const string Debug = "DEBUG";
        private const string Warn = "WARN";

        public LogDisplayListView()
        {
            View = View.Details;
            LabelEdit = false;
            AllowColumnReorder = false;
            FullRowSelect = true;
            GridLines = true;

            //Columns.Add($"{Application.ProductName} Log Messages", -1, HorizontalAlignment.Left);
            Columns.Add("Date", -1, HorizontalAlignment.Left);
            Columns.Add("Thread", -2, HorizontalAlignment.Left);
            Columns.Add("Sender", -1, HorizontalAlignment.Left);
            Columns.Add("Message", -1, HorizontalAlignment.Left);
            
            InitUI();
            GotFocus += (sender, args) => Refresh_Click(sender, args);
        }

        private void InitUI()
        {
            ContextMenuStrip = CreateContextMenu();
            DoubleBuffered = true;
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
            BeginUpdate();
            using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string logLine;
                var counter = 0;
                while ((logLine = sr.ReadLine()) != null)
                {
                    if(counter ++ < Items.Count) continue;
                    AddItem(logLine);
                }
                sr.Close();
                fs.Close();
            }
            AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            EndUpdate();
        }

        private void AddItem(string log)
        {
            var tokens = log.Split(SPLITTER, 4, StringSplitOptions.None);
            var lvi  = new ListViewItem(tokens[0]);
            for (var i = 1; i < tokens.Length; i++)
            {
                if (i == 2)
                {
                    switch (tokens[i])
                    {    
                        case Error:
                            lvi.ForeColor = Color.Firebrick;
                            lvi.BackColor = Color.Pink;
                            break;
                        case Debug:
                            lvi.ForeColor = SystemColors.GrayText;
                            break;
                        case Warn:
                            lvi.ForeColor = Color.OrangeRed;
                            break;
                        default:
                            lvi.ForeColor = SystemColors.WindowText;
                            break;
                    }
                } 
                lvi.SubItems.Add(tokens[i].Trim());
            }
            Items.Add(lvi);
        }
    }
}