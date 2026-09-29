using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;

namespace GamingApp.UI
{
    /// <summary>Module shop: tabs for weapon modules (per weapon), armor, engines and sensors; buy, upgrade Mk I..V, equip.</summary>
    public class ModuleShopScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Button openButton;
        [SerializeField] Button closeButton;
        [SerializeField] Button[] tabButtons;
        [SerializeField] Image[] tabBacks;
        [SerializeField] GameObject weaponBar;
        [SerializeField] Button[] weaponChips;
        [SerializeField] Image[] weaponChipRings;
        [SerializeField] Text headerText;
        [SerializeField] Text biomassText;
        [SerializeField] Text coresText;
        [SerializeField] RectTransform list;
        [SerializeField] GameObject rowTemplate;
        [SerializeField] Sprite[] statIcons; // indexed by ModStat

        static readonly string[] Roman = { "", "I", "II", "III", "IV", "V" };
        static readonly ModuleKind[] Tabs = { ModuleKind.Weapon, ModuleKind.Armor, ModuleKind.Engine, ModuleKind.Sensor };

        GameDatabase db;
        int tab;
        int weapon;
        readonly List<GameObject> rows = new();

        void Awake()
        {
            db = GameDatabase.Get();
            root.SetActive(false);
            rowTemplate.SetActive(false);
            openButton.onClick.AddListener(() => { root.SetActive(true); Refresh(); });
            closeButton.onClick.AddListener(() => root.SetActive(false));
            for (int i = 0; i < tabButtons.Length; i++)
            {
                int t = i;
                tabButtons[i].onClick.AddListener(() => { tab = t; Refresh(); });
            }
            for (int i = 0; i < weaponChips.Length; i++)
            {
                int w = i;
                weaponChips[i].onClick.AddListener(() => { weapon = w; Refresh(); });
            }
            Wallet.Changed += RefreshIfOpen;
            Profile.Changed += RefreshIfOpen;
        }

        void OnDestroy()
        {
            Wallet.Changed -= RefreshIfOpen;
            Profile.Changed -= RefreshIfOpen;
        }

        void RefreshIfOpen() { if (root.activeSelf) Refresh(); }

        Color KindColor(ModuleKind k) => k switch
        {
            ModuleKind.Weapon => db.weapons[weapon].color,
            ModuleKind.Armor => UiTheme.Blue,
            ModuleKind.Engine => UiTheme.Teal,
            _ => UiTheme.Violet,
        };

        void Refresh()
        {
            biomassText.text = Wallet.Biomass.ToString();
            coresText.text = Wallet.Cores.ToString();

            var kind = Tabs[tab];
            for (int i = 0; i < tabBacks.Length; i++)
                tabBacks[i].color = i == tab ? UiTheme.Violet : UiTheme.PanelLight;
            weaponBar.SetActive(kind == ModuleKind.Weapon);
            for (int i = 0; i < weaponChipRings.Length; i++)
            {
                var c = db.weapons[i].color;
                c.a = i == weapon ? 1f : 0.2f;
                weaponChipRings[i].color = c;
            }

            List<ModuleDef> mods = kind == ModuleKind.Weapon
                ? ModuleCatalog.ForWeapon(db.weapons[weapon].id)
                : ModuleCatalog.OfKind(kind);
            headerText.text = kind == ModuleKind.Weapon
                ? $"{db.weapons[weapon].displayName.ToUpper()} MODULES   {ModuleShop.OwnedCount(mods)}/{mods.Count}   ·   all owned modules are active"
                : $"{kind.ToString().ToUpper()}   ·   equip one, upgrade to Mk V";

            foreach (var r in rows) Destroy(r);
            rows.Clear();
            foreach (var m in mods) rows.Add(BuildRow(m, KindColor(kind)));
        }

        GameObject BuildRow(ModuleDef m, Color accent)
        {
            var row = Instantiate(rowTemplate, list);
            row.SetActive(true);
            var t = row.transform;
            int tier = Profile.ModuleTier(m.id);
            var state = ModuleShop.GetState(m);
            bool gear = m.kind != ModuleKind.Weapon;
            bool equipped = gear && tier > 0 && Profile.EquippedGear(m.kind) == m.id;

            var icon = t.Find("Icon").GetComponent<Image>();
            icon.sprite = statIcons[(int)m.stats[0].stat];
            icon.color = state == ModuleShop.State.Locked ? new Color(1f, 1f, 1f, 0.25f) : accent;

            var frame = t.Find("Frame").GetComponent<Image>();
            frame.color = equipped ? UiTheme.Teal : tier > 0 ? new Color(accent.r, accent.g, accent.b, 0.6f) : new Color(1f, 1f, 1f, 0.12f);

            t.Find("Name").GetComponent<Text>().text = tier > 0 ? $"{m.name}  <color=#22D4C4>Mk {Roman[tier]}</color>" : m.name;
            t.Find("Stats").GetComponent<Text>().text = tier == 0 ? $"Mk I: {m.Describe(1)}"
                : tier < ModuleDef.Tiers ? $"{m.Describe(tier)}   >   {m.Describe(tier + 1)}"
                : m.Describe(tier);

            var pips = t.Find("Pips");
            for (int i = 0; i < pips.childCount; i++)
                pips.GetChild(i).GetComponent<Image>().color = i < tier ? accent : new Color(1f, 1f, 1f, 0.15f);

            t.Find("Lock").gameObject.SetActive(state == ModuleShop.State.Locked);

            var buy = t.Find("Buy").GetComponent<Button>();
            var buyLabel = buy.transform.Find("Label").GetComponent<Text>();
            var (bio, cores) = ModuleShop.NextPrice(m);
            string price = cores > 0 ? $"{bio} bio + {cores} cores" : $"{bio} bio";
            switch (state)
            {
                case ModuleShop.State.Locked: buyLabel.text = "LOCKED"; buy.interactable = false; break;
                case ModuleShop.State.Maxed: buyLabel.text = "MAX"; buy.interactable = false; break;
                case ModuleShop.State.Buy: buyLabel.text = $"BUY\n<size=20>{price}</size>"; buy.interactable = ModuleShop.CanAfford(m); break;
                default: buyLabel.text = $"UPGRADE\n<size=20>{price}</size>"; buy.interactable = ModuleShop.CanAfford(m); break;
            }
            buy.onClick.AddListener(() => ModuleShop.TryBuyNext(m));

            var equip = t.Find("Equip").GetComponent<Button>();
            equip.gameObject.SetActive(gear && tier > 0);
            equip.transform.Find("Label").GetComponent<Text>().text = equipped ? "ON" : "EQUIP";
            equip.interactable = !equipped;
            equip.onClick.AddListener(() => Profile.EquipGear(m.kind, m.id));
            return row;
        }
    }
}
