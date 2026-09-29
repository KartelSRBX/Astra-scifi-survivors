using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Weapons;

namespace GamingApp.UI
{
    /// <summary>Shows up to three upgrade cards; tapping one applies it and resumes the run.</summary>
    public class LevelUpPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] UpgradeManager upgrades;
        [SerializeField] Button[] cardButtons;
        [SerializeField] Text[] cardTitles;
        [SerializeField] Text[] cardDescriptions;

        static readonly Color WeaponTitle = new Color(0.78f, 0.62f, 1f);

        void Awake() => root.SetActive(false);
        void OnEnable() => upgrades.ChoicesRolled += Show;
        void OnDisable() => upgrades.ChoicesRolled -= Show;

        void Show(IReadOnlyList<UpgradeOption> choices)
        {
            root.SetActive(true);
            for (int i = 0; i < cardButtons.Length; i++)
            {
                bool has = i < choices.Count;
                cardButtons[i].gameObject.SetActive(has);
                if (!has) continue;

                var choice = choices[i];
                cardTitles[i].text = choice.isWeaponStep ? "WEAPON: " + choice.title : choice.title;
                cardTitles[i].color = choice.isWeaponStep ? WeaponTitle : UiTheme.Teal;
                cardDescriptions[i].text = choice.description;
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() =>
                {
                    root.SetActive(false);
                    upgrades.Pick(choice);
                });
            }
        }
    }
}
