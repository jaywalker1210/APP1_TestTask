using System.Collections.Generic;
using Assets._Project.Scripts.Data;
using Assets._Project.Scripts.Inventory;

namespace Assets._Project.Scripts.Services
{
    public class CombatService
    {
        public bool TryShoot(InventoryStorage storage, out int damage, out string log)
        {
            damage = 0;
            log = "";

            List<int> weaponSlots = new List<int>();
            for (int i = 0; i < storage.Slots.Count; i++)
            {
                if (!storage.IsUnlocked(i)) continue;
                var item = storage.GetItem(i);
                if (item != null && item.itemData is WeaponData)
                    weaponSlots.Add(i);
            }

            if (weaponSlots.Count == 0)
            {
                log = "Нет оружия";
                return false;
            }

            int randomIndex = UnityEngine.Random.Range(0, weaponSlots.Count);
            int weaponSlotIndex = weaponSlots[randomIndex];

            var weaponItem = storage.GetItem(weaponSlotIndex);
            var weapon = weaponItem.itemData as WeaponData;

            int ammoSlotIndex = FindCompatibleAmmoSlot(storage, weapon);
            if (ammoSlotIndex == -1)
            {
                log = $"Нет патронов для {weapon.itemName}";
                return false;
            }

            var ammoItem = storage.GetItem(ammoSlotIndex);
            var ammoData = ammoItem.itemData as AmmoData;

            ammoItem.amount--;
            if (ammoItem.amount <= 0)
            {
                storage.SetItem(ammoSlotIndex, null);
            }

            damage = weapon.damage;
            log = $"Выстрел из {weapon.itemName}, патроны: {ammoData.itemName}, урон: {damage}";
            return true;
        }

        private int FindCompatibleAmmoSlot(InventoryStorage storage, WeaponData weapon)
        {
            for (int i = 0; i < storage.Slots.Count; i++)
            {
                if (!storage.IsUnlocked(i)) continue;
                var item = storage.GetItem(i);
                if (item != null && item.itemData is AmmoData ammo)
                {
                    foreach (var compatible in weapon.compatibleAmmoIds)
                    {
                        if (compatible == ammo) return i;
                    }
                }
            }
            return -1;
        }
    }
}