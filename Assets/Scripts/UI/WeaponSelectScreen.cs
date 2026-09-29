using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;
using GamingApp.Weapons;

namespace GamingApp.UI
{
    /// <summary>Pre-run weapon choice: pick one of the weapon cards, then launch.</summary>
    public class WeaponSelectScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Button openButton;
        [SerializeField] Button closeButton;
        [SerializeField] Button launchButton;
        [SerializeField] Button[] cards;
        [SerializeField] Image[] cardFrames;
        [SerializeField] Image[] cardGlows;
        [SerializeField] Text detailName;
        [SerializeField] Text detailText;

        GameDatabase db;
        int selected;

        void Awake()
        {
            db = GameDatabase.Get();
            root.SetActive(false);
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(() => root.SetActive(false));
            launchButton.onClick.AddListener(() =>
            {
                RunSetup.WeaponId = db.weapons[selected].id;
                SceneLoader.Load(SceneLoader.Game);
            });
            for (int i = 0; i < cards.Length; i++)
            {
                int index = i;
                cards[i].onClick.AddListener(() => Select(index));
            }
        }

        void Open()
        {
            selected = Mathf.Max(0, db.weapons.FindIndex(w => w.id == RunSetup.WeaponId));
            root.SetActive(true);
            Select(selected);
        }

        void Select(int index)
        {
            selected = index;
            for (int i = 0; i < cards.Length; i++)
            {
                var c = i < db.weapons.Count ? db.weapons[i].color : Color.white;
                bool on = i == index;
                cardFrames[i].color = on ? c : new Color(c.r, c.g, c.b, 0.18f);
                cardGlows[i].enabled = on;
                cardGlows[i].color = new Color(c.r, c.g, c.b, 0.35f);
            }
            var w = db.weapons[index];
            var s = w.stats;
            detailName.text = w.displayName.ToUpper();
            detailName.color = w.color;
            string extra = w.mode == WeaponMode.Drones ? $"   Drones {s.drones}" : w.mode == WeaponMode.Beam ? "   Continuous beam" : "";
            float dps = s.damage * s.projectiles * Mathf.Max(1, s.drones) / s.fireInterval;
            detailText.text = $"{w.description}\nDPS ~{dps:0}   Range {s.range:0}{extra}";
        }
    }
}
