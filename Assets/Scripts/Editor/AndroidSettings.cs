using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace GamingApp.EditorTools
{
    /// <summary>Baseline Android player settings from the roadmap (portrait, IL2CPP/ARM64, target API 36).</summary>
    public static class AndroidSettings
    {
        [MenuItem("Tools/GamingApp/Apply Android Settings")]
        public static void Apply()
        {
            try
            {
                PlayerSettings.companyName = "GamingApp";
                PlayerSettings.productName = "SciFi Survivors";
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.gamingapp.scifisurvivors"); // placeholder, final id is permanent once published
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36; // Google Play requirement for new apps
                EditorUserBuildSettings.buildAppBundle = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[GamingApp] Could not apply all Android settings (set them manually in Player Settings): " + e.Message);
            }
        }
    }
}
