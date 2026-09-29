using System.Collections.Generic;
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
        Transform visual;
        Vector3 visualScale;
        float punch;

        public EnemyData Data => data;
        public bool Alive => health != null && !health.IsDead;
        public Health Health => health;

        /// <summary>Live enemies, for targeting without scene searches.</summary>
        public static readonly List<Enemy> All = new();

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);

        public static Enemy Nearest(Vector3 pos, float range, Enemy exclude = null)
        {
            Enemy best = null;
            float bestSqr = range * range;
            foreach (var e in All)
            {
                if (e == exclude || !e.Alive) continue;
                Vector3 d = e.transform.position - pos;
                d.y = 0f;
                float sq = d.sqrMagnitude;
                if (sq < bestSqr) { bestSqr = sq; best = e; }
            }
            return best;
        }

        /// <summary>Fills result with up to count nearest live enemies in range, closest first.</summary>
        public static void NearestN(Vector3 pos, float range, int count, List<Enemy> result)
        {
            result.Clear();
            float r2 = range * range;
            foreach (var e in All)
            {
                if (!e.Alive) continue;
                Vector3 d = e.transform.position - pos;
                d.y = 0f;
                if (d.sqrMagnitude <= r2) result.Add(e);
            }
            result.Sort((x, y) => (x.transform.position - pos).sqrMagnitude.CompareTo((y.transform.position - pos).sqrMagnitude));
            if (result.Count > count) result.RemoveRange(count, result.Count - count);
        }

        void Awake()
        {
            health = GetComponent<Health>();
            health.Died += OnDied;
            health.Changed += (_, _) => punch = 1f;
            visual = transform.Find("Visual");
            if (visual != null) visualScale = visual.localScale;
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
            if (visual != null && punch > 0f)
            {
                punch = Mathf.Max(0f, punch - Time.deltaTime * 8f);
                visual.localScale = visualScale * (1f + 0.18f * punch);
            }
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
            if (GameManager.Instance != null && data != null) GameManager.Instance.RegisterKill(data.isMiniBoss);
            if (xpGemPrefab != null && data != null)
            {
                var gem = Instantiate(xpGemPrefab, transform.position, Quaternion.identity);
                gem.Init(data.xpReward);
            }
            Destroy(gameObject);
        }
    }
}
