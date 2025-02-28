using System;
using System.Collections.Generic;
using System.Linq;
using TestTask._Scripts.Items;
using UnityEngine;
using Random = System.Random;

namespace TestTask._Scripts.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int _maxInventory;
        [SerializeField] private int _availableInventory;
        [SerializeField] private Slot[] _slots;
        
        private void OnEnable()
        {
#if !UNITY_EDITOR
            _slots = new Slot[_maxInventory];
            for (int i = 0; i < _availableInventory; i++)
            {
                _slots[i] = new Slot();
                _slots[i].Unlock();
            }      
#endif
        }
        
        public void UnlockLastSlot()
        {
            if(_availableInventory <= _maxInventory -1)
                _slots[_availableInventory++].Unlock();
            else
                Debug.Log("Available inventory size is equal to max inventory size");
        }

        public void TryAddItem(InventoryItem item)
        {
            var allSameSlots = _slots.Where(slot => slot.GetItem().Item == item.Item).ToList();
            ushort resultValue = 0;
            
            foreach (var slot in allSameSlots)
            {
                resultValue = slot.TryAddItem(item);
                if (resultValue == 0)
                    return;
            }
            
            var emptySlot = _slots.FirstOrDefault(slot => slot.IsEmpty && !slot.IsLocked);
            if (emptySlot != null)
            {
                emptySlot.AddNewItem(item);
                return;
            }

            Debug.Log(resultValue == 0
                ? $"There is no space for {item.Item.Name}"
                : $"Cant add more this {item.Item.Name} current remainder is {resultValue}");
        }

        public void TryRemoveRandomItem()
        {
            var notEmptySlots = _slots.Where(slot => !slot.IsEmpty).ToList();
            if (notEmptySlots.Count > 0)
            {
                notEmptySlots[new Random().Next(notEmptySlots.Count)].TryRemoveItem();
            }
            else
            {
                Debug.Log("Cant remove item. Inventory is empty");
            }
        }

        public void TryShoot()
        {
            var weaponList = TryGetAllWeapon();
            if (weaponList.Count == 0)
            {
                Debug.Log("Pew, pew. You shot from your finger. Because you have no any weapon");
                return;
            }

            var ammoList = TryGetAllAmmo();
            if (ammoList.Count == 0)
            {
                Debug.Log("CLICK, CLICK. You have no ammo");
                return;
            }
            
            var weaponWithAmmo = new List<InventoryItem>();
            foreach (var ammo in ammoList)
            {
                var wep = weaponList.FirstOrDefault(weapon => ((WeaponItem)weapon.Item).Ammo == ammo.Item);
                if(wep != null)
                    weaponWithAmmo.Add(wep);
            }

            int randomIndex = new Random().Next(weaponWithAmmo.Count);
            ammoList.FirstOrDefault(ammo =>
                    ((WeaponItem)weaponWithAmmo[randomIndex].Item).Ammo == ammo.Item).RemoveCount(1);
        }
        
        public void Shoot()
        {
            TryShoot();
        }

        private List<InventoryItem> TryGetAllWeapon()
        {
            var slots = _slots.Where(slot => slot.ItemType == ItemType.WEAPON).ToList();

            return slots.Select(slot => slot.GetItem()).ToList();
        }
        
        private List<InventoryItem> TryGetAllAmmo()
        {
            var slots = _slots.Where(slot => slot.ItemType == ItemType.AMMO).ToList();

            return slots.Select(slot => slot.GetItem()).ToList();
        }
        /*
         * Load availableInventory, and slots with info about they
         */
    }

    [Serializable]
    public class Slot
    {
        [SerializeField] private bool _isLocked = true;
        [SerializeField] private InventoryItem _item;
        public bool IsLocked => _isLocked;
        public bool IsEmpty => _item.Item == null;
        public ItemType ItemType => _item.Item.ItemType;
        public void Unlock()
        {
            if (_isLocked)
                _isLocked = false;
        }

        public void AddNewItem(InventoryItem item)
        {
            _item = item;
        }
        
        public ushort TryAddItem(InventoryItem item)
        {
            return _item.AddItem(item.Count);
        }

        public bool TryRemoveItem()
        {
            if(IsEmpty)
                return false;
            _item.RemoveItem();
            return true;
        }
        
        //Добавить удаление предмета если его количество равно 0
        
        public InventoryItem GetItem() => _item;
    }
}