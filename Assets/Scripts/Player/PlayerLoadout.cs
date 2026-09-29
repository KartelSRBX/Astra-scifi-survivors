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
            if (db == null || db.ships.Count == 0) return;
            var l = Loadout.Compute(db);

            if (visual != null && l.ship.sprite != null)
            {
                visual.sprite = l.ship.sprite;
                visual.transform.localScale = Vector3.one * (l.ship.visualSize * l.ship.sprite.pixelsPerUnit / l.ship.sprite.rect.width);
            }
            GetComponent<Health>().SetMax(l.maxHealth);
            GetComponent<PlayerController>().BaseSpeed = l.moveSpeed;
            GetComponent<PlayerExperience>().PickupRadius = l.pickupRadius;
            var w = GetComponent<AutoWeapon>();
            w.weaponName = l.weaponName;
            w.damage = l.damage;
            w.fireInterval = l.fireInterval;
            w.range = l.range;
            w.projectilesPerShot = l.projectiles;
        }
    }
}
