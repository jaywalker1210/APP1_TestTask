using Assets._Project.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Core
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Roguelike/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemData> allItems;

        private Dictionary<string, ItemData> itemDictionary;

        public void Initialize()
        {
            itemDictionary = new Dictionary<string, ItemData>();
            foreach (var item in allItems)
            {
                if (!string.IsNullOrEmpty(item.id))
                {
                    itemDictionary[item.id] = item;
                }
                else
                {
                    Debug.LogError($"Item {item.itemName} has no ID!");
                }
            }
        }

        public ItemData GetItemById(string id)
        {
            if (itemDictionary == null) Initialize();

            if (itemDictionary.TryGetValue(id, out ItemData item))
                return item;

            Debug.LogError($"Item with ID {id} not found!");
            return null;
        }
    }
}
