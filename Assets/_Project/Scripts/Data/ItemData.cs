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

        public virtual string GetDescription(int amount)
        {
            string description = $"Вес: {weight}\n";
            description += $"Количество: {amount}\n";
            return description;
        }
    }
}
