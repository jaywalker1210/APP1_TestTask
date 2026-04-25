using Assets._Project.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Inventory
{
    public class InventorySlotUnlocker
    {
        private InventoryStorage storage;
        private EconomyService economy;

        public event Action<int> OnSlotUnlocked;

        public InventorySlotUnlocker(InventoryStorage storage, EconomyService economy)
        {
            this.storage = storage;
            this.economy = economy;
        }

        public bool TryUnlockSlot(int slotIndex, GameSettings settings)
        {
            if (slotIndex < 0 || slotIndex >= storage.Slots.Count) return false;
            if (storage.IsUnlocked(slotIndex)) return false;

            if (slotIndex > 0 && !storage.IsUnlocked(slotIndex - 1)) return false;

            int cost = settings.GetSlotUnlockCost(slotIndex);
            if (economy.SpendCoins(cost))
            {
                storage.SetUnlocked(slotIndex, true);
                OnSlotUnlocked?.Invoke(slotIndex);
                return true;
            }

            return false;
        }
    }
}
