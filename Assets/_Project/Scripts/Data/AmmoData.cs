
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewAmmo", menuName = "Roguelike/Ammo")]
    public class AmmoData : ItemData
    {
        public string ammoId;

        public override string GetDescription(int amount)
        {
            string description = base.GetDescription(amount);
            description += "\n";
            description += $"Тип: Патроны\n";
            description += $"Макс. стак: {maxStack}\n"; // нужен ли он в информации для игрока?
            return description;
        }
    }
}
