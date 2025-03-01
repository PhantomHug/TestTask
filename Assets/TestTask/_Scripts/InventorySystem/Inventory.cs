using System;
using System.Collections.Generic;
using System.Linq;
using TestTask._Scripts.DataPersistenceSystem;
using TestTask._Scripts.Items;
using UnityEngine;
using Random = System.Random;

namespace TestTask._Scripts.InventorySystem
{
    public class Inventory : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private int _maxInventory;
        [SerializeField] private int _availableInventory;
        [SerializeField] private Slot[] _slots;
        
        private void OnEnable()
        {
            
/*#if UNITY_EDITOR
            _slots = new Slot[_maxInventory];
            for (int i = 0; i < _availableInventory; i++)
            {
                _slots[i] = new Slot();
                _slots[i].Unlock();
            }      
#endif*/
        }
        
        public void UnlockLastSlot()
        {
            if(_availableInventory <= _maxInventory - 1)
                _slots[_availableInventory++].Unlock();
            else
                Debug.Log("Available inventory size is equal to max inventory size");
        }

        public void TryAddItem(InventoryItem item)
        {
            var result = TryAddExistingItem(item);
            if (result == 0 || TryAddNewItem(item))
                return;
            
            Debug.Log(result == -1
                ? $"There is no space for {item.Item.Name}"
                : $"Cant add more this {item.Item.Name} current remainder is {result}");
        }

        /// <summary>
        /// Method for trying to add item that exists in inventory
        /// </summary>
        /// <param name="item">Item that can exists in inventory</param>
        /// <returns>Count of remaining items. Return -1 if the item is not in the inventory</returns>
        private int TryAddExistingItem(InventoryItem item)
        {
            short resultValue = -1;
            var allSameSlots = _slots.Where(slot => slot.GetItem().Item == item.Item).ToList();
            foreach (var slot in allSameSlots)
            {
                resultValue = (short)slot.TryAddItem(item);
                if (resultValue == 0)
                    return 0;
            }

            return resultValue;
        }

        private bool TryAddNewItem(InventoryItem item)
        {
            var emptySlot = _slots.FirstOrDefault(slot => slot.IsEmpty && !slot.IsLocked);
            if (emptySlot == null) return false;
            emptySlot.AddNewItem(item);
            return true;

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
        
        public void Shoot()
        {
            var weaponSlots = GetAllWeaponSlots();
            if (weaponSlots.Count == 0)
            {
                Debug.Log("Pew, pew. You shot from your finger. Because you have no any weapon");
                return;
            }

            var ammoSlots = GetAllAmmoSlots();
            if (ammoSlots.Count == 0)
            {
                Debug.Log("CLICK, CLICK. You have no ammo");
                return;
            }
            
            var weaponWithAmmo = new List<Slot>();
            foreach (var ammo in ammoSlots)
            {
                var wep = weaponSlots.FirstOrDefault(weapon => ((WeaponItem)weapon.GetItem().Item).Ammo == ammo.GetItem().Item);
                if(wep != null)
                    weaponWithAmmo.Add(wep);
            }
            
            int randomIndex = new Random().Next(weaponWithAmmo.Count);
            ammoSlots.FirstOrDefault(ammo =>
                ((WeaponItem)weaponWithAmmo[randomIndex].GetItem().Item).Ammo == ammo.GetItem().Item).RemoveItem(1);
        }
        
        private List<Slot> GetAllWeaponSlots()
        {
            return _slots.Where(slot => !slot.IsEmpty && slot.ItemType == ItemType.WEAPON).Distinct().ToList();
        }
        
        private List<Slot> GetAllAmmoSlots()
        {
            return _slots.Where(slot => !slot.IsEmpty && slot.ItemType == ItemType.AMMO).Distinct().ToList();
        }

        public void Load(GameSaveData persistence)
        {
            _maxInventory = persistence.maxInventory;
            _availableInventory = persistence.availableInventory;
            _slots = new Slot[_maxInventory];
            Array.Copy(persistence.slots, _slots, persistence.slots.Length);
            for (int i = persistence.slots.Length; i < _slots.Length; i++)
                _slots[i] = new Slot();
        }
        
        public void Save(ref GameSaveData persistence)
        {
            var notEmptySlots = _slots.Where(slot => !slot.IsEmpty).ToArray();
            persistence.slots = notEmptySlots;
            persistence.maxInventory = _maxInventory;
            persistence.availableInventory = _availableInventory;
        }
    }

    [Serializable]
    public class Slot
    {
        [SerializeField]
        private bool _isLocked = true;
        [SerializeField]
        private InventoryItem _item;
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
        public void RemoveItem(ushort count)
        {
            _item.RemoveCount(count);
            if (_item.Count == 0)
                TryRemoveItem();
        }
        //Добавить удаление предмета если его количество равно 0
        public InventoryItem GetItem() => _item;
    }
}