using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GamingApp.Core;
using GamingApp.Player;
using GamingApp.UI;
using GamingApp.Weapons;

namespace GamingApp.EditorTools
{
    /// <summary>Builds the HUD, level-up and result UI with legacy uGUI (no TMP setup required).</summary>
    static class UiBuilder
    {
        static Font font;
        static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static void Build(Health playerHealth, PlayerExperience xp, UpgradeManager upgrades)
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                es.transform.SetAsLastSibling();
            }

            var canvasGo = new GameObject("UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            var rt = (RectTransform)canvasGo.transform;

            BuildHud(rt, canvasGo, playerHealth, xp);
            BuildLevelUp(rt, canvasGo, upgrades);
            BuildResult(rt, canvasGo);
        }

        // ---- HUD ----
        static void BuildHud(RectTransform parent, GameObject host, Health hp, PlayerExperience xp)
        {
            var hud = host.AddComponent<Hud>();
            var timer = MakeText(parent, "Timer", "10:00", 72, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(400, 100));
            var level = MakeText(parent, "Level", "Lv 1", 48, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(40, -120), new Vector2(300, 70));
            var hpBar = Bar(parent, "HealthBar", new Color(0.9f, 0.2f, 0.2f), new Vector2(0f, 1f), new Vector2(40, -70), new Vector2(420, 36));
            var xpBar = Bar(parent, "XpBar", new Color(0.2f, 0.6f, 1f), new Vector2(0.5f, 0f), new Vector2(0, 40), new Vector2(1000, 28));

            BuilderUtil.SetRef(hud, "timerText", timer);
            BuilderUtil.SetRef(hud, "levelText", level);
            BuilderUtil.SetRef(hud, "healthBar", hpBar);
            BuilderUtil.SetRef(hud, "xpBar", xpBar);
            BuilderUtil.SetRef(hud, "playerHealth", hp);
            BuilderUtil.SetRef(hud, "experience", xp);
        }

        // ---- Level-up ----
        static void BuildLevelUp(RectTransform parent, GameObject host, UpgradeManager upgrades)
        {
            var panel = Panel(parent, "LevelUpPanel", new Color(0f, 0f, 0f, 0.75f));
            MakeText((RectTransform)panel.transform, "Title", "LEVEL UP!", 80, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -260), new Vector2(900, 120));

            var buttons = new Object[3];
            var titles = new Object[3];
            var descs = new Object[3];
            for (int i = 0; i < 3; i++)
            {
                var btn = MakeButton((RectTransform)panel.transform, $"Card{i}", new Vector2(0.5f, 0.5f),
                    new Vector2(0, 260 - i * 330), new Vector2(880, 290));
                titles[i] = MakeText((RectTransform)btn.transform, "Name", "Upgrade", 54, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0, -70), new Vector2(840, 90));
                descs[i] = MakeText((RectTransform)btn.transform, "Desc", "Description", 38, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0, 90), new Vector2(840, 120));
                buttons[i] = btn;
            }

            var lp = host.AddComponent<LevelUpPanel>();
            BuilderUtil.SetRef(lp, "root", panel);
            BuilderUtil.SetRef(lp, "upgrades", upgrades);
            BuilderUtil.SetRefArray(lp, "cardButtons", buttons);
            BuilderUtil.SetRefArray(lp, "cardTitles", titles);
            BuilderUtil.SetRefArray(lp, "cardDescriptions", descs);
            panel.SetActive(false); // scene starts hidden; script also hides it in Awake
        }

        // ---- Result ----
        static void BuildResult(RectTransform parent, GameObject host)
        {
            var panel = Panel(parent, "ResultPanel", new Color(0f, 0f, 0f, 0.85f));
            var title = MakeText((RectTransform)panel.transform, "Title", "Result", 96, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 300), new Vector2(900, 140));
            var details = MakeText((RectTransform)panel.transform, "Details", "", 56, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(900, 240));
            var restart = MakeButton((RectTransform)panel.transform, "Restart", new Vector2(0.5f, 0.5f), new Vector2(0, -220), new Vector2(600, 160));
            MakeText((RectTransform)restart.transform, "Label", "Restart", 60, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560, 120));

            var rp = host.AddComponent<ResultPanel>();
            BuilderUtil.SetRef(rp, "root", panel);
            BuilderUtil.SetRef(rp, "titleText", title);
            BuilderUtil.SetRef(rp, "detailsText", details);
            UnityEventTools.AddPersistentListener(restart.onClick, rp.Restart);
            panel.SetActive(false);
        }

        // ---- widgets ----
        static GameObject Panel(RectTransform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return go;
        }

        static Text MakeText(RectTransform parent, string name, string content, int size, TextAnchor align,
            Vector2 anchor, Vector2 pos, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;
            var t = go.GetComponent<Text>();
            t.font = Font; t.text = content; t.fontSize = size; t.alignment = align; t.color = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.raycastTarget = false;
            return t;
        }

        static Button MakeButton(RectTransform parent, string name, Vector2 anchor, Vector2 pos, Vector2 dims)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = dims;
            go.GetComponent<Image>().color = new Color(0.16f, 0.22f, 0.35f, 1f);
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
            go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            var frt = (RectTransform)fill.transform;
            frt.SetParent(rt, false);
            frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
            frt.offsetMin = frt.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = fillColor;

            var s = go.GetComponent<Slider>();
            s.transition = Selectable.Transition.None;
            s.interactable = false;
            s.fillRect = frt;
            s.direction = Slider.Direction.LeftToRight;
            s.minValue = 0f; s.maxValue = 1f; s.value = 1f;
            return s;
        }
    }
}
