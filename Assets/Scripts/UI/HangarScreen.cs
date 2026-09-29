using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;

namespace GamingApp.UI
{
    /// <summary>Browse ships: buy, select, and upgrade the hull level with Biomass.</summary>
    public class HangarScreen : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] Button openButton;
        [SerializeField] Button closeButton;
        [SerializeField] Button prevButton;
        [SerializeField] Button nextButton;
        [SerializeField] Button actionButton;
        [SerializeField] Text actionLabel;
        [SerializeField] Button upgradeButton;
        [SerializeField] Text upgradeLabel;
        [SerializeField] Image shipImage;
        [SerializeField] Text nameText;
        [SerializeField] Text classText;
        [SerializeField] Text statsText;
        [SerializeField] Text levelText;
        [SerializeField] Text counterText;

        GameDatabase db;
        int index;

        void Awake()
        {
            db = GameDatabase.Get();
            root.SetActive(false);
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(() => root.SetActive(false));
            prevButton.onClick.AddListener(() => { index = Mathf.Max(0, index - 1); Refresh(); });
            nextButton.onClick.AddListener(() => { index = Mathf.Min(db.ships.Count - 1, index + 1); Refresh(); });
            actionButton.onClick.AddListener(OnAction);
            upgradeButton.onClick.AddListener(OnUpgrade);
            Profile.Changed += RefreshIfOpen;
            Wallet.Changed += RefreshIfOpen;
        }

        void OnDestroy()
        {
            Profile.Changed -= RefreshIfOpen;
            Wallet.Changed -= RefreshIfOpen;
        }

        void Open()
        {
            index = Mathf.Max(0, db.ShipIndex(Profile.SelectedShip));
            root.SetActive(true);
            Refresh();
        }

        void RefreshIfOpen() { if (root.activeSelf) Refresh(); }

        void Refresh()
        {
            var s = db.ships[index];
            bool owned = Profile.OwnsShip(s.id);
            bool selected = Profile.SelectedShip == s.id;
            int level = Profile.ShipLevel(s.id);
            var l = Loadout.Compute(db, s);

            shipImage.sprite = s.sprite;
            shipImage.color = owned ? Color.white : new Color(0.45f, 0.5f, 0.65f, 1f);
            nameText.text = s.displayName.ToUpper();
            classText.text = s.shipClass;
            counterText.text = $"{index + 1} / {db.ships.Count}";
            statsText.text = $"{l.maxHealth:0}\n{l.moveSpeed:0.0}\nx{l.damageMult:0.00}\nx{l.fireRateMult:0.00}";
            levelText.text = owned ? $"LEVEL {level} / {ShipData.MaxLevel}" : "NOT OWNED";

            if (selected) { actionLabel.text = "SELECTED"; actionButton.interactable = false; }
            else if (owned) { actionLabel.text = "SELECT"; actionButton.interactable = true; }
            else
            {
                actionLabel.text = s.priceCores > 0 ? $"BUY  {s.priceBiomass} BIO + {s.priceCores} CORES" : $"BUY  {s.priceBiomass} BIOMASS";
                actionButton.interactable = Wallet.Biomass >= s.priceBiomass && Wallet.Cores >= s.priceCores;
            }

            upgradeButton.gameObject.SetActive(owned);
            if (level >= ShipData.MaxLevel) { upgradeLabel.text = "MAX LEVEL"; upgradeButton.interactable = false; }
            else
            {
                int cost = s.UpgradeCost(level, index);
                upgradeLabel.text = $"UPGRADE  {cost} BIOMASS";
                upgradeButton.interactable = Wallet.Biomass >= cost;
            }
            prevButton.interactable = index > 0;
            nextButton.interactable = index < db.ships.Count - 1;
        }

        void OnAction()
        {
            var s = db.ships[index];
            if (!Profile.OwnsShip(s.id))
            {
                if (Wallet.Biomass < s.priceBiomass || Wallet.Cores < s.priceCores) return;
                Wallet.Biomass -= s.priceBiomass;
                Wallet.Cores -= s.priceCores;
                Profile.AddShip(s.id);
            }
            Profile.SelectShip(s.id);
        }

        void OnUpgrade()
        {
            var s = db.ships[index];
            int level = Profile.ShipLevel(s.id);
            int cost = s.UpgradeCost(level, index);
            if (level >= ShipData.MaxLevel || Wallet.Biomass < cost) return;
            Wallet.Biomass -= cost;
            Profile.SetShipLevel(s.id, level + 1);
        }
    }
}
