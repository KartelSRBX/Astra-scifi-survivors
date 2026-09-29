using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Meta;

namespace GamingApp.UI
{
    /// <summary>Main menu: start, settings, currencies, selected ship avatar, placeholder screens.</summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] Button settingsButton;
        [SerializeField] Button closeSettingsButton;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Text biomassText;
        [SerializeField] Text coresText;
        [SerializeField] Image avatarShip;
        [SerializeField] Text pilotLevelText;

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
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
            closeSettingsButton.onClick.AddListener(() => { settingsPanel.SetActive(false); PlayerPrefs.Save(); });
            placeholderClose.onClick.AddListener(() => placeholderPanel.SetActive(false));

            for (int i = 0; i < navButtons.Length; i++)
            {
                string title = navTitles[i];
                navButtons[i].onClick.AddListener(() =>
                {
                    placeholderTitle.text = title;
                    placeholderPanel.SetActive(true);
                });
            }

            Wallet.Changed += RefreshWallet;
            Profile.Changed += RefreshShip;
            RefreshWallet();
            RefreshShip();
        }

        void OnDestroy()
        {
            Wallet.Changed -= RefreshWallet;
            Profile.Changed -= RefreshShip;
        }

        void RefreshWallet()
        {
            biomassText.text = Wallet.Biomass.ToString();
            coresText.text = Wallet.Cores.ToString();
        }

        void RefreshShip()
        {
            var db = GameDatabase.Get();
            var ship = db != null ? db.Ship(Profile.SelectedShip) : null;
            if (ship == null) return;
            avatarShip.sprite = ship.sprite;
            pilotLevelText.text = Profile.ShipLevel(ship.id).ToString();
        }
    }
}
