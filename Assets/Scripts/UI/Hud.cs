using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Player;

namespace GamingApp.UI
{
    /// <summary>Timer, health bar, level and XP bar.</summary>
    public class Hud : MonoBehaviour
    {
        [SerializeField] TMP_Text timerText;
        [SerializeField] TMP_Text levelText;
        [SerializeField] Slider healthBar;
        [SerializeField] Slider xpBar;
        [SerializeField] Health playerHealth;
        [SerializeField] PlayerExperience experience;

        void OnEnable()
        {
            playerHealth.Changed += OnHealthChanged;
            experience.XpChanged += OnXpChanged;
            experience.LeveledUp += OnLeveledUp;
        }

        void OnDisable()
        {
            playerHealth.Changed -= OnHealthChanged;
            experience.XpChanged -= OnXpChanged;
            experience.LeveledUp -= OnLeveledUp;
        }

        void Start()
        {
            OnHealthChanged(playerHealth.Current, playerHealth.Max);
            OnXpChanged(experience.Xp, experience.XpToNext);
            OnLeveledUp(experience.Level);
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
        void OnLeveledUp(int level) => levelText.text = $"Lv {level}"; // TODO: localize
    }
}
