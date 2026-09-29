namespace GamingApp.Meta
{
    /// <summary>Run modifiers from the selected ship, its level and the equipped items.</summary>
    public struct Loadout
    {
        public ShipData ship;
        public int level;
        public float maxHealth, moveSpeed, pickupRadius, armor;
        public float damageMult, fireRateMult, rangeMult, projectileSpeedMult;
        public int extraProjectiles;

        public const float BasePickupRadius = 2.5f;

        public static Loadout Compute(GameDatabase db) => Compute(db, db.Ship(Profile.SelectedShip) ?? db.ships[0]);

        public static Loadout Compute(GameDatabase db, ShipData ship)
        {
            int level = Profile.ShipLevel(ship.id);
            float lv = ShipData.LevelMultiplier(level);

            float hp = 0f, spd = 0f, dmg = 0f, rate = 0f, rng = 0f, pickup = 0f;
            int proj = 0;
            for (int s = 0; s < 4; s++)
            {
                var it = Profile.Equipped((ItemSlot)s, db);
                if (it == null) continue;
                hp += it.healthPct; spd += it.speedPct; dmg += it.damagePct;
                rate += it.fireRatePct; rng += it.rangePct; pickup += it.pickupBonus; proj += it.extraProjectiles;
            }

            var gear = ModuleShop.GearBonus();
            hp += gear.health; spd += gear.speed; dmg += gear.damage; rate += gear.fireRate; rng += gear.range; pickup += gear.pickup;

            return new Loadout
            {
                ship = ship,
                level = level,
                maxHealth = ship.maxHealth * lv * (1f + hp),
                moveSpeed = ship.moveSpeed * (1f + spd),
                damageMult = ship.damageMultiplier * lv * (1f + dmg),
                fireRateMult = ship.fireRateMultiplier * (1f + rate),
                rangeMult = 1f + rng,
                extraProjectiles = proj,
                pickupRadius = BasePickupRadius + pickup,
                armor = gear.armor,
                projectileSpeedMult = 1f + gear.projectileSpeed,
            };
        }
    }
}
