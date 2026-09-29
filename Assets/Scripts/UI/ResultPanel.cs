using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using GamingApp.Core;

namespace GamingApp.UI
{
    /// <summary>End-of-run screen: result, survival time, best time, restart.</summary>
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text detailsText;

        void Awake() => root.SetActive(false);
        void Start() => GameManager.Instance.StateChanged += OnStateChanged;
        void OnDestroy() { if (GameManager.Instance != null) GameManager.Instance.StateChanged -= OnStateChanged; }

        void OnStateChanged(RunState state)
        {
            if (state != RunState.Won && state != RunState.Lost) return;
            var gm = GameManager.Instance;
            root.SetActive(true);
            titleText.text = state == RunState.Won ? "Survived!" : "You died"; // TODO: localize
            detailsText.text = $"Time: {Format(gm.Elapsed)}\nBest: {Format(GameManager.BestTime)}";
        }

        // Hook to the Restart button's OnClick.
        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        static string Format(float s) => $"{(int)s / 60:00}:{(int)s % 60:00}";
    }
}
