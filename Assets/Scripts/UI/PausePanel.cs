using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;

namespace GamingApp.UI
{
    /// <summary>Pause button and overlay (resume, volume, back to menu). Also auto-pauses when the app is backgrounded.</summary>
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Button pauseButton;
        [SerializeField] Button resumeButton;
        [SerializeField] Button menuButton;

        void Awake()
        {
            root.SetActive(false);
            pauseButton.onClick.AddListener(Pause);
            resumeButton.onClick.AddListener(Resume);
            menuButton.onClick.AddListener(() => { PlayerPrefs.Save(); SceneLoader.Load(SceneLoader.Menu); });
        }

        void Start() => GameManager.Instance.StateChanged += OnStateChanged;
        void OnDestroy() { if (GameManager.Instance != null) GameManager.Instance.StateChanged -= OnStateChanged; }

        void OnApplicationPause(bool paused) { if (paused) Pause(); }

        void Pause()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State != RunState.Playing) return;
            gm.Pause();
            root.SetActive(true);
        }

        void Resume()
        {
            root.SetActive(false);
            PlayerPrefs.Save();
            GameManager.Instance.Resume();
        }

        void OnStateChanged(RunState state)
        {
            pauseButton.gameObject.SetActive(state == RunState.Playing);
            if (state != RunState.Paused) root.SetActive(false);
        }
    }
}
