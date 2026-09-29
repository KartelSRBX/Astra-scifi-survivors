using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Player;
using GamingApp.UI;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>Builds HUD, joystick, pause, level-up and result UI with legacy uGUI (no TMP setup required).</summary>
    static class UiBuilder
    {
        const string UiDir = "Assets/Art/UI";

        static Font font;
        static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        // ---- Scene-level entry points ----
        public static RectTransform CreateCanvas(out GameObject canvasGo)
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            canvasGo = new GameObject("UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            return (RectTransform)canvasGo.transform;
        }

        public static void Build(PlayerController player, Health playerHealth, PlayerExperience xp, UpgradeManager upgrades)
        {
            var rt = CreateCanvas(out var canvasGo);
            BuildJoystick(rt, canvasGo, player);
            BuildHud(rt, canvasGo, playerHealth, xp);
            BuildPause(rt, canvasGo);
            BuildLevelUp(rt, canvasGo, upgrades);
            BuildResult(rt, canvasGo);
        }

        // ---- HUD ----
        static void BuildHud(RectTransform parent, GameObject host, Health hp, PlayerExperience xp)
        {
            var hud = host.AddComponent<Hud>();
            var timer = MakeText(parent, "Timer", "10:00", 72, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(400, 100));
            timer.fontStyle = FontStyle.Bold;
            var level = MakeText(parent, "Level", "Lv 1", 48, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(40, -120), new Vector2(300, 70));
            level.color = UiTheme.Teal;
            level.fontStyle = FontStyle.Bold;
            var hpBar = Bar(parent, "HealthBar", UiTheme.Teal, new Vector2(0f, 1f), new Vector2(40, -60), new Vector2(420, 36));
            var xpBar = Bar(parent, "XpBar", UiTheme.Violet, new Vector2(0.5f, 0f), new Vector2(0, 40), new Vector2(1000, 28));

            BuilderUtil.SetRef(hud, "timerText", timer);
            BuilderUtil.SetRef(hud, "levelText", level);
            BuilderUtil.SetRef(hud, "healthBar", hpBar);
            BuilderUtil.SetRef(hud, "xpBar", xpBar);
            BuilderUtil.SetRef(hud, "playerHealth", hp);
            BuilderUtil.SetRef(hud, "experience", xp);
        }

        // ---- Joystick ----
        static void BuildJoystick(RectTransform parent, GameObject host, PlayerController player)
        {
            var ring = MakeImage(parent, "JoystickRing", MakeCircle("Ring", true), new Color(UiTheme.Teal.r, UiTheme.Teal.g, UiTheme.Teal.b, 0.55f), new Vector2(200, 200));
            var knob = MakeImage(parent, "JoystickKnob", MakeCircle("Disc", false), new Color(UiTheme.White.r, UiTheme.White.g, UiTheme.White.b, 0.6f), new Vector2(80, 80));
            ring.gameObject.SetActive(false);
            knob.gameObject.SetActive(false);

            var view = host.AddComponent<JoystickView>();
            BuilderUtil.SetRef(view, "controller", player);
            BuilderUtil.SetRef(view, "ring", ring);
            BuilderUtil.SetRef(view, "knob", knob);
        }

        // ---- Pause ----
        static void BuildPause(RectTransform parent, GameObject host)
        {
            var pauseBtn = MakeButton(parent, "PauseButton", new Vector2(1f, 1f), new Vector2(-40, -40), new Vector2(120, 120), UiTheme.PanelLight, "II", 56);
            ((RectTransform)pauseBtn.transform).pivot = new Vector2(1f, 1f);

            var panel = Panel(parent, "PausePanel", UiTheme.Overlay);
            var prt = (RectTransform)panel.transform;
            var title = MakeText(prt, "Title", "PAUSED", 96, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 500), new Vector2(900, 140));
            title.fontStyle = FontStyle.Bold;
            var resume = MakeButton(prt, "Resume", new Vector2(0.5f, 0.5f), new Vector2(0, 250), new Vector2(640, 150), UiTheme.Violet, "RESUME", 60);
            AddVolumeSlider(prt, new Vector2(0, -20));
            var menu = MakeButton(prt, "Menu", new Vector2(0.5f, 0.5f), new Vector2(0, -280), new Vector2(640, 150), UiTheme.PanelLight, "MENU", 60);

            var pp = host.AddComponent<PausePanel>();
            BuilderUtil.SetRef(pp, "root", panel);
            BuilderUtil.SetRef(pp, "pauseButton", pauseBtn);
            BuilderUtil.SetRef(pp, "resumeButton", resume);
            BuilderUtil.SetRef(pp, "menuButton", menu);
            AddPopIn(panel);
            panel.SetActive(false);
        }

        // ---- Level-up ----
        static void BuildLevelUp(RectTransform parent, GameObject host, UpgradeManager upgrades)
        {
            var panel = Panel(parent, "LevelUpPanel", UiTheme.Overlay);
            var title = MakeText((RectTransform)panel.transform, "Title", "LEVEL UP!", 88, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -260), new Vector2(900, 120));
            title.color = UiTheme.Teal;
            title.fontStyle = FontStyle.Bold;

            var buttons = new Object[3];
            var titles = new Object[3];
            var descs = new Object[3];
            for (int i = 0; i < 3; i++)
            {
                var btn = MakeButton((RectTransform)panel.transform, $"Card{i}", new Vector2(0.5f, 0.5f),
                    new Vector2(0, 260 - i * 330), new Vector2(880, 290), UiTheme.Panel, null, 0);
                var accent = MakeImage((RectTransform)btn.transform, "Accent", null, i == 1 ? UiTheme.Violet : (i == 2 ? UiTheme.Blue : UiTheme.Teal), new Vector2(14, 0));
                accent.anchorMin = new Vector2(0f, 0f); accent.anchorMax = new Vector2(0f, 1f);
                accent.pivot = new Vector2(0f, 0.5f); accent.anchoredPosition = new Vector2(10, 0); accent.sizeDelta = new Vector2(10, -80);
                var name = MakeText((RectTransform)btn.transform, "Name", "Upgrade", 54, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -70), new Vector2(820, 90));
                name.color = UiTheme.Teal; name.fontStyle = FontStyle.Bold;
                var desc = MakeText((RectTransform)btn.transform, "Desc", "Description", 38, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0, 90), new Vector2(820, 120));
                desc.color = UiTheme.SoftText;
                titles[i] = name;
                descs[i] = desc;
                buttons[i] = btn;
            }

            var lp = host.AddComponent<LevelUpPanel>();
            BuilderUtil.SetRef(lp, "root", panel);
            BuilderUtil.SetRef(lp, "upgrades", upgrades);
            BuilderUtil.SetRefArray(lp, "cardButtons", buttons);
            BuilderUtil.SetRefArray(lp, "cardTitles", titles);
            BuilderUtil.SetRefArray(lp, "cardDescriptions", descs);
            AddPopIn(panel);
            panel.SetActive(false);
        }

        // ---- Result ----
        static void BuildResult(RectTransform parent, GameObject host)
        {
            var panel = Panel(parent, "ResultPanel", UiTheme.Overlay);
            var prt = (RectTransform)panel.transform;
            var title = MakeText(prt, "Title", "Result", 104, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 380), new Vector2(900, 140));
            title.fontStyle = FontStyle.Bold;
            var details = MakeText(prt, "Details", "", 60, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 130), new Vector2(900, 240));
            var restart = MakeButton(prt, "Restart", new Vector2(0.5f, 0.5f), new Vector2(0, -160), new Vector2(640, 150), UiTheme.Violet, "PLAY AGAIN", 60);
            var menu = MakeButton(prt, "Menu", new Vector2(0.5f, 0.5f), new Vector2(0, -350), new Vector2(640, 150), UiTheme.PanelLight, "MENU", 60);

            var rp = host.AddComponent<ResultPanel>();
            BuilderUtil.SetRef(rp, "root", panel);
            BuilderUtil.SetRef(rp, "titleText", title);
            BuilderUtil.SetRef(rp, "detailsText", details);
            BuilderUtil.SetRef(rp, "restartButton", restart);
            BuilderUtil.SetRef(rp, "menuButton", menu);
            AddPopIn(panel);
            panel.SetActive(false);
        }

        static void AddPopIn(GameObject panel)
        {
            panel.AddComponent<CanvasGroup>();
            panel.AddComponent<PopIn>();
        }

        // ---- Shared widgets ----
        public static void AddVolumeSlider(RectTransform parent, Vector2 pos)
        {
            var group = new GameObject("Volume", typeof(RectTransform));
            var grt = (RectTransform)group.transform;
            grt.SetParent(parent, false);
            grt.anchorMin = grt.anchorMax = new Vector2(0.5f, 0.5f);
            grt.anchoredPosition = pos;
            grt.sizeDelta = new Vector2(760, 200);

            var label = MakeText(grt, "Label", "Volume 100%", 50, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(760, 70));
            var slider = MakeSlider(grt, "Slider", new Vector2(0.5f, 0f), new Vector2(0, 60), new Vector2(720, 70));

            var vs = group.AddComponent<VolumeSlider>();
            BuilderUtil.SetRef(vs, "slider", slider);
            BuilderUtil.SetRef(vs, "label", label);
        }

        public static GameObject Panel(RectTransform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return go;
        }

        public static Text MakeText(RectTransform parent, string name, string content, int size, TextAnchor align,
            Vector2 anchor, Vector2 pos, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;
            var t = go.GetComponent<Text>();
            t.font = Font; t.text = content; t.fontSize = size; t.alignment = align; t.color = UiTheme.White;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.raycastTarget = false;
            return t;
        }

        public static RectTransform MakeImage(RectTransform parent, string name, Sprite sprite, Color color, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = dims;
            var img = go.GetComponent<Image>();
            img.sprite = sprite; img.color = color; img.raycastTarget = false;
            return rt;
        }

        public static Button MakeButton(RectTransform parent, string name, Vector2 anchor, Vector2 pos, Vector2 dims,
            Color color, string label, int labelSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;
            var img = go.GetComponent<Image>();
            img.color = color;
            img.sprite = UiSprites.Get("rr");
            img.type = Image.Type.Sliced;
            go.AddComponent<ButtonPress>();
            if (!string.IsNullOrEmpty(label))
            {
                var t = MakeText(rt, "Label", label, labelSize, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, dims - new Vector2(20, 10));
                t.fontStyle = FontStyle.Bold;
            }
            return go.GetComponent<Button>();
        }

        static Slider Bar(RectTransform parent, string name, Color fillColor, Vector2 anchor, Vector2 pos, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Slider));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(anchor.x == 0f ? 0f : 0.5f, anchor.y);
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;
            var bg = go.GetComponent<Image>();
            bg.color = UiTheme.BarBackground; bg.raycastTarget = false;

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            var frt = (RectTransform)fill.transform;
            frt.SetParent(rt, false);
            frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
            frt.offsetMin = frt.offsetMax = Vector2.zero;
            var fimg = fill.GetComponent<Image>();
            fimg.color = fillColor; fimg.raycastTarget = false;

            var s = go.GetComponent<Slider>();
            s.transition = Selectable.Transition.None;
            s.interactable = false;
            s.fillRect = frt;
            s.direction = Slider.Direction.LeftToRight;
            s.minValue = 0f; s.maxValue = 1f; s.value = 1f;
            return s;
        }

        static Slider MakeSlider(RectTransform parent, string name, Vector2 anchor, Vector2 pos, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Slider));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;

            var bg = Child(rt, "Background", typeof(Image));
            Stretch(bg, new Vector2(0f, 0.3f), new Vector2(1f, 0.7f), Vector2.zero, Vector2.zero);
            bg.GetComponent<Image>().color = UiTheme.PanelLight;

            var fillArea = Child(rt, "Fill Area", null);
            Stretch(fillArea, new Vector2(0f, 0.3f), new Vector2(1f, 0.7f), new Vector2(10, 0), new Vector2(-10, 0));
            var fill = Child(fillArea, "Fill", typeof(Image));
            Stretch(fill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            fill.GetComponent<Image>().color = UiTheme.Teal;

            var handleArea = Child(rt, "Handle Slide Area", null);
            Stretch(handleArea, Vector2.zero, Vector2.one, new Vector2(35, 0), new Vector2(-35, 0));
            var handle = Child(handleArea, "Handle", typeof(Image));
            handle.anchorMin = new Vector2(0f, 0f); handle.anchorMax = new Vector2(0f, 1f);
            handle.sizeDelta = new Vector2(70, 0);
            var himg = handle.GetComponent<Image>();
            himg.sprite = MakeCircle("Disc", false);
            himg.color = UiTheme.White;

            var s = go.GetComponent<Slider>();
            s.fillRect = fill;
            s.handleRect = handle;
            s.targetGraphic = himg;
            s.direction = Slider.Direction.LeftToRight;
            s.minValue = 0f; s.maxValue = 1f; s.value = 1f;
            return s;
        }

        static RectTransform Child(RectTransform parent, string name, System.Type graphic)
        {
            var go = graphic == null ? new GameObject(name, typeof(RectTransform)) : new GameObject(name, typeof(RectTransform), graphic);
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        static void Stretch(RectTransform rt, Vector2 min, Vector2 max, Vector2 offMin, Vector2 offMax)
        {
            rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = offMin; rt.offsetMax = offMax;
        }

        // ---- Generated sprites ----
        public static Sprite MakeCircle(string name, bool ring)
        {
            BuilderUtil.EnsureDir(UiDir);
            string path = $"{UiDir}/{name}.png";
            if (!File.Exists(path))
            {
                const int n = 128;
                var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                        float a = Mathf.Clamp01((1f - d) * (n / 2f) / 1.5f);
                        if (ring) a = Mathf.Min(a, Mathf.Clamp01((d - 0.9f) * (n / 2f) / 1.5f));
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
            }
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
