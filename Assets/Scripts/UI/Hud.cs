using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Player;
using GamingApp.Weapons;

namespace GamingApp.UI
{
    /// <summary>Timer, health bar, level and XP bar.</summary>
    public class Hud : MonoBehaviour
    {
        [SerializeField] Text timerText;
        [SerializeField] Text levelText;
        [SerializeField] Slider healthBar;
        [SerializeField] Slider xpBar;
        [SerializeField] Health playerHealth;
        [SerializeField] PlayerExperience experience;
        [SerializeField] PlayerWeapon weapon;
        [SerializeField] Text weaponText;

        void OnEnable()
        {
            playerHealth.Changed += OnHealthChanged;
            experience.XpChanged += OnXpChanged;
            experience.LeveledUp += OnLeveledUp;
            weapon.Changed += OnWeaponChanged;
        }

        void OnDisable()
        {
            playerHealth.Changed -= OnHealthChanged;
            experience.XpChanged -= OnXpChanged;
            experience.LeveledUp -= OnLeveledUp;
            weapon.Changed -= OnWeaponChanged;
        }

        void Start()
        {
            OnHealthChanged(playerHealth.Current, playerHealth.Max);
            OnXpChanged(experience.Xp, experience.XpToNext);
            OnLeveledUp(experience.Level);
            OnWeaponChanged();
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            int secs = Mathf.CeilToInt(gm.Remaining);
            timerText.text = $"{secs / 60:00}:{secs % 60:00}";
        }

        void OnHealthChanged(float cur, float max) { healthBar.maxValue = max; healthBar.value = cur; }
        void OnXpChanged(int xp, int next) { xpBar.maxValue = next; xpBar.value = xp; }
        void OnWeaponChanged()
        {
            if (weapon.Def == null) return;
            weaponText.text = $"{weapon.Def.displayName.ToUpper()}  Lv {weapon.Level}/{weapon.Def.steps.Length}";
            weaponText.color = weapon.Def.color;
        }

        void OnLeveledUp(int level) => levelText.text = $"Lv {level}"; // TODO: localize
    }
}
