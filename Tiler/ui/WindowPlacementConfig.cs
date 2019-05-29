using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.runtime;
using Tiler.ui.custom;

namespace Tiler.ui
{
    public partial class WindowPlacementConfig : Form
    {
        private readonly ProcessListView _lvw;
        private readonly AppPlacementEditor _editor;

        public WindowPlacementConfig()
        {
            InitializeComponent();
            _lvw = new ProcessListView();
            _editor = new AppPlacementEditor();
            Icon = (Icon) Resources.ResourceManager.GetObject("app_16");
            Text = "Window Placement Editor";
            InitUi();
        }
        

        private void InitUi()
        {
            SuspendLayout();
            
            _lvw.Dock = DockStyle.Fill;
            _lvw.ItemSelectionChanged += (sender, args) =>
            {
                if (!args.IsSelected || args.Item == null) return;
                var item = (ProcessListItem) args.Item;
                _editor.SetApplication(item);
            };
            
            _editor.Dock = DockStyle.Fill;
            
            split.Panel1.Controls.Add(_lvw);
            split.Panel2.Controls.Add(_editor);

            ResumeLayout();
            
            Closing += (sender, args) => 
            {
                if (DialogResult.Yes == 
                    MessageBox.Show("Apply these changes immediately?", "Apply Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    TilerTasks.ReArrangeWindows();
                }
            }; 
        }
        
    }
}