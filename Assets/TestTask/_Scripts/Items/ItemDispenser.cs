using System.Linq;
using TestTask._Scripts.InventorySystem;
using UnityEngine;
using Random = System.Random;

namespace TestTask._Scripts.Items
{
    public class ItemDispenser : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        
        [SerializeField] private AmmoItem[] _ammoItems;
        [SerializeField] private ArmorItem[] _armorItems;
        [SerializeField] private WeaponItem[] _weaponItems;
        
        public void AddRandomItem()
        {
            var randomHeads = _armorItems.Where(armor => armor.ArmorPart == ArmorPart.HEAD).ToList();
            var randomBodies = _armorItems.Where(armor => armor.ArmorPart == ArmorPart.BODY).ToList();
            
            var randomWeapon = _weaponItems[new Random().Next(_weaponItems.Length)];
            var randomHead = randomHeads[new Random().Next(0, randomHeads.Count)];
            var randomBody = randomBodies[new Random().Next(0, randomBodies.Count)];
            
            _inventory.TryAddItem(new InventoryItem(randomWeapon, randomWeapon.MaxCount));
            _inventory.TryAddItem(new InventoryItem(randomHead, randomHead.MaxCount));
            _inventory.TryAddItem(new InventoryItem(randomBody, randomBody.MaxCount));
        }

        public void AddAllTypesAmmo()
        {
            foreach (var ammo in _ammoItems)
            {
                var inventoryItem = new InventoryItem(ammo, ammo.MaxCount);
                _inventory.TryAddItem(inventoryItem);
            }
        }
    }
}