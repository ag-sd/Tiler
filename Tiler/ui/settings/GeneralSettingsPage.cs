using System.Drawing;
using System.Windows.Forms;
using log4net;
using Microsoft.Win32;
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

            layout.Controls.Add(GetLogViewerAction(), 0, 0);
            layout.Controls.Add(GetTaskBarClickAction(), 0, 1);
            layout.Controls.Add(GetLaunchOnStartupAction(), 1, 0);

            layout.ResumeLayout();
            
            Controls.Add(layout);

            ResumeLayout();
        }

        private CheckBox GetLaunchOnStartupAction()
        {
            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            var currentStatus = registryKey.GetValue(Application.ProductName, "").Equals(Application.ExecutablePath);
            
            var launchOnStartup = new CheckBox{Text = $"Launch {Application.ProductName} on Windows start up", Checked = currentStatus, AutoSize = true};
            launchOnStartup.Click += (sender, args) =>
            {
                if (launchOnStartup.Checked)
                {
                    registryKey.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    registryKey.DeleteValue(Application.ProductName);
                }
            };
            return launchOnStartup;
        }

        private Label GetLogViewerAction()
        {
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
            return logViewer;
        }

        private GroupBox GetTaskBarClickAction()
        {
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
            
            var grpBox = new GroupBox{Text = "Clicking on System Tray Icon will..."};
            grpBox.Controls.Add(rbShowSettings);
            grpBox.Controls.Add(rbArrangeWindows);
            grpBox.Height = rbArrangeWindows.Height * 3;

            return grpBox;
        }
    }
}