using UnityEngine;

namespace GamingApp.Weapons
{
    public enum UpgradeStat
    {
        WeaponDamage,
        WeaponFireRate,
        WeaponRange,
        ExtraProjectile,
        MoveSpeed,
        MaxHealth,
        PickupRadius
    }

    /// <summary>
    /// One entry in the level-up pool. Weapons = active, Gear = passive stat bonus (see roadmap).
    /// Create via Assets > Create > GamingApp > Upgrade.
    /// </summary>
    [CreateAssetMenu(menuName = "GamingApp/Upgrade", fileName = "Upgrade_")]
    public class UpgradeData : ScriptableObject
    {
        public string displayName = "Damage +20%";
        [TextArea] public string description;
        public bool isWeaponUpgrade = true;
        public UpgradeStat stat = UpgradeStat.WeaponDamage;
        [Tooltip("Multiplicative for damage/fire rate/range/speed/HP (0.2 = +20%). Additive for projectiles/pickup radius.")]
        public float amount = 0.2f;
        public int maxStacks = 5;
    }
}
