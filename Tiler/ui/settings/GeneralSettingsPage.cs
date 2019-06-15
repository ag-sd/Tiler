using System.Drawing;
using System.Windows.Forms;
using log4net;
using Tiler.ui.custom;

namespace Tiler.ui.settings
{
    public class GeneralSettingsPage : UserControl
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly LogDisplayListView _lvw;
        private readonly SplitContainer _split;

        public GeneralSettingsPage()
        {
            _lvw = new LogDisplayListView();
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
            
            Controls.Add(_split);
            
            _split.FixedPanel = FixedPanel.Panel1;
            _split.Size = new Size(500, 273);
            _split.SplitterDistance = 275;
            _split.TabIndex = 0;
            _split.Panel1.Controls.Add(_lvw);

            _split.ResumeLayout();

            ResumeLayout();
        }
    }
}