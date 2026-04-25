using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Inventory
{
    public class InventoryStorage: MonoBehaviour 
    {
        public List<InventorySlotData> slots = new List<InventorySlotData>();

        public IReadOnlyList<InventorySlotData> Slots => slots;

        public void Initialize(int totalSlots, int initialUnlocked)
        {
            slots.Clear();
            for (int i = 0; i < totalSlots; i++)
            {
                slots.Add(new InventorySlotData
                {
                    SlotIndex = i,
                    IsUnlocked = i < initialUnlocked,
                    CurrentItem = null
                });
            }
        }

        public InventoryItem GetItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return null;
            return slots[slotIndex].CurrentItem;
        }

        public void SetItem(int slotIndex, InventoryItem item)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            slots[slotIndex].CurrentItem = item;
        }

        public bool IsUnlocked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return false;
            return slots[slotIndex].IsUnlocked;
        }

        public void SetUnlocked(int slotIndex, bool unlocked)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            slots[slotIndex].IsUnlocked = unlocked;
        }

        public void SwapItems(int from, int to)
        {
            var fromItem = slots[from].CurrentItem;
            var toItem = slots[to].CurrentItem;

            slots[from].CurrentItem = toItem;
            slots[to].CurrentItem = fromItem;
        }

        public bool TryMergeItems(int from, int to)
        {
            var fromItem = slots[from].CurrentItem;
            var toItem = slots[to].CurrentItem;

            if (fromItem == null || toItem == null) return false;
            if (fromItem.itemData.id != toItem.itemData.id) return false;
            if (fromItem.itemData.maxStack <= 1) return false;

            int total = fromItem.amount + toItem.amount;
            int maxStack = fromItem.itemData.maxStack;

            if (total <= maxStack)
            {
                toItem.amount = total;
                slots[from].CurrentItem = null;
            }
            else
            {
                toItem.amount = maxStack;
                fromItem.amount = total - maxStack;
            }

            return true;
        }
    }
}
