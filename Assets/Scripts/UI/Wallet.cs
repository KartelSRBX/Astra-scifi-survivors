using UnityEngine;

namespace GamingApp.UI
{
    /// <summary>Meta currencies persisted between runs.</summary>
    public static class Wallet
    {
        public static event System.Action Changed;

        public static int Biomass
        {
            get => PlayerPrefs.GetInt("wallet_biomass", 0);
            set { PlayerPrefs.SetInt("wallet_biomass", Mathf.Max(0, value)); Changed?.Invoke(); }
        }

        public static int Cores
        {
            get => PlayerPrefs.GetInt("wallet_cores", 0);
            set { PlayerPrefs.SetInt("wallet_cores", Mathf.Max(0, value)); Changed?.Invoke(); }
        }
    }
}
