using System.Windows.Forms;

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
            foreach (var screen in Screen.AllScreens)
            {
                var item = new ScreenItem(screen);
                Items.Add(item);
                if (screen.Primary)
                {
                    SelectedItem = item;
                }
            }

            DropDownStyle = ComboBoxStyle.DropDownList;
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
    }
}