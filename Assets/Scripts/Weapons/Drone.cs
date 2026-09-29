using UnityEngine;
using GamingApp.Core;
using GamingApp.Enemies;

namespace GamingApp.Weapons
{
    /// <summary>Orbits the player and fires the owning weapon's projectiles at the nearest enemy.</summary>
    public class Drone : MonoBehaviour
    {
        public float orbitRadius = 2.2f;
        public float orbitSpeed = 90f;

        PlayerWeapon weapon;
        int index;
        float cooldown;

        public void Init(PlayerWeapon owner, int slot)
        {
            weapon = owner;
            index = slot;
            cooldown = Random.Range(0f, 0.5f);
            transform.position = OrbitPoint();
        }

        public void SetSlot(int slot) => index = slot;

        Vector3 OrbitPoint()
        {
            int n = Mathf.Max(1, weapon.Stats.drones);
            float a = (Time.time * orbitSpeed + index * 360f / n) * Mathf.Deg2Rad;
            return weapon.transform.position + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * orbitRadius;
        }

        void Update()
        {
            if (weapon == null) { Destroy(gameObject); return; }
            transform.position = Vector3.Lerp(transform.position, OrbitPoint(), 1f - Mathf.Exp(-10f * Time.deltaTime));

            var gm = GameManager.Instance;
            if (gm == null || gm.State != RunState.Playing) return;

            var s = weapon.Stats;
            var target = Enemy.Nearest(transform.position, s.range);
            if (target != null)
            {
                Vector3 to = target.transform.position - transform.position;
                to.y = 0f;
                if (to.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to, Vector3.up), 1f - Mathf.Exp(-12f * Time.deltaTime));
            }

            cooldown -= Time.deltaTime;
            if (cooldown > 0f || target == null) return;
            cooldown = s.fireInterval * Random.Range(0.9f, 1.1f);
            weapon.FireVolley(transform.position, target);
        }
    }
}
