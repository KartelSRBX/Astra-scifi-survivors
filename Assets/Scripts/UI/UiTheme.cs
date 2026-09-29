using UnityEngine;

namespace GamingApp.UI
{
    /// <summary>Dark sci-fi palette: blue, violet, teal and white on near-black navy.</summary>
    public static class UiTheme
    {
        public static readonly Color Background = new Color(0.027f, 0.039f, 0.094f);
        public static readonly Color Panel = new Color(0.075f, 0.10f, 0.21f);
        public static readonly Color PanelLight = new Color(0.115f, 0.15f, 0.31f);
        public static readonly Color Overlay = new Color(0.02f, 0.03f, 0.08f, 0.88f);
        public static readonly Color Blue = new Color(0.24f, 0.42f, 1f);
        public static readonly Color Violet = new Color(0.54f, 0.30f, 1f);
        public static readonly Color Teal = new Color(0.13f, 0.83f, 0.77f);
        public static readonly Color White = new Color(0.95f, 0.96f, 1f);
        public static readonly Color SoftText = new Color(0.68f, 0.73f, 0.9f);
        public static readonly Color BarBackground = new Color(0f, 0f, 0.05f, 0.6f);
        public static readonly Color Danger = new Color(1f, 0.25f, 0.3f);

        public static Color RarityColor(GamingApp.Meta.Rarity r) => r switch
        {
            GamingApp.Meta.Rarity.Rare => new Color(0.35f, 0.58f, 1f),
            GamingApp.Meta.Rarity.Epic => new Color(0.68f, 0.42f, 1f),
            GamingApp.Meta.Rarity.Legendary => new Color(0.2f, 0.95f, 0.85f),
            _ => new Color(0.7f, 0.75f, 0.85f),
        };
    }
}
