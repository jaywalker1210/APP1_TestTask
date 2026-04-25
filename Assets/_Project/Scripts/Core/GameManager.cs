using UnityEngine;
using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.Inventory;

namespace Assets._Project.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public EconomyService Economy { get; private set; }
        public SaveService SaveService { get; private set; }

        [SerializeField] private MainUIManager uiManager;
        [SerializeField] private InventoryStorage inventoryStorage;
        [SerializeField] private ItemDatabase itemDatabase;

        void Awake()
        {
            Economy = new EconomyService();
            SaveService = new SaveService();

            if (itemDatabase != null)
                itemDatabase.Initialize();
        }

        void Start()
        {
            Invoke(nameof(LoadGame), 0.001f);
        }

        private void LoadGame()
        {
            if (uiManager == null)
            {
                Debug.LogError("GameManager: uiManager is null!");
                return;
            }

            int savedCoins = SaveService.LoadCoins();
            Economy.SetCoins(savedCoins);

            if (inventoryStorage != null && itemDatabase != null)
            {
                SaveService.LoadInventoryState(inventoryStorage, itemDatabase);
            }

            uiManager.RefreshAllSlots();
            uiManager.RefreshWeight();

            Economy.OnCoinsChanged += (coins) => SaveService.SaveCoins(coins);
        }

        public void SaveInventory()
        {
            if (inventoryStorage != null)
            {
                SaveService.SaveInventoryState(inventoryStorage);
            }
        }

        public void AddRandomCoins()
        {
            int random = Random.Range(9, 100);
            Economy.AddCoins(random);
            Debug.Log($"Добавлено {random} монет");
        }

        private void OnApplicationQuit()
        {
            SaveInventory();
        }
    }
}