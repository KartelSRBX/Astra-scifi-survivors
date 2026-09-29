using System.Collections.Generic;
using UnityEngine;
using GamingApp.Weapons;

namespace GamingApp.Meta
{
    /// <summary>All ships and items, loaded from Resources so both scenes can resolve saved ids.</summary>
    public class GameDatabase : ScriptableObject
    {
        public List<ShipData> ships = new();
        public List<ItemData> items = new();
        public List<WeaponDef> weapons = new();
        public List<StarSystemData> systems = new();

        static GameDatabase instance;
        public static GameDatabase Get()
        {
            if (instance == null) instance = Resources.Load<GameDatabase>("GameDatabase");
            return instance;
        }

        public ShipData Ship(string id) => ships.Find(s => s.id == id);
        public int ShipIndex(string id) => ships.FindIndex(s => s.id == id);
        public ItemData Item(string id) => items.Find(i => i.id == id);
        public WeaponDef Weapon(string id) => weapons.Find(w => w.id == id);
    }
}
