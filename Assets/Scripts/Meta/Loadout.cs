namespace GamingApp.Meta
{
    /// <summary>Final run stats from the selected ship, its level and the equipped items.</summary>
    public struct Loadout
    {
        public ShipData ship;
        public int level;
        public string weaponName;
        public float maxHealth, moveSpeed, damage, fireInterval, range, pickupRadius;
        public int projectiles;

        public const float BasePickupRadius = 2.5f;

        public static Loadout Compute(GameDatabase db) => Compute(db, db.Ship(Profile.SelectedShip) ?? db.ships[0]);

        public static Loadout Compute(GameDatabase db, ShipData ship)
        {
            int level = Profile.ShipLevel(ship.id);
            float lv = ShipData.LevelMultiplier(level);
            var weapon = Profile.Equipped(ItemSlot.Weapon, db);

            float hp = 0f, spd = 0f, dmg = 0f, rate = 0f, rng = 0f, pickup = 0f;
            for (int s = 0; s < 4; s++)
            {
                var it = Profile.Equipped((ItemSlot)s, db);
                if (it == null) continue;
                hp += it.healthPct; spd += it.speedPct; dmg += it.damagePct;
                rate += it.fireRatePct; rng += it.rangePct; pickup += it.pickupBonus;
            }

            return new Loadout
            {
                ship = ship,
                level = level,
                weaponName = weapon != null ? weapon.displayName : "Salvaged Gun",
                maxHealth = ship.maxHealth * lv * (1f + hp),
                moveSpeed = ship.moveSpeed * (1f + spd),
                damage = (weapon != null ? weapon.baseDamage : 3f) * ship.damageMultiplier * lv * (1f + dmg),
                fireInterval = (weapon != null ? weapon.fireInterval : 1f) / (ship.fireRateMultiplier * (1f + rate)),
                range = (weapon != null ? weapon.range : 8f) * (1f + rng),
                projectiles = weapon != null ? weapon.projectiles : 1,
                pickupRadius = BasePickupRadius + pickup,
            };
        }
    }
}
