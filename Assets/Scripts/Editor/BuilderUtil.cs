using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamingApp.EditorTools
{
    /// <summary>Small helpers shared by the test-scene builders.</summary>
    static class BuilderUtil
    {
        public const string PrefabDir = "Assets/Prefabs";
        public const string DataDir = "Assets/Data";
        public const string MaterialDir = "Assets/Art/Materials";
        public const string SceneDir = "Assets/Scenes";

        public static void EnsureDir(string assetPath)
        {
            if (!Directory.Exists(assetPath)) Directory.CreateDirectory(assetPath);
        }

        public static Material MakeMaterial(string name, Color color)
        {
            EnsureDir(MaterialDir);
            string path = $"{MaterialDir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                existing.color = color;
                if (existing.HasProperty("_BaseColor")) existing.SetColor("_BaseColor", color);
                EditorUtility.SetDirty(existing);
                return existing;
            }

            // Built-in pipeline by default; fall back to URP if the project uses it.
            bool srp = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null;
            Shader shader = Shader.Find(srp ? "Universal Render Pipeline/Lit" : "Standard");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            var mat = new Material(shader) { color = color };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        /// <summary>Gives a material a tiled grid texture so movement is visible on a flat ground.</summary>
        public static void ApplyGrid(Material mat, float tiles)
        {
            string path = $"{MaterialDir}/GroundGridDark.png";
            if (!File.Exists(path))
            {
                const int n = 64;
                var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
                var bg = new Color(0.05f, 0.06f, 0.14f);
                var line = new Color(0.16f, 0.14f, 0.36f);
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                        tex.SetPixel(x, y, (x < 2 || y < 2) ? line : bg);
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
            }
            var grid = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            mat.mainTexture = grid;
            mat.mainTextureScale = new Vector2(tiles, tiles);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", 0f);
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", grid);
            EditorUtility.SetDirty(mat);
        }

        /// <summary>Assigns a private [SerializeField] field by name.</summary>
        public static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogError($"[Builder] {target.GetType().Name} has no serialized field '{field}'"); return; }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFloat(Object target, string field, float value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogError($"[Builder] {target.GetType().Name} has no serialized field '{field}'"); return; }
            prop.floatValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetRefArray(Object target, string field, Object[] values)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogError($"[Builder] {target.GetType().Name} has no serialized field '{field}'"); return; }
            prop.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static T SavePrefab<T>(GameObject go, string name) where T : Component
        {
            EnsureDir(PrefabDir);
            string path = $"{PrefabDir}/{name}.prefab";
            var saved = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return saved.GetComponent<T>();
        }

        public static GameObject SavePrefab(GameObject go, string name)
        {
            EnsureDir(PrefabDir);
            var saved = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabDir}/{name}.prefab");
            Object.DestroyImmediate(go);
            return saved;
        }

        /// <summary>Primitive with its default collider swapped for a trigger, plus a kinematic Rigidbody.</summary>
        public static GameObject TriggerPrimitive(PrimitiveType type, string name, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            go.GetComponent<Collider>().isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            return go;
        }
    }
}
