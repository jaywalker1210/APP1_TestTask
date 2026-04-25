using Assets._Project.Scripts.Data;
using Assets._Project.Scripts.Inventory;
using Assets._Project.Scripts.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Scripts.Core
{
    public class SaveService
    {
        private readonly ISaveSystem saveSystem;
        private const string COINS_FILE = "coins.json";
        private const string INVENTORY_FILE = "inventory.json";

        public SaveService(ISaveSystem saveSystem = null)
        {
            this.saveSystem = saveSystem ?? new JsonSaveSystem();
        }

        public void SaveCoins(int coins)
        {
            saveSystem.Save(new CoinsSaveData { coins = coins }, COINS_FILE);
            Debug.Log($"Сохранено {coins} монет в {COINS_FILE}");
        }

        public int LoadCoins()
        {
            var data = saveSystem.Load<CoinsSaveData>(COINS_FILE);
            Debug.Log($"Загружено {data.coins} монет из {COINS_FILE}");
            return data.coins;
        }

        public void SaveInventory(InventorySaveData data)
        {
            saveSystem.Save(data, INVENTORY_FILE);
            Debug.Log($"Сохранен инвертарь в {INVENTORY_FILE}");
        }

        public InventorySaveData LoadInventory()
        {
            Debug.Log($"Загружен инвертарь из {INVENTORY_FILE}");
            return saveSystem.Load<InventorySaveData>(INVENTORY_FILE);
        }

        public void SaveInventoryState(InventoryStorage storage)
        {
            var data = new InventorySaveData();
            data.unlockedSlots = new List<bool>();
            data.items = new List<SerializedInventorySlot>();

            for (int i = 0; i < storage.Slots.Count; i++)
            {
                data.unlockedSlots.Add(storage.IsUnlocked(i));
            }

            for (int i = 0; i < storage.Slots.Count; i++)
            {
                var item = storage.GetItem(i);
                if (item != null && item.amount > 0 && item.itemData != null)
                {
                    data.items.Add(new SerializedInventorySlot
                    {
                        itemId = item.itemData.id,
                        amount = item.amount,
                        slotIndex = i
                    });
                }
            }

            SaveInventory(data);
            Debug.Log($"Сохранено предметов: {data.items.Count}, слотов: {data.unlockedSlots.Count}");
        }

        public void LoadInventoryState(InventoryStorage storage, ItemDatabase itemDatabase)
        {
            var data = LoadInventory();
            if (data == null) return;

            if (data.unlockedSlots != null)
            {
                for (int i = 0; i < data.unlockedSlots.Count && i < storage.Slots.Count; i++)
                {
                    storage.SetUnlocked(i, data.unlockedSlots[i]);
                }
            }

            for (int i = 0; i < storage.Slots.Count; i++)
            {
                storage.SetItem(i, null);
            }

            if (data.items != null)
            {
                foreach (var savedItem in data.items)
                {
                    ItemData itemData = itemDatabase.GetItemById(savedItem.itemId);
                    if (itemData != null)
                    {
                        storage.SetItem(savedItem.slotIndex, new InventoryItem
                        {
                            itemData = itemData,
                            amount = savedItem.amount
                        });
                        Debug.Log($"Загружен предмет {itemData.itemName} (x{savedItem.amount}) в слот {savedItem.slotIndex}");
                    }
                }
            }

            Debug.Log($"Загружено предметов: {data.items?.Count ?? 0}");
        }

        [System.Serializable]
        private class CoinsSaveData
        {
            public int coins;
        }
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<bool> unlockedSlots;
        public List<SerializedInventorySlot> items;
    }

    [System.Serializable]
    public class SerializedInventorySlot
    {
        public string itemId;
        public int amount;
        public int slotIndex;
    }
}