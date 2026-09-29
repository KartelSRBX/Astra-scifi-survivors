using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>Builds the 8 pre-run weapons, their projectiles/drones/effects and 5 in-run steps each.</summary>
    static class WeaponBuilder
    {
        const string Dir = BuilderUtil.DataDir + "/Weapons";

        public static List<WeaponDef> BuildAll(Projectile blasterBolt, FxBurst explosion, FxBurst spark)
        {
            BuilderUtil.EnsureDir(Dir);
            var beamMat = BeamMaterial();

            var ciws = Shot("Proj_Ciws", "bullet_ciws", 0.2f, 0.22f);
            var heavyMissile = Shot("Proj_HeavyMissile", "missile_heavy", 0.35f, 1.4f);
            var swarmMissile = Shot("Proj_SwarmMissile", "missile_swarm", 0.3f, 0.9f);
            var plasma = Shot("Proj_Plasma", "plasma_orb", 0.6f, 1.3f);
            var droneBolt = Shot("Proj_DroneBolt", "bolt_player", 0.2f, 0.3f);
            var heavyBolt = Shot("Proj_HeavyBolt", "bolt_heavy", 0.3f, 0.5f);
            var smallDrone = DronePrefab("Drone_Small", "drone_small", 0.8f, 2.0f, 110f);
            var heavyDrone = DronePrefab("Drone_Heavy", "drone_heavy", 1.3f, 2.7f, 60f);

            var list = new List<WeaponDef>
            {
                Make("blaster", "Blaster", "Reliable homing bolts. Good all-rounder.", new Color(0.3f, 1f, 0.92f), "ico_w_blaster", WeaponMode.Projectile,
                    new WeaponStats { damage = 6f, fireInterval = 0.55f, range = 11f, projectiles = 1, speed = 16f, turnRate = 220f, spread = 10f },
                    new[]
                    {
                        S("Twin Barrels", "+1 bolt per shot", proj: 1),
                        S("Overcharge", "Damage +30%", dmg: 1.3f),
                        S("Rapid Cycler", "Fire rate +30%", rate: 1.3f),
                        S("Triple Array", "+1 bolt per shot", proj: 1),
                        S("Piercing Bolts", "Bolts pierce 2 enemies, damage +20%", dmg: 1.2f, pierce: 2),
                    }, blasterBolt, null, explosion, spark, null),

                Make("laser", "Laser", "Continuous beam that locks on the nearest target.", new Color(0.45f, 0.72f, 1f), "ico_w_laser", WeaponMode.Beam,
                    new WeaponStats { damage = 2.4f, fireInterval = 0.1f, range = 8.5f, projectiles = 1, turnRate = 0f },
                    new[]
                    {
                        S("Focused Lens", "Damage +30%", dmg: 1.3f),
                        S("Split Beam", "+1 beam target", proj: 1),
                        S("Long Focus", "Range +25%", range: 1.25f),
                        S("Prism", "+1 beam target", proj: 1),
                        S("Burning Core", "Damage +50%", dmg: 1.5f),
                    }, null, null, explosion, spark, beamMat),

                Make("ciws", "CIWS", "Point-defence gatling: tiny damage, huge fire rate.", new Color(0.92f, 0.96f, 1f), "ico_w_ciws", WeaponMode.Projectile,
                    new WeaponStats { damage = 1.5f, fireInterval = 0.08f, range = 7f, projectiles = 1, speed = 26f, turnRate = 400f, spread = 6f },
                    new[]
                    {
                        S("Tungsten Rounds", "Damage +30%", dmg: 1.3f),
                        S("Dual Mount", "+1 barrel", proj: 1),
                        S("Fire Control", "Range +25%", range: 1.25f),
                        S("Spin-up", "Fire rate +35%", rate: 1.35f),
                        S("Quad Mount", "+2 barrels", proj: 2),
                    }, ciws, null, explosion, spark, null),

                Make("heavy_missiles", "Heavy Missiles", "Slow homing missiles with a big blast.", new Color(0.35f, 1f, 0.85f), "ico_w_missile", WeaponMode.Projectile,
                    new WeaponStats { damage = 32f, fireInterval = 2f, range = 14f, projectiles = 1, speed = 9f, turnRate = 140f, aoe = 2.2f, spread = 30f },
                    new[]
                    {
                        S("Bigger Warhead", "Blast radius +30%", aoe: 1.3f),
                        S("Twin Launch", "+1 missile", proj: 1),
                        S("Fast Loader", "Fire rate +30%", rate: 1.3f),
                        S("Shaped Charge", "Damage +50%", dmg: 1.5f),
                        S("Tri Launch", "+1 missile, blast +20%", proj: 1, aoe: 1.2f),
                    }, heavyMissile, null, explosion, spark, null),

                Make("swarm_missiles", "Swarm Missiles", "Salvo of small agile missiles.", new Color(0.75f, 0.5f, 1f), "ico_w_swarm", WeaponMode.Projectile,
                    new WeaponStats { damage = 6f, fireInterval = 1.8f, range = 12f, projectiles = 5, speed = 12f, turnRate = 320f, aoe = 0.9f, spread = 22f },
                    new[]
                    {
                        S("Wider Salvo", "+2 missiles", proj: 2),
                        S("Hot Warheads", "Damage +30%", dmg: 1.3f),
                        S("Auto Loader", "Fire rate +25%", rate: 1.25f),
                        S("Hive Salvo", "+3 missiles", proj: 3),
                        S("Cluster Heads", "Blast +50%, damage +20%", dmg: 1.2f, aoe: 1.5f),
                    }, swarmMissile, null, explosion, spark, null),

                Make("plasma", "Plasma Cannon", "Heavy orb that burns through lines of enemies.", new Color(0.4f, 1f, 0.8f), "ico_w_plasma", WeaponMode.Projectile,
                    new WeaponStats { damage = 20f, fireInterval = 1.3f, range = 12f, projectiles = 1, speed = 8f, turnRate = 70f, pierce = 4, spread = 12f },
                    new[]
                    {
                        S("Dense Plasma", "Damage +30%", dmg: 1.3f),
                        S("Superheated", "Pierce +3", pierce: 3),
                        S("Twin Plasma", "+1 orb", proj: 1),
                        S("Magnetic Rails", "Fire rate +30%", rate: 1.3f),
                        S("Supernova", "Orbs detonate on every hit", setAoe: 1.6f),
                    }, plasma, null, explosion, spark, null),

                Make("drone_swarm", "Drone Swarm", "Four light drones orbit you and fire on their own.", new Color(0.3f, 1f, 0.92f), "ico_w_drones", WeaponMode.Drones,
                    new WeaponStats { damage = 3.5f, fireInterval = 0.8f, range = 9f, projectiles = 1, speed = 16f, turnRate = 300f, drones = 4, spread = 8f },
                    new[]
                    {
                        S("Reinforcements", "+2 drones", drones: 2),
                        S("Hardpoints", "Drone damage +30%", dmg: 1.3f),
                        S("Overclock", "Drone fire rate +30%", rate: 1.3f),
                        S("Hive Mind", "+2 drones", drones: 2),
                        S("Hunter Protocol", "Damage +40%, range +20%", dmg: 1.4f, range: 1.2f),
                    }, droneBolt, smallDrone, explosion, spark, null),

                Make("heavy_drones", "Heavy Drones", "Two gunship drones with explosive shells.", new Color(0.45f, 0.62f, 1f), "ico_w_hdrones", WeaponMode.Drones,
                    new WeaponStats { damage = 16f, fireInterval = 1.5f, range = 12f, projectiles = 1, speed = 13f, turnRate = 200f, aoe = 0.9f, drones = 2, spread = 10f },
                    new[]
                    {
                        S("Heavy Shells", "Damage +30%", dmg: 1.3f),
                        S("Twin Guns", "+1 shell per volley", proj: 1),
                        S("Autoloaders", "Fire rate +30%", rate: 1.3f),
                        S("Third Gunship", "+1 heavy drone", drones: 1),
                        S("Siege Mode", "Blast +60%, damage +30%", dmg: 1.3f, aoe: 1.6f),
                    }, heavyBolt, heavyDrone, explosion, spark, null),
            };
            return list;
        }

        static WeaponStep S(string title, string desc, float dmg = 1f, float rate = 1f, float range = 1f, float aoe = 1f,
            int proj = 0, int pierce = 0, int drones = 0, float setAoe = 0f) => new WeaponStep
        {
            title = title, description = desc, damageMult = dmg, fireRateMult = rate, rangeMult = range, aoeMult = aoe,
            addProjectiles = proj, addPierce = pierce, addDrones = drones, setAoe = setAoe,
        };

        static WeaponDef Make(string id, string name, string desc, Color color, string icon, WeaponMode mode, WeaponStats stats,
            WeaponStep[] steps, Projectile projectile, Drone drone, FxBurst explosion, FxBurst hit, Material beam)
        {
            var w = ScriptableObject.CreateInstance<WeaponDef>();
            w.id = id; w.displayName = name; w.description = desc; w.color = color; w.mode = mode;
            w.icon = UiSprites.Get(icon);
            w.stats = stats; w.steps = steps;
            w.projectilePrefab = projectile; w.dronePrefab = drone;
            w.explosionPrefab = explosion; w.hitPrefab = hit; w.beamMaterial = beam;
            AssetDatabase.CreateAsset(w, $"{Dir}/Weapon_{id}.asset");
            return w;
        }

        static Projectile Shot(string name, string sprite, float radius, float worldWidth)
        {
            var go = AssetBuilder.TriggerRoot(name, radius);
            AssetBuilder.AddVisual(go, sprite, worldWidth, 1.5f, AssetBuilder.OrderProjectile);
            go.AddComponent<Projectile>();
            return BuilderUtil.SavePrefab<Projectile>(go, name);
        }

        static Drone DronePrefab(string name, string sprite, float size, float orbit, float orbitSpeed)
        {
            var go = new GameObject(name);
            AssetBuilder.AddVisual(go, sprite, size, 1f, AssetBuilder.OrderPlayer);
            var d = go.AddComponent<Drone>();
            d.orbitRadius = orbit;
            d.orbitSpeed = orbitSpeed;
            return BuilderUtil.SavePrefab<Drone>(go, name);
        }

        public static FxBurst Fx(string name, string sprite, float duration, float startScale)
        {
            var go = new GameObject(name);
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = UiSprites.Get(sprite);
            sr.sortingOrder = 6;
            var fx = go.AddComponent<FxBurst>();
            fx.duration = duration;
            fx.startScale = startScale;
            return BuilderUtil.SavePrefab<FxBurst>(go, name);
        }

        static Material BeamMaterial()
        {
            var mat = new Material(Shader.Find("Sprites/Default")) { mainTexture = UiSprites.Get("beam").texture };
            BuilderUtil.EnsureDir(BuilderUtil.MaterialDir);
            AssetDatabase.CreateAsset(mat, BuilderUtil.MaterialDir + "/Beam.mat");
            return mat;
        }
    }
}
