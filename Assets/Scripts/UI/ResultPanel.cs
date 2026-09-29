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
            detailsText.text = $"Time {Format(gm.Elapsed)}   Best {Format(GameManager.BestTime)}\nKills {gm.Kills}\n{loot}";
        }

        static string Format(float s) => $"{(int)s / 60:00}:{(int)s % 60:00}";
    }
}
