using System.Collections.Generic;
using UnityEngine;
using GamingApp.Core;
using GamingApp.Player;

namespace GamingApp.Weapons
{
    /// <summary>A level-up card: either a generic stat upgrade or the weapon's next step.</summary>
    public class UpgradeOption
    {
        public string title;
        public string description;
        public bool isWeaponStep;
        public UpgradeData data;
    }

    /// <summary>Rolls level-up choices (generic upgrades plus the weapon's next step) and applies the pick.</summary>
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] List<UpgradeData> pool = new();
        [SerializeField] PlayerWeapon weapon;
        [SerializeField] PlayerController controller;
        [SerializeField] Health playerHealth;
        [SerializeField] PlayerExperience experience;
        [SerializeField] int choicesPerLevelUp = 3;
        [SerializeField, Range(0f, 1f)] float weaponStepChance = 0.75f;

        readonly Dictionary<UpgradeData, int> stacks = new();

        public event System.Action<IReadOnlyList<UpgradeOption>> ChoicesRolled;

        void OnEnable() => experience.LeveledUp += OnLeveledUp;
        void OnDisable() => experience.LeveledUp -= OnLeveledUp;

        void OnLeveledUp(int level)
        {
            var choices = Roll();
            if (choices.Count == 0) { GameManager.Instance.SetState(RunState.Playing); return; }
            ChoicesRolled?.Invoke(choices);
        }

        List<UpgradeOption> Roll()
        {
            var result = new List<UpgradeOption>();
            if (weapon.CanLevel && Random.value < weaponStepChance)
            {
                var step = weapon.NextStep;
                result.Add(new UpgradeOption
                {
                    title = step.title,
                    description = $"{weapon.Def.displayName} Lv {weapon.Level + 1}/{weapon.Def.steps.Length}\n{step.description}",
                    isWeaponStep = true,
                });
            }

            var available = pool.FindAll(u => stacks.GetValueOrDefault(u) < u.maxStacks);
            while (result.Count < choicesPerLevelUp && available.Count > 0)
            {
                int i = Random.Range(0, available.Count);
                result.Add(new UpgradeOption { title = available[i].displayName, description = available[i].description, data = available[i] });
                available.RemoveAt(i);
            }
            return result;
        }

        public void Pick(UpgradeOption option)
        {
            if (option.isWeaponStep) weapon.ApplyNextStep();
            else
            {
                stacks[option.data] = stacks.GetValueOrDefault(option.data) + 1;
                Apply(option.data);
            }
            GameManager.Instance.SetState(RunState.Playing);
        }

        void Apply(UpgradeData u)
        {
            switch (u.stat)
            {
                case UpgradeStat.WeaponDamage: weapon.damageMult *= 1f + u.amount; weapon.Recompute(); break;
                case UpgradeStat.WeaponFireRate: weapon.fireRateMult *= 1f + u.amount; weapon.Recompute(); break;
                case UpgradeStat.WeaponRange: weapon.rangeMult *= 1f + u.amount; weapon.Recompute(); break;
                case UpgradeStat.ExtraProjectile: weapon.extraProjectiles += Mathf.RoundToInt(u.amount); weapon.Recompute(); break;
                case UpgradeStat.MoveSpeed: controller.SpeedMultiplier *= 1f + u.amount; break;
                case UpgradeStat.MaxHealth: playerHealth.SetMax(playerHealth.Max * (1f + u.amount), refill: false); playerHealth.Heal(playerHealth.Max * u.amount); break;
                case UpgradeStat.PickupRadius: experience.PickupRadius += u.amount; break;
            }
        }
    }
}
