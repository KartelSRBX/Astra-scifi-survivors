using UnityEngine;
using UnityEngine.EventSystems;

namespace GamingApp.UI
{
    /// <summary>Squash feedback while a button is held.</summary>
    public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float pressedScale = 0.9f;

        Vector3 baseScale;
        float target = 1f, current = 1f;

        void Awake() => baseScale = transform.localScale;

        public void OnPointerDown(PointerEventData e) => target = pressedScale;
        public void OnPointerUp(PointerEventData e) => target = 1f;
        public void OnPointerExit(PointerEventData e) => target = 1f;

        void OnDisable()
        {
            target = current = 1f;
            transform.localScale = baseScale;
        }

        void Update()
        {
            if (Mathf.Approximately(current, target)) return;
            current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-25f * Time.unscaledDeltaTime));
            if (Mathf.Abs(current - target) < 0.001f) current = target;
            transform.localScale = baseScale * current;
        }
    }
}
