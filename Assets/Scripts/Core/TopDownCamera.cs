using UnityEngine;

namespace GamingApp.Core
{
    /// <summary>Follows the target from above at a fixed offset (top-down view, slight tilt), with screen shake.</summary>
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new(0f, 18f, -6f);
        [SerializeField] float smoothing = 8f;
        [SerializeField] float shakeOffset = 0.6f;

        Vector3 basePos;
        bool started;

        void LateUpdate()
        {
            if (target == null) return;
            if (!started) { basePos = transform.position; started = true; }
            Vector3 desired = target.position + offset;
            basePos = Vector3.Lerp(basePos, desired, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));
            Vector3 shake = CameraShake.Sample(shakeOffset);
            transform.position = basePos + shake;
            transform.LookAt(target.position + shake * 0.5f);
        }
    }
}
