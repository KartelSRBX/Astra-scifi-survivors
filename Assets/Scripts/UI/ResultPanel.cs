using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Meta;

namespace GamingApp.UI
{
    /// <summary>End-of-run screen: result, survival time, best time, restart / back to menu.</summary>
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Text titleText;
        [SerializeField] Text detailsText;
        [SerializeField] Button restartButton;
        [SerializeField] Button menuButton;

        void Awake()
        {
            root.SetActive(false);
            restartButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Game));
            menuButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Menu));
        }

        void Start() => GameManager.Instance.StateChanged += OnStateChanged;
        void OnDestroy() { if (GameManager.Instance != null) GameManager.Instance.StateChanged -= OnStateChanged; }

        void OnStateChanged(RunState state)
        {
            if (state != RunState.Won && state != RunState.Lost) return;
            var gm = GameManager.Instance;
            root.SetActive(true);
            titleText.text = state == RunState.Won ? "Survived!" : "You died"; // TODO: localize
            titleText.color = state == RunState.Won ? UiTheme.Teal : UiTheme.Violet;
            var r = RunRewards.Last;
            string loot = $"<color=#22D4C4>+{r.biomass} Biomass</color>   <color=#AE6BFF>+{r.cores} Cores</color>";
            if (r.item != null) loot += $"\n<color=#{ColorUtility.ToHtmlStringRGB(UiTheme.RarityColor(r.item.rarity))}>Found: {r.item.displayName}</color>";
            string sysLine = "";
            var sys = GamingApp.Weapons.RunSetup.System;
            if (sys != null)
            {
                float clear = Profile.SystemClearance(sys.index);
                sysLine = clear >= 1f
                    ? $"<color=#22D4C4>{sys.displayName.ToUpper()} CLEARED</color>" + (state == RunState.Won && sys.index < GameDatabase.Get().systems.Count - 1 ? "  ·  next system unlocked" : "")
                    : $"{sys.displayName.ToUpper()}: threat cleared {clear * 100f:0}%";
                sysLine += "\n";
            }
            detailsText.text = $"{sysLine}Time {Format(gm.Elapsed)}   Kills {gm.Kills}\n{loot}";
        }

        static string Format(float s) => $"{(int)s / 60:00}:{(int)s % 60:00}";
    }
}
