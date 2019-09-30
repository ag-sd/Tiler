using System.Drawing;
using System.Windows.Forms;
using log4net;
using Tiler.runtime;
using Tiler.ui.custom;

namespace Tiler.ui.settings
{
    public class GeneralSettingsPage : UserControl
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GeneralSettingsPage()
        {
            InitUi();
        }

        private void InitUi()
        {
            SuspendLayout();
            
            var layout = new TableLayoutPanel {Dock = DockStyle.Fill};
            layout.SuspendLayout();
            
            var rbShowSettings = new RadioButton{Text = "Show Settings", Dock = DockStyle.Top};
            rbShowSettings.Click += (sender, args) =>
            {
                Program.Settings.TaskIconClickShowsSettings = rbShowSettings.Checked;
                INISettings.SaveSettings(Program.Settings);
            };
            
            var rbArrangeWindows = new RadioButton{Text = "Arrange Windows", Dock = DockStyle.Bottom};
            rbArrangeWindows.Click += (sender, args) =>
            {
                Program.Settings.TaskIconClickShowsSettings = rbShowSettings.Checked;
                INISettings.SaveSettings(Program.Settings);
            };
            rbShowSettings.Checked = Program.Settings.TaskIconClickShowsSettings;
            
            var logViewer = new LinkLabel{Text = "Show Logs"};
            logViewer.Click += (sender, args) =>
            {
                var form = new Form{
                    Text = $"Logs - {Application.ProductName} {Application.ProductVersion}", 
                    Icon = ParentForm.Icon
                };
                form.Controls.Add(new LogDisplayListView{Dock = DockStyle.Fill});
                form.ShowDialog(this);
            }; 
            
            var grpBox = new GroupBox{Text = "Clicking on System Tray Icon will..."};
            grpBox.Controls.Add(rbShowSettings);
            grpBox.Controls.Add(rbArrangeWindows);
            grpBox.Height = rbArrangeWindows.Height * 3;

            layout.Controls.Add(grpBox, 0, 1);
            layout.Controls.Add(logViewer, 0, 0);
            
            layout.ResumeLayout();
            
            Controls.Add(layout);

            ResumeLayout();
        }
    }
}