using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Roguelike/Item")]
    public class ItemData: ScriptableObject
    {
        public string itemName;
        public string id;
        public ItemType type;
        public Sprite icon;
        public float weight;
        public int maxStack = 1;
    }
}
