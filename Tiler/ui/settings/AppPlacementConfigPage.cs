using System.Drawing;
using System.Windows.Forms;
using Tiler.ui.custom;

namespace Tiler.ui.settings
{
    public class AppPlacementConfigPage : UserControl
    {
        private readonly ProcessListView _lvw;
        private readonly AppPlacementEditor _editor;
        private readonly SplitContainer _split;

        public AppPlacementConfigPage()
        {
            _lvw = new ProcessListView();
            _editor = new AppPlacementEditor();
            _split = new SplitContainer();
            InitUi();
        }
        

        private void InitUi()
        {
            SuspendLayout();
            
            ((System.ComponentModel.ISupportInitialize) _split).BeginInit();
            _split.SuspendLayout();
            _split.Dock = DockStyle.Fill;

            _lvw.Dock = DockStyle.Fill;
            _lvw.ItemSelectionChanged += (sender, args) =>
            {
                if (!args.IsSelected || args.Item == null) return;
                var item = (ProcessListItem) args.Item;
                _editor.SetApplication(item);
            };
            
            _editor.Dock = DockStyle.Fill;
            
            Controls.Add(_split);
            
            _split.FixedPanel = FixedPanel.Panel1;
            _split.Size = new Size(500, 273);
            _split.SplitterDistance = 275;
            _split.TabIndex = 0;
            _split.Panel1.Controls.Add(_lvw); 
            _split.Panel2.Controls.Add(_editor);

            _split.ResumeLayout();

            ResumeLayout();
            
//            Closing += (sender, args) => 
//            {
//                if (DialogResult.Yes == 
//                    MessageBox.Show("Apply these changes immediately?", "Apply Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
//                {
//                    TilerTasks.ReArrangeWindows();
//                }
//            }; 
        }
        
    }
}