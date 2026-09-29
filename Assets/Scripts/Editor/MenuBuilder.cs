using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.UI;

namespace GamingApp.EditorTools
{
    /// <summary>Builds and saves the main menu scene.</summary>
    static class MenuBuilder
    {
        public static void Build(string scenePath)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camGo.tag = "MainCamera";
            var cam = camGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = UiTheme.Background;

            var rt = UiBuilder.CreateCanvas(out var host);

            var bgPanel = UiBuilder.Panel(rt, "Background", UiTheme.Background);
            bgPanel.GetComponent<Image>().raycastTarget = false;

            var title = UiBuilder.MakeText(rt, "Title", "SCI-FI\nSURVIVORS", 130, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 520), new Vector2(1000, 340));
            title.fontStyle = FontStyle.Bold;
            title.color = UiTheme.White;
            var accent = UiBuilder.MakeImage(rt, "TitleAccent", null, UiTheme.Teal, new Vector2(420, 10));
            accent.anchoredPosition = new Vector2(0, 320);

            var best = UiBuilder.MakeText(rt, "Best", "", 52, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 200), new Vector2(800, 80));
            best.color = UiTheme.SoftText;

            var play = UiBuilder.MakeButton(rt, "Play", new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(680, 170), UiTheme.Violet, "PLAY", 76);
            var settings = UiBuilder.MakeButton(rt, "Settings", new Vector2(0.5f, 0.5f), new Vector2(0, -240), new Vector2(680, 140), UiTheme.PanelLight, "SETTINGS", 56);

            var panel = UiBuilder.Panel(rt, "SettingsPanel", UiTheme.Overlay);
            var prt = (RectTransform)panel.transform;
            var stitle = UiBuilder.MakeText(prt, "Title", "SETTINGS", 96, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 420), new Vector2(900, 140));
            stitle.fontStyle = FontStyle.Bold;
            UiBuilder.AddVolumeSlider(prt, new Vector2(0, 150));
            var back = UiBuilder.MakeButton(prt, "Back", new Vector2(0.5f, 0.5f), new Vector2(0, -150), new Vector2(640, 150), UiTheme.Violet, "BACK", 60);

            var mc = host.AddComponent<MenuController>();
            BuilderUtil.SetRef(mc, "playButton", play);
            BuilderUtil.SetRef(mc, "settingsButton", settings);
            BuilderUtil.SetRef(mc, "closeSettingsButton", back);
            BuilderUtil.SetRef(mc, "settingsPanel", panel);
            BuilderUtil.SetRef(mc, "bestText", best);
            panel.SetActive(false);

            EditorSceneManager.SaveScene(scene, scenePath);
        }
    }
}
