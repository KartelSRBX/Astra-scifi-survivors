using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;

namespace GamingApp.UI
{
    /// <summary>Main menu: play, settings overlay, best time.</summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] Button playButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button closeSettingsButton;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Text bestText;

        void Awake()
        {
            Time.timeScale = 1f;
            settingsPanel.SetActive(false);
            playButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Game));
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
            closeSettingsButton.onClick.AddListener(() => { settingsPanel.SetActive(false); PlayerPrefs.Save(); });

            float best = GameManager.BestTime;
            bestText.text = best > 0f ? $"Best: {(int)best / 60:00}:{(int)best % 60:00}" : "";
        }
    }
}
