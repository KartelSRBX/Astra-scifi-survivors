using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;

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
            detailsText.text = $"Time: {Format(gm.Elapsed)}\nBest: {Format(GameManager.BestTime)}";
        }

        static string Format(float s) => $"{(int)s / 60:00}:{(int)s % 60:00}";
    }
}
