using Assets._Project.Scripts.Intefaces;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    public enum ArmorType { Head, Torso }

    [CreateAssetMenu(fileName = "NewArmor", menuName = "Roguelike/Armor")]
    public class ArmorData : ItemData, IAddableItem
    {
        public ArmorType armorType;
        public int protection;
    }
}