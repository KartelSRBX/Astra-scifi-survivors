using UnityEngine;
using GamingApp.Player;

namespace GamingApp.UI
{
    /// <summary>Draws the floating joystick ring and knob under the player's finger.</summary>
    public class JoystickView : MonoBehaviour
    {
        [SerializeField] PlayerController controller;
        [SerializeField] RectTransform ring;
        [SerializeField] RectTransform knob;

        RectTransform canvasRect;
        Canvas canvas;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>().rootCanvas;
            canvasRect = (RectTransform)canvas.transform;
        }

        void Update()
        {
            bool on = controller.IsDragging;
            ring.gameObject.SetActive(on);
            knob.gameObject.SetActive(on);
            if (!on) return;

            float diameter = controller.JoystickRadiusPixels * 2f / canvas.scaleFactor;
            ring.sizeDelta = new Vector2(diameter, diameter);
            knob.sizeDelta = new Vector2(diameter * 0.4f, diameter * 0.4f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, controller.DragOriginScreen, null, out var o);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, controller.DragCurrentScreen, null, out var c);
            ring.anchoredPosition = o;
            knob.anchoredPosition = c;
        }
    }
}
