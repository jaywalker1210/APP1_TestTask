using UnityEngine;

namespace Assets._Project.Scripts.Data
{
    public abstract class ItemData: ScriptableObject
    {
        public string itemName;
        public string id;
        public Sprite icon;
        public float weight;
        public int maxStack = 1;
    }
}
