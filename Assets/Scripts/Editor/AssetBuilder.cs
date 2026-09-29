using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GamingApp.Enemies;
using GamingApp.Meta;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>Creates sprite-based prefabs, the 10 enemy tiers, upgrades, ships, items and the GameDatabase.</summary>
    static class AssetBuilder
    {
        public class Result
        {
            public Projectile PlayerProjectile;
            public List<EnemyData> Enemies = new();
            public List<UpgradeData> Upgrades = new();
            public GameDatabase Database;
        }

        // Sprite visuals lie flat on the ground at this height regardless of the root's height.
        public const float GroundY = 0.1f;
        public const int OrderGem = 0, OrderEnemy = 1, OrderPlayer = 3, OrderProjectile = 4;

        public static Result BuildAll()
        {
            BuilderUtil.EnsureDir(BuilderUtil.DataDir);
            RemoveLegacy();
            var r = new Result();

            var gem = BuildXpGem();
            r.PlayerProjectile = BuildProjectile("Projectile_Player", "bolt_player", 0.25f, 0.9f);
            var enemyProjectile = BuildProjectile("Projectile_Enemy", "bolt_enemy", 0.3f, 1.1f);

            //                     tier name           size  hp     speed dmg  xp  weight unlock behaviour                          boss
            r.Enemies.Add(BuildEnemy(1, "Mite",        0.9f, 6f,    3.8f, 3f,  1,  6f,   0f,    EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(2, "Wasp",        1.1f, 10f,   4.2f, 4f,  1,  4f,   0.05f, EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(3, "Leech",       1.3f, 22f,   3.0f, 6f,  2,  3f,   0.12f, EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(4, "Spitter",     1.4f, 18f,   2.4f, 6f,  2,  2f,   0.2f,  EnemyBehaviour.KeepDistanceAndShoot, false, gem, enemyProjectile));
            r.Enemies.Add(BuildEnemy(5, "Reaver",      1.6f, 40f,   3.4f, 9f,  3,  2f,   0.3f,  EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(6, "Brute",       2.2f, 110f,  1.8f, 14f, 5,  1.2f, 0.4f,  EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(7, "Lancer",      1.9f, 60f,   2.6f, 12f, 4,  1f,   0.5f,  EnemyBehaviour.KeepDistanceAndShoot, false, gem, enemyProjectile));
            r.Enemies.Add(BuildEnemy(8, "Harbinger",   2.6f, 220f,  2.2f, 18f, 8,  0.7f, 0.62f, EnemyBehaviour.Chase, false, gem, null));
            r.Enemies.Add(BuildEnemy(9, "Warden",      4.0f, 900f,  2.0f, 22f, 30, 1f,   0.5f,  EnemyBehaviour.Chase, true, gem, null));
            r.Enemies.Add(BuildEnemy(10, "Leviathan",  5.5f, 2600f, 2.2f, 32f, 60, 1f,   0.9f,  EnemyBehaviour.Chase, true, gem, null));

            r.Upgrades.Add(BuildUpgrade("Damage", "Damage +25%", "Your weapon hits harder.", true, UpgradeStat.WeaponDamage, 0.25f, 6));
            r.Upgrades.Add(BuildUpgrade("FireRate", "Fire rate +20%", "Shoot more often.", true, UpgradeStat.WeaponFireRate, 0.2f, 6));
            r.Upgrades.Add(BuildUpgrade("Range", "Range +20%", "Target enemies further away.", true, UpgradeStat.WeaponRange, 0.2f, 4));
            r.Upgrades.Add(BuildUpgrade("Multishot", "+1 projectile", "Fire an extra bolt per shot.", true, UpgradeStat.ExtraProjectile, 1f, 4));
            r.Upgrades.Add(BuildUpgrade("Speed", "Thrusters: speed +12%", "Move faster.", false, UpgradeStat.MoveSpeed, 0.12f, 5));
            r.Upgrades.Add(BuildUpgrade("Health", "Plating: hull +20%", "More hull, and repairs the difference.", false, UpgradeStat.MaxHealth, 0.2f, 5));
            r.Upgrades.Add(BuildUpgrade("Magnet", "Tractor beam +1.5", "Collect XP from further away.", false, UpgradeStat.PickupRadius, 1.5f, 4));

            r.Database = BuildDatabase();
            AssetDatabase.SaveAssets();
            return r;
        }

        static void RemoveLegacy()
        {
            foreach (var n in new[] { "Swarmer", "Tank", "MiniBoss_A", "MiniBoss_B" })
            {
                AssetDatabase.DeleteAsset($"{BuilderUtil.DataDir}/Enemy_{n}.asset");
                AssetDatabase.DeleteAsset($"{BuilderUtil.PrefabDir}/Enemy_{n}.prefab");
                AssetDatabase.DeleteAsset($"{BuilderUtil.MaterialDir}/Enemy_{n}.mat");
            }
            AssetDatabase.DeleteAsset($"{BuilderUtil.MaterialDir}/Enemy_Spitter.mat");
            foreach (var n in new[] { "Gem", "Projectile_Player", "Projectile_Enemy" })
                AssetDatabase.DeleteAsset($"{BuilderUtil.MaterialDir}/{n}.mat");
        }

        /// <summary>Flat sprite child: lies on the ground, faces the camera, points along the root's forward.</summary>
        public static SpriteRenderer AddVisual(GameObject root, string sprite, float worldSize, float rootY, int order)
        {
            var v = new GameObject("Visual");
            v.transform.SetParent(root.transform, false);
            v.transform.localPosition = new Vector3(0f, GroundY - rootY, 0f);
            v.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            var sr = v.AddComponent<SpriteRenderer>();
            sr.sprite = UiSprites.Get(sprite);
            sr.sortingOrder = order;
            v.transform.localScale = Vector3.one * (worldSize * sr.sprite.pixelsPerUnit / sr.sprite.rect.width);
            return sr;
        }

        static GameObject TriggerRoot(string name, float radius)
        {
            var go = new GameObject(name);
            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = radius;
            var rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            return go;
        }

        static XpGem BuildXpGem()
        {
            var go = new GameObject("XpGem");
            AddVisual(go, "gem_xp", 0.7f, 1f, OrderGem);
            go.AddComponent<XpGem>();
            return BuilderUtil.SavePrefab<XpGem>(go, "XpGem");
        }

        static Projectile BuildProjectile(string name, string sprite, float radius, float length)
        {
            var go = TriggerRoot(name, radius);
            AddVisual(go, sprite, length * 0.5f, 1.5f, OrderProjectile);
            go.AddComponent<Projectile>();
            return BuilderUtil.SavePrefab<Projectile>(go, name);
        }

        static EnemyData BuildEnemy(int tier, string name, float size, float hp, float speed, float damage,
            int xp, float weight, float unlock, EnemyBehaviour behaviour, bool boss, XpGem gem, Projectile projectile)
        {
            var go = TriggerRoot("Enemy_" + name, size * 0.4f);
            AddVisual(go, $"enemy_{tier:00}", size, 1f, OrderEnemy);
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

        // ---------- ships, items, database ----------
        static GameDatabase BuildDatabase()
        {
            BuilderUtil.EnsureDir("Assets/Resources");
            BuilderUtil.EnsureDir(BuilderUtil.DataDir + "/Ships");
            BuilderUtil.EnsureDir(BuilderUtil.DataDir + "/Items");

            var db = ScriptableObject.CreateInstance<GameDatabase>();
            //                     id            name           class                 size  hp    speed dmg   rate  bio   cores
            db.ships.Add(Ship("corvette",    "Corvette",    "Light escort",        2.0f, 100f, 5.5f, 1f,   1f,   0,    0));
            db.ships.Add(Ship("frigate",     "Frigate",     "Multi-role patrol",   2.3f, 140f, 5.3f, 1.15f, 1.05f, 600, 0));
            db.ships.Add(Ship("destroyer",   "Destroyer",   "Strike vessel",       2.6f, 180f, 5.1f, 1.35f, 1.1f, 1500, 10));
            db.ships.Add(Ship("cruiser",     "Cruiser",     "Heavy line ship",     3.0f, 260f, 4.8f, 1.6f, 1.15f, 3500, 30));
            db.ships.Add(Ship("dreadnought", "Dreadnought", "Capital battleship",  3.6f, 380f, 4.5f, 2f,   1.25f, 8000, 80));

            db.items.Add(Weapon("pulse_blaster", "Pulse Blaster", Rarity.Common, 5f, 0.8f, 10f, 1));
            db.items.Add(Weapon("twin_laser", "Twin Laser", Rarity.Rare, 4f, 0.7f, 10f, 2));
            db.items.Add(Weapon("scatter_array", "Scatter Array", Rarity.Rare, 3f, 0.9f, 8f, 4));
            db.items.Add(Weapon("plasma_cannon", "Plasma Cannon", Rarity.Epic, 14f, 1.2f, 11f, 1));
            db.items.Add(Weapon("rail_driver", "Rail Driver", Rarity.Legendary, 24f, 1f, 14f, 1));

            db.items.Add(Part("composite_plating", "Composite Plating", ItemSlot.Hull, Rarity.Common, hp: 0.15f));
            db.items.Add(Part("reactive_hull", "Reactive Hull", ItemSlot.Hull, Rarity.Rare, hp: 0.25f));
            db.items.Add(Part("titan_armor", "Titan Armor", ItemSlot.Hull, Rarity.Epic, hp: 0.45f, speed: -0.05f));

            db.items.Add(Part("ion_thruster", "Ion Thruster", ItemSlot.Engine, Rarity.Common, speed: 0.08f));
            db.items.Add(Part("fusion_drive", "Fusion Drive", ItemSlot.Engine, Rarity.Rare, speed: 0.15f));
            db.items.Add(Part("warp_core", "Warp Core", ItemSlot.Engine, Rarity.Legendary, speed: 0.25f, rate: 0.1f));

            db.items.Add(Part("magnet_coil", "Magnet Coil", ItemSlot.Module, Rarity.Common, pickup: 1.5f));
            db.items.Add(Part("targeting_cpu", "Targeting CPU", ItemSlot.Module, Rarity.Rare, range: 0.2f, dmg: 0.1f));
            db.items.Add(Part("overclock_chip", "Overclock Chip", ItemSlot.Module, Rarity.Epic, rate: 0.25f));

            AssetDatabase.CreateAsset(db, "Assets/Resources/GameDatabase.asset");
            return db;
        }

        static ShipData Ship(string id, string name, string cls, float size, float hp, float speed, float dmg, float rate, int bio, int cores)
        {
            var s = ScriptableObject.CreateInstance<ShipData>();
            s.id = id; s.displayName = name; s.shipClass = cls; s.visualSize = size;
            s.sprite = UiSprites.Get("ship_" + id);
            s.maxHealth = hp; s.moveSpeed = speed; s.damageMultiplier = dmg; s.fireRateMultiplier = rate;
            s.priceBiomass = bio; s.priceCores = cores;
            AssetDatabase.CreateAsset(s, $"{BuilderUtil.DataDir}/Ships/Ship_{id}.asset");
            return s;
        }

        static ItemData Weapon(string id, string name, Rarity rarity, float dmg, float interval, float range, int projectiles)
        {
            var it = NewItem(id, name, ItemSlot.Weapon, rarity);
            it.baseDamage = dmg; it.fireInterval = interval; it.range = range; it.projectiles = projectiles;
            return Save(it);
        }

        static ItemData Part(string id, string name, ItemSlot slot, Rarity rarity,
            float hp = 0, float speed = 0, float dmg = 0, float rate = 0, float range = 0, float pickup = 0)
        {
            var it = NewItem(id, name, slot, rarity);
            it.healthPct = hp; it.speedPct = speed; it.damagePct = dmg; it.fireRatePct = rate; it.rangePct = range; it.pickupBonus = pickup;
            return Save(it);
        }

        static ItemData NewItem(string id, string name, ItemSlot slot, Rarity rarity)
        {
            var it = ScriptableObject.CreateInstance<ItemData>();
            it.id = id; it.displayName = name; it.slot = slot; it.rarity = rarity;
            it.icon = UiSprites.Get(SlotIcon(slot));
            return it;
        }

        public static string SlotIcon(ItemSlot slot) => slot switch
        {
            ItemSlot.Weapon => "ico_weapon",
            ItemSlot.Hull => "ico_hull",
            ItemSlot.Engine => "ico_engine",
            _ => "ico_module",
        };

        static ItemData Save(ItemData it)
        {
            AssetDatabase.CreateAsset(it, $"{BuilderUtil.DataDir}/Items/Item_{it.id}.asset");
            return it;
        }
    }
}
