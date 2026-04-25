using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Scripts.Inventory
{
    public class InventorySlotData
    {
        public int SlotIndex { get; set; }
        public bool IsUnlocked { get; set; }
        public InventoryItem CurrentItem { get; set; }
    }
}
