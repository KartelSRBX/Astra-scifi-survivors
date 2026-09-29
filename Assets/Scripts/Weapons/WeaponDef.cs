using System;
using UnityEngine;

namespace GamingApp.Weapons
{
    public enum WeaponMode { Projectile, Beam, Drones }

    [Serializable]
    public class WeaponStats
    {
        public float damage = 5f;
        public float fireInterval = 0.6f;
        public float range = 10f;
        [Tooltip("Projectiles per shot; beams: targets; drones: shots per drone.")]
        public int projectiles = 1;
        public float speed = 14f;
        [Tooltip("Homing turn rate in degrees per second (0 = straight).")]
        public float turnRate = 200f;
        public float aoe;
        public int pierce;
        public int drones;
        public float spread = 10f;

        public WeaponStats Clone() => (WeaponStats)MemberwiseClone();
    }

    /// <summary>One of the five in-run upgrades of a weapon, applied in order.</summary>
    [Serializable]
    public class WeaponStep
    {
        public string title;
        public string description;
        public float damageMult = 1f;
        public float fireRateMult = 1f;
        public float rangeMult = 1f;
        public float aoeMult = 1f;
        public float setAoe;
        public int addProjectiles;
        public int addPierce;
        public int addDrones;
    }

    /// <summary>A weapon the player picks before a run. Create via Assets > Create > GamingApp > Weapon.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Weapon", fileName = "Weapon_")]
    public class WeaponDef : ScriptableObject
    {
        public string id = "blaster";
        public string displayName = "Blaster";
        [TextArea] public string description;
        public Color color = Color.cyan;
        public Sprite icon;
        public WeaponMode mode;
        public WeaponStats stats = new();
        public WeaponStep[] steps = new WeaponStep[5];

        [Header("Prefabs")]
        public Projectile projectilePrefab;
        public Drone dronePrefab;
        public FxBurst explosionPrefab;
        public FxBurst hitPrefab;
        public Material beamMaterial;
    }
}
