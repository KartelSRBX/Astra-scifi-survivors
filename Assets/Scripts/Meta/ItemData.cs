using System.Text;
using UnityEngine;

namespace GamingApp.Meta
{
    public enum ItemSlot { Weapon, Hull, Engine, Module }
    public enum Rarity { Common, Rare, Epic, Legendary }

    /// <summary>Equippable ship part. Weapons define the base gun; the rest add percentage bonuses.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Item", fileName = "Item_")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName;
        public ItemSlot slot;
        public Rarity rarity;
        public Sprite icon;

        [Header("Weapon (slot = Weapon)")]
        public float baseDamage = 5f;
        public float fireInterval = 0.8f;
        public float range = 10f;
        public int projectiles = 1;

        [Header("Bonuses (0.1 = +10%)")]
        public float healthPct;
        public float speedPct;
        public float damagePct;
        public float fireRatePct;
        public float rangePct;
        public float pickupBonus;

        public string Describe()
        {
            var sb = new StringBuilder();
            if (slot == ItemSlot.Weapon)
            {
                sb.AppendLine($"Damage {baseDamage:0.#} x{projectiles}");
                sb.AppendLine($"Rate {1f / fireInterval:0.0}/s   Range {range:0}");
            }
            Pct(sb, "Hull", healthPct);
            Pct(sb, "Speed", speedPct);
            Pct(sb, "Damage", damagePct);
            Pct(sb, "Fire rate", fireRatePct);
            Pct(sb, "Range", rangePct);
            if (pickupBonus != 0f) sb.AppendLine($"Pickup radius +{pickupBonus:0.#}");
            return sb.ToString().TrimEnd();
        }

        static void Pct(StringBuilder sb, string label, float v)
        {
            if (v != 0f) sb.AppendLine($"{label} {(v > 0 ? "+" : "")}{v * 100f:0}%");
        }
    }
}
