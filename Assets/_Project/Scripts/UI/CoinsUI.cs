using TMPro;
using UnityEngine;
using Assets._Project.Scripts.Core;

namespace Assets._Project.Scripts.UI.Components
{
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;

        public void Initialize(EconomyService economy)
        {
            economy.OnCoinsChanged += UpdateCoins;
            UpdateCoins(economy.Coins);
        }

        private void UpdateCoins(int coins)
        {
            if (coinsText != null)
                coinsText.text = $"Монеты: {coins}";
        }
    }
}