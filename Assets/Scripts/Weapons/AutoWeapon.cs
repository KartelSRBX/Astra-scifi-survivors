using UnityEngine;
using GamingApp.Core;
using GamingApp.Enemies;

namespace GamingApp.Weapons
{
    /// <summary>Fires automatically at the nearest enemy in range. Upgrades tweak the public stats.</summary>
    public class AutoWeapon : MonoBehaviour
    {
        [SerializeField] Projectile projectilePrefab;
        public string weaponName = "Pulse Blaster";
        public float damage = 5f;
        public float fireInterval = 0.8f;
        public float range = 10f;
        public int projectilesPerShot = 1;
        [SerializeField] float spreadDegrees = 12f;

        float nextFire;

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State != RunState.Playing || Time.time < nextFire) return;

            Enemy target = FindNearest();
            if (target == null) return;

            nextFire = Time.time + fireInterval;
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            Fire(dir.normalized);
        }

        void Fire(Vector3 baseDir)
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                float offset = projectilesPerShot == 1 ? 0f : (i - (projectilesPerShot - 1) * 0.5f) * spreadDegrees;
                Vector3 dir = Quaternion.Euler(0f, offset, 0f) * baseDir;
                var p = Instantiate(projectilePrefab, transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(dir));
                p.Init(dir, damage);
            }
        }

        Enemy FindNearest()
        {
            Enemy best = null;
            float bestSqr = range * range;
            foreach (var e in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                float d = (e.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = e; }
            }
            return best;
        }
    }
}
