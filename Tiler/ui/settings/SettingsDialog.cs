using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tiler.Properties;
using Tiler.ui.settings;

namespace Tiler.ui
{
    public partial class SettingsDialog : Form
    {
        private readonly ToolStrip _tb;
        private readonly Panel _container;

        private readonly ApplicationConfigPage _applicationConfigPage;
        private readonly PlacementConfigPage _placementConfigPage;
        
        public SettingsDialog()
        {
            InitializeComponent();
            _tb = new ToolStrip{Dock = DockStyle.Top, BackColor = SystemColors.Window};
            _container = new Panel {Dock = DockStyle.Fill};
            _applicationConfigPage = new ApplicationConfigPage{Dock = DockStyle.Fill};
            _placementConfigPage = new PlacementConfigPage {Dock = DockStyle.Fill};
            Icon = (Icon) Resources.ResourceManager.GetObject("app_tiler");
            
            InitToolBar();
            InitUi();
        }
        
        private void InitUi()
        {
            SuspendLayout();
            
            ClientSize = new Size(590, 400);
            Controls.Add(_container);
            Controls.Add(new Label{AutoSize = false, BorderStyle = BorderStyle.Fixed3D, Dock = DockStyle.Top, Height = 2});
            Controls.Add(_tb);

            Text = $"Settings - {Application.ProductName} {Application.ProductVersion}";
            
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitToolBar()
        {
            _tb.GripStyle = ToolStripGripStyle.Hidden;
            _tb.ImageScalingSize = new Size(48,48);

            var tsbSettings = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("tb_settings_png"), 
                (sender, args) => {_container.Controls.Clear(); })
                {Alignment = ToolStripItemAlignment.Right, Tag = "Configure Tiler"};
            var tsbAppPlacements = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("tb_app_placements_png"),
                (sender, args) =>  SwapControl(_container, _applicationConfigPage))
                {Alignment = ToolStripItemAlignment.Right, Tag = "Configure Applications"};
            var tsbPlacements = new ToolStripButton(string.Empty, (Bitmap) Resources.ResourceManager.GetObject("tb_placements_png"),
                (sender, args) => SwapControl(_container, _placementConfigPage))
                {Alignment = ToolStripItemAlignment.Right, Tag = "Configure Regions"};
            var tsLblCaption = new ToolStripLabel {Alignment = ToolStripItemAlignment.Left,
                Font = new Font(SystemFonts.CaptionFont.Name, 18, FontStyle.Bold)};
            
            var allButtons = new[] {tsbPlacements, tsbSettings, tsbAppPlacements};
            foreach (var button in allButtons)
            {
                button.Click += (sender, args) => ToggleGroupButtons(allButtons, button);
                button.Click += (sender, args) => tsLblCaption.Text = button.Tag.ToString();
            }

            
            _tb.Items.Add(tsLblCaption);
            
            _tb.Items.Add(tsbSettings);
            _tb.Items.Add(new ToolStripSeparator{Alignment = ToolStripItemAlignment.Right});
            _tb.Items.Add(tsbAppPlacements);
            _tb.Items.Add(new ToolStripSeparator{Alignment = ToolStripItemAlignment.Right});
            _tb.Items.Add(tsbPlacements);
            
            tsbPlacements.PerformClick();
        }


        private static void ToggleGroupButtons(IEnumerable<ToolStripButton> buttons, IDisposable checkedButton)
        {
            foreach (var btn in buttons)
            {
                btn.Checked = btn.Equals(checkedButton);
                
            }
        }

        private static void SwapControl(Control parent, Control child)
        {
            parent.SuspendLayout();
            parent.Controls.Clear();
            parent.Controls.Add(child);
            parent.ResumeLayout();
        }
    }
}