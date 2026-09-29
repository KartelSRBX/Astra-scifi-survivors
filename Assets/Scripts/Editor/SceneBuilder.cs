using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using GamingApp.Core;
using GamingApp.Enemies;
using GamingApp.Player;
using GamingApp.UI;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>
    /// Tools > GamingApp > Build Test Scene: creates placeholder prefabs/data and a playable
    /// Assets/Scenes/Main.unity. Safe to re-run; it regenerates everything.
    /// </summary>
    public static class SceneBuilder
    {
        const string ScenePath = BuilderUtil.SceneDir + "/Main.unity";
        const string MenuPath = BuilderUtil.SceneDir + "/Menu.unity";
        const float TestRunSeconds = 300f; // 5 min for quick playtests; roadmap target is 10-15 min

        [MenuItem("Tools/GamingApp/Build Test Scene")]
        public static void BuildTestScene()
        {
            // The scene must exist before assets are built: NewScene unloads unreferenced asset objects,
            // which would turn every reference in `assets` into null.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var assets = AssetBuilder.BuildAll();
            BuilderUtil.EnsureDir(BuilderUtil.SceneDir);

            BuildWorld();
            var player = BuildPlayer(assets.PlayerProjectile, out var weapon);
            BuildCamera(player.transform);
            BuildManagers(player, weapon, assets, out var upgrades);
            UiBuilder.Build(player.GetComponent<PlayerController>(), player.GetComponent<Health>(),
                player.GetComponent<PlayerExperience>(), upgrades);

            EditorSceneManager.SaveScene(scene, ScenePath);

            MenuBuilder.Build(MenuPath, TestRunSeconds);
            SetBuildScenes(MenuPath, ScenePath);
            AndroidSettings.Apply();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!Application.isBatchMode) EditorSceneManager.OpenScene(MenuPath);
            Debug.Log("[GamingApp] Scenes built: Menu + Main. Open Menu and press Play.");
        }

        // Entry point for CLI: Unity -batchmode -quit -executeMethod GamingApp.EditorTools.SceneBuilder.BuildFromCommandLine
        public static void BuildFromCommandLine() => BuildTestScene();

        static void BuildWorld()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(40f, 1f, 40f); // 400x400 units
            var groundMat = BuilderUtil.MakeMaterial("Ground", new Color(0.05f, 0.06f, 0.14f));
            BuilderUtil.ApplyGrid(groundMat, 200f);
            ground.GetComponent<Renderer>().sharedMaterial = groundMat;
        }

        static GameObject BuildPlayer(Projectile projectile, out AutoWeapon weapon)
        {
            var go = new GameObject("Player");
            go.transform.position = new Vector3(0f, 1f, 0f);
            var visual = AssetBuilder.AddVisual(go, "ship_corvette", 2f, 1f, AssetBuilder.OrderPlayer);

            var cc = go.AddComponent<CharacterController>();
            cc.height = 2f; cc.radius = 0.5f; cc.center = Vector3.zero;

            go.AddComponent<Health>();
            go.AddComponent<PlayerHealth>();
            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerExperience>();
            weapon = go.AddComponent<AutoWeapon>();
            BuilderUtil.SetRef(weapon, "projectilePrefab", projectile);
            BuilderUtil.SetRef(go.AddComponent<PlayerLoadout>(), "visual", visual);
            return go;
        }

        static void BuildCamera(Transform target)
        {
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            go.tag = "MainCamera";
            var cam = go.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = UiTheme.Background;
            go.transform.position = target.position + new Vector3(0f, 18f, -6f);
            go.transform.LookAt(target.position);
            var follow = go.AddComponent<TopDownCamera>();
            BuilderUtil.SetRef(follow, "target", target);
        }

        static void BuildManagers(GameObject player, AutoWeapon weapon, AssetBuilder.Result assets, out UpgradeManager upgrades)
        {
            var gm = new GameObject("GameManager").AddComponent<GameManager>();
            BuilderUtil.SetFloat(gm, "runDurationSeconds", TestRunSeconds);

            var spawner = new GameObject("WaveSpawner").AddComponent<WaveSpawner>();
            BuilderUtil.SetRef(spawner, "player", player.transform);
            BuilderUtil.SetRefArray(spawner, "archetypes", assets.Enemies.ToArray());

            upgrades = new GameObject("UpgradeManager").AddComponent<UpgradeManager>();
            BuilderUtil.SetRefArray(upgrades, "pool", assets.Upgrades.ToArray());
            BuilderUtil.SetRef(upgrades, "weapon", weapon);
            BuilderUtil.SetRef(upgrades, "controller", player.GetComponent<PlayerController>());
            BuilderUtil.SetRef(upgrades, "playerHealth", player.GetComponent<Health>());
            BuilderUtil.SetRef(upgrades, "experience", player.GetComponent<PlayerExperience>());
        }

        static void SetBuildScenes(params string[] paths)
        {
            var scenes = new List<EditorBuildSettingsScene>();
            foreach (var p in paths) scenes.Add(new EditorBuildSettingsScene(p, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
