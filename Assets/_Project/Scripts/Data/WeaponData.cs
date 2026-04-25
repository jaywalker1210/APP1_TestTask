using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Roguelike/Weapon")]
    public class WeaponData : ItemData
    {
        public int damage;
        public AmmoData[] compatibleAmmoIds;

        public int Damage => damage;
        public AmmoData[] CompatibleAmmoIds => compatibleAmmoIds;

        public override string GetDescription(int amount)
        {
            string description = base.GetDescription(amount);
            description += "\n";
            description += $"Тип: Оружие\n";
            description += $"Урон: {damage}\n";
            description += $"Патроны: ";

            if (compatibleAmmoIds != null && compatibleAmmoIds.Length > 0)
            {
                for (int i = 0; i < compatibleAmmoIds.Length; i++)
                {
                    description += compatibleAmmoIds[i].itemName;
                    if (i < compatibleAmmoIds.Length - 1)
                        description += ", ";
                }
            }
            else
            {
                description += "нет";
            }

            return description;
        }
    }
}
