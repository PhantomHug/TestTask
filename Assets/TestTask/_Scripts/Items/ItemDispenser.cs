using System;
using System.Collections.Generic;
using System.Linq;
using TestTask._Scripts.InventorySystem;
using UnityEngine;
using Random = System.Random;

namespace TestTask._Scripts.Items
{
    //TODO: add load items using resources
    public class ItemDispenser : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private string _storePath;

        private List<AmmoItem> _ammoItems;
        private List<ArmorItem> _armorItems;
        private List<WeaponItem> _weaponItems;

        private void OnEnable()
        {
            LoadAllItemTypes();
        }

        private void LoadAllItemTypes()
        {
            _ammoItems = new List<AmmoItem>();
            _armorItems = new List<ArmorItem>();
            _weaponItems = new List<WeaponItem>();
            var items = Resources.LoadAll<BaseItem>(_storePath);
            foreach (var item in items)
            {
                switch (item)
                {
                    case AmmoItem ammoItem:
                        _ammoItems.Add(ammoItem);
                        break;
                    case ArmorItem armorItem:
                        _armorItems.Add(armorItem);
                        break;
                    case WeaponItem weaponItem:
                        _weaponItems.Add(weaponItem);
                        break;
                    default:
                        Debug.LogError($"Item {item.name} has not been implemented. Add list in ItemDispenser");
                        break;
                }
            }
        }
        
        public void AddRandomItem()
        {
            var randomHeads = _armorItems.Where(armor => armor.ArmorPart == ArmorPart.HEAD).ToList();
            var randomBodies = _armorItems.Where(armor => armor.ArmorPart == ArmorPart.BODY).ToList();
            
            var randomWeapon = _weaponItems[new Random().Next(_weaponItems.Count)];
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