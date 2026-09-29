using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.UI;

namespace GamingApp.EditorTools
{
    /// <summary>Builds the main menu scene: animated space backdrop, chapter showcase, top bar, bottom nav dock.</summary>
    static partial class MenuBuilder
    {
        static readonly Color Black = Color.black;
        static readonly Color SoftViolet = new Color(0.72f, 0.56f, 1f);
        static readonly Color SoftBlue = new Color(0.5f, 0.65f, 1f);

        public static void Build(string scenePath, float runSeconds)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camGo.tag = "MainCamera";
            var cam = camGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = UiTheme.Background;

            var root = UiBuilder.CreateCanvas(out var host);
            var nebulae = BuildBackdrop(Stretch(root, "Backdrop"));

            var safe = Stretch(root, "SafeArea");
            safe.gameObject.AddComponent<SafeAreaFitter>();

            var stage = Node(safe, "Stage", new Vector2(0.5f, 0.5f), new Vector2(0, 80));
            var stageRefs = BuildStage(stage);

            var topLeft = Node(safe, "TopLeft", new Vector2(0f, 1f), Vector2.zero);
            var avatarShip = BuildProfile(topLeft, out var pilotLevel);
            var topRight = Node(safe, "TopRight", new Vector2(1f, 1f), Vector2.zero);
            var cores = Currency(topRight, "Cores", "cur_cores", new Vector2(-165, -102));
            var biomass = Currency(topRight, "Biomass", "cur_biomass", new Vector2(-435, -102));
            var settings = CircleButton(topRight, "Settings", new Vector2(-95, -225), 110, "ico_gear", UiTheme.SoftText, 62);

            var bottom = Node(safe, "BottomNav", new Vector2(0.5f, 0f), Vector2.zero);
            var play = BuildDock(bottom, out var navButtons, out var navTitles);

            var settingsPanel = Popup(root, "SettingsPanel", new Vector2(860, 760), out var settingsCard);
            Label(settingsCard, "Title", "SETTINGS", 80, UiTheme.White, new Vector2(0, 270), new Vector2(760, 110), shadow: true);
            Img(settingsCard, "Divider", "line_glow", A(UiTheme.Teal, 0.8f), new Vector2(0, 205), new Vector2(520, 12));
            UiBuilder.AddVolumeSlider(settingsCard, new Vector2(0, 50));
            var back = UiBuilder.MakeButton(settingsCard, "Back", new Vector2(0.5f, 0.5f), new Vector2(0, -250), new Vector2(480, 130), UiTheme.Violet, "BACK", 56);

            var placeholder = Popup(root, "PlaceholderPanel", new Vector2(840, 580), out var phCard);
            var phTitle = Label(phCard, "Title", "", 72, UiTheme.White, new Vector2(0, 150), new Vector2(780, 110), shadow: true);
            Label(phCard, "Soon", "COMING SOON", 38, UiTheme.Teal, new Vector2(0, 60), new Vector2(700, 60));
            Img(phCard, "Divider", "line_glow", A(UiTheme.Teal, 0.8f), new Vector2(0, 5), new Vector2(520, 12));
            var phClose = UiBuilder.MakeButton(phCard, "Close", new Vector2(0.5f, 0.5f), new Vector2(0, -170), new Vector2(480, 130), UiTheme.Violet, "BACK", 56);

            BuildHangar(root, host, (Button)navButtons[0]);
            BuildInventory(root, host, (Button)navButtons[1]);
            BuildWeaponSelect(root, host, play);
            navButtons.RemoveRange(0, 2);
            navTitles.RemoveRange(0, 2);

            var sel = host.AddComponent<SystemSelector>();
            BuilderUtil.SetRef(sel, "prevButton", stageRefs.prev);
            BuilderUtil.SetRef(sel, "nextButton", stageRefs.next);
            BuilderUtil.SetRef(sel, "startButton", play);
            BuilderUtil.SetRef(sel, "systemImage", stageRefs.system);
            BuilderUtil.SetRef(sel, "glow", stageRefs.glow);
            BuilderUtil.SetRef(sel, "nebulaA", nebulae.a);
            BuilderUtil.SetRef(sel, "nebulaB", nebulae.b);
            BuilderUtil.SetRef(sel, "numberText", stageRefs.number);
            BuilderUtil.SetRef(sel, "nameText", stageRefs.name);
            BuilderUtil.SetRef(sel, "bestText", stageRefs.best);
            BuilderUtil.SetRef(sel, "threatText", stageRefs.threat);
            BuilderUtil.SetRef(sel, "clearanceText", stageRefs.clearText);
            BuilderUtil.SetRef(sel, "clearanceFill", stageRefs.clearFill);
            BuilderUtil.SetFloat(sel, "clearanceWidth", StageRefs.BarWidth);
            BuilderUtil.SetRef(sel, "lockGroup", stageRefs.lockGroup);
            BuilderUtil.SetRef(sel, "lockText", stageRefs.lockText);

            var mc = host.AddComponent<MenuController>();
            BuilderUtil.SetRef(mc, "settingsButton", settings);
            BuilderUtil.SetRef(mc, "closeSettingsButton", back);
            BuilderUtil.SetRef(mc, "settingsPanel", settingsPanel);
            BuilderUtil.SetRef(mc, "biomassText", biomass);
            BuilderUtil.SetRef(mc, "coresText", cores);
            BuilderUtil.SetRef(mc, "avatarShip", avatarShip);
            BuilderUtil.SetRef(mc, "pilotLevelText", pilotLevel);
            BuilderUtil.SetRefArray(mc, "navButtons", navButtons.ToArray());
            var so = new SerializedObject(mc);
            var tp = so.FindProperty("navTitles");
            tp.arraySize = navTitles.Count;
            for (int i = 0; i < navTitles.Count; i++) tp.GetArrayElementAtIndex(i).stringValue = navTitles[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            BuilderUtil.SetRef(mc, "placeholderPanel", placeholder);
            BuilderUtil.SetRef(mc, "placeholderTitle", phTitle);
            BuilderUtil.SetRef(mc, "placeholderClose", phClose);
            settingsPanel.SetActive(false);
            placeholder.SetActive(false);

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        // ---------- sections ----------
        static (Image a, Image b) BuildBackdrop(RectTransform bd)
        {
            StretchImg(bd, "Gradient", "grad_bg", Color.white);
            var nebA = Img(bd, "NebulaA", "neb_01_a", A(Color.white, 0.9f), new Vector2(-200, 350), new Vector2(1500, 1500));
            nebA.rectTransform.localEulerAngles = new Vector3(0, 0, 20);
            Motion(nebA).spin = 1.2f;
            var nebB = Img(bd, "NebulaB", "neb_01_b", A(Color.white, 0.55f), new Vector2(300, -400), new Vector2(1300, 1300));
            nebB.rectTransform.localEulerAngles = new Vector3(0, 0, -30);
            Motion(nebB).spin = -0.8f;

            var sa = Motion(StretchImg(bd, "StarsA", "stars_a", A(Color.white, 0.95f)));
            sa.alphaMin = 0.55f; sa.alphaSpeed = 0.9f;
            var sb = Motion(StretchImg(bd, "StarsB", "stars_b", A(Color.white, 0.75f)));
            sb.alphaMin = 0.15f; sb.alphaSpeed = 1.4f; sb.phase = 1.7f;
            StretchImg(bd, "Vignette", "vignette", A(Black, 0.8f));
            return (nebA, nebB);
        }

        class StageRefs
        {
            public const float BarWidth = 740f;
            public Button prev, next;
            public Image system, glow;
            public Text number, name, best, threat, clearText, lockText;
            public RectTransform clearFill;
            public GameObject lockGroup;
        }

        static StageRefs BuildStage(RectTransform stage)
        {
            var r = new StageRefs();
            var show = new Vector2(0, 60);
            var hex = Motion(Img(stage, "HexFloor", "hex_field", A(UiTheme.Teal, 0.3f), new Vector2(0, -330), new Vector2(1150, 575)));
            hex.alphaMin = 0.55f; hex.alphaSpeed = 0.7f;
            var glowBig = Motion(Img(stage, "GlowStar", "glow", A(Color.white, 0.5f), show, new Vector2(1000, 1000)));
            glowBig.pulseAmplitude = 0.04f; glowBig.pulseSpeed = 0.8f;
            r.glow = glowBig.GetComponent<Image>();
            Motion(Img(stage, "TechRing", "ring_tech", A(UiTheme.Teal, 0.35f), show, new Vector2(780, 780))).spin = 4f;
            Motion(Img(stage, "OrbitRing", "ring_orbit", new Color(0.75f, 0.6f, 1f, 0.6f), show, new Vector2(700, 700))).spin = -10f;
            var sys = Motion(Img(stage, "System", "system_01", Color.white, show, new Vector2(760, 760)));
            sys.bobAmplitude = 10f; sys.bobSpeed = 0.8f;
            r.system = sys.GetComponent<Image>();

            var rnd = new System.Random(3);
            for (int i = 0; i < 14; i++)
            {
                var pos = new Vector2((float)(rnd.NextDouble() * 900 - 450), (float)(rnd.NextDouble() * 750 - 300));
                float size = 14f + (float)rnd.NextDouble() * 18f;
                var m = Motion(Img(stage, "Mote" + i, "glow", A(i % 2 == 0 ? UiTheme.Teal : UiTheme.Violet, 0.9f), pos, new Vector2(size, size)));
                m.bobAmplitude = 8f + (float)rnd.NextDouble() * 18f;
                m.bobSpeed = 0.4f + (float)rnd.NextDouble() * 0.6f;
                m.alphaMin = 0.1f;
                m.alphaSpeed = 0.6f + (float)rnd.NextDouble() * 1.2f;
                m.phase = (float)rnd.NextDouble() * 10f;
            }

            // Lock overlay for systems that are not reachable yet.
            var lockGo = new GameObject("Locked", typeof(RectTransform));
            var lockRt = (RectTransform)lockGo.transform;
            lockRt.SetParent(stage, false);
            lockRt.anchoredPosition = show;
            lockRt.sizeDelta = Vector2.zero;
            Img(lockRt, "LockDisc", "disc", A(UiTheme.Background, 0.85f), Vector2.zero, new Vector2(200, 200));
            Img(lockRt, "LockRing", "ring_thin", A(UiTheme.Danger, 0.8f), Vector2.zero, new Vector2(206, 206));
            Img(lockRt, "LockIcon", "ico_lock", UiTheme.White, Vector2.zero, new Vector2(100, 100));
            r.lockText = Label(lockRt, "LockText", "", 32, UiTheme.White, new Vector2(0, -150), new Vector2(900, 50), shadow: true);
            r.lockGroup = lockGo;

            r.number = Label(stage, "SystemNumber", "SYSTEM 1", 38, UiTheme.Teal, new Vector2(0, 560), new Vector2(400, 60));
            Img(stage, "LineL", "line_glow", UiTheme.Teal, new Vector2(-220, 560), new Vector2(180, 12));
            Img(stage, "LineR", "line_glow", UiTheme.Teal, new Vector2(220, 560), new Vector2(180, 12));
            r.name = Label(stage, "SystemName", "SOL OUTPOST", 78, UiTheme.White, new Vector2(0, 485), new Vector2(1040, 110), shadow: true);

            r.prev = CircleButton(stage, "PrevSystem", show + new Vector2(-440, 0), 120, "ico_left", UiTheme.SoftText, 56);
            r.next = CircleButton(stage, "NextSystem", show + new Vector2(440, 0), 120, "ico_right", UiTheme.SoftText, 56);

            var cardPos = new Vector2(0, -420);
            var cardSize = new Vector2(820, 250);
            Img(stage, "CardShadow", "rr_soft", A(Black, 0.5f), cardPos + new Vector2(0, -10), cardSize + new Vector2(60, 60), sliced: true);
            var card = Img(stage, "InfoCard", "rr", A(UiTheme.Panel, 0.92f), cardPos, cardSize, sliced: true).rectTransform;
            Img(card, "Outline", "rr_line", A(UiTheme.Teal, 0.3f), Vector2.zero, cardSize, sliced: true);
            Img(card, "Divider", null, A(UiTheme.Teal, 0.2f), new Vector2(20, 45), new Vector2(3, 100));
            Img(card, "Trophy", "ico_trophy", UiTheme.Teal, new Vector2(-330, 45), new Vector2(64, 64));
            Label(card, "BestLabel", "BEST RUN", 26, UiTheme.SoftText, new Vector2(-160, 73), new Vector2(260, 40), TextAnchor.MiddleLeft);
            r.best = Label(card, "BestValue", "--:--", 50, UiTheme.White, new Vector2(-160, 23), new Vector2(260, 64), TextAnchor.MiddleLeft);
            Img(card, "Threat", "enemy_05", Color.white, new Vector2(80, 45), new Vector2(76, 76));
            Label(card, "ThreatLabel", "ENEMY POWER", 26, UiTheme.SoftText, new Vector2(250, 73), new Vector2(260, 40), TextAnchor.MiddleLeft);
            r.threat = Label(card, "ThreatValue", "+0%", 50, new Color(1f, 0.45f, 0.5f), new Vector2(250, 23), new Vector2(260, 64), TextAnchor.MiddleLeft);

            r.clearText = Label(card, "ClearLabel", "THREAT CLEARED 0%", 26, UiTheme.SoftText, new Vector2(0, -52), new Vector2(StageRefs.BarWidth, 40), TextAnchor.MiddleLeft);
            Img(card, "ClearBack", "pill", UiTheme.BarBackground, new Vector2(0, -90), new Vector2(StageRefs.BarWidth, 24), sliced: true);
            var fill = Img(card, "ClearFill", "pill", UiTheme.Teal, new Vector2(-StageRefs.BarWidth / 2f, -90), new Vector2(StageRefs.BarWidth, 24), sliced: true);
            fill.rectTransform.pivot = new Vector2(0f, 0.5f);
            r.clearFill = fill.rectTransform;
            return r;
        }

        static Image BuildProfile(RectTransform tl, out Text level)
        {
            var c = new Vector2(115, -115);
            var glow = Motion(Img(tl, "AvatarGlow", "glow", A(UiTheme.Teal, 0.3f), c, new Vector2(240, 240)));
            glow.alphaMin = 0.5f; glow.alphaSpeed = 1.3f;
            Img(tl, "Avatar", "disc", UiTheme.PanelLight, c, new Vector2(150, 150));
            Img(tl, "AvatarRing", "ring_thin", A(UiTheme.Teal, 0.9f), c, new Vector2(158, 158));
            var ship = Img(tl, "AvatarShip", "ship_corvette", Color.white, c, new Vector2(120, 120));
            ship.preserveAspect = true;
            var badge = Img(tl, "LevelBadge", "disc", UiTheme.Violet, c + new Vector2(55, -55), new Vector2(56, 56));
            level = Label(badge.rectTransform, "Level", "1", 32, UiTheme.White, Vector2.zero, new Vector2(56, 56));

            Label(tl, "Name", "PILOT", 42, UiTheme.White, new Vector2(360, -92), new Vector2(300, 60), TextAnchor.MiddleLeft, shadow: true);
            Img(tl, "XpBack", "pill", UiTheme.BarBackground, new Vector2(330, -150), new Vector2(240, 26), sliced: true);
            Img(tl, "XpFill", "pill", UiTheme.Teal, new Vector2(255, -150), new Vector2(90, 26), sliced: true);
            return ship;
        }

        static Text Currency(RectTransform parent, string name, string icon, Vector2 pos)
        {
            var size = new Vector2(250, 84);
            var pill = Img(parent, "Currency_" + name, "pill", A(UiTheme.Panel, 0.92f), pos, size, sliced: true).rectTransform;
            Img(pill, "Outline", "pill_line", A(UiTheme.Teal, 0.35f), Vector2.zero, size, sliced: true);
            Img(pill, "Icon", icon, Color.white, new Vector2(-115, 0), new Vector2(100, 100));
            var t = Label(pill, "Amount", "0", 44, UiTheme.White, new Vector2(15, 0), new Vector2(170, 70), TextAnchor.MiddleRight);
            return t;
        }

        static Button BuildDock(RectTransform bottom, out System.Collections.Generic.List<Object> nav, out System.Collections.Generic.List<string> titles)
        {
            Img(bottom, "DockShadow", "rr_soft", A(Black, 0.6f), new Vector2(0, 120), new Vector2(1100, 280), sliced: true);
            Img(bottom, "Dock", "rr", A(UiTheme.Panel, 0.96f), new Vector2(0, 129), new Vector2(1032, 210), sliced: true);
            Img(bottom, "DockOutline", "rr_line", A(UiTheme.Teal, 0.22f), new Vector2(0, 129), new Vector2(1032, 210), sliced: true);
            var edge = Motion(Img(bottom, "DockEdge", "line_glow", UiTheme.Teal, new Vector2(0, 233), new Vector2(900, 12)));
            edge.alphaMin = 0.5f; edge.alphaSpeed = 1.5f;

            nav = new System.Collections.Generic.List<Object>();
            titles = new System.Collections.Generic.List<string>();
            var items = new (float x, string title, string label, string icon, Color color)[]
            {
                (-400, "HANGAR", "HANGAR", "ico_ship", UiTheme.Teal),
                (-210, "INVENTORY", "INVENTORY", "ico_inventory", SoftBlue),
                (210, "UPGRADES", "UPGRADES", "ico_upgrades", SoftViolet),
                (400, "FORGE", "FORGE", "ico_forge", UiTheme.White),
            };
            foreach (var it in items)
            {
                nav.Add(CircleButton(bottom, "Nav_" + it.title, new Vector2(it.x, 150), 120, it.icon, it.color, 64));
                Label(bottom, "Label_" + it.title, it.label, 24, UiTheme.SoftText, new Vector2(it.x, 58), new Vector2(190, 40));
                titles.Add(it.title);
            }

            var sglow = Motion(Img(bottom, "StartGlow", "glow", A(UiTheme.Violet, 0.75f), new Vector2(0, 205), new Vector2(480, 480)));
            sglow.pulseAmplitude = 0.06f; sglow.pulseSpeed = 2.2f;
            Motion(Img(bottom, "StartRing", "ring_tech", A(UiTheme.Teal, 0.85f), new Vector2(0, 205), new Vector2(300, 300))).spin = -30f;
            var orb = Img(bottom, "Start", "orb", Color.white, new Vector2(0, 205), new Vector2(230, 230));
            orb.raycastTarget = true;
            var play = orb.gameObject.AddComponent<Button>();
            play.targetGraphic = orb;
            orb.gameObject.AddComponent<ButtonPress>();
            Img(orb.rectTransform, "Icon", "ico_play", UiTheme.White, new Vector2(6, 0), new Vector2(96, 96));
            Label(bottom, "StartLabel", "START", 34, UiTheme.White, new Vector2(0, 58), new Vector2(200, 44), shadow: true);
            return play;
        }

        static GameObject Popup(RectTransform root, string name, Vector2 size, out RectTransform card)
        {
            var panel = UiBuilder.Panel(root, name, UiTheme.Overlay);
            var prt = (RectTransform)panel.transform;
            Img(prt, "Shadow", "rr_soft", A(Black, 0.6f), new Vector2(0, -12), size + new Vector2(70, 70), sliced: true);
            card = Img(prt, "Card", "rr", UiTheme.Panel, Vector2.zero, size, sliced: true).rectTransform;
            Img(card, "Outline", "rr_line", A(UiTheme.Teal, 0.4f), Vector2.zero, size, sliced: true);
            panel.AddComponent<CanvasGroup>();
            panel.AddComponent<PopIn>().content = card;
            return panel;
        }

        // ---------- helpers ----------
        static Color A(Color c, float a) { c.a = a; return c; }

        static UiMotion Motion(Graphic g) => g.gameObject.AddComponent<UiMotion>();

        static RectTransform Node(RectTransform parent, string name, Vector2 anchor, Vector2 pos)
        {
            var rt = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = Vector2.zero;
            return rt;
        }

        static RectTransform Stretch(RectTransform parent, string name)
        {
            var rt = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return rt;
        }

        static Image StretchImg(RectTransform parent, string name, string sprite, Color color)
        {
            var img = Stretch(parent, name).gameObject.AddComponent<Image>();
            img.sprite = UiSprites.Get(sprite);
            img.color = color;
            img.raycastTarget = false;
            return img;
        }

        static Image Img(RectTransform parent, string name, string sprite, Color color, Vector2 pos, Vector2 size, bool sliced = false)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = go.GetComponent<Image>();
            if (sprite != null) img.sprite = UiSprites.Get(sprite);
            img.color = color;
            img.raycastTarget = false;
            if (sliced)
            {
                img.type = Image.Type.Sliced;
                var b = img.sprite.border;
                float need = Mathf.Max((b.x + b.z) / size.x, (b.y + b.w) / size.y);
                if (need > 1f) img.pixelsPerUnitMultiplier = need;
            }
            return img;
        }

        static Text Label(RectTransform parent, string name, string text, int size, Color color, Vector2 pos, Vector2 dims,
            TextAnchor align = TextAnchor.MiddleCenter, bool shadow = false)
        {
            var t = UiBuilder.MakeText(parent, name, text, size, align, new Vector2(0.5f, 0.5f), pos, dims);
            t.color = color;
            t.fontStyle = FontStyle.Bold;
            if (shadow)
            {
                var sh = t.gameObject.AddComponent<Shadow>();
                sh.effectColor = new Color(0f, 0f, 0f, 0.6f);
                sh.effectDistance = new Vector2(0f, -4f);
            }
            return t;
        }

        static Button CircleButton(RectTransform parent, string name, Vector2 pos, float size, string icon, Color iconColor, float iconSize)
        {
            var bg = Img(parent, name, "disc", A(UiTheme.PanelLight, 0.92f), pos, new Vector2(size, size));
            bg.raycastTarget = true;
            var btn = bg.gameObject.AddComponent<Button>();
            btn.targetGraphic = bg;
            bg.gameObject.AddComponent<ButtonPress>();
            Img(bg.rectTransform, "Ring", "ring_thin", A(UiTheme.Teal, 0.4f), Vector2.zero, new Vector2(size + 6, size + 6));
            Img(bg.rectTransform, "Icon", icon, iconColor, Vector2.zero, new Vector2(iconSize, iconSize));
            return btn;
        }
    }
}
