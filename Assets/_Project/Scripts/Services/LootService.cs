using System;
using System.Collections.Generic;
using Assets._Project.Scripts.Data;
using Assets._Project.Scripts.Inventory;
using UnityEngine;
using Random = System.Random;

namespace Assets._Project.Scripts.Services
{
    public class LootService
    {
        private Random random = new Random();

        public bool TryAddRandomItem(InventoryStorage storage, List<WeaponData> weapons, List<ArmorData> armors)
        {
            List<ItemData> available = new List<ItemData>();
            available.AddRange(weapons);
            available.AddRange(armors);

            if (available.Count == 0) return false;

            ItemData randomItem = available[UnityEngine.Random.Range(0, available.Count)];

            for (int i = 0; i < storage.Slots.Count; i++)
            {
                if (!storage.IsUnlocked(i)) continue;
                if (storage.GetItem(i) == null)
                {
                    storage.SetItem(i, new InventoryItem { itemData = randomItem, amount = 1 });
                    Debug.Log($"Добавлено {randomItem.itemName} в слот: {i}");
                    return true;
                }
            }
            Debug.LogWarning($"Инвентарь полон");
            return false;
        }

        public bool TryAddRandomAmmo(InventoryStorage storage, List<AmmoData> ammoTypes, out string log)
        {
            log = "";

            if (ammoTypes.Count == 0)
            {
                log = "Нет доступных типов патронов";
                Debug.LogError(log);
                return false;
            }

            AmmoData selectedAmmo = ammoTypes[UnityEngine.Random.Range(0, ammoTypes.Count)];
            int amountToAdd = UnityEngine.Random.Range(10, 31);
            int remaining = amountToAdd;

            List<int> usedSlots = new List<int>();

            for (int i = 0; i < storage.Slots.Count && remaining > 0; i++)
            {
                if (!storage.IsUnlocked(i)) continue;
                var item = storage.GetItem(i);
                if (item != null && item.itemData == selectedAmmo && item.amount < selectedAmmo.maxStack)
                {
                    int space = selectedAmmo.maxStack - item.amount;
                    int add = Mathf.Min(space, remaining);
                    item.amount += add;
                    remaining -= add;

                    if (!usedSlots.Contains(i))
                        usedSlots.Add(i);
                }
            }

            for (int i = 0; i < storage.Slots.Count && remaining > 0; i++)
            {
                if (!storage.IsUnlocked(i)) continue;
                if (storage.GetItem(i) == null)
                {
                    int add = Mathf.Min(selectedAmmo.maxStack, remaining);
                    storage.SetItem(i, new InventoryItem { itemData = selectedAmmo, amount = add });
                    remaining -= add;
                    usedSlots.Add(i);
                }
            }

            if (remaining < amountToAdd)
            {
                int addedAmount = amountToAdd - remaining;
                string slotsInfo = string.Join(", ", usedSlots);
                log = $"Добавлено ({addedAmount}) {selectedAmmo.itemName} в слоты: [{slotsInfo}]";
                Debug.Log(log);
                return true;
            }

            log = "Инвентарь полон";
            Debug.LogError(log);
            return false;
        }

        public bool TryRemoveRandomItem(InventoryStorage storage, out string log)
        {
            log = "";

            List<int> nonEmptySlots = new List<int>();
            for (int i = 0; i < storage.Slots.Count; i++)
            {
                if (storage.IsUnlocked(i) && storage.GetItem(i) != null)
                    nonEmptySlots.Add(i);
            }

            if (nonEmptySlots.Count == 0)
            {
                log = "Инвентарь пуст";
                Debug.LogError(log);
                return false;
            }

            int randomSlot = nonEmptySlots[UnityEngine.Random.Range(0, nonEmptySlots.Count)];
            var item = storage.GetItem(randomSlot);
            int amount = item.amount;
            string itemName = item.itemData.itemName;

            storage.SetItem(randomSlot, null);

            log = $"Удалено ({amount}) {itemName} из слота: {randomSlot}";
            Debug.Log(log);

            return true;
        }
    }
}