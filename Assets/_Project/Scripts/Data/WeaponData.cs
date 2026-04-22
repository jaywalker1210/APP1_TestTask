using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Roguelike/Weapon")]
    public class WeaponData: ItemData
    {
        public int damage;
        public string ammoId;
    }
}
