using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;
using GamingApp.UI;

namespace GamingApp.EditorTools
{
    /// <summary>Full-screen menu screens: Hangar (ships) and Inventory (loadout + cargo).</summary>
    static partial class MenuBuilder
    {
        static RectTransform ScreenPanel(RectTransform root, string name, string title, out GameObject panel, out Button back)
        {
            var prt = Stretch(root, name);
            panel = prt.gameObject;
            var bg = panel.AddComponent<Image>();
            bg.color = UiTheme.Background;
            StretchImg(prt, "Gradient", "grad_bg", Color.white);
            var stars = Motion(StretchImg(prt, "Stars", "stars_a", A(Color.white, 0.7f)));
            stars.alphaMin = 0.5f; stars.alphaSpeed = 1f;
            StretchImg(prt, "Vignette", "vignette", A(Black, 0.8f));

            var safe = Stretch(prt, "SafeArea");
            safe.gameObject.AddComponent<SafeAreaFitter>();
            var content = Node(safe, "Content", new Vector2(0.5f, 0.5f), Vector2.zero);
            Label(content, "Title", title, 64, UiTheme.White, new Vector2(0, 820), new Vector2(700, 90), shadow: true);
            Img(content, "TitleLine", "line_glow", UiTheme.Teal, new Vector2(0, 765), new Vector2(420, 12));
            back = CircleButton(content, "Back", new Vector2(-440, 820), 110, "ico_left", UiTheme.SoftText, 56);

            panel.AddComponent<CanvasGroup>();
            panel.AddComponent<PopIn>();
            return content;
        }

        static Text ButtonLabel(Button b) => b.transform.Find("Label").GetComponent<Text>();

        static void BuildHangar(RectTransform root, GameObject host, Button open)
        {
            var c = ScreenPanel(root, "HangarScreen", "HANGAR", out var panel, out var back);
            var show = new Vector2(0, 430);
            Img(c, "Floor", "hex_field", A(UiTheme.Teal, 0.3f), new Vector2(0, 200), new Vector2(900, 450));
            var glow = Motion(Img(c, "Glow", "glow", A(UiTheme.Violet, 0.5f), show, new Vector2(820, 820)));
            glow.pulseAmplitude = 0.04f; glow.pulseSpeed = 1f;
            Motion(Img(c, "Ring", "ring_tech", A(UiTheme.Teal, 0.4f), show, new Vector2(640, 640))).spin = 6f;
            Motion(Img(c, "Orbit", "ring_orbit", new Color(0.75f, 0.6f, 1f, 0.7f), show, new Vector2(560, 560))).spin = -12f;
            var ship = Img(c, "Ship", "ship_corvette", Color.white, show, new Vector2(440, 440));
            ship.preserveAspect = true;
            var bob = Motion(ship); bob.bobAmplitude = 10f; bob.bobSpeed = 1.1f;
            var prev = CircleButton(c, "Prev", show + new Vector2(-430, 0), 110, "ico_left", UiTheme.SoftText, 54);
            var next = CircleButton(c, "Next", show + new Vector2(430, 0), 110, "ico_right", UiTheme.SoftText, 54);

            var counter = Label(c, "Counter", "1 / 5", 30, UiTheme.SoftText, new Vector2(0, 170), new Vector2(300, 44));
            var name = Label(c, "Name", "CORVETTE", 72, UiTheme.White, new Vector2(0, 105), new Vector2(900, 100), shadow: true);
            var cls = Label(c, "Class", "Light escort", 34, UiTheme.Teal, new Vector2(0, 40), new Vector2(900, 50));

            var cardPos = new Vector2(0, -175);
            Img(c, "CardShadow", "rr_soft", A(Black, 0.5f), cardPos + new Vector2(0, -10), new Vector2(920, 390), sliced: true);
            var card = Img(c, "StatsCard", "rr", A(UiTheme.Panel, 0.94f), cardPos, new Vector2(860, 330), sliced: true).rectTransform;
            Img(card, "Outline", "rr_line", A(UiTheme.Teal, 0.3f), Vector2.zero, new Vector2(860, 330), sliced: true);
            var labels = Label(card, "Labels", "HULL\nSPEED\nFIREPOWER\nFIRE RATE", 38, UiTheme.SoftText, new Vector2(-150, 0), new Vector2(420, 280), TextAnchor.MiddleLeft);
            labels.lineSpacing = 1.25f;
            var stats = Label(card, "Values", "", 42, UiTheme.White, new Vector2(230, 0), new Vector2(300, 280), TextAnchor.MiddleRight);
            stats.lineSpacing = 1.13f;

            var level = Label(c, "Level", "LEVEL 1 / 10", 40, UiTheme.Teal, new Vector2(0, -395), new Vector2(700, 60));
            var upgrade = UiBuilder.MakeButton(c, "Upgrade", new Vector2(0.5f, 0.5f), new Vector2(0, -510), new Vector2(720, 120), UiTheme.Blue, "UPGRADE", 40);
            var action = UiBuilder.MakeButton(c, "Action", new Vector2(0.5f, 0.5f), new Vector2(0, -660), new Vector2(720, 140), UiTheme.Violet, "SELECT", 44);

            var hs = host.AddComponent<HangarScreen>();
            BuilderUtil.SetRef(hs, "root", panel);
            BuilderUtil.SetRef(hs, "openButton", open);
            BuilderUtil.SetRef(hs, "closeButton", back);
            BuilderUtil.SetRef(hs, "prevButton", prev);
            BuilderUtil.SetRef(hs, "nextButton", next);
            BuilderUtil.SetRef(hs, "actionButton", action);
            BuilderUtil.SetRef(hs, "actionLabel", ButtonLabel(action));
            BuilderUtil.SetRef(hs, "upgradeButton", upgrade);
            BuilderUtil.SetRef(hs, "upgradeLabel", ButtonLabel(upgrade));
            BuilderUtil.SetRef(hs, "shipImage", ship);
            BuilderUtil.SetRef(hs, "nameText", name);
            BuilderUtil.SetRef(hs, "classText", cls);
            BuilderUtil.SetRef(hs, "statsText", stats);
            BuilderUtil.SetRef(hs, "levelText", level);
            BuilderUtil.SetRef(hs, "counterText", counter);
            panel.SetActive(false);
        }

        static void BuildInventory(RectTransform root, GameObject host, Button open)
        {
            var c = ScreenPanel(root, "InventoryScreen", "INVENTORY", out var panel, out var back);
            var show = new Vector2(0, 500);
            Motion(Img(c, "Glow", "glow", A(UiTheme.Teal, 0.3f), show, new Vector2(560, 560))).alphaMin = 0.6f;
            Motion(Img(c, "Ring", "ring_tech", A(UiTheme.Teal, 0.4f), show, new Vector2(420, 420))).spin = 6f;
            var ship = Img(c, "Ship", "ship_corvette", Color.white, show, new Vector2(300, 300));
            ship.preserveAspect = true;
            var bob = Motion(ship); bob.bobAmplitude = 8f; bob.bobSpeed = 1.1f;

            var slotNames = new[] { "WEAPON", "HULL", "ENGINE", "MODULE" };
            var slotPos = new[] { new Vector2(-330, 590), new Vector2(330, 590), new Vector2(-330, 410), new Vector2(330, 410) };
            var slotButtons = new Object[4];
            var slotIcons = new Object[4];
            var slotFrames = new Object[4];
            var placeholders = new Object[4];
            for (int i = 0; i < 4; i++)
            {
                var bg = Img(c, "Slot_" + slotNames[i], "rr", UiTheme.PanelLight, slotPos[i], new Vector2(150, 150), sliced: true);
                bg.raycastTarget = true;
                var btn = bg.gameObject.AddComponent<Button>();
                btn.targetGraphic = bg;
                bg.gameObject.AddComponent<ButtonPress>();
                slotFrames[i] = Img(bg.rectTransform, "Frame", "rr_line", A(Color.white, 0.15f), Vector2.zero, new Vector2(150, 150), sliced: true);
                slotIcons[i] = Img(bg.rectTransform, "Icon", AssetBuilder.SlotIcon((ItemSlot)i), A(Color.white, 0.2f), Vector2.zero, new Vector2(92, 92));
                Label(c, "SlotLabel_" + slotNames[i], slotNames[i], 24, UiTheme.SoftText, slotPos[i] + new Vector2(0, -100), new Vector2(200, 36));
                slotButtons[i] = btn;
                placeholders[i] = UiSprites.Get(AssetBuilder.SlotIcon((ItemSlot)i));
            }
            var stats = Label(c, "Stats", "", 30, UiTheme.White, new Vector2(0, 290), new Vector2(1040, 50));

            Label(c, "CargoTitle", "CARGO HOLD", 30, UiTheme.SoftText, new Vector2(0, 232), new Vector2(600, 44));
            var cardPos = new Vector2(0, -250);
            var cardSize = new Vector2(980, 900);
            var card = Img(c, "Cargo", "rr", A(UiTheme.Panel, 0.94f), cardPos, cardSize, sliced: true).rectTransform;
            Img(card, "Outline", "rr_line", A(UiTheme.Teal, 0.3f), Vector2.zero, cardSize, sliced: true);

            var viewport = Stretch(card, "Viewport");
            viewport.offsetMin = new Vector2(20, 20);
            viewport.offsetMax = new Vector2(-20, -20);
            var vpImg = viewport.gameObject.AddComponent<Image>();
            vpImg.color = new Color(0, 0, 0, 0);
            viewport.gameObject.AddComponent<RectMask2D>();

            var grid = (RectTransform)new GameObject("Grid", typeof(RectTransform)).transform;
            grid.SetParent(viewport, false);
            grid.anchorMin = new Vector2(0f, 1f);
            grid.anchorMax = new Vector2(1f, 1f);
            grid.pivot = new Vector2(0.5f, 1f);
            grid.offsetMin = grid.offsetMax = Vector2.zero;
            var layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(160, 160);
            layout.spacing = new Vector2(20, 20);
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 5;
            layout.childAlignment = TextAnchor.UpperCenter;
            grid.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = card.gameObject.AddComponent<ScrollRect>();
            scroll.content = grid;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.scrollSensitivity = 30f;

            var cell = Img(grid, "Cell", "rr", UiTheme.PanelLight, Vector2.zero, new Vector2(160, 160), sliced: true);
            cell.raycastTarget = true;
            var cellBtn = cell.gameObject.AddComponent<Button>();
            cellBtn.targetGraphic = cell;
            cell.gameObject.AddComponent<ButtonPress>();
            Img(cell.rectTransform, "Frame", "rr_line", Color.white, Vector2.zero, new Vector2(160, 160), sliced: true);
            Img(cell.rectTransform, "Icon", "ico_weapon", Color.white, Vector2.zero, new Vector2(96, 96));
            var eq = Img(cell.rectTransform, "Equipped", "disc", UiTheme.Teal, new Vector2(54, 54), new Vector2(40, 40));
            Label(eq.rectTransform, "E", "E", 24, UiTheme.Background, Vector2.zero, new Vector2(40, 40));
            var empty = Label(card, "Empty", "Cargo hold is empty", 34, UiTheme.SoftText, Vector2.zero, new Vector2(800, 60));

            // Detail popup: tapping the dim area closes it, the card itself swallows taps.
            var overlay = Stretch((RectTransform)panel.transform, "Detail");
            var ovImg = overlay.gameObject.AddComponent<Image>();
            ovImg.color = UiTheme.Overlay;
            var dismiss = overlay.gameObject.AddComponent<Button>();
            dismiss.transition = Selectable.Transition.None;
            var dSize = new Vector2(820, 680);
            Img(overlay, "Shadow", "rr_soft", A(Black, 0.6f), new Vector2(0, -12), dSize + new Vector2(70, 70), sliced: true);
            var dCard = Img(overlay, "Card", "rr", UiTheme.Panel, Vector2.zero, dSize, sliced: true);
            dCard.raycastTarget = true;
            dCard.gameObject.AddComponent<Button>().transition = Selectable.Transition.None;
            var dcr = dCard.rectTransform;
            var dFrame = Img(dcr, "Frame", "rr_line", Color.white, Vector2.zero, dSize, sliced: true);
            Img(dcr, "IconGlow", "glow", A(Color.white, 0.12f), new Vector2(0, 200), new Vector2(300, 300));
            var dIcon = Img(dcr, "Icon", "ico_weapon", Color.white, new Vector2(0, 200), new Vector2(160, 160));
            var dName = Label(dcr, "Name", "", 54, UiTheme.White, new Vector2(0, 80), new Vector2(760, 70), shadow: true);
            var dRarity = Label(dcr, "Rarity", "", 28, UiTheme.SoftText, new Vector2(0, 30), new Vector2(760, 40));
            var dStats = Label(dcr, "Stats", "", 34, UiTheme.SoftText, new Vector2(0, -95), new Vector2(760, 190));
            dStats.fontStyle = FontStyle.Normal;
            var dAction = UiBuilder.MakeButton(dcr, "Action", new Vector2(0.5f, 0.5f), new Vector2(0, -265), new Vector2(520, 120), UiTheme.Violet, "EQUIP", 48);
            overlay.gameObject.AddComponent<CanvasGroup>();
            overlay.gameObject.AddComponent<PopIn>().content = dcr;

            var inv = host.AddComponent<InventoryScreen>();
            BuilderUtil.SetRef(inv, "root", panel);
            BuilderUtil.SetRef(inv, "openButton", open);
            BuilderUtil.SetRef(inv, "closeButton", back);
            BuilderUtil.SetRef(inv, "shipImage", ship);
            BuilderUtil.SetRef(inv, "statsText", stats);
            BuilderUtil.SetRefArray(inv, "slotButtons", slotButtons);
            BuilderUtil.SetRefArray(inv, "slotIcons", slotIcons);
            BuilderUtil.SetRefArray(inv, "slotFrames", slotFrames);
            BuilderUtil.SetRefArray(inv, "slotPlaceholders", placeholders);
            BuilderUtil.SetRef(inv, "grid", grid);
            BuilderUtil.SetRef(inv, "cellTemplate", cell.gameObject);
            BuilderUtil.SetRef(inv, "emptyText", empty);
            BuilderUtil.SetRef(inv, "detailRoot", overlay.gameObject);
            BuilderUtil.SetRef(inv, "detailDismiss", dismiss);
            BuilderUtil.SetRef(inv, "detailIcon", dIcon);
            BuilderUtil.SetRef(inv, "detailFrame", dFrame);
            BuilderUtil.SetRef(inv, "detailName", dName);
            BuilderUtil.SetRef(inv, "detailRarity", dRarity);
            BuilderUtil.SetRef(inv, "detailStats", dStats);
            BuilderUtil.SetRef(inv, "detailAction", dAction);
            BuilderUtil.SetRef(inv, "detailActionLabel", ButtonLabel(dAction));
            overlay.gameObject.SetActive(false);
            panel.SetActive(false);
        }
    }
}
