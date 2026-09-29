using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamingApp.Meta
{
    /// <summary>Persistent player progress: owned ships and their levels, item inventory, equipped items.</summary>
    public static class Profile
    {
        [Serializable]
        class Data
        {
            public List<string> ships = new();
            public List<string> shipLevelIds = new();
            public List<int> shipLevels = new();
            public string ship = "corvette";
            public List<string> items = new();
            public int[] equipped = { -1, -1, -1, -1 }; // index into items, per ItemSlot
            public List<float> systemClearance = new(); // 0..1 per star system
            public List<float> systemBest = new();      // best survival seconds per system
            public List<string> moduleIds = new();
            public List<int> moduleTiers = new();       // 1..5 (Mk I..V)
            public string armor = "", engine = "", sensor = "";
        }

        const string Key = "profile_v1";
        static Data data;

        public static event Action Changed;

        static Data D
        {
            get
            {
                if (data != null) return data;
                string json = PlayerPrefs.GetString(Key, "");
                data = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<Data>(json);
                if (data == null || data.ships.Count == 0) data = CreateDefault();
                if (data.equipped == null || data.equipped.Length != 4) data.equipped = new[] { -1, -1, -1, -1 };
                return data;
            }
        }

        static Data CreateDefault()
        {
            var d = new Data();
            d.ships.Add("corvette");
            d.items.AddRange(new[] { "pulse_blaster", "composite_plating", "ion_thruster", "twin_laser", "magnet_coil" });
            d.equipped = new[] { 0, 1, 2, -1 };
            return d;
        }

        static void Save()
        {
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(D));
            PlayerPrefs.Save();
            Changed?.Invoke();
        }

        // ---- ships ----
        public static string SelectedShip => D.ship;
        public static bool OwnsShip(string id) => D.ships.Contains(id);

        public static void SelectShip(string id)
        {
            if (!OwnsShip(id)) return;
            D.ship = id;
            Save();
        }

        public static void AddShip(string id)
        {
            if (!OwnsShip(id)) D.ships.Add(id);
            Save();
        }

        public static int ShipLevel(string id)
        {
            int i = D.shipLevelIds.IndexOf(id);
            return i < 0 ? 1 : D.shipLevels[i];
        }

        public static void SetShipLevel(string id, int level)
        {
            int i = D.shipLevelIds.IndexOf(id);
            if (i < 0) { D.shipLevelIds.Add(id); D.shipLevels.Add(level); }
            else D.shipLevels[i] = level;
            Save();
        }

        // ---- star systems ----
        public static float SystemClearance(int i) => i >= 0 && i < D.systemClearance.Count ? D.systemClearance[i] : 0f;
        public static float SystemBest(int i) => i >= 0 && i < D.systemBest.Count ? D.systemBest[i] : 0f;
        public static bool IsSystemUnlocked(int i) => i == 0 || SystemClearance(i - 1) >= 1f;

        /// <summary>Clearance keeps the best result: surviving the full run clears the system (100%).</summary>
        public static void RecordSystem(int i, float clearance, float seconds)
        {
            while (D.systemClearance.Count <= i) D.systemClearance.Add(0f);
            while (D.systemBest.Count <= i) D.systemBest.Add(0f);
            D.systemClearance[i] = Mathf.Max(D.systemClearance[i], Mathf.Clamp01(clearance));
            D.systemBest[i] = Mathf.Max(D.systemBest[i], seconds);
            Save();
        }

        // ---- shop modules ----
        public static int ModuleTier(string id)
        {
            int i = D.moduleIds.IndexOf(id);
            return i < 0 ? 0 : D.moduleTiers[i];
        }

        public static void SetModuleTier(string id, int tier)
        {
            int i = D.moduleIds.IndexOf(id);
            if (i < 0) { D.moduleIds.Add(id); D.moduleTiers.Add(tier); }
            else D.moduleTiers[i] = tier;
            Save();
        }

        public static string EquippedGear(ModuleKind kind) => kind switch
        {
            ModuleKind.Armor => D.armor,
            ModuleKind.Engine => D.engine,
            ModuleKind.Sensor => D.sensor,
            _ => "",
        };

        public static void EquipGear(ModuleKind kind, string id)
        {
            if (kind == ModuleKind.Armor) D.armor = id;
            else if (kind == ModuleKind.Engine) D.engine = id;
            else if (kind == ModuleKind.Sensor) D.sensor = id;
            Save();
        }

        // ---- items ----
        public static IReadOnlyList<string> Items => D.items;

        public static void AddItem(string id)
        {
            D.items.Add(id);
            Save();
        }

        public static int EquippedIndex(ItemSlot slot) => D.equipped[(int)slot];
        public static bool IsEquipped(int itemIndex) => Array.IndexOf(D.equipped, itemIndex) >= 0;

        public static void Equip(int itemIndex, ItemSlot slot)
        {
            D.equipped[(int)slot] = itemIndex;
            Save();
        }

        public static void Unequip(ItemSlot slot)
        {
            D.equipped[(int)slot] = -1;
            Save();
        }

        public static ItemData Equipped(ItemSlot slot, GameDatabase db)
        {
            int i = EquippedIndex(slot);
            return i >= 0 && i < D.items.Count ? db.Item(D.items[i]) : null;
        }
    }
}
