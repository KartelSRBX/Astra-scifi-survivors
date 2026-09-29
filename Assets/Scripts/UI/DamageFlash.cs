using UnityEngine;
using UnityEngine.UI;
using GamingApp.Core;

namespace GamingApp.UI
{
    /// <summary>Red screen-edge flash and a small shake whenever the player loses hull.</summary>
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] Health playerHealth;
        [SerializeField] Image vignette;

        float last;
        float intensity;

        void Start()
        {
            last = playerHealth.Current;
            playerHealth.Changed += OnChanged;
        }

        void OnDestroy() => playerHealth.Changed -= OnChanged;

        void OnChanged(float current, float max)
        {
            if (current < last - 0.01f)
            {
                float frac = (last - current) / Mathf.Max(1f, max);
                intensity = Mathf.Clamp01(intensity + 0.35f + frac * 3f);
                CameraShake.Add(0.15f + frac * 2f);
            }
            last = current;
        }

        void Update()
        {
            intensity = Mathf.Max(0f, intensity - Time.unscaledDeltaTime * 2.5f);
            var c = vignette.color;
            c.a = intensity * 0.75f;
            vignette.color = c;
        }
    }
}
