using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Scripts.Saving
{
    [Serializable]
    public class SaveData
    {
        public int coins;
        public List<bool> unlockedSlots; // true - открыт, false - закрыт
        public List<InventorySlotData> items;
    }

    [Serializable]
    public class InventorySlotData
    {
        public string itemId;
        public int amount;
        public int slotIndex;
    }
}