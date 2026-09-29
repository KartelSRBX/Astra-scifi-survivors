using System;
using UnityEngine;

namespace GamingApp.Core
{
    /// <summary>Shared HP component for the player and enemies.</summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;

        /// <summary>Fraction of incoming damage ignored (player armor).</summary>
        [NonSerialized] public float damageReduction;

        public float Max => maxHealth;
        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;

        public event Action<float, float> Changed; // current, max
        public event Action<Health> Died;

        void Awake() => Current = maxHealth;

        public void SetMax(float value, bool refill = true)
        {
            maxHealth = value;
            if (refill) Current = value;
            Changed?.Invoke(Current, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            Current = Mathf.Max(0f, Current - amount * (1f - damageReduction));
            Changed?.Invoke(Current, maxHealth);
            if (IsDead) Died?.Invoke(this);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            Current = Mathf.Min(maxHealth, Current + amount);
            Changed?.Invoke(Current, maxHealth);
        }
    }
}
