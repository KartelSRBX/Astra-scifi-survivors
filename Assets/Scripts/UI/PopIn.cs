using UnityEngine;

namespace GamingApp.UI
{
    /// <summary>Fades a panel in (and optionally springs its content) each time it is enabled. Works while paused.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class PopIn : MonoBehaviour
    {
        public RectTransform content;
        public float duration = 0.22f;

        CanvasGroup group;
        float t = 1f;

        void Awake() => group = GetComponent<CanvasGroup>();

        void OnEnable()
        {
            t = 0f;
            Apply(0f);
        }

        void Update()
        {
            if (t >= 1f) return;
            t = Mathf.Min(1f, t + Time.unscaledDeltaTime / duration);
            Apply(t);
        }

        void Apply(float k)
        {
            group.alpha = 1f - Mathf.Pow(1f - k, 3f);
            if (content != null) content.localScale = Vector3.one * Mathf.LerpUnclamped(0.85f, 1f, BackOut(k));
        }

        static float BackOut(float x)
        {
            const float c1 = 1.70158f, c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}
