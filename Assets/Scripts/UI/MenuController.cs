using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;

namespace GamingApp.UI
{
    /// <summary>Main menu: bottom nav (character, inventory, start, upgrades, forge), currencies, settings.</summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] Button playButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button closeSettingsButton;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Text bestText;
        [SerializeField] Text biomassText;
        [SerializeField] Text coresText;

        [SerializeField] Button[] navButtons;
        [SerializeField] string[] navTitles;
        [SerializeField] GameObject placeholderPanel;
        [SerializeField] Text placeholderTitle;
        [SerializeField] Button placeholderClose;

        void Awake()
        {
            Time.timeScale = 1f;
            settingsPanel.SetActive(false);
            placeholderPanel.SetActive(false);
            playButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Game));
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
            closeSettingsButton.onClick.AddListener(() => { settingsPanel.SetActive(false); PlayerPrefs.Save(); });
            placeholderClose.onClick.AddListener(() => placeholderPanel.SetActive(false));

            for (int i = 0; i < navButtons.Length; i++)
            {
                string title = navTitles[i];
                navButtons[i].onClick.AddListener(() =>
                {
                    placeholderTitle.text = title + "\nComing soon"; // TODO: localize
                    placeholderPanel.SetActive(true);
                });
            }

            float best = GameManager.BestTime;
            bestText.text = best > 0f ? $"Best: {(int)best / 60:00}:{(int)best % 60:00}" : "";
            biomassText.text = Wallet.Biomass.ToString();
            coresText.text = Wallet.Cores.ToString();
        }
    }
}
