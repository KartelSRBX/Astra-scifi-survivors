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

        /// <summary>Builds a test APK (not an .aab) and installs + launches it on the USB-connected phone.</summary>
        [MenuItem("Tools/GamingApp/Build and Run on Phone (APK)")]
        public static void BuildAndRunApk()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                EditorUtility.DisplayDialog("Android module missing",
                    "Install 'Android Build Support' (with OpenJDK and Android SDK & NDK Tools) for this Unity version in Unity Hub > Installs.", "OK");
                return;
            }
            Apply();
            bool wasBundle = EditorUserBuildSettings.buildAppBundle;
            EditorUserBuildSettings.buildAppBundle = false;
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Menu.unity", "Assets/Scenes/Main.unity" },
                locationPathName = "Builds/SciFiSurvivors-test.apk",
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.AutoRunPlayer | BuildOptions.Development,
            };
            var report = BuildPipeline.BuildPlayer(options);
            EditorUserBuildSettings.buildAppBundle = wasBundle;
            Debug.Log($"[GamingApp] APK build: {report.summary.result}, {report.summary.totalSize / (1024 * 1024)} MB -> {options.locationPathName}");
        }
    }
}
