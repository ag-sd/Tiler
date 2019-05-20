using System.Windows.Forms;

namespace Tiler.ui.custom
{
    public class DesktopComboBox : ComboBox
    {
        private class ScreenItem
        {
            private readonly Screen _screen;
            public ScreenItem(Screen screen)
            {
                _screen = screen;
            }

            public override string ToString()
            {
                return _screen.DeviceName;
            }
        }
        public DesktopComboBox()
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
        
    }
}