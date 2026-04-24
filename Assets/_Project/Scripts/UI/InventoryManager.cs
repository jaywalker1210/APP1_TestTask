using Assets._Project.Scripts.Data;
using Assets._Project.Scripts.Managers;
using Assets._Project.Scripts.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Scripts.UI
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("References")]
        public Transform inventoryGrid;
        public GameObject slotPrefab;
        public GameSettings gameSettings;

        [Header("Item Lists (drag & drop .asset files here)")]
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<AmmoData> ammoTypes = new List<AmmoData>();

        [Header("State")]
        public List<InventorySlot> slots = new List<InventorySlot>();

        private float totalWeight;
        private GameManager gameManager;

        public float TotalWeight => totalWeight;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            InitializeInventory();
            LoadInventory();
        }

        private void OnApplicationQuit()
        {
            SaveInventory();
        }

        void InitializeInventory()
        {
            slots.Clear();
            for (int i = 0; i < gameSettings.totalSlots; i++)
            {
                GameObject slotGO = Instantiate(slotPrefab, inventoryGrid);
                InventorySlot slot = slotGO.GetComponent<InventorySlot>();
                bool unlocked = i < gameSettings.initialUnlockedSlots;
                slot.Initialize(i, unlocked, this);
                slots.Add(slot);
            }
            Debug.Log($"Создано слотов: {slots.Count}");
            UpdateTotalWeight();
        }

        public void TryUnlockSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            if (slots[slotIndex].IsUnlocked) return;

            if (slotIndex > 0 && !slots[slotIndex - 1].IsUnlocked)
            {
                Debug.Log($"Нельзя разблокировать слот {slotIndex}, пока не разблокирован слот {slotIndex - 1}");
                return;
            }

            int cost = gameSettings.GetSlotUnlockCost(slotIndex);
            if (gameManager.SpendCoins(cost))
            {
                slots[slotIndex].SetUnlocked(true);
                slots[slotIndex].UpdateVisual();
                Debug.Log($"Слот {slotIndex} разблокирован за {cost} монет");
            }
            else
            {
                Debug.Log($"Недостаточно монет для разблокировки слота {slotIndex}. Нужно {cost}");
            }
        }

        public bool AddItem(ItemData item, int amount = 1)
        {
            // TODO: поиск стаков, пустых слотов
            Debug.Log($"Добавлен предмет {item.itemName} в количестве {amount}");
            UpdateTotalWeight();
            return true;
        }

        public void UpdateTotalWeight()
        {
            totalWeight = 0;
            foreach (var slot in slots)
            {
                var item = slot.GetCurrentItem();
                if (item != null && item.itemData != null)
                {
                    totalWeight += item.itemData.weight * item.amount;
                }
            }
            UIManager ui = FindObjectOfType<UIManager>();
            ui?.UpdateWeightUI(totalWeight);
        }

        public InventoryItem GetItemInSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return null;
            return slots[slotIndex].GetCurrentItem();
        }

        public void Shoot()
        {
            List<int> weaponSlots = new List<int>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].IsUnlocked) continue;
                var item = slots[i].GetCurrentItem();
                if (item != null && item.itemData is WeaponData)
                {
                    weaponSlots.Add(i);
                }
            }

            if (weaponSlots.Count == 0)
            {
                Debug.LogError("Нет оружия");
                return;
            }

            int randomWeaponSlot = weaponSlots[Random.Range(0, weaponSlots.Count)];
            WeaponData weapon = slots[randomWeaponSlot].GetCurrentItem().itemData as WeaponData;

            int ammoSlotIndex = -1;
            AmmoData foundAmmo = null;
            foreach (var slot in slots)
            {
                if (!slot.IsUnlocked) continue;
                var item = slot.GetCurrentItem();
                if (item != null && item.itemData is AmmoData ammo)
                {
                    foreach (AmmoData compatibleAmmo in weapon.compatibleAmmoIds)
                    {
                        if (compatibleAmmo == ammo)
                        {
                            ammoSlotIndex = slots.IndexOf(slot);
                            foundAmmo = ammo;
                            break;
                        }
                    }
                    if (ammoSlotIndex != -1) break;
                }
            }

            if (ammoSlotIndex == -1)
            {
                Debug.LogError($"Нет патронов для {weapon.itemName}");
                return;
            }

            InventoryItem ammoItem = slots[ammoSlotIndex].GetCurrentItem();
            ammoItem.amount--;
            if (ammoItem.amount <= 0)
            {
                slots[ammoSlotIndex].currentItem = null;
            }
            slots[ammoSlotIndex].UpdateVisual();

            Debug.Log($"Выстрел из {weapon.itemName}, патроны: {foundAmmo.itemName}, урон: {weapon.damage}");
            UpdateTotalWeight();
        }

        public void AddRandomAmmo()
        {
            if (ammoTypes.Count == 0)
            {
                Debug.LogError("Нет настроенных типов патронов!");
                return;
            }
            AmmoData selectedAmmo = ammoTypes[Random.Range(0, ammoTypes.Count)];
            int amountToAdd = Random.Range(10, 31);

            int remainingAmount = amountToAdd;

            foreach (var slot in slots)
            {
                if (!slot.IsUnlocked) continue;
                var item = slot.GetCurrentItem();
                if (item != null && item.itemData == selectedAmmo && item.amount < selectedAmmo.maxStack)
                {
                    int space = selectedAmmo.maxStack - item.amount;
                    int add = Mathf.Min(space, remainingAmount);
                    item.amount += add;
                    remainingAmount -= add;
                    slot.UpdateVisual();
                    Debug.Log($"Добавлено ({add}) {selectedAmmo.itemName} в слот: {slots.IndexOf(slot)}");

                    if (remainingAmount == 0) break;
                }
            }

            while (remainingAmount > 0)
            {
                bool added = false;
                for (int i = 0; i < slots.Count; i++)
                {
                    if (!slots[i].IsUnlocked) continue;
                    if (slots[i].GetCurrentItem() == null)
                    {
                        int add = Mathf.Min(selectedAmmo.maxStack, remainingAmount);
                        slots[i].currentItem = new InventoryItem
                        {
                            itemData = selectedAmmo,
                            amount = add
                        };
                        slots[i].UpdateVisual();
                        Debug.Log($"Добавлено {add} {selectedAmmo.itemName} в слот: {i}");
                        remainingAmount -= add;
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    Debug.LogError("Инвентарь полон");
                    break;
                }
            }

            UpdateTotalWeight();
        }

        public void AddRandomItem()
        {
            List<ItemData> availableItems = new List<ItemData>();
            availableItems.AddRange(weapons);
            availableItems.AddRange(armors);

            if (availableItems.Count == 0)
            {
                Debug.LogError("Нет настроенных предметов для добавления!");
                return;
            }

            ItemData randomItem = availableItems[Random.Range(0, availableItems.Count)];

            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].IsUnlocked) continue;
                if (slots[i].GetCurrentItem() == null)
                {
                    slots[i].currentItem = new InventoryItem
                    {
                        itemData = randomItem,
                        amount = 1
                    };
                    slots[i].UpdateVisual();
                    UpdateTotalWeight();

                    Debug.Log($"Добавлено {randomItem.itemName} в слот: {i}");
                    return;
                }
            }

            Debug.LogError("Инвентарь полон");
        }

        public void RemoveRandomItem()
        {
            List<int> nonEmptySlots = new List<int>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].IsUnlocked) continue;
                if (slots[i].GetCurrentItem() != null)
                {
                    nonEmptySlots.Add(i);
                }
            }

            if (nonEmptySlots.Count == 0)
            {
                Debug.LogError("Инвентарь пуст");
                return;
            }

            int randomSlot = nonEmptySlots[Random.Range(0, nonEmptySlots.Count)];
            InventoryItem itemToRemove = slots[randomSlot].GetCurrentItem();
            int amountRemoved = itemToRemove.amount;
            string itemName = itemToRemove.itemData.itemName;

            slots[randomSlot].currentItem = null;
            slots[randomSlot].UpdateVisual();
            UpdateTotalWeight();

            Debug.Log($"Удалено ({amountRemoved}) {itemName} из слота: {randomSlot}");
        }

        public void SaveInventory()
        {
            SaveData data = new SaveData();
            data.unlockedSlots = GetUnlockedSlotsList();
            data.items = GetItemsForSave();
            SaveSystem.SaveInventory(data);
            Debug.Log("Инвентарь сохранён");
        }

        public void LoadInventory()
        {
            SaveData data = SaveSystem.LoadInventory();
            if (data != null)
            {
                LoadFromSave(data);
                Debug.Log("Инвентарь загружен");
            }
            else
            {
                Debug.Log("Нет сохранения инвентаря. Начинаем с нуля.");
            }
        }

        private List<bool> GetUnlockedSlotsList()
        {
            List<bool> unlocked = new List<bool>();
            foreach (var slot in slots)
            {
                unlocked.Add(slot.IsUnlocked);
            }
            return unlocked;
        }

        public List<InventorySlotData> GetItemsForSave()
        {
            List<InventorySlotData> saveItems = new List<InventorySlotData>();
            for (int i = 0; i < slots.Count; i++)
            {
                var item = slots[i].GetCurrentItem();
                if (item != null && item.itemData != null)
                {
                    saveItems.Add(new InventorySlotData
                    {
                        itemId = item.itemData.id,
                        amount = item.amount,
                        slotIndex = i
                    });
                }
            }
            return saveItems;
        }

        public void LoadFromSave(SaveData data)
        {
            foreach (var slot in slots)
            {
                slot.currentItem = null;
            }

            if (data.items != null)
            {
                foreach (var savedItem in data.items)
                {
                    ItemData itemData = FindItemById(savedItem.itemId);
                    if (itemData != null && savedItem.slotIndex < slots.Count)
                    {
                        slots[savedItem.slotIndex].currentItem = new InventoryItem
                        {
                            itemData = itemData,
                            amount = savedItem.amount
                        };
                        slots[savedItem.slotIndex].UpdateVisual();
                    }
                }
            }

            if (data.unlockedSlots != null)
            {
                for (int i = 0; i < data.unlockedSlots.Count && i < slots.Count; i++)
                {
                    if (data.unlockedSlots[i] && !slots[i].IsUnlocked)
                    {
                        slots[i].SetUnlocked(true);
                    }
                }
            }

            UpdateTotalWeight();
            Debug.Log("Инвентарь восстановлен из сохранения");
        }

        private ItemData FindItemById(string id)
        {
            if (weapons != null)
                foreach (var w in weapons)
                    if (w.id == id) return w;

            if (armors != null)
                foreach (var a in armors)
                    if (a.id == id) return a;

            if (ammoTypes != null)
                foreach (var a in ammoTypes)
                    if (a.id == id) return a;

            Debug.LogWarning($"Предмет с id '{id}' не найден!");
            return null;
        }

        public void SwapOrDragItems(int fromSlot, int toSlot)
        {
            if (fromSlot < 0 || fromSlot >= slots.Count) return;
            if (toSlot < 0 || toSlot >= slots.Count) return;

            InventoryItem fromItem = slots[fromSlot].currentItem;
            InventoryItem toItem = slots[toSlot].currentItem;

            if (fromItem == null && toItem == null) return;

            if (toItem == null)
            {
                slots[toSlot].currentItem = new InventoryItem
                {
                    itemData = fromItem.itemData,
                    amount = fromItem.amount
                };
                slots[fromSlot].currentItem = null;

                slots[fromSlot].UpdateVisual();
                slots[toSlot].UpdateVisual();
                UpdateTotalWeight();
                return;
            }
            if (fromItem.itemData.id == toItem.itemData.id && fromItem.itemData.maxStack > 1)
            {
                int totalAmount = fromItem.amount + toItem.amount;
                int maxStack = fromItem.itemData.maxStack;

                if (totalAmount <= maxStack)
                {
                    toItem.amount = totalAmount;
                    slots[fromSlot].currentItem = null;
                }
                else
                {
                    toItem.amount = maxStack;
                    fromItem.amount = totalAmount - maxStack;
                }

                slots[fromSlot].UpdateVisual();
                slots[toSlot].UpdateVisual();
                UpdateTotalWeight();
                return;
            }
            slots[toSlot].currentItem = new InventoryItem
            {
                itemData = fromItem.itemData,
                amount = fromItem.amount
            };
            slots[fromSlot].currentItem = new InventoryItem
            {
                itemData = toItem.itemData,
                amount = toItem.amount
            };

            slots[fromSlot].UpdateVisual();
            slots[toSlot].UpdateVisual();
            UpdateTotalWeight();
        }
    }

    public class InventoryItem
    {
        public ItemData itemData;
        public int amount;
    }
}