using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Roguelike/Armor")]
    public class ArmorData : ItemData
    {
        public int protection;
    }
}