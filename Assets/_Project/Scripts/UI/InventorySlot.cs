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
        private InventoryManager inventoryManager;

        public InventoryItem currentItem;

        public void Initialize(int index, bool unlocked, InventoryManager manager)
        {
            SlotIndex = index;
            IsUnlocked = unlocked;
            inventoryManager = manager;

            UpdateVisual();

            // Добавляем слушатель на кнопку (она должна быть на корневом InventorySlot)
            Button btn = GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(OnSlotClick);
        }

        public void UpdateVisual()
        {
            // Показываем нужную панель
            if (unlockOverlay != null) unlockOverlay.SetActive(IsUnlocked);
            if (lockOverlay != null) lockOverlay.SetActive(!IsUnlocked);

            // Если слот открыт — обновляем предметы
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
                        amountText.text = item.amount > 1 ? item.amount.ToString() : "";
                }
                else
                {
                    if (icon != null) icon.gameObject.SetActive(false);
                    if (amountText != null) amountText.text = "";
                }
            }
            else // Если слот закрыт — показываем стоимость
            {
                if (costText != null && inventoryManager.gameSettings != null)
                    costText.text = inventoryManager.gameSettings.slotUnlockCost.ToString();
            }
        }

        private void OnSlotClick()
        {
            if (!IsUnlocked)
            {
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