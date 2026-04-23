using Assets._Project.Scripts.Intefaces;
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Roguelike/Weapon")]
    public class WeaponData : ItemData, IAddableItem, IShootableWeapon
    {
        public int damage;           // ← поле, отображается в инспекторе
        public AmmoData[] compatibleAmmoIds; // ← поле, отображается в инспекторе

        public int Damage => damage; // свойство для интерфейса
        public AmmoData[] CompatibleAmmoIds => compatibleAmmoIds; // свойство для интерфейса
    }
}
