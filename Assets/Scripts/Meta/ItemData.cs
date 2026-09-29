using System.Text;
using UnityEngine;

namespace GamingApp.Meta
{
    public enum ItemSlot { Weapon, Hull, Engine, Module }
    public enum Rarity { Common, Rare, Epic, Legendary }

    /// <summary>Equippable ship part. Every slot adds bonuses; the gun itself is chosen before the run.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Item", fileName = "Item_")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName;
        public ItemSlot slot;
        public Rarity rarity;
        public Sprite icon;

        [Header("Bonuses (0.1 = +10%)")]
        public float healthPct;
        public float speedPct;
        public float damagePct;
        public float fireRatePct;
        public float rangePct;
        public float pickupBonus;
        public int extraProjectiles;

        public string Describe()
        {
            var sb = new StringBuilder();
            Pct(sb, "Hull", healthPct);
            Pct(sb, "Speed", speedPct);
            Pct(sb, "Damage", damagePct);
            Pct(sb, "Fire rate", fireRatePct);
            Pct(sb, "Range", rangePct);
            if (extraProjectiles != 0) sb.AppendLine($"+{extraProjectiles} projectile");
            if (pickupBonus != 0f) sb.AppendLine($"Pickup radius +{pickupBonus:0.#}");
            return sb.ToString().TrimEnd();
        }

        static void Pct(StringBuilder sb, string label, float v)
        {
            if (v != 0f) sb.AppendLine($"{label} {(v > 0 ? "+" : "")}{v * 100f:0}%");
        }
    }
}
