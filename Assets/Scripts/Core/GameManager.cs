using System;
using UnityEngine;
using GamingApp.Meta;

namespace GamingApp.Core
{
    public enum RunState { Playing, LevelUp, Paused, Won, Lost }

    /// <summary>Owns the run timer and the high-level run state.</summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] float runDurationSeconds = 600f; // 10 min; roadmap allows 10-15

        public RunState State { get; private set; } = RunState.Playing;
        public float Elapsed { get; private set; }
        public float Remaining => Mathf.Max(0f, runDurationSeconds - Elapsed);
        /// <summary>0..1, grows over the run; drives wave difficulty.</summary>
        public float Progress => Mathf.Clamp01(Elapsed / runDurationSeconds);

        public int Kills { get; private set; }
        public int BossKills { get; private set; }

        public event Action<RunState> StateChanged;

        const string BestTimeKey = "best_survival_time";
        public static float BestTime => PlayerPrefs.GetFloat(BestTimeKey, 0f);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Time.timeScale = 1f;
        }

        void Update()
        {
            if (State != RunState.Playing) return;
            Elapsed += Time.deltaTime;
            if (Elapsed >= runDurationSeconds) SetState(RunState.Won);
        }

        public void SetState(RunState next)
        {
            if (State == next) return;
            State = next;
            Time.timeScale = next == RunState.Playing ? 1f : 0f;
            if (next == RunState.Won || next == RunState.Lost)
            {
                SaveBestTime();
                bool won = next == RunState.Won;
                Profile.RecordSystem(Weapons.RunSetup.SystemIndex, won ? 1f : Progress, Elapsed);
                RunRewards.Grant(Kills, BossKills, Elapsed, won);
            }
            StateChanged?.Invoke(next);
        }

        public void RegisterKill(bool boss)
        {
            Kills++;
            if (boss) BossKills++;
        }

        public void PlayerDied() => SetState(RunState.Lost);

        public void Pause() { if (State == RunState.Playing) SetState(RunState.Paused); }
        public void Resume() { if (State == RunState.Paused) SetState(RunState.Playing); }

        void SaveBestTime()
        {
            if (Elapsed > BestTime)
            {
                PlayerPrefs.SetFloat(BestTimeKey, Elapsed);
                PlayerPrefs.Save();
            }
        }
    }
}
