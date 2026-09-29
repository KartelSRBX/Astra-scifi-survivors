using UnityEngine;

namespace GamingApp.Meta
{
    /// <summary>A playable hull. The player starts with the corvette and buys the rest in the hangar.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Ship", fileName = "Ship_")]
    public class ShipData : ScriptableObject
    {
        public string id = "corvette";
        public string displayName = "Corvette";
        public string shipClass = "Light escort";
        public Sprite sprite;
        public float visualSize = 2f;

        [Header("Base stats (level 1)")]
        public float maxHealth = 100f;
        public float moveSpeed = 5.5f;
        public float damageMultiplier = 1f;
        public float fireRateMultiplier = 1f;

        [Header("Price")]
        public int priceBiomass;
        public int priceCores;

        public const int MaxLevel = 10;
        /// <summary>Each level adds +10% hull and firepower.</summary>
        public static float LevelMultiplier(int level) => 1f + 0.1f * (level - 1);
        public int UpgradeCost(int level, int shipIndex) => Mathf.RoundToInt(40f * level * (1f + shipIndex * 0.75f));
    }
}
