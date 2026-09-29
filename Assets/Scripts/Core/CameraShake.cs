using UnityEngine;

namespace GamingApp.Core
{
    /// <summary>Trauma-based screen shake read by the camera; call Add from anywhere.</summary>
    public static class CameraShake
    {
        static float trauma;

        public static void Add(float amount) => trauma = Mathf.Clamp01(trauma + amount);

        /// <summary>Returns this frame's offset and decays trauma. Uses unscaled time so it settles while paused.</summary>
        public static Vector3 Sample(float maxOffset)
        {
            if (trauma <= 0f) return Vector3.zero;
            float s = trauma * trauma * maxOffset;
            float t = Time.unscaledTime * 30f;
            var offset = new Vector3(Mathf.PerlinNoise(t, 0.3f) - 0.5f, 0f, Mathf.PerlinNoise(0.7f, t) - 0.5f) * (2f * s);
            trauma = Mathf.Max(0f, trauma - Time.unscaledDeltaTime * 1.6f);
            return offset;
        }
    }
}
