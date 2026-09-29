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
    }
}
