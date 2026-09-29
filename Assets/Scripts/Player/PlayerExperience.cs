using System;
using UnityEngine;
using GamingApp.Core;

namespace GamingApp.Player
{
    /// <summary>XP and level tracking. Emits LeveledUp so the UI can offer a random upgrade.</summary>
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField] int baseXpToLevel = 5;
        [SerializeField] float xpGrowth = 1.35f;
        [SerializeField] float pickupRadius = 2.5f;

        public int Level { get; private set; } = 1;
        public int Xp { get; private set; }
        public int XpToNext { get; private set; }
        public float PickupRadius { get => pickupRadius; set => pickupRadius = value; }

        public event Action<int, int> XpChanged;   // xp, xpToNext
        public event Action<int> LeveledUp;        // new level

        void Awake() => XpToNext = baseXpToLevel;

        public void AddXp(int amount)
        {
            Xp += amount;
            while (Xp >= XpToNext)
            {
                Xp -= XpToNext;
                Level++;
                XpToNext = Mathf.CeilToInt(XpToNext * xpGrowth);
                LeveledUp?.Invoke(Level);
                GameManager.Instance.SetState(RunState.LevelUp);
            }
            XpChanged?.Invoke(Xp, XpToNext);
        }
    }
}
