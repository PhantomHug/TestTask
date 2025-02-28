using UnityEngine;

namespace TestTask._Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Create new weapon", order = 0)]
    public class WeaponItem : BaseItem
    {
        [Tooltip("Dont set to use random ammo")]
        [SerializeField] private AmmoItem _ammo;
        [SerializeField] private float _damage;

        public AmmoItem Ammo => _ammo;

        public float Damage => _damage;
    }
}