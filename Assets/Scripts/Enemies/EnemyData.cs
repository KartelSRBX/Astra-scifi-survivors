using UnityEngine;

namespace GamingApp.Enemies
{
    public enum EnemyBehaviour { Chase, KeepDistanceAndShoot }

    /// <summary>One enemy archetype. Create via Assets > Create > GamingApp > Enemy Data.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Enemy Data", fileName = "Enemy_")]
    public class EnemyData : ScriptableObject
    {
        public string displayName = "Swarmer";
        public GameObject prefab;
        public EnemyBehaviour behaviour = EnemyBehaviour.Chase;

        [Header("Stats")]
        public float maxHealth = 10f;
        public float moveSpeed = 3f;
        public float contactDamage = 5f;
        public int xpReward = 1;

        [Header("Ranged (KeepDistanceAndShoot only)")]
        public float preferredDistance = 8f;
        public float shootInterval = 2f;
        public GameObject projectilePrefab;

        [Header("Spawning")]
        [Tooltip("Relative chance among archetypes that are already unlocked.")]
        public float spawnWeight = 1f;
        [Tooltip("Run progress 0..1 at which this archetype starts appearing.")]
        [Range(0f, 1f)] public float unlockAtProgress = 0f;
        [Tooltip("Boss: spawned once at the given progress instead of by weight.")]
        public bool isMiniBoss;
    }
}
