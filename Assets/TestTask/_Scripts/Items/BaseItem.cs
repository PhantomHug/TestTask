using UnityEngine;

namespace TestTask._Scripts.Items
{
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private ItemType _itemType;
        
        [Header("Game settings")]
        [Space(5)]
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;
        [SerializeField] private ushort _maxCount;
        [SerializeField] private float _weightOneItem;

        public Sprite Icon => _icon;
        public string Name => _name;
        public ushort MaxCount => _maxCount;
        public float WeightOneItem => _weightOneItem;
        public ItemType ItemType => _itemType;
    }

    public enum ItemType
    {
        AMMO = 1,
        WEAPON = 2,
        ARMOR = 3,
    }
}
