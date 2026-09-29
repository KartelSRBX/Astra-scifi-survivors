using System.Collections.Generic;
using UnityEngine;
using GamingApp.Core;
using GamingApp.Player;

namespace GamingApp.Weapons
{
    /// <summary>Rolls random upgrade choices on level-up and applies the picked one to the player.</summary>
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] List<UpgradeData> pool = new();
        [SerializeField] AutoWeapon weapon;
        [SerializeField] PlayerController controller;
        [SerializeField] Health playerHealth;
        [SerializeField] PlayerExperience experience;
        [SerializeField] int choicesPerLevelUp = 3;

        readonly Dictionary<UpgradeData, int> stacks = new();

        public IReadOnlyList<UpgradeData> CurrentChoices => choices;
        List<UpgradeData> choices = new();

        public event System.Action<IReadOnlyList<UpgradeData>> ChoicesRolled;

        void OnEnable() => experience.LeveledUp += OnLeveledUp;
        void OnDisable() => experience.LeveledUp -= OnLeveledUp;

        void OnLeveledUp(int level)
        {
            choices = Roll();
            if (choices.Count == 0) { GameManager.Instance.SetState(RunState.Playing); return; }
            ChoicesRolled?.Invoke(choices);
        }

        List<UpgradeData> Roll()
        {
            var available = pool.FindAll(u => stacks.GetValueOrDefault(u) < u.maxStacks);
            var result = new List<UpgradeData>();
            while (result.Count < choicesPerLevelUp && available.Count > 0)
            {
                int i = Random.Range(0, available.Count);
                result.Add(available[i]);
                available.RemoveAt(i);
            }
            return result;
        }

        public void Pick(UpgradeData upgrade)
        {
            stacks[upgrade] = stacks.GetValueOrDefault(upgrade) + 1;
            Apply(upgrade);
            GameManager.Instance.SetState(RunState.Playing);
        }

        void Apply(UpgradeData u)
        {
            switch (u.stat)
            {
                case UpgradeStat.WeaponDamage: weapon.damage *= 1f + u.amount; break;
                case UpgradeStat.WeaponFireRate: weapon.fireInterval /= 1f + u.amount; break;
                case UpgradeStat.WeaponRange: weapon.range *= 1f + u.amount; break;
                case UpgradeStat.ExtraProjectile: weapon.projectilesPerShot += Mathf.RoundToInt(u.amount); break;
                case UpgradeStat.MoveSpeed: controller.SpeedMultiplier *= 1f + u.amount; break;
                case UpgradeStat.MaxHealth: playerHealth.SetMax(playerHealth.Max * (1f + u.amount), refill: false); playerHealth.Heal(playerHealth.Max * u.amount); break;
                case UpgradeStat.PickupRadius: experience.PickupRadius += u.amount; break;
            }
        }
    }
}
