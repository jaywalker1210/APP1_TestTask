using Assets._Project.Scripts.Intefaces;
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewAmmo", menuName = "Roguelike/Ammo")]
    public class AmmoData : ItemData, IAddableAmmo
    {
        public string ammoId;
    }
}
