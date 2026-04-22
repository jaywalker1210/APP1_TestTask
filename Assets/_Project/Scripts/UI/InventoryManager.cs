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

        [Header("State")]
        public List<InventorySlot> slots = new List<InventorySlot>();

        private float totalWeight;
        private GameManager gameManager;

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
            if (ui != null) ui.UpdateWeightUI(totalWeight);
        }

        public InventoryItem GetItemInSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return null;
            return slots[slotIndex].GetCurrentItem();
        }

        public void Shoot()
        {
            Debug.Log("Логика выстрела будет здесь");
        }

        public void AddRandomAmmo()
        {
            Debug.Log("Логика добавления патронов будет здесь");
        }

        public void AddRandomItem()
        {
            Debug.Log("Логика добавления случайного предмета будет здесь");
        }

        public void RemoveRandomItem()
        {
            Debug.Log("Логика удаления случайного предмета будет здесь");
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