using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class PlacementComboBox : ComboBox
    {
        public PlacementComboBox()
        {
            foreach (var placement in Placement.Values)
            {
                Items.Add(placement);
            }

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void SetItem(Placement placement)
        {
            foreach (var item in Items)
            {
                if (!placement.Equals(item)) continue;
                SelectedItem = item;
                return;
            }
        }
    }
}