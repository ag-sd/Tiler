using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tiler.ui.custom
{
    public class ScreenItem
    {
        public Screen Screen { get; }

        public string Text => ToString();

        public ScreenItem(Screen screen)
        {
            Screen = screen;
        }

        public override string ToString()
        {
            return Screen.DeviceName;
        }
    }
    
    public class MonitorComboBox : ComboBox
    {
        public MonitorComboBox()
        {
            PopulateComboBox();

            DropDownStyle = ComboBoxStyle.DropDownList;
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
        }

        private void SystemEventsOnDisplaySettingsChanged(object sender, EventArgs e)
        {
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            Items.Clear();
            foreach (var screen in Screen.AllScreens)
            {
                var item = new ScreenItem(screen);
                Items.Add(item);
                if (screen.Primary)
                {
                    SelectedItem = item;
                }
            }
        }

        public void SetItem(string monitor)
        {
            foreach (var item in Items)
            {
                if (!item.Equals(monitor)) continue;
                SelectedItem = item;
                return;
            }
        }
        
        public new void Dispose()
        {
            base.Dispose();
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;
        }
    }
}