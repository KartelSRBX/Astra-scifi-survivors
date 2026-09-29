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

            var title = UiBuilder.MakeText(rt, "Title", "SCI-FI\nSURVIVORS", 130, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 420), new Vector2(1000, 340));
            title.fontStyle = FontStyle.Bold;
            var accent = UiBuilder.MakeImage(rt, "TitleAccent", null, UiTheme.Teal, new Vector2(420, 10));
            accent.anchoredPosition = new Vector2(0, 220);

            var best = UiBuilder.MakeText(rt, "Best", "", 52, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 130), new Vector2(800, 80));
            best.color = UiTheme.SoftText;

            var biomass = Currency(rt, "Biomass", UiTheme.Teal, new Vector2(-40, -50));
            var cores = Currency(rt, "Cores", UiTheme.Violet, new Vector2(-40, -170));

            var settings = UiBuilder.MakeButton(rt, "Settings", new Vector2(0f, 1f), new Vector2(40, -50), new Vector2(130, 130), UiTheme.PanelLight, null, 0);
            settings.GetComponent<Image>().sprite = UiBuilder.MakeCircle("Disc", false);
            var sg = UiBuilder.MakeText((RectTransform)settings.transform, "Glyph", "SET", 34, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(120, 60));
            sg.fontStyle = FontStyle.Bold;
            sg.color = UiTheme.Teal;

            string[] names = { "CHARACTER", "INVENTORY", "START", "UPGRADES", "FORGE" };
            string[] glyphs = { "C", "I", ">", "+", "F" };
            float[] xs = { -360, -180, 0, 180, 360 };
            var nav = new Object[4];
            var titles = new string[4];
            Button play = null;
            int ni = 0;
            for (int i = 0; i < 5; i++)
            {
                bool isPlay = i == 2;
                float size = isPlay ? 190 : 140;
                var btn = UiBuilder.MakeButton(rt, "Nav_" + names[i], new Vector2(0.5f, 0f), new Vector2(xs[i], isPlay ? 240 : 220), new Vector2(size, size),
                    isPlay ? UiTheme.Violet : UiTheme.PanelLight, null, 0);
                btn.GetComponent<Image>().sprite = UiBuilder.MakeCircle("Disc", false);
                var g = UiBuilder.MakeText((RectTransform)btn.transform, "Glyph", glyphs[i], isPlay ? 90 : 64, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(size, size));
                g.fontStyle = FontStyle.Bold;
                g.color = isPlay ? UiTheme.White : UiTheme.Teal;
                var lbl = UiBuilder.MakeText(rt, "Label_" + names[i], names[i], isPlay ? 34 : 26, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(xs[i], isPlay ? 110 : 120), new Vector2(180, 44));
                lbl.color = UiTheme.SoftText;
                if (isPlay) play = btn;
                else { nav[ni] = btn; titles[ni] = names[i]; ni++; }
            }

            var panel = UiBuilder.Panel(rt, "SettingsPanel", UiTheme.Overlay);
            var prt = (RectTransform)panel.transform;
            var stitle = UiBuilder.MakeText(prt, "Title", "SETTINGS", 96, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 420), new Vector2(900, 140));
            stitle.fontStyle = FontStyle.Bold;
            UiBuilder.AddVolumeSlider(prt, new Vector2(0, 150));
            var back = UiBuilder.MakeButton(prt, "Back", new Vector2(0.5f, 0.5f), new Vector2(0, -150), new Vector2(640, 150), UiTheme.Violet, "BACK", 60);

            var ph = UiBuilder.Panel(rt, "PlaceholderPanel", UiTheme.Overlay);
            var phrt = (RectTransform)ph.transform;
            var phTitle = UiBuilder.MakeText(phrt, "Title", "", 88, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 250), new Vector2(950, 300));
            phTitle.fontStyle = FontStyle.Bold;
            var phClose = UiBuilder.MakeButton(phrt, "Close", new Vector2(0.5f, 0.5f), new Vector2(0, -150), new Vector2(640, 150), UiTheme.PanelLight, "BACK", 60);

            var mc = host.AddComponent<MenuController>();
            BuilderUtil.SetRef(mc, "playButton", play);
            BuilderUtil.SetRef(mc, "settingsButton", settings);
            BuilderUtil.SetRef(mc, "closeSettingsButton", back);
            BuilderUtil.SetRef(mc, "settingsPanel", panel);
            BuilderUtil.SetRef(mc, "bestText", best);
            BuilderUtil.SetRef(mc, "biomassText", biomass);
            BuilderUtil.SetRef(mc, "coresText", cores);
            BuilderUtil.SetRefArray(mc, "navButtons", nav);
            var so = new SerializedObject(mc);
            var tp = so.FindProperty("navTitles");
            tp.arraySize = titles.Length;
            for (int i = 0; i < titles.Length; i++) tp.GetArrayElementAtIndex(i).stringValue = titles[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            BuilderUtil.SetRef(mc, "placeholderPanel", ph);
            BuilderUtil.SetRef(mc, "placeholderTitle", phTitle);
            BuilderUtil.SetRef(mc, "placeholderClose", phClose);
            panel.SetActive(false);
            ph.SetActive(false);

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        static Text Currency(RectTransform rt, string name, Color color, Vector2 pos)
        {
            var pill = UiBuilder.MakeImage(rt, "Currency_" + name, null, UiTheme.Panel, new Vector2(360, 100));
            pill.anchorMin = pill.anchorMax = pill.pivot = new Vector2(1f, 1f);
            pill.anchoredPosition = pos;
            var dot = UiBuilder.MakeImage(pill, "Icon", UiBuilder.MakeCircle("Disc", false), color, new Vector2(60, 60));
            dot.anchoredPosition = new Vector2(-130, 0);
            var t = UiBuilder.MakeText(pill, "Amount", "0", 52, TextAnchor.MiddleRight, new Vector2(1f, 0.5f), new Vector2(-24, 0), new Vector2(240, 80));
            t.fontStyle = FontStyle.Bold;
            return t;
        }
    }
}
