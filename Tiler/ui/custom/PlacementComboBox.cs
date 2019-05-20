using System.Windows.Forms;
using Tiler.runtime;

namespace Tiler.ui.custom
{
    public class PlacementComboBox : ComboBox
    {
        public PlacementComboBox()
        {
            foreach (var placement in Placement.values)
            {
                Items.Add(placement);
            }
        }
    }
}