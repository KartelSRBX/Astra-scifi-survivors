using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;

namespace GamingApp.UI
{
    /// <summary>Ship loadout (4 slots) plus a grid of owned items; tap an item to equip or unequip it.</summary>
    public class InventoryScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Button openButton;
        [SerializeField] Button closeButton;
        [SerializeField] Image shipImage;
        [SerializeField] Text statsText;

        [Header("Slots, in ItemSlot order")]
        [SerializeField] Button[] slotButtons;
        [SerializeField] Image[] slotIcons;
        [SerializeField] Image[] slotFrames;
        [SerializeField] Sprite[] slotPlaceholders;

        [Header("Grid")]
        [SerializeField] RectTransform grid;
        [SerializeField] GameObject cellTemplate;
        [SerializeField] Text emptyText;

        [Header("Detail popup")]
        [SerializeField] GameObject detailRoot;
        [SerializeField] Button detailDismiss;
        [SerializeField] Image detailIcon;
        [SerializeField] Image detailFrame;
        [SerializeField] Text detailName;
        [SerializeField] Text detailRarity;
        [SerializeField] Text detailStats;
        [SerializeField] Button detailAction;
        [SerializeField] Text detailActionLabel;

        GameDatabase db;
        readonly List<GameObject> cells = new();
        int selected = -1;

        void Awake()
        {
            db = GameDatabase.Get();
            root.SetActive(false);
            detailRoot.SetActive(false);
            cellTemplate.SetActive(false);
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(() => root.SetActive(false));
            detailDismiss.onClick.AddListener(() => detailRoot.SetActive(false));
            detailAction.onClick.AddListener(OnDetailAction);
            for (int s = 0; s < slotButtons.Length; s++)
            {
                var slot = (ItemSlot)s;
                slotButtons[s].onClick.AddListener(() =>
                {
                    int i = Profile.EquippedIndex(slot);
                    if (i >= 0) ShowDetail(i);
                });
            }
        }

        void Open()
        {
            root.SetActive(true);
            Refresh();
        }

        void Refresh()
        {
            var l = Loadout.Compute(db);
            shipImage.sprite = l.ship.sprite;
            statsText.text = $"HULL {l.maxHealth:0}    DMG x{l.damageMult:0.00}    RATE x{l.fireRateMult:0.00}    SPD {l.moveSpeed:0.0}";

            for (int s = 0; s < slotIcons.Length; s++)
            {
                var it = Profile.Equipped((ItemSlot)s, db);
                slotIcons[s].sprite = it != null ? it.icon : slotPlaceholders[s];
                slotIcons[s].color = it != null ? UiTheme.RarityColor(it.rarity) : new Color(1f, 1f, 1f, 0.2f);
                slotFrames[s].color = it != null ? UiTheme.RarityColor(it.rarity) : new Color(1f, 1f, 1f, 0.15f);
            }

            foreach (var c in cells) Destroy(c);
            cells.Clear();
            var items = Profile.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var it = db.Item(items[i]);
                if (it == null) continue;
                var cell = Instantiate(cellTemplate, grid);
                cell.SetActive(true);
                var col = UiTheme.RarityColor(it.rarity);
                var icon = cell.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = it.icon;
                icon.color = col;
                cell.transform.Find("Frame").GetComponent<Image>().color = col;
                cell.transform.Find("Equipped").gameObject.SetActive(Profile.IsEquipped(i));
                int index = i;
                cell.GetComponent<Button>().onClick.AddListener(() => ShowDetail(index));
                cells.Add(cell);
            }
            emptyText.gameObject.SetActive(cells.Count == 0);
        }

        void ShowDetail(int itemIndex)
        {
            var it = db.Item(Profile.Items[itemIndex]);
            if (it == null) return;
            selected = itemIndex;
            var col = UiTheme.RarityColor(it.rarity);
            detailIcon.sprite = it.icon;
            detailIcon.color = col;
            detailFrame.color = col;
            detailName.text = it.displayName.ToUpper();
            detailRarity.text = $"{it.rarity.ToString().ToUpper()}  ·  {it.slot.ToString().ToUpper()}";
            detailRarity.color = col;
            detailStats.text = it.Describe();
            detailActionLabel.text = Profile.EquippedIndex(it.slot) == itemIndex ? "UNEQUIP" : "EQUIP";
            detailRoot.SetActive(true);
        }

        void OnDetailAction()
        {
            var it = db.Item(Profile.Items[selected]);
            if (Profile.EquippedIndex(it.slot) == selected) Profile.Unequip(it.slot);
            else Profile.Equip(selected, it.slot);
            detailRoot.SetActive(false);
            Refresh();
        }
    }
}
