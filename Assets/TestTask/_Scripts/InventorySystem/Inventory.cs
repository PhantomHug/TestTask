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
        [Header("")]
        [Space(5)]
        [SerializeField] private int _maxInventory;
        [SerializeField] private int _availableInventory;
        [SerializeField] private Slot[] _slots;
        
        [Header("Temporary data for unlock slots")]
        [Space(5)]
        [SerializeField] private float _money = 99999;
        
        [Header("Data persistence settings")]
        [Space(5)]
        [Tooltip("Set true if you want load saved data")]
        [SerializeField] private bool _useLoad;
        
        private void AfterLoad()
        {
            var visualSlots = FindObjectsOfType<InventorySlot>();
            for (int i = _slots.Length - 1; i >= 0; i--)
            {
                _slots[i].InitVisual(visualSlots[_slots.Length - i - 1]);
                _slots[i].UpdateVisual();
            }

            for (int i = 0; i < _availableInventory; i++)
            {
                _slots[i].TryUnlock(ref _money);
            }
        }

        public void UnlockLastSlot()
        {
            if (_availableInventory <= _maxInventory - 1)
            {
                if (_slots[_availableInventory].TryUnlock(ref _money))
                {
                    _availableInventory++;
                }
            }
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
            var allSameSlots = _slots.Where(slot => slot.GetInventoryItem().Item == item.Item).ToList();
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
        
        public void RemoveRandomItem()
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
            var weaponSlots = GetSlotsByType(ItemType.WEAPON);
            if (weaponSlots.Count == 0)
            {
                Debug.Log("You cant shoot because you have no weapon.");
                return;
            }

            var ammoSlots = GetSlotsByType(ItemType.AMMO);
            if (ammoSlots.Count == 0)
            {
                Debug.Log("You have no ammo");
                return;
            }
            
            var weaponWithAmmo = new List<Slot>();
            foreach (var ammo in ammoSlots)
            {
                var wep = weaponSlots.FirstOrDefault(weapon => ((WeaponItem)weapon.GetInventoryItem().Item).Ammo == ammo.GetInventoryItem().Item);
                if(wep != null)
                    weaponWithAmmo.Add(wep);
            }

            if (weaponWithAmmo.Count == 0)
            {
                Debug.Log("You dont have ammo for your weapon");
                return;
            }

            int randomIndex = new Random().Next(weaponWithAmmo.Count);
            ammoSlots.FirstOrDefault(ammo =>
                ((WeaponItem)weaponWithAmmo[randomIndex].GetInventoryItem().Item).Ammo == ammo.GetInventoryItem().Item).RemoveItem(1);
        }
        
        private List<Slot> GetSlotsByType(ItemType itemType) 
        {
            return _slots.Where(slot => !slot.IsEmpty && slot.ItemType == itemType).Distinct().ToList();
        }

        public void Load(GameSaveData persistence)
        {
            if (!_useLoad) return;
            
            _maxInventory = persistence.maxInventory;
            _availableInventory = persistence.availableInventory;
            _slots = new Slot[_maxInventory];
            for (int i = 0; i < _slots.Length; i++)
            {
                if (persistence.slots[i] == null)
                {
                    _slots[i] = new Slot();
                }
                else
                {
                    _slots[i] = persistence.slots[i];
                }
            }

            AfterLoad();
        }
        
        public void Save(ref GameSaveData persistence)
        {
            persistence.slots = _slots;
            persistence.maxInventory = _maxInventory;
            persistence.availableInventory = _availableInventory;
        }
    }

    [Serializable]
    public class Slot
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private float _cost;
        private bool _isLocked = true;
        private InventorySlot _visualSlot;
        
        public bool IsLocked => _isLocked;
        public bool IsEmpty => _item.Item == null;
        public ItemType ItemType => _item.Item.ItemType;
        
        public bool TryUnlock(ref float money)
        {
            if (!_isLocked || money <= _cost) return false;
            money -= _cost;
            _isLocked = false;
            _cost = 0;
            _visualSlot.Unlock();
            return true;
        }

        public void InitVisual(InventorySlot visualSlot)
        {
            _visualSlot = visualSlot;
        }
        
        public void AddNewItem(InventoryItem item)
        {
            _item = item;
            UpdateVisual();
        }
        public ushort TryAddItem(InventoryItem item)
        {
            var result = _item.AddItem(item.Count);
            _visualSlot.UpdateCount(_item.Count);
            return result;
        }
        public bool TryRemoveItem()
        {
            if(IsEmpty)
                return false;
            _item.RemoveItem();
            _visualSlot.UpdateCount(0);
            _visualSlot.UpdateIcon(null);
            return true;
        }
        public void RemoveItem(ushort count)
        {
            _item.RemoveCount(count);
            if (_item.Count == 0)
                TryRemoveItem();
            UpdateVisual();
        }
        public InventoryItem GetInventoryItem() => _item;

        public void UpdateVisual()
        {
            if (_item != null)
            {
                _visualSlot.UpdateIcon(IsEmpty ? null : _item.Item.Icon);
                _visualSlot.UpdateCount(_item.Count);
            }
            else
            {
                _visualSlot.UpdateIcon(null);
                _visualSlot.UpdateCount(0);
            }
        }
    }
}