using UnityEngine;

namespace TestTask._Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Create new weapon", order = 0)]
    public class WeaponItem : BaseItem
    {
        [SerializeField] private AmmoItem _ammo;
        [SerializeField] private float _damage;

        public AmmoItem Ammo => _ammo;
    }
}