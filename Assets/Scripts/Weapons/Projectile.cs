using System.Collections.Generic;
using UnityEngine;
using GamingApp.Core;
using GamingApp.Enemies;
using GamingApp.Player;

namespace GamingApp.Weapons
{
    /// <summary>
    /// Bullet on the XZ plane. Player shots home in on the nearest enemy, can pierce and can explode (AoE).
    /// Needs a trigger Collider and a kinematic Rigidbody on the prefab.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 14f;
        [SerializeField] float lifetime = 3f;

        const float RetargetRadius = 9f;

        Vector3 direction;
        float damage;
        bool targetsPlayer;
        float turnRate;
        int pierceLeft;
        float aoe;
        Color color = Color.white;
        FxBurst explosionFx, hitFx;
        Enemy target;
        readonly HashSet<Health> alreadyHit = new();

        /// <summary>Straight shot (enemy fire).</summary>
        public void Init(Vector3 dir, float dmg, bool targetsPlayer = false)
        {
            direction = dir.normalized;
            damage = dmg;
            this.targetsPlayer = targetsPlayer;
            Destroy(gameObject, lifetime);
        }

        /// <summary>Player shot configured by the weapon.</summary>
        public void Launch(Vector3 dir, WeaponStats s, WeaponDef def, Enemy initialTarget)
        {
            direction = dir.normalized;
            damage = s.damage;
            speed = s.speed;
            turnRate = s.turnRate;
            pierceLeft = s.pierce;
            aoe = s.aoe;
            color = def.color;
            explosionFx = def.explosionPrefab;
            hitFx = def.hitPrefab;
            target = initialTarget;
            Destroy(gameObject, s.range / Mathf.Max(1f, s.speed) * 2f + 0.5f);
        }

        void Update()
        {
            if (turnRate > 0f)
            {
                if (target == null || !target.Alive) target = FindFreshTarget();
                if (target != null)
                {
                    Vector3 to = target.transform.position - transform.position;
                    to.y = 0f;
                    if (to.sqrMagnitude > 0.0001f)
                        direction = Vector3.RotateTowards(direction, to.normalized, turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f);
                }
            }
            transform.position += direction * (speed * Time.deltaTime);
            if (direction.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        /// <summary>Nearest live enemy this shot has not hit yet, so piercing shots don't circle a victim.</summary>
        Enemy FindFreshTarget()
        {
            Enemy best = null;
            float bestSqr = RetargetRadius * RetargetRadius;
            foreach (var e in Enemy.All)
            {
                if (!e.Alive || alreadyHit.Contains(e.Health)) continue;
                Vector3 d = e.transform.position - transform.position;
                d.y = 0f;
                if (d.sqrMagnitude < bestSqr) { bestSqr = d.sqrMagnitude; best = e; }
            }
            return best;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Health hp)) return;
            bool isPlayer = other.TryGetComponent(out PlayerHealth _);
            if (isPlayer != targetsPlayer) return;

            if (targetsPlayer)
            {
                hp.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
            if (!alreadyHit.Add(hp)) return;

            if (aoe > 0f) Explode();
            else
            {
                hp.TakeDamage(damage);
                FxBurst.Spawn(hitFx, transform.position, 0.8f, color);
            }

            if (pierceLeft-- <= 0) Destroy(gameObject);
            else target = null;
        }

        void Explode()
        {
            Vector3 c = transform.position;
            FxBurst.Spawn(explosionFx, c, aoe * 2f, color);
            float r2 = aoe * aoe;
            var hits = new List<Enemy>();
            foreach (var e in Enemy.All)
            {
                if (!e.Alive) continue;
                Vector3 d = e.transform.position - c;
                d.y = 0f;
                if (d.sqrMagnitude <= r2) hits.Add(e);
            }
            foreach (var e in hits) e.Health.TakeDamage(damage);
        }
    }
}
