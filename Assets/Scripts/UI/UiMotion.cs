using UnityEngine;
using UnityEngine.UI;

namespace GamingApp.UI
{
    /// <summary>Lightweight idle animation for menu decoration: spin, bob, pulse and alpha twinkle.</summary>
    public class UiMotion : MonoBehaviour
    {
        public float spin;
        public float bobAmplitude;
        public float bobSpeed = 1f;
        public float pulseAmplitude;
        public float pulseSpeed = 1f;
        public float alphaMin = 1f;
        public float alphaMax = 1f;
        public float alphaSpeed = 1f;
        public float phase;

        RectTransform rt;
        Graphic graphic;
        Vector2 basePos;
        Vector3 baseScale;
        float baseAlpha;

        void Awake()
        {
            rt = (RectTransform)transform;
            graphic = GetComponent<Graphic>();
            basePos = rt.anchoredPosition;
            baseScale = rt.localScale;
            if (graphic != null) baseAlpha = graphic.color.a;
        }

        void Update()
        {
            float t = Time.unscaledTime + phase;
            if (spin != 0f) rt.Rotate(0f, 0f, spin * Time.unscaledDeltaTime);
            if (bobAmplitude != 0f) rt.anchoredPosition = basePos + Vector2.up * (Mathf.Sin(t * bobSpeed) * bobAmplitude);
            if (pulseAmplitude != 0f) rt.localScale = baseScale * (1f + Mathf.Sin(t * pulseSpeed) * pulseAmplitude);
            if (graphic != null && alphaMin != alphaMax)
            {
                var c = graphic.color;
                c.a = baseAlpha * Mathf.Lerp(alphaMin, alphaMax, Mathf.Sin(t * alphaSpeed) * 0.5f + 0.5f);
                graphic.color = c;
            }
        }
    }
}
