using Assets._Project.Scripts.Saving;
using Assets._Project.Scripts.UI;
using UnityEngine;

namespace Assets._Project.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        public InventoryManager inventoryManager;
        public UIManager uiManager;

        [Header("Player Data")]
        public int coins;

        private SaveData saveData;

        void Start()
        {
            LoadGame();
            UpdateCoinsUI();
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            SaveGame();
            UpdateCoinsUI();
        }

        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                Debug.Log($"Потрачено {amount} монет. Осталось: {coins}");
                SaveGame();
                UpdateCoinsUI();
                return true;
            }
            else
            {
                Debug.Log($"Недостаточно монет. Нужно {amount}, есть {coins}");
                return false;
            }
        }

        public void AddRandomCoins()
        {
            int randomCoins = Random.Range(9, 100); // от 9 до 99 включительно
            AddCoins(randomCoins);
            Debug.Log($"Добавлено ({randomCoins}) монет");
        }

        private void UpdateCoinsUI()
        {
            if (uiManager != null)
                uiManager.UpdateCoinsUI(coins);
        }

        private void SaveGame()
        {
            SaveData data = new SaveData();
            data.coins = coins;

            // Сохраняем инвентарь, если есть
            if (inventoryManager != null)
            {
                data.items = inventoryManager.GetItemsForSave();
            }

            SaveSystem.Save(data);
        }

        private void LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data != null)
            {
                coins = data.coins;

                // Загружаем инвентарь
                if (inventoryManager != null && data.unlockedSlots != null)
                {
                    inventoryManager.LoadFromSave(data);
                }
            }
            else
            {
                coins = 0;
                Debug.Log("Нет сохранения. Начинаем с нуля.");
            }
        }
    }
}