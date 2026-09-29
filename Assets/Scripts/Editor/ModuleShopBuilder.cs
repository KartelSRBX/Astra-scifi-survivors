using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;
using GamingApp.UI;

namespace GamingApp.EditorTools
{
    /// <summary>Module shop screen: tabs, weapon chips, scrolling module list built from a row template.</summary>
    static partial class MenuBuilder
    {
        static readonly string[] StatIconNames =
        {
            "ico_weapon", "ico_w_ciws", "ico_w_missile", "ico_sensor", "ico_hull", "ico_hull", "ico_engine", "ico_module",
        };

        static void BuildModuleShop(RectTransform root, GameObject host, Button open)
        {
            var db = AssetDatabase.LoadAssetAtPath<GameDatabase>("Assets/Resources/GameDatabase.asset");
            var c = ScreenPanel(root, "ModuleShopScreen", "MODULE SHOP", out var panel, out var back);

            Img(c, "BioIcon", "cur_biomass", Color.white, new Vector2(-190, 715), new Vector2(64, 64));
            var bio = Label(c, "Biomass", "0", 38, UiTheme.White, new Vector2(-60, 715), new Vector2(200, 56), TextAnchor.MiddleLeft);
            Img(c, "CoreIcon", "cur_cores", Color.white, new Vector2(110, 715), new Vector2(64, 64));
            var cores = Label(c, "Cores", "0", 38, UiTheme.White, new Vector2(240, 715), new Vector2(200, 56), TextAnchor.MiddleLeft);

            string[] tabs = { "WEAPONS", "ARMOR", "ENGINES", "SENSORS" };
            var tabButtons = new Object[4];
            var tabBacks = new Object[4];
            for (int i = 0; i < 4; i++)
            {
                var b = UiBuilder.MakeButton(c, "Tab_" + tabs[i], new Vector2(0.5f, 0.5f), new Vector2(-363 + i * 242, 620), new Vector2(230, 96), UiTheme.PanelLight, tabs[i], 30);
                tabButtons[i] = b;
                tabBacks[i] = b.GetComponent<Image>();
            }

            var bar = Node(c, "WeaponBar", new Vector2(0.5f, 0.5f), new Vector2(0, 500));
            var chips = new Object[db.weapons.Count];
            var rings = new Object[db.weapons.Count];
            for (int i = 0; i < db.weapons.Count; i++)
            {
                var w = db.weapons[i];
                var chip = CircleButton(bar, "Chip_" + w.id, new Vector2(-420 + i * 120, 0), 104, w.icon.name, w.color, 60);
                chips[i] = chip;
                rings[i] = chip.transform.Find("Ring").GetComponent<Image>();
            }
            var header = Label(c, "Header", "", 26, UiTheme.SoftText, new Vector2(0, 415), new Vector2(1000, 44));
            header.fontStyle = FontStyle.Normal;

            var cardPos = new Vector2(0, -225);
            var cardSize = new Vector2(1000, 1180);
            var card = Img(c, "ListCard", "rr", A(UiTheme.Panel, 0.94f), cardPos, cardSize, sliced: true).rectTransform;
            Img(card, "Outline", "rr_line", A(UiTheme.Teal, 0.25f), Vector2.zero, cardSize, sliced: true);
            var viewport = Stretch(card, "Viewport");
            viewport.offsetMin = new Vector2(16, 16);
            viewport.offsetMax = new Vector2(-16, -16);
            viewport.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            viewport.gameObject.AddComponent<RectMask2D>();

            var list = (RectTransform)new GameObject("List", typeof(RectTransform)).transform;
            list.SetParent(viewport, false);
            list.anchorMin = new Vector2(0f, 1f);
            list.anchorMax = new Vector2(1f, 1f);
            list.pivot = new Vector2(0.5f, 1f);
            list.offsetMin = list.offsetMax = Vector2.zero;
            var layout = list.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 14;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            list.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = card.gameObject.AddComponent<ScrollRect>();
            scroll.content = list;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.scrollSensitivity = 40f;

            var row = BuildModuleRow(list);

            var shop = host.AddComponent<ModuleShopScreen>();
            BuilderUtil.SetRef(shop, "root", panel);
            BuilderUtil.SetRef(shop, "openButton", open);
            BuilderUtil.SetRef(shop, "closeButton", back);
            BuilderUtil.SetRefArray(shop, "tabButtons", tabButtons);
            BuilderUtil.SetRefArray(shop, "tabBacks", tabBacks);
            BuilderUtil.SetRef(shop, "weaponBar", bar.gameObject);
            BuilderUtil.SetRefArray(shop, "weaponChips", chips);
            BuilderUtil.SetRefArray(shop, "weaponChipRings", rings);
            BuilderUtil.SetRef(shop, "headerText", header);
            BuilderUtil.SetRef(shop, "biomassText", bio);
            BuilderUtil.SetRef(shop, "coresText", cores);
            BuilderUtil.SetRef(shop, "list", list);
            BuilderUtil.SetRef(shop, "rowTemplate", row);
            var icons = new Object[StatIconNames.Length];
            for (int i = 0; i < icons.Length; i++) icons[i] = UiSprites.Get(StatIconNames[i]);
            BuilderUtil.SetRefArray(shop, "statIcons", icons);
            panel.SetActive(false);
        }

        static GameObject BuildModuleRow(RectTransform list)
        {
            var size = new Vector2(952, 150);
            var bg = Img(list, "Row", "rr", UiTheme.PanelLight, Vector2.zero, size, sliced: true);
            var r = bg.rectTransform;
            Img(r, "Frame", "rr_line", Color.white, Vector2.zero, size, sliced: true);
            Img(r, "IconBack", "disc", A(UiTheme.Background, 0.8f), new Vector2(-400, 0), new Vector2(104, 104));
            Img(r, "Icon", "ico_weapon", Color.white, new Vector2(-400, 0), new Vector2(64, 64));
            var name = Label(r, "Name", "Module", 30, UiTheme.White, new Vector2(-110, 38), new Vector2(440, 44), TextAnchor.MiddleLeft);
            name.horizontalOverflow = HorizontalWrapMode.Overflow;
            var stats = Label(r, "Stats", "", 22, UiTheme.SoftText, new Vector2(-110, -4), new Vector2(440, 40), TextAnchor.MiddleLeft);
            stats.fontStyle = FontStyle.Normal;

            var pips = (RectTransform)new GameObject("Pips", typeof(RectTransform)).transform;
            pips.SetParent(r, false);
            pips.anchoredPosition = new Vector2(-320, -46);
            for (int i = 0; i < ModuleDef.Tiers; i++)
                Img(pips, "Pip" + i, "disc", Color.white, new Vector2(i * 30, 0), new Vector2(20, 20));

            var equip = UiBuilder.MakeButton(r, "Equip", new Vector2(0.5f, 0.5f), new Vector2(175, 0), new Vector2(120, 80), UiTheme.Teal, "EQUIP", 22);
            ButtonLabel(equip).color = UiTheme.Background;
            var buy = UiBuilder.MakeButton(r, "Buy", new Vector2(0.5f, 0.5f), new Vector2(355, 0), new Vector2(210, 110), UiTheme.Violet, "BUY", 26);
            ButtonLabel(buy).lineSpacing = 0.9f;

            var lockGo = Img(r, "Lock", "ico_lock", A(UiTheme.White, 0.5f), new Vector2(-400, 0), new Vector2(48, 48)).gameObject;
            lockGo.SetActive(false);
            return bg.gameObject;
        }
    }
}
