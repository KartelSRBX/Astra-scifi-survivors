using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GamingApp.Enemies;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>Creates placeholder prefabs (primitives) and ScriptableObject data for the test scene.</summary>
    static class AssetBuilder
    {
        public class Result
        {
            public Projectile PlayerProjectile;
            public List<EnemyData> Enemies = new();
            public List<UpgradeData> Upgrades = new();
        }

        public static Result BuildAll()
        {
            BuilderUtil.EnsureDir(BuilderUtil.DataDir);
            var r = new Result();

            var gem = BuildXpGem();
            r.PlayerProjectile = BuildProjectile("Projectile_Player", new Color(0.3f, 0.9f, 1f), 0.3f);
            var enemyProjectile = BuildProjectile("Projectile_Enemy", new Color(0.7f, 1f, 0.2f), 0.4f);

            //                          name       scale  color                       hp   speed dmg xp  weight unlock behaviour                          boss
            r.Enemies.Add(BuildEnemy("Swarmer", 0.7f, new Color(0.5f, 0.9f, 0.3f), 8f, 3.6f, 4f, 1, 5f, 0f, EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy("Tank", 1.6f, new Color(0.2f, 0.5f, 0.2f), 60f, 1.6f, 12f, 4, 1.5f, 0.1f, EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy("Spitter", 1.0f, new Color(0.8f, 0.9f, 0.1f), 14f, 2.4f, 6f, 2, 2f, 0.25f, EnemyBehaviour.KeepDistanceAndShoot, false, gem, enemyProjectile));
            r.Enemies.Add(BuildEnemy("MiniBoss_A", 3.0f, new Color(0.7f, 0.1f, 0.5f), 600f, 2.0f, 20f, 30, 1f, 0.5f, EnemyBehaviour.Chase, true, gem, null));
            r.Enemies.Add(BuildEnemy("MiniBoss_B", 3.6f, new Color(0.9f, 0.1f, 0.2f), 1800f, 2.2f, 30f, 60, 1f, 0.9f, EnemyBehaviour.Chase, true, gem, null));

            r.Upgrades.Add(BuildUpgrade("Damage", "Damage +25%", "Your weapon hits harder.", true, UpgradeStat.WeaponDamage, 0.25f, 6));
            r.Upgrades.Add(BuildUpgrade("FireRate", "Fire rate +20%", "Shoot more often.", true, UpgradeStat.WeaponFireRate, 0.2f, 6));
            r.Upgrades.Add(BuildUpgrade("Range", "Range +20%", "Target enemies further away.", true, UpgradeStat.WeaponRange, 0.2f, 4));
            r.Upgrades.Add(BuildUpgrade("Multishot", "+1 projectile", "Fire an extra bullet per shot.", true, UpgradeStat.ExtraProjectile, 1f, 4));
            r.Upgrades.Add(BuildUpgrade("Speed", "Boots: speed +12%", "Move faster.", false, UpgradeStat.MoveSpeed, 0.12f, 5));
            r.Upgrades.Add(BuildUpgrade("Health", "Armor: max HP +20%", "More health, and heals the difference.", false, UpgradeStat.MaxHealth, 0.2f, 5));
            r.Upgrades.Add(BuildUpgrade("Magnet", "Magnet: pickup +1.5", "Collect XP from further away.", false, UpgradeStat.PickupRadius, 1.5f, 4));

            AssetDatabase.SaveAssets();
            return r;
        }

        static XpGem BuildXpGem()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "XpGem";
            go.transform.localScale = Vector3.one * 0.35f;
            go.GetComponent<Renderer>().sharedMaterial = BuilderUtil.MakeMaterial("Gem", new Color(0.2f, 0.6f, 1f));
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.AddComponent<XpGem>();
            return BuilderUtil.SavePrefab<XpGem>(go, "XpGem");
        }

        static Projectile BuildProjectile(string name, Color color, float size)
        {
            var go = BuilderUtil.TriggerPrimitive(PrimitiveType.Sphere, name, Vector3.one * size,
                BuilderUtil.MakeMaterial(name, color));
            go.AddComponent<Projectile>();
            return BuilderUtil.SavePrefab<Projectile>(go, name);
        }

        static EnemyData BuildEnemy(string name, float scale, Color color, float hp, float speed, float damage,
            int xp, float weight, float unlock, EnemyBehaviour behaviour, bool boss, XpGem gem, Projectile projectile)
        {
            var go = BuilderUtil.TriggerPrimitive(PrimitiveType.Capsule, "Enemy_" + name,
                new Vector3(scale, scale, scale), BuilderUtil.MakeMaterial("Enemy_" + name, color));
            go.AddComponent<GamingApp.Core.Health>();
            var enemy = go.AddComponent<Enemy>();
            BuilderUtil.SetRef(enemy, "xpGemPrefab", gem);
            var prefab = BuilderUtil.SavePrefab(go, "Enemy_" + name);

            var data = ScriptableObject.CreateInstance<EnemyData>();
            data.displayName = name;
            data.prefab = prefab;
            data.behaviour = behaviour;
            data.maxHealth = hp;
            data.moveSpeed = speed;
            data.contactDamage = damage;
            data.xpReward = xp;
            data.spawnWeight = weight;
            data.unlockAtProgress = unlock;
            data.isMiniBoss = boss;
            data.projectilePrefab = projectile != null ? projectile.gameObject : null;
            AssetDatabase.CreateAsset(data, $"{BuilderUtil.DataDir}/Enemy_{name}.asset");
            return data;
        }

        static UpgradeData BuildUpgrade(string id, string title, string desc, bool weapon, UpgradeStat stat,
            float amount, int maxStacks)
        {
            var u = ScriptableObject.CreateInstance<UpgradeData>();
            u.displayName = title;
            u.description = desc;
            u.isWeaponUpgrade = weapon;
            u.stat = stat;
            u.amount = amount;
            u.maxStacks = maxStacks;
            AssetDatabase.CreateAsset(u, $"{BuilderUtil.DataDir}/Upgrade_{id}.asset");
            return u;
        }
    }
}
