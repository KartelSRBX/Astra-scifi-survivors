using UnityEngine;

namespace GamingApp.Core
{
    /// <summary>Follows the target from above at a fixed offset (top-down view, slight tilt).</summary>
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new(0f, 18f, -6f);
        [SerializeField] float smoothing = 8f;

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));
            transform.LookAt(target.position);
        }
    }
}
