using UnityEngine;

namespace GamingApp.Player
{
    /// <summary>
    /// Top-down drag movement on the XZ plane. The finger acts as a floating joystick:
    /// the touch-down point is the origin, dragging away from it sets direction.
    /// Mouse is used as a fallback in the editor.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float joystickRadiusPixels = 120f;

        CharacterController controller;
        Vector2 dragOrigin;
        bool dragging;

        public Vector3 MoveDirection { get; private set; }
        public float SpeedMultiplier { get; set; } = 1f;

        void Awake() => controller = GetComponent<CharacterController>();

        void Update()
        {
            Vector2 input = ReadInput();
            MoveDirection = new Vector3(input.x, 0f, input.y);
            if (MoveDirection.sqrMagnitude > 0.0001f)
            {
                controller.Move(MoveDirection * (moveSpeed * SpeedMultiplier * Time.deltaTime));
                transform.rotation = Quaternion.LookRotation(MoveDirection, Vector3.up);
            }
        }

        Vector2 ReadInput()
        {
            Vector2 pos;
            bool down, held;

            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                pos = t.position;
                down = t.phase == TouchPhase.Began;
                held = t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled;
            }
            else
            {
                pos = Input.mousePosition;
                down = Input.GetMouseButtonDown(0);
                held = Input.GetMouseButton(0);
            }

            if (down) { dragOrigin = pos; dragging = true; }
            if (!held) { dragging = false; return Vector2.zero; }
            if (!dragging) return Vector2.zero;

            Vector2 delta = pos - dragOrigin;
            return Vector2.ClampMagnitude(delta / joystickRadiusPixels, 1f);
        }
    }
}
