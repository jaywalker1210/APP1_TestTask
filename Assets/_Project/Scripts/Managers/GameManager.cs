using Assets._Project.Scripts.Saving;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public int coins;

        private void Start()
        {
            LoadGame();
        }

        private void LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data != null)
            {
                coins = data.coins;
            }
            else
            {
                coins = 0;
            }
            Debug.Log("Монет: " + coins);
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            Debug.Log("Монет: " + coins);
            SaveGame();
        }

        public void SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                Debug.Log("Монет: " + coins);
                SaveGame();
            }
            else
            {
                Debug.Log("Недостаточно монет!");
            }
        }

        private void SaveGame()
        {
            SaveData data = new SaveData();
            data.coins = coins;

            SaveSystem.Save(data);
        }
    }
}