using UnityEngine;

namespace TestTask._Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Create new armor", order = 0)]
    public class ArmorItem : BaseItem
    {
        [SerializeField] private ArmorPart _armorPart;
        [SerializeField] private float _protection;
        
        public ArmorPart ArmorPart => _armorPart;
    }

    public enum ArmorPart
    {
        HEAD = 0,
        BODY = 1,
    }
}