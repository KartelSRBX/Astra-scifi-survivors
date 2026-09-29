using UnityEngine;
using GamingApp.Core;
using GamingApp.Player;
using GamingApp.Weapons;

namespace GamingApp.Enemies
{
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] EnemyData data;
        [SerializeField] XpGem xpGemPrefab;

        Health health;
        Transform target;
        float nextShotTime;
        float nextContactTime;

        public EnemyData Data => data;

        void Awake()
        {
            health = GetComponent<Health>();
            health.Died += OnDied;
        }

        /// <summary>Called by the spawner right after instantiation.</summary>
        public void Init(EnemyData enemyData, Transform player, float healthMultiplier)
        {
            data = enemyData;
            target = player;
            health.SetMax(data.maxHealth * healthMultiplier);
        }

        void Update()
        {
            if (data == null || target == null) return;

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;
            Vector3 dir = dist > 0.001f ? toTarget / dist : Vector3.zero;

            if (data.behaviour == EnemyBehaviour.Chase)
            {
                Move(dir);
            }
            else
            {
                if (dist > data.preferredDistance) Move(dir);
                else if (dist < data.preferredDistance * 0.7f) Move(-dir);
                TryShoot(dir);
            }
            if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        void Move(Vector3 dir) => transform.position += dir * (data.moveSpeed * Time.deltaTime);

        void TryShoot(Vector3 dir)
        {
            if (data.projectilePrefab == null || Time.time < nextShotTime) return;
            nextShotTime = Time.time + data.shootInterval;
            var go = Instantiate(data.projectilePrefab, transform.position + Vector3.up * 0.5f,
                Quaternion.LookRotation(dir, Vector3.up));
            if (go.TryGetComponent(out Projectile p)) p.Init(dir, data.contactDamage, targetsPlayer: true);
        }

        void OnTriggerStay(Collider other)
        {
            if (data == null || Time.time < nextContactTime) return;
            if (other.TryGetComponent(out PlayerHealth _) && other.TryGetComponent(out Health playerHealth))
            {
                nextContactTime = Time.time + 0.5f;
                playerHealth.TakeDamage(data.contactDamage);
            }
        }

        void OnDied(Health _)
        {
            if (xpGemPrefab != null && data != null)
            {
                var gem = Instantiate(xpGemPrefab, transform.position, Quaternion.identity);
                gem.Init(data.xpReward);
            }
            Destroy(gameObject);
        }
    }
}
