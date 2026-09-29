using UnityEngine;
using UnityEngine.UI;

namespace GamingApp.UI
{
    /// <summary>Big centred announcement (system intro, boss warning) that fades in and out on its own.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Banner : MonoBehaviour
    {
        [SerializeField] Text titleText;
        [SerializeField] Text subtitleText;
        [SerializeField] Image line;
        [SerializeField] float hold = 1.8f;

        static Banner instance;
        CanvasGroup group;
        RectTransform rt;
        float t = -1f;

        void Awake()
        {
            instance = this;
            group = GetComponent<CanvasGroup>();
            rt = (RectTransform)transform;
            group.alpha = 0f;
        }

        void OnDestroy() { if (instance == this) instance = null; }

        public static void Show(string title, string subtitle, Color color)
        {
            if (instance == null) return;
            instance.titleText.text = title;
            instance.titleText.color = color;
            instance.subtitleText.text = subtitle;
            var c = color; c.a = 0.9f;
            instance.line.color = c;
            instance.t = 0f;
        }

        void Update()
        {
            if (t < 0f) return;
            t += Time.unscaledDeltaTime;
            const float fade = 0.35f;
            float total = fade * 2f + hold;
            float a = t < fade ? t / fade : t > fade + hold ? 1f - (t - fade - hold) / fade : 1f;
            group.alpha = Mathf.Clamp01(a);
            rt.localScale = Vector3.one * Mathf.Lerp(1.15f, 1f, Mathf.Clamp01(t / fade));
            if (t >= total) { t = -1f; group.alpha = 0f; }
        }
    }
}
