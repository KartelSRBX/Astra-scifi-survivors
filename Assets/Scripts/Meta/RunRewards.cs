using UnityEngine;
using GamingApp.UI;

namespace GamingApp.Meta
{
    /// <summary>Converts a finished run into Biomass, Cores and an optional item drop.</summary>
    public static class RunRewards
    {
        public struct Result
        {
            public int biomass, cores;
            public ItemData item;
        }

        public static Result Last { get; private set; }

        static readonly float[] RarityWeights = { 60f, 28f, 10f, 2f };

        public static Result Grant(int kills, int bossKills, float seconds, bool won)
        {
            var r = new Result
            {
                biomass = Mathf.RoundToInt((kills + seconds / 5f) * (won ? 1.5f : 1f)),
                cores = bossKills * 5 + (won ? 5 : 0),
            };
            var db = GameDatabase.Get();
            if (db != null && db.items.Count > 0 && (won || Random.value < 0.35f))
            {
                r.item = RollItem(db);
                Profile.AddItem(r.item.id);
            }
            Wallet.Biomass += r.biomass;
            Wallet.Cores += r.cores;
            PlayerPrefs.Save();
            Last = r;
            return r;
        }

        static ItemData RollItem(GameDatabase db)
        {
            float total = 0f;
            foreach (var it in db.items) total += RarityWeights[(int)it.rarity];
            float roll = Random.value * total;
            foreach (var it in db.items)
            {
                roll -= RarityWeights[(int)it.rarity];
                if (roll <= 0f) return it;
            }
            return db.items[0];
        }
    }
}
