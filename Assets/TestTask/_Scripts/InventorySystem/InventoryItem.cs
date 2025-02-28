using System;
using TestTask._Scripts.Items;
using UnityEngine;

namespace TestTask._Scripts.InventorySystem
{
    //TODO: добавить ограничение на _count. Он не может быть больше чем _item.MaxCount;
    [Serializable]
    public class InventoryItem
    {
        [SerializeField] private BaseItem _item;
        [SerializeField] private ushort _count;
        
        public BaseItem Item => _item;
        public ushort Count => _count;
        public ushort AddItem(ushort value)
        {
            if (value + _count > _item.MaxCount)
            {
                int temp = value + _count;
                _count = _item.MaxCount;
                temp -= _item.MaxCount;
                
                return (ushort)Mathf.Abs(temp);
            }
            _count += value;
            return 0;
        }
        
        public void RemoveCount(ushort value)
        {
            _count = (ushort)Mathf.Max(0, _count - value);
        }
        
        public InventoryItem(BaseItem item, ushort count)
        {
            _item = item;
            _count = count;
        }

        public void RemoveItem()
        {
            _item = null;
            _count = 0;
        }
    }
}