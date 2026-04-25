using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Core
{
    public class EconomyService
    {
        public int Coins { get; private set; }
        public event Action<int> OnCoinsChanged;

        public void AddCoins(int amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }

        public bool SpendCoins(int amount)
        {
            if (Coins >= amount)
            {
                Coins -= amount;
                OnCoinsChanged?.Invoke(Coins);
                Debug.Log($"Потрачено монет {amount}. Осталось {Coins}");
                return true;
            }
            Debug.LogWarning($"Не хватает денег");
            return false;
        }

        public void SetCoins(int amount)
        {
            Coins = amount;
            OnCoinsChanged?.Invoke(Coins);
        }
    }
}
