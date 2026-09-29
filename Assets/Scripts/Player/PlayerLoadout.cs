using UnityEngine;
using GamingApp.Core;
using GamingApp.Meta;
using GamingApp.Weapons;

namespace GamingApp.Player
{
    /// <summary>Applies the selected ship, its level and equipped items to the player at run start.</summary>
    [DefaultExecutionOrder(-100)]
    public class PlayerLoadout : MonoBehaviour
    {
        [SerializeField] SpriteRenderer visual;

        void Awake()
        {
            var db = GameDatabase.Get();
            if (db == null || db.ships.Count == 0 || db.weapons.Count == 0) return;
            var l = Loadout.Compute(db);

            if (visual != null && l.ship.sprite != null)
            {
                visual.sprite = l.ship.sprite;
                visual.transform.localScale = Vector3.one * (l.ship.visualSize * l.ship.sprite.pixelsPerUnit / l.ship.sprite.rect.width);
            }
            var hp = GetComponent<Health>();
            hp.SetMax(l.maxHealth);
            hp.damageReduction = ModuleShop.ArmorReduction(l.armor);
            GetComponent<PlayerController>().BaseSpeed = l.moveSpeed;
            GetComponent<PlayerExperience>().PickupRadius = l.pickupRadius;

            var def = db.Weapon(RunSetup.WeaponId) ?? db.weapons[0];
            var mods = ModuleShop.WeaponBonus(def.id);
            var w = GetComponent<PlayerWeapon>();
            w.damageMult = l.damageMult * (1f + mods.damage);
            w.fireRateMult = l.fireRateMult * (1f + mods.fireRate);
            w.rangeMult = l.rangeMult * (1f + mods.range);
            w.projectileSpeedMult = l.projectileSpeedMult * (1f + mods.projectileSpeed);
            w.extraProjectiles = l.extraProjectiles;
            w.Equip(def);
        }
    }
}
