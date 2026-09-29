using UnityEngine;

namespace GamingApp.UI
{
    /// <summary>Persistent player settings (currently master volume).</summary>
    public static class GameSettings
    {
        const string VolumeKey = "master_volume";

        public static float Volume
        {
            get => PlayerPrefs.GetFloat(VolumeKey, 1f);
            set
            {
                PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value));
                AudioListener.volume = Mathf.Clamp01(value);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Apply() => AudioListener.volume = Volume;
    }
}
