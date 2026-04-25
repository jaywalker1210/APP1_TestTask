using Assets._Project.Scripts.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Scripts.Services
{
    public class WeightCalculator
    {
        public float CalculateTotalWeight(InventoryStorage storage)
        {
            float total = 0;
            foreach (var slot in storage.Slots)
            {
                if (!slot.IsUnlocked) continue;
                if (slot.CurrentItem != null)
                {
                    total += slot.CurrentItem.itemData.weight * slot.CurrentItem.amount;
                }
            }
            return total;
        }
    }
}
