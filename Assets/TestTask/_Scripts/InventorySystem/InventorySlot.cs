using UnityEngine;
using UnityEngine.UI;

namespace TestTask._Scripts.InventorySystem
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Text _itemsCount;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _locker;
        
        public void UpdateCount(ushort count)
        {
            _itemsCount.text = count <= 1 ? "" : count.ToString();
        }

        public void UpdateIcon(Sprite newIcon)
        {
            _icon.enabled = newIcon != null;
            _icon.sprite = newIcon;
        }

        public void Unlock()
        {
            _locker.enabled = false;
        }
    }
}