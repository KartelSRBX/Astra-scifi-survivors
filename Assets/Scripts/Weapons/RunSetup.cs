using UnityEngine;

namespace GamingApp.Weapons
{
    /// <summary>Choices made in the menu before a run (currently the weapon).</summary>
    public static class RunSetup
    {
        public static string WeaponId
        {
            get => PlayerPrefs.GetString("run_weapon", "blaster");
            set { PlayerPrefs.SetString("run_weapon", value); PlayerPrefs.Save(); }
        }

        public static int SystemIndex
        {
            get => PlayerPrefs.GetInt("run_system", 0);
            set { PlayerPrefs.SetInt("run_system", value); PlayerPrefs.Save(); }
        }

        /// <summary>Selected star system, or null before the database exists.</summary>
        public static GamingApp.Meta.StarSystemData System
        {
            get
            {
                var db = GamingApp.Meta.GameDatabase.Get();
                if (db == null || db.systems.Count == 0) return null;
                return db.systems[Mathf.Clamp(SystemIndex, 0, db.systems.Count - 1)];
            }
        }
    }
}
