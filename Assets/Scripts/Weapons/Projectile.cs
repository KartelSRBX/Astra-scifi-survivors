using UnityEngine;
using GamingApp.Core;
using GamingApp.Player;

namespace GamingApp.Weapons
{
    /// <summary>Straight-flying bullet. Needs a trigger Collider and a kinematic Rigidbody on the prefab.</summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 14f;
        [SerializeField] float lifetime = 3f;

        Vector3 direction;
        float damage;
        bool targetsPlayer;

        public void Init(Vector3 dir, float dmg, bool targetsPlayer = false)
        {
            direction = dir.normalized;
            damage = dmg;
            this.targetsPlayer = targetsPlayer;
            Destroy(gameObject, lifetime);
        }

        void Update() => transform.position += direction * (speed * Time.deltaTime);

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Health hp)) return;
            bool isPlayer = other.TryGetComponent(out PlayerHealth _);
            if (isPlayer != targetsPlayer) return;

            hp.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
