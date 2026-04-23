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
        private GameManager gameManager; // нужен DI?

        public float TotalWeight => totalWeight;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            InitializeInventory();
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

            int cost = gameSettings.slotUnlockCost;
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

        public List<InventorySlotData> GetItemsForSave()
        {
            // TODO: сохранить предметы
            return new List<InventorySlotData>();
        }

        public void LoadFromSave(SaveData data)
        {
            // TODO: загрузить предметы
        }
    }

    public class InventoryItem
    {
        public ItemData itemData;
        public int amount;
    }
}