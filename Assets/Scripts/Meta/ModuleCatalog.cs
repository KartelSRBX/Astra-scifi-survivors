using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GamingApp.Meta
{
    public enum ModuleKind { Weapon, Armor, Engine, Sensor }
    public enum ModStat { Damage, FireRate, ProjectileSpeed, Range, Health, Armor, Speed, Pickup }

    /// <summary>A shop module line with 5 versions (Mk I..V). Values and prices are per version.</summary>
    public class ModuleDef
    {
        public const int Tiers = 5;

        public string id;
        public string name;
        public ModuleKind kind;
        public string weaponId;     // weapon modules only
        public int order;           // position in its list; each needs the previous one owned
        public (ModStat stat, float[] values)[] stats;
        public int[] priceBiomass = new int[Tiers];
        public int[] priceCores = new int[Tiers];

        public float Value(ModStat stat, int tier)
        {
            if (tier <= 0) return 0f;
            foreach (var s in stats) if (s.stat == stat) return s.values[tier - 1];
            return 0f;
        }

        public string Describe(int tier)
        {
            var sb = new StringBuilder();
            foreach (var s in stats)
            {
                if (sb.Length > 0) sb.Append("   ");
                sb.Append(Format(s.stat, s.values[Mathf.Clamp(tier, 1, Tiers) - 1]));
            }
            return sb.ToString();
        }

        public static string Format(ModStat stat, float v) => stat switch
        {
            ModStat.Armor => $"Armor {Sign(v)}{v:0}",
            ModStat.Pickup => $"Pickup {Sign(v)}{v:0.0}",
            _ => $"{Label(stat)} {Sign(v)}{v * 100f:0}%",
        };

        static string Sign(float v) => v >= 0f ? "+" : "";

        public static string Label(ModStat s) => s switch
        {
            ModStat.Damage => "Damage",
            ModStat.FireRate => "Fire rate",
            ModStat.ProjectileSpeed => "Shot speed",
            ModStat.Range => "Range",
            ModStat.Health => "Hull",
            ModStat.Armor => "Armor",
            ModStat.Speed => "Speed",
            _ => "Pickup",
        };
    }

    /// <summary>
    /// Every purchasable module, generated from compact tables: 10 lines per weapon (damage / fire rate / shot speed)
    /// plus 5 armor, 5 engine and 5 sensor lines. Each line has 5 versions.
    /// </summary>
    public static class ModuleCatalog
    {
        public static readonly List<ModuleDef> All = new();
        static readonly Dictionary<string, ModuleDef> byId = new();
        static int gearOrder;
        static ModuleKind lastGearKind = (ModuleKind)(-1);

        public static readonly (string id, string name)[] Weapons =
        {
            ("blaster", "Blaster"), ("laser", "Laser"), ("ciws", "CIWS"), ("heavy_missiles", "Missile"),
            ("swarm_missiles", "Swarm"), ("plasma", "Plasma"), ("drone_swarm", "Drone"), ("heavy_drones", "Gunship"),
        };

        static readonly ModStat[] WeaponPattern =
        {
            ModStat.Damage, ModStat.FireRate, ModStat.ProjectileSpeed, ModStat.Damage, ModStat.FireRate,
            ModStat.Damage, ModStat.ProjectileSpeed, ModStat.FireRate, ModStat.Damage, ModStat.ProjectileSpeed,
        };

        static readonly string[] DamageNames = { "Capacitor Bank", "Power Coupler", "Overcharge Core", "Hyper Condenser" };
        static readonly string[] RateNames = { "Cycler", "Cooling Array", "Rapid Feed" };
        static readonly string[] SpeedNames = { "Accelerator", "Mag Rail", "Velocity Coil" };
        static readonly string[] RangeNames = { "Beam Collimator", "Long Lens", "Optic Array" };

        static ModuleCatalog()
        {
            foreach (var (wid, wname) in Weapons)
            {
                int d = 0, r = 0, s = 0;
                for (int i = 0; i < WeaponPattern.Length; i++)
                {
                    var stat = WeaponPattern[i];
                    // Beams have no projectile: their "speed" modules extend range instead.
                    if (wid == "laser" && stat == ModStat.ProjectileSpeed) stat = ModStat.Range;
                    float scale = 1f + i * 0.12f;
                    string title = stat switch
                    {
                        ModStat.Damage => DamageNames[d++ % DamageNames.Length],
                        ModStat.FireRate => RateNames[r++ % RateNames.Length],
                        ModStat.Range => RangeNames[s++ % RangeNames.Length],
                        _ => SpeedNames[s++ % SpeedNames.Length],
                    };
                    var (min, max) = stat switch
                    {
                        ModStat.Damage => (0.04f, 0.18f),
                        ModStat.FireRate => (0.03f, 0.14f),
                        ModStat.Range => (0.03f, 0.12f),
                        _ => (0.05f, 0.24f),
                    };
                    Add(new ModuleDef
                    {
                        id = $"{wid}_m{i + 1:00}", name = $"{wname} {title}", kind = ModuleKind.Weapon, weaponId = wid, order = i,
                        stats = new[] { S(stat, min * scale, max * scale) },
                    }, 60f, 0.35f);
                }
            }

            Gear(ModuleKind.Armor, "armor_composite", "Composite Plating", S(ModStat.Health, 0.08f, 0.35f));
            Gear(ModuleKind.Armor, "armor_reactive", "Reactive Armor", S(ModStat.Armor, 8f, 38f));
            Gear(ModuleKind.Armor, "armor_ablative", "Ablative Shell", S(ModStat.Health, 0.1f, 0.3f), S(ModStat.Armor, 6f, 26f));
            Gear(ModuleKind.Armor, "armor_titan", "Titan Bulwark", S(ModStat.Health, 0.18f, 0.5f), S(ModStat.Armor, 12f, 45f), S(ModStat.Speed, -0.06f, -0.04f));
            Gear(ModuleKind.Armor, "armor_nano", "Nano Weave", S(ModStat.Health, 0.2f, 0.6f), S(ModStat.Armor, 12f, 50f));

            Gear(ModuleKind.Engine, "engine_ion", "Ion Drive", S(ModStat.Speed, 0.04f, 0.16f));
            Gear(ModuleKind.Engine, "engine_fusion", "Fusion Thruster", S(ModStat.Speed, 0.06f, 0.18f), S(ModStat.Health, 0.03f, 0.12f));
            Gear(ModuleKind.Engine, "engine_plasma", "Plasma Jet", S(ModStat.Speed, 0.08f, 0.2f), S(ModStat.FireRate, 0.03f, 0.1f));
            Gear(ModuleKind.Engine, "engine_graviton", "Graviton Drive", S(ModStat.Speed, 0.1f, 0.24f), S(ModStat.Pickup, 0.5f, 2.5f));
            Gear(ModuleKind.Engine, "engine_warp", "Warp Coil", S(ModStat.Speed, 0.14f, 0.32f));

            Gear(ModuleKind.Sensor, "sensor_radar", "Radar Array", S(ModStat.Range, 0.05f, 0.2f));
            Gear(ModuleKind.Sensor, "sensor_lidar", "Lidar Suite", S(ModStat.Range, 0.06f, 0.22f), S(ModStat.Damage, 0.03f, 0.1f));
            Gear(ModuleKind.Sensor, "sensor_quantum", "Quantum Scanner", S(ModStat.Range, 0.08f, 0.25f), S(ModStat.Pickup, 0.5f, 2f));
            Gear(ModuleKind.Sensor, "sensor_tachyon", "Tachyon Array", S(ModStat.Range, 0.1f, 0.28f), S(ModStat.ProjectileSpeed, 0.08f, 0.25f));
            Gear(ModuleKind.Sensor, "sensor_deep", "Deep Scan", S(ModStat.Range, 0.14f, 0.38f));
        }

        static void Gear(ModuleKind kind, string id, string name, params (ModStat, float[])[] stats)
        {
            if (kind != lastGearKind) { gearOrder = 0; lastGearKind = kind; }
            Add(new ModuleDef { id = id, name = name, kind = kind, order = gearOrder++, stats = stats }, 120f, 0.6f);
        }

        /// <summary>Linear 5-step progression from Mk I to Mk V.</summary>
        static (ModStat, float[]) S(ModStat stat, float min, float max)
        {
            var v = new float[ModuleDef.Tiers];
            for (int t = 0; t < v.Length; t++) v[t] = Mathf.Lerp(min, max, t / (float)(v.Length - 1));
            return (stat, v);
        }

        static void Add(ModuleDef m, float basePrice, float orderGrowth)
        {
            for (int t = 0; t < ModuleDef.Tiers; t++)
            {
                m.priceBiomass[t] = Mathf.RoundToInt(basePrice * Mathf.Pow(t + 1, 1.7f) * (1f + m.order * orderGrowth) / 5f) * 5;
                m.priceCores[t] = t < 3 ? 0 : t == 3 ? 1 + m.order / 3 : 3 + m.order / 2;
            }
            All.Add(m);
            byId[m.id] = m;
        }

        public static ModuleDef Get(string id) => byId.TryGetValue(id, out var m) ? m : null;

        public static List<ModuleDef> ForWeapon(string weaponId) => All.FindAll(m => m.kind == ModuleKind.Weapon && m.weaponId == weaponId);
        public static List<ModuleDef> OfKind(ModuleKind kind) => All.FindAll(m => m.kind == kind);
    }
}
