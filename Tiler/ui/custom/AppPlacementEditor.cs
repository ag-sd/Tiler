using System;
using System.Drawing;
using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class AppPlacementEditor : Panel
    {
        private ProcessListItem _currentApplication;
        public AppPlacementEditor()
        {
            InitUi();
        }

        public void SetApplication(ProcessListItem applicationItem)
        {
            _cmbPlacement.SelectedItem = applicationItem.Placement;
            _lblCaption.Text = applicationItem.Caption;
            _currentApplication = applicationItem;
        }

        private void InitUi()
        {
            SuspendLayout();
            Margin = new Padding(5);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Percent, 5),
                    new ColumnStyle(SizeType.Percent, 90),
                    new ColumnStyle(SizeType.Percent, 5)
                },
                RowStyles =
                {
                    new RowStyle(),
                    new RowStyle(SizeType.Percent,80),
                    new RowStyle()
                }
            };
            layout.SuspendLayout();
            _lblCaption.Text = "Caption";
            _lblCaption.Anchor = AnchorStyles.None;
            _lblCaption.BackColor = Color.Azure;
            layout.Controls.Add(_lblCaption, 1, 0);
            layout.Controls.Add(_desktopMap, 1, 1);
            
            var cmbLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                ColumnStyles = { 
                    new ColumnStyle(),
                    new ColumnStyle(SizeType.Percent, 50), 
                    new ColumnStyle(),
                    new ColumnStyle(SizeType.Percent, 50)
                }
            };
            cmbLayout.Controls.Add(new Label{Text = "Placement", TextAlign = ContentAlignment.MiddleRight},0, 0);
            cmbLayout.Controls.Add(_cmbPlacement, 1, 0);
            cmbLayout.Controls.Add(new Label{Text = "Desktop", TextAlign = ContentAlignment.MiddleRight},2, 0);
            cmbLayout.Controls.Add(_cmbDesktopCount, 3, 0);
            
            layout.Controls.Add(cmbLayout, 1, 2);
            
            foreach (var placement in Placement.values)
            {
                _cmbPlacement.Items.Add(placement);
            }

            _cmbPlacement.SelectedIndexChanged += CmbPlacement_SelectionChanged;

            layout.ResumeLayout();
            
            Controls.Add(layout);
            
            ResumeLayout();
        }

        private readonly Label _lblCaption = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter, 
            Anchor = AnchorStyles.None, 
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoEllipsis = true

        };
        private readonly DesktopMap _desktopMap = new DesktopMap {Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly ComboBox _cmbDesktopCount = new DesktopComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        private readonly ComboBox _cmbPlacement = new ComboBox{Anchor = AnchorStyles.None, Dock = DockStyle.Fill};
        
        private void CmbPlacement_SelectionChanged(object sender, EventArgs  e)
        {
            if (_cmbPlacement.SelectedItem != null && _currentApplication != null)
            {
                _currentApplication.Placement = (Placement) _cmbPlacement.SelectedItem;
                _currentApplication.SavePlacement();
            }
        }
    }
}