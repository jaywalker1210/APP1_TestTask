using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets._Project.Scripts.Inventory;
using Unity.VisualScripting;

namespace Assets._Project.Scripts.UI
{
    public class InventorySlot : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IScrollHandler
    {
        [Header("UI Elements")]
        public GameObject unlockOverlay;
        public GameObject lockOverlay;
        public Image icon;
        public TextMeshProUGUI amountText;
        public TextMeshProUGUI costText;

        private int slotIndex;
        private InventoryStorage storage;
        private InventorySlotUnlocker unlocker;
        private GameSettings settings;
        private MainUIManager uiManager;

        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private GameObject dragGhost;
        private static InventorySlot draggedFrom;
        private ScrollRect parentScrollRect;

        // Флаг для отслеживания активного перетаскивания
        private bool isDragging = false;
        public void Initialize(int index, InventoryStorage storage, InventorySlotUnlocker unlocker,
                               GameSettings settings, MainUIManager uiManager)
        {
            this.slotIndex = index;
            this.storage = storage;
            this.unlocker = unlocker;
            this.settings = settings;
            this.uiManager = uiManager;

            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            parentScrollRect = GetComponentInParent<ScrollRect>();

            UpdateView();
        }

        public void UpdateView()
        {
            bool unlocked = storage.IsUnlocked(slotIndex);

            if (lockOverlay != null) lockOverlay.SetActive(!unlocked);
            if (unlockOverlay != null) unlockOverlay.SetActive(unlocked);

            if (!unlocked)
            {
                if (icon != null) icon.gameObject.SetActive(false);
                if (amountText != null) amountText.gameObject.SetActive(false);
                if (costText != null)
                {
                    costText.gameObject.SetActive(true);
                    costText.text = settings.GetSlotUnlockCost(slotIndex).ToString();
                }
                return;
            }

            if (costText != null) costText.gameObject.SetActive(false);

            var item = storage.GetItem(slotIndex);
            if (item == null || item.amount == 0)
            {
                if (icon != null) icon.gameObject.SetActive(false);
                if (amountText != null) amountText.gameObject.SetActive(false);
                if (item == null) amountText.transform.parent.gameObject.SetActive(false);
                return;
            }

            if (unlockOverlay != null) unlockOverlay.SetActive(true);

            if (icon != null)
            {
                icon.sprite = item.itemData.icon;
                icon.gameObject.SetActive(true);
            }

            if (amountText != null)
            {
                amountText.text = item.amount > 1 ? item.amount.ToString() : "";
                amountText.gameObject.SetActive(item.amount > 1);

                if (item.amount > 1)
                {
                    amountText.transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    amountText.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDragging) return;

            if (!storage.IsUnlocked(slotIndex))
            {
                if (unlocker.TryUnlockSlot(slotIndex, settings))
                {
                    UpdateView();
                    uiManager?.RefreshWeight();
                    uiManager?.SaveInventory();
                }
            }
            else
            {
                var item = storage.GetItem(slotIndex);
                if (item != null)
                {
                    uiManager?.ShowTooltip(item.itemData, item.amount);
                }
                else
                {
                    uiManager?.HideTooltip();
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!storage.IsUnlocked(slotIndex)) return;

            var item = storage.GetItem(slotIndex);
            if (item == null) return;

            // Начинаем перетаскивание
            isDragging = true;
            draggedFrom = this;
            
            // Отключаем скролл во время перетаскивания
            if (parentScrollRect != null)
                parentScrollRect.enabled = false;
            
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            dragGhost = new GameObject("DragGhost");
            dragGhost.transform.SetParent(canvas.transform, false);
            dragGhost.transform.SetAsLastSibling();
            
            var ghostImage = dragGhost.AddComponent<Image>();
            ghostImage.sprite = icon.sprite;
            ghostImage.raycastTarget = false;
            
            dragGhost.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            dragGhost.transform.position = Input.mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragGhost != null)
                dragGhost.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (draggedFrom == null) return;

            // Восстанавливаем скролл
            if (parentScrollRect != null)
                parentScrollRect.enabled = true;

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            
            if (dragGhost != null) 
                Destroy(dragGhost);
            
            draggedFrom = null;
            isDragging = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (draggedFrom == null || draggedFrom == this) return;
            if (!storage.IsUnlocked(draggedFrom.slotIndex) || !storage.IsUnlocked(slotIndex)) return;

            if (!storage.TryMergeItems(draggedFrom.slotIndex, slotIndex))
            {
                storage.SwapItems(draggedFrom.slotIndex, slotIndex);
            }

            draggedFrom.UpdateView();
            UpdateView();

            uiManager?.RefreshWeight();
        }

        public void OnScroll(PointerEventData eventData)
        {
            // Если перетаскиваем предмет - не скроллим
            if (isDragging) return;
            
            // Если есть ScrollRect - скроллим его
            if (parentScrollRect != null)
            {
                // Прокручиваем контент вертикально
                parentScrollRect.verticalNormalizedPosition += 
                    eventData.scrollDelta.y * 0.1f;
                
                // Альтернативный способ через ExecuteEvents
                // ExecuteEvents.Execute(parentScrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
            }
        }
    }
}