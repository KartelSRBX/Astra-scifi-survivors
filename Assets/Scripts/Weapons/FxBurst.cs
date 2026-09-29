using UnityEngine;

namespace GamingApp.Weapons
{
    /// <summary>Flat sprite flash that expands and fades out (explosions, hit sparks).</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FxBurst : MonoBehaviour
    {
        public float duration = 0.35f;
        public float startScale = 0.35f;

        SpriteRenderer sr;
        Color color;
        Vector3 fullScale;
        float t;

        public static void Spawn(FxBurst prefab, Vector3 pos, float worldSize, Color color)
        {
            if (prefab == null) return;
            pos.y = 0.15f;
            var fx = Instantiate(prefab, pos, prefab.transform.rotation);
            var sprite = fx.GetComponent<SpriteRenderer>().sprite;
            fx.fullScale = Vector3.one * (worldSize * sprite.pixelsPerUnit / sprite.rect.width);
            fx.color = color;
            fx.Apply(0f);
        }

        void Awake() => sr = GetComponent<SpriteRenderer>();

        void Update()
        {
            t += Time.deltaTime / duration;
            if (t >= 1f) { Destroy(gameObject); return; }
            Apply(t);
        }

        void Apply(float k)
        {
            float e = 1f - (1f - k) * (1f - k);
            transform.localScale = fullScale * Mathf.Lerp(startScale, 1f, e);
            var c = color;
            c.a *= 1f - k * k;
            sr.color = c;
        }
    }
}
