using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI
{
    public class InventorySlot : MonoBehaviour
    {
        [Header("Корневые панели")]
        public GameObject unlockOverlay;   // Панель открытого слота
        public GameObject lockOverlay;     // Панель заблокированного слота

        [Header("Элементы открытого слота (UnlockOverlay)")]
        public Image icon;                 // Иконка предмета
        public TextMeshProUGUI amountText; // Количество предметов

        [Header("Элементы заблокированного слота (LockOverlay)")]
        public Image lockIcon;             // Иконка замка (опционально)
        public TextMeshProUGUI costText;   // Текст стоимости разблокировки

        public int SlotIndex { get; set; }
        public bool IsUnlocked { get; private set; }
        private InventoryManager inventoryManager; // DI

        public InventoryItem currentItem;

        public void Initialize(int index, bool unlocked, InventoryManager manager)
        {
            SlotIndex = index;
            IsUnlocked = unlocked;
            inventoryManager = manager;

            UpdateVisual();

            Button btn = GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(OnSlotClick);
        }


        public void UpdateVisual()
        {
            if (unlockOverlay != null) unlockOverlay.SetActive(IsUnlocked);
            if (lockOverlay != null) lockOverlay.SetActive(!IsUnlocked);

            if (IsUnlocked)
            {
                var item = inventoryManager?.GetItemInSlot(SlotIndex);

                if (item != null && item.amount > 0)
                {
                    if (icon != null)
                    {
                        icon.sprite = item.itemData.icon;
                        icon.gameObject.SetActive(true);
                    }
                    if (amountText != null)
                    {
                        if (item.amount > 1)
                        {
                            amountText.text = item.amount.ToString();
                            amountText.gameObject.SetActive(true);

                            if (amountText.transform.parent != null)
                            {
                                amountText.transform.parent.gameObject.SetActive(true);
                            } 
                        }
                        else
                        {
                            amountText.text = "";
                            amountText.gameObject.SetActive(false);

                            if (amountText.transform.parent != null)
                                amountText.transform.parent.gameObject.SetActive(false);
                        }
                    }
                        
                }
                else
                {
                    if (icon != null) icon.gameObject.SetActive(false);
                    if (amountText != null)
                    {
                        amountText.text = "";
                        amountText.gameObject.SetActive(false);

                        if (amountText.transform.parent != null)
                            amountText.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (costText != null && inventoryManager.gameSettings != null)
                {
                    int currentCost = inventoryManager.gameSettings.GetSlotUnlockCost(SlotIndex);
                    costText.text = currentCost.ToString();
                }
            }
        }

        private void OnSlotClick()
        {
            if (!IsUnlocked)
            {
                UpdateVisual();
                inventoryManager.TryUnlockSlot(SlotIndex);
            }
            else
            {
                Debug.Log($"Нажат открытый слот {SlotIndex}");
                // Здесь потом можно сделать показ информации о предмете
            }
        }

        public void SetUnlocked(bool unlocked)
        {
            IsUnlocked = unlocked;
            UpdateVisual();
        }

        public InventoryItem GetCurrentItem()
        {
            return currentItem;
        }


    }
}