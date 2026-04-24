using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI
{
    public class InventorySlot : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IPointerClickHandler
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

        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private GameObject dragGhost;
        private static InventorySlot draggedFromSlot;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void Initialize(int index, bool unlocked, InventoryManager manager)
        {
            SlotIndex = index;
            IsUnlocked = unlocked;
            inventoryManager = manager;

            UpdateVisual();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsUnlocked) return;
            if (currentItem == null) return;

            draggedFromSlot = this;
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            dragGhost = new GameObject("DragGhost");
            dragGhost.transform.SetParent(canvas.transform, false);
            dragGhost.transform.SetAsLastSibling();

            Image ghostImage = dragGhost.AddComponent<Image>();
            ghostImage.sprite = icon.sprite;
            ghostImage.raycastTarget = false;

            RectTransform ghostRect = dragGhost.GetComponent<RectTransform>();
            ghostRect.sizeDelta = new Vector2(100, 100);
            ghostRect.position = Input.mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragGhost != null)
            {
                dragGhost.transform.position = Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (dragGhost != null)
            {
                Destroy(dragGhost);
            }

            draggedFromSlot = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (draggedFromSlot == null) return;
            if (draggedFromSlot == this) return;
            if (!IsUnlocked || !draggedFromSlot.IsUnlocked) return;

            inventoryManager.SwapOrDragItems(draggedFromSlot.SlotIndex, SlotIndex);

            draggedFromSlot = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsUnlocked)
            {
                OnSlotClick();
                return;
            }

            if (currentItem != null)
            {
                UIManager ui = FindObjectOfType<UIManager>();
                if (ui != null)
                {
                    ui.ShowTooltip(currentItem.itemData, currentItem.amount);
                }
            }
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