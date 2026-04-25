using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    public enum ArmorType { Head, Torso }

    [CreateAssetMenu(fileName = "NewArmor", menuName = "Roguelike/Armor")]
    public class ArmorData : ItemData
    {
        public ArmorType armorType;
        public int protection;

        public override string GetDescription(int amount)
        {
            string description = base.GetDescription(amount);
            description += "\n";
            description += $"Тип: Броня\n";
            description += $"Защита: {protection}\n";
            description += $"Слот: {armorType}";
            return description;
        }
    }
}