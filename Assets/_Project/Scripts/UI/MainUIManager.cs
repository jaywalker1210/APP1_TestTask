using Assets._Project.Scripts.Core;
using Assets._Project.Scripts.Data;
using Assets._Project.Scripts.Inventory;
using Assets._Project.Scripts.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI
{
    public class MainUIManager : MonoBehaviour
    {
        [Header("Button References")]
        public Button shootButton;
        public Button addAmmoButton;
        public Button addItemButton;
        public Button removeItemButton;
        public Button addCoinsButton;

        [Header("UI Text Elements (drag from scene)")]
        public TextMeshProUGUI coinsText;
        public TextMeshProUGUI weightText;

        [Header("Tooltip (drag from scene)")]
        public GameObject tooltipPanel;
        public TextMeshProUGUI tooltipNameText;
        public TextMeshProUGUI tooltipInfoText;
        public Button closeButton;

        [Header("Inventory Grid")]
        public Transform inventoryGrid;

        [Header("Prefabs")]
        public GameObject slotPrefab;

        [Header("Settings")]
        public GameSettings gameSettings;

        [Header("Item Lists (drag from InventoryManager)")]
        public List<WeaponData> weapons;
        public List<ArmorData> armors;
        public List<AmmoData> ammoTypes;

        [Header("References")]
        public GameManager gameManager;
        public InventoryStorage storage;

        private WeightCalculator weightCalc;
        private LootService loot;
        private CombatService combat;
        private InventorySlotUnlocker unlocker;

        void Start()
        {
            if (storage == null)
            {
                Debug.LogError("MainUIManager: storage is null!");
                return;
            }

            storage.Initialize(gameSettings.totalSlots, gameSettings.initialUnlockedSlots);

            weightCalc = new WeightCalculator();
            loot = new LootService();
            combat = new CombatService();
            unlocker = new InventorySlotUnlocker(storage, gameManager.Economy);

            gameManager.Economy.OnCoinsChanged += UpdateCoinsUI;
            UpdateCoinsUI(gameManager.Economy.Coins);

            CreateInventorySlots();

            shootButton?.onClick.AddListener(OnShoot);
            addAmmoButton?.onClick.AddListener(OnAddAmmo);
            addItemButton?.onClick.AddListener(OnAddItem);
            removeItemButton?.onClick.AddListener(OnRemoveItem);
            addCoinsButton?.onClick.AddListener(() => gameManager.AddRandomCoins());
            closeButton?.onClick.AddListener(() => HideTooltip());

            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }

        private void CreateInventorySlots()
        {
            foreach (Transform child in inventoryGrid)
                Destroy(child.gameObject);

            for (int i = 0; i < gameSettings.totalSlots; i++)
            {
                var slotGO = Instantiate(slotPrefab, inventoryGrid);
                var slot = slotGO.GetComponent<InventorySlot>();

                slot.Initialize(i, storage, unlocker, gameSettings, this);
            }
        }

        private void OnShoot()
        {
            if (combat.TryShoot(storage, out int damage, out string log))
            {
                Debug.Log(log);
                RefreshAllSlots();
                RefreshWeight();
            }
            else
            {
                Debug.LogError(log);
            }
        }

        private void OnAddAmmo()
        {
            if (loot.TryAddRandomAmmo(storage, ammoTypes, out string log))
            {
                RefreshAllSlots();
                RefreshWeight();
                SaveInventory();
            }
        }

        private void OnAddItem()
        {
            if (loot.TryAddRandomItem(storage, weapons, armors))
            {

                RefreshAllSlots();
                RefreshWeight();
                SaveInventory();
            }
        }

        private void OnRemoveItem()
        {
            if (loot.TryRemoveRandomItem(storage, out string log))
            {
                RefreshAllSlots();
                RefreshWeight();
                SaveInventory();
            }
        }

        public void RefreshWeight()
        {
            if (weightCalc == null)
            {
                Debug.LogWarning("weightCalc is null");
                return;
            }

            if (storage == null)
            {
                Debug.LogWarning("storage is null");
                return;
            }

            if (weightText == null)
            {
                Debug.LogWarning("weightText is null");
                return;
            }

            float weight = weightCalc.CalculateTotalWeight(storage);
            weightText.text = $"Вес: {weight:0.###}";
        }

        private void UpdateCoinsUI(int coins)
        {
            if (coinsText != null)
                coinsText.text = $"Монеты: {coins}";
        }

        public void RefreshAllSlots()
        {
            var slots = FindObjectsOfType<InventorySlot>();
            foreach (var slot in slots)
            {
                slot.UpdateView();
            }
        }

        public void ShowTooltip(ItemData item, int amount)
        {
            if (tooltipPanel == null) return;

            if (tooltipNameText != null)
                tooltipNameText.text = item.itemName;

            if (tooltipInfoText != null)
                tooltipInfoText.text = item.GetDescription(amount);

            tooltipPanel.SetActive(true);
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }

        public void SaveInventory()
        {
            if (gameManager != null)
                gameManager.SaveInventory();
        }
    }
}