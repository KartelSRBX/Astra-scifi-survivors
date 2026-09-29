using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;
using GamingApp.UI;

namespace GamingApp.EditorTools
{
    /// <summary>Pre-run weapon choice screen opened by the START button.</summary>
    static partial class MenuBuilder
    {
        static void BuildWeaponSelect(RectTransform root, GameObject host, Button start)
        {
            var db = AssetDatabase.LoadAssetAtPath<GameDatabase>("Assets/Resources/GameDatabase.asset");
            var c = ScreenPanel(root, "WeaponSelectScreen", "CHOOSE WEAPON", out var panel, out var back);

            int n = db.weapons.Count;
            var cards = new Object[n];
            var frames = new Object[n];
            var glows = new Object[n];
            var size = new Vector2(470, 230);
            for (int i = 0; i < n; i++)
            {
                var w = db.weapons[i];
                var pos = new Vector2(i % 2 == 0 ? -245 : 245, 560 - (i / 2) * 250);
                var glow = Img(c, "Glow_" + w.id, "rr_soft", A(w.color, 0.35f), pos, size + new Vector2(70, 70), sliced: true);
                glows[i] = glow;
                var bg = Img(c, "Card_" + w.id, "rr", A(UiTheme.PanelLight, 0.95f), pos, size, sliced: true);
                bg.raycastTarget = true;
                var btn = bg.gameObject.AddComponent<Button>();
                btn.targetGraphic = bg;
                bg.gameObject.AddComponent<ButtonPress>();
                var r = bg.rectTransform;
                frames[i] = Img(r, "Frame", "rr_line", A(w.color, 0.18f), Vector2.zero, size, sliced: true);
                Img(r, "IconGlow", "glow", A(w.color, 0.25f), new Vector2(-150, 0), new Vector2(200, 200));
                Img(r, "Icon", w.icon != null ? w.icon.name : "ico_w_blaster", w.color, new Vector2(-150, 0), new Vector2(120, 120));
                Label(r, "Name", w.displayName.ToUpper(), 34, UiTheme.White, new Vector2(85, 55), new Vector2(280, 50), TextAnchor.MiddleLeft);
                var d = Label(r, "Desc", w.description, 22, UiTheme.SoftText, new Vector2(85, -30), new Vector2(280, 110), TextAnchor.UpperLeft);
                d.fontStyle = FontStyle.Normal;
                cards[i] = btn;
            }

            var dName = Label(c, "DetailName", "", 48, UiTheme.White, new Vector2(0, -420), new Vector2(1000, 70), shadow: true);
            var dText = Label(c, "DetailText", "", 30, UiTheme.SoftText, new Vector2(0, -510), new Vector2(980, 100));
            dText.fontStyle = FontStyle.Normal;
            var launch = UiBuilder.MakeButton(c, "Launch", new Vector2(0.5f, 0.5f), new Vector2(0, -690), new Vector2(720, 150), UiTheme.Violet, "LAUNCH", 60);
            var lg = Motion(Img(c, "LaunchGlow", "rr_soft", A(UiTheme.Violet, 0.5f), new Vector2(0, -690), new Vector2(800, 230), sliced: true));
            lg.alphaMin = 0.4f; lg.alphaSpeed = 2f;
            lg.transform.SetSiblingIndex(launch.transform.GetSiblingIndex());

            var ws = host.AddComponent<WeaponSelectScreen>();
            BuilderUtil.SetRef(ws, "root", panel);
            BuilderUtil.SetRef(ws, "openButton", start);
            BuilderUtil.SetRef(ws, "closeButton", back);
            BuilderUtil.SetRef(ws, "launchButton", launch);
            BuilderUtil.SetRefArray(ws, "cards", cards);
            BuilderUtil.SetRefArray(ws, "cardFrames", frames);
            BuilderUtil.SetRefArray(ws, "cardGlows", glows);
            BuilderUtil.SetRef(ws, "detailName", dName);
            BuilderUtil.SetRef(ws, "detailText", dText);
            panel.SetActive(false);
        }
    }
}
