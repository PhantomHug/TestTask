using TestTask._Scripts.InventorySystem;
using TestTask._Scripts.Items;
using UnityEngine;

namespace TestTask._Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private ItemDispenser _dispenser;
        
        public void Shoot()
        {
            _inventory.Shoot();
        }

        public void AddAmmo()
        {
            _dispenser.AddAllTypesAmmo();
        }
        
        public void AddRandomItem()
        {
            _dispenser.AddRandomItem();
        }

        public void RemoveRandomItem()
        {
            _inventory.RemoveRandomItem();
        }

        public void UnlockInventorySlot()
        {
            _inventory.UnlockLastSlot();
        }
        
    }
}