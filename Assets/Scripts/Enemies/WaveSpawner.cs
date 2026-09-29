using System.Collections.Generic;
using UnityEngine;
using GamingApp.Core;
using GamingApp.UI;
using GamingApp.Weapons;

namespace GamingApp.Enemies
{
    /// <summary>
    /// Spawns enemies in a ring around the player. Spawn rate and enemy HP grow with run progress;
    /// mini-bosses (EnemyData.isMiniBoss) appear once when progress passes their unlock point.
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] List<EnemyData> archetypes = new();

        [Header("Difficulty curve")]
        [SerializeField] float startSpawnsPerSecond = 0.8f;
        [SerializeField] float endSpawnsPerSecond = 6f;
        [SerializeField] float endHealthMultiplier = 4f;
        [SerializeField] int maxAlive = 250;

        [Header("Placement")]
        [SerializeField] float spawnRadius = 18f;

        float spawnBudget;
        readonly HashSet<EnemyData> spawnedBosses = new();
        int alive;
        float difficulty = 1f;

        void Start()
        {
            var sys = RunSetup.System;
            if (sys != null) difficulty = sys.Difficulty;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State != RunState.Playing || player == null) return;

            float p = gm.Progress;
            SpawnBossesIfDue(p);

            spawnBudget += Mathf.Lerp(startSpawnsPerSecond, endSpawnsPerSecond, p) * Time.deltaTime;
            while (spawnBudget >= 1f)
            {
                spawnBudget -= 1f;
                if (alive >= maxAlive) continue;
                var pick = PickArchetype(p);
                if (pick != null) Spawn(pick, p);
            }
        }

        void SpawnBossesIfDue(float progress)
        {
            foreach (var a in archetypes)
                if (a.isMiniBoss && progress >= a.unlockAtProgress && spawnedBosses.Add(a))
                    Spawn(a, progress);
        }

        EnemyData PickArchetype(float progress)
        {
            float total = 0f;
            foreach (var a in archetypes)
                if (!a.isMiniBoss && progress >= a.unlockAtProgress) total += a.spawnWeight;
            if (total <= 0f) return null;

            float roll = Random.value * total;
            foreach (var a in archetypes)
            {
                if (a.isMiniBoss || progress < a.unlockAtProgress) continue;
                roll -= a.spawnWeight;
                if (roll <= 0f) return a;
            }
            return null;
        }

        void Spawn(EnemyData data, float progress)
        {
            if (data.prefab == null) return;
            float angle = Random.value * Mathf.PI * 2f;
            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spawnRadius;

            var go = Instantiate(data.prefab, pos, Quaternion.identity);
            if (go.TryGetComponent(out Enemy enemy))
                enemy.Init(data, player, Mathf.Lerp(1f, endHealthMultiplier, progress) * difficulty, difficulty);
            if (data.isMiniBoss)
            {
                Banner.Show("WARNING", $"{data.displayName.ToUpper()} INBOUND", new Color(1f, 0.3f, 0.3f));
                CameraShake.Add(0.5f);
            }

            alive++;
            go.AddComponent<AliveCounter>().Init(() => alive--);
        }

        /// <summary>Tiny helper so the spawner knows when an enemy is destroyed.</summary>
        class AliveCounter : MonoBehaviour
        {
            System.Action onGone;
            public void Init(System.Action cb) => onGone = cb;
            void OnDestroy() => onGone?.Invoke();
        }
    }
}
