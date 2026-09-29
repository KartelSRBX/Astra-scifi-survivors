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
            if (existing != null) return existing;

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
