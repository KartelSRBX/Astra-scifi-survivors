using System.Collections.Generic;
using GamingApp.UI;

namespace GamingApp.Meta
{
    /// <summary>Buying, upgrading and equipping shop modules, and summing their bonuses.</summary>
    public static class ModuleShop
    {
        public enum State { Locked, Buy, Upgrade, Maxed }

        public struct Bonus
        {
            public float damage, fireRate, projectileSpeed, range, health, armor, speed, pickup;

            public void Add(ModuleDef m, int tier)
            {
                damage += m.Value(ModStat.Damage, tier);
                fireRate += m.Value(ModStat.FireRate, tier);
                projectileSpeed += m.Value(ModStat.ProjectileSpeed, tier);
                range += m.Value(ModStat.Range, tier);
                health += m.Value(ModStat.Health, tier);
                armor += m.Value(ModStat.Armor, tier);
                speed += m.Value(ModStat.Speed, tier);
                pickup += m.Value(ModStat.Pickup, tier);
            }
        }

        /// <summary>Each module in a list needs the previous one owned (progressive unlock).</summary>
        public static bool IsUnlocked(ModuleDef m)
        {
            if (m.order == 0) return true;
            var list = m.kind == ModuleKind.Weapon ? ModuleCatalog.ForWeapon(m.weaponId) : ModuleCatalog.OfKind(m.kind);
            var prev = list.Find(x => x.order == m.order - 1);
            return prev == null || Profile.ModuleTier(prev.id) > 0;
        }

        public static State GetState(ModuleDef m)
        {
            int tier = Profile.ModuleTier(m.id);
            if (tier >= ModuleDef.Tiers) return State.Maxed;
            if (tier > 0) return State.Upgrade;
            return IsUnlocked(m) ? State.Buy : State.Locked;
        }

        /// <summary>Price of the next version (index = current tier).</summary>
        public static (int biomass, int cores) NextPrice(ModuleDef m)
        {
            int t = Profile.ModuleTier(m.id);
            return t >= ModuleDef.Tiers ? (0, 0) : (m.priceBiomass[t], m.priceCores[t]);
        }

        public static bool CanAfford(ModuleDef m)
        {
            var (b, c) = NextPrice(m);
            return Wallet.Biomass >= b && Wallet.Cores >= c;
        }

        public static bool TryBuyNext(ModuleDef m)
        {
            var state = GetState(m);
            if (state == State.Locked || state == State.Maxed || !CanAfford(m)) return false;
            var (b, c) = NextPrice(m);
            Wallet.Biomass -= b;
            Wallet.Cores -= c;
            int tier = Profile.ModuleTier(m.id) + 1;
            Profile.SetModuleTier(m.id, tier);
            // First piece of gear of a kind is equipped automatically.
            if (tier == 1 && m.kind != ModuleKind.Weapon && string.IsNullOrEmpty(Profile.EquippedGear(m.kind)))
                Profile.EquipGear(m.kind, m.id);
            return true;
        }

        /// <summary>Equipped armor, engine and sensor.</summary>
        public static Bonus GearBonus()
        {
            var b = new Bonus();
            foreach (var kind in new[] { ModuleKind.Armor, ModuleKind.Engine, ModuleKind.Sensor })
            {
                var m = ModuleCatalog.Get(Profile.EquippedGear(kind));
                if (m != null) b.Add(m, Profile.ModuleTier(m.id));
            }
            return b;
        }

        /// <summary>All owned modules of one weapon (they are always active).</summary>
        public static Bonus WeaponBonus(string weaponId)
        {
            var b = new Bonus();
            foreach (var m in ModuleCatalog.ForWeapon(weaponId)) b.Add(m, Profile.ModuleTier(m.id));
            return b;
        }

        public static int OwnedCount(IEnumerable<ModuleDef> list)
        {
            int n = 0;
            foreach (var m in list) if (Profile.ModuleTier(m.id) > 0) n++;
            return n;
        }

        /// <summary>Armor points to damage reduction, with diminishing returns (100 armor = 50%).</summary>
        public static float ArmorReduction(float armor) => armor <= 0f ? 0f : armor / (armor + 100f);
    }
}
