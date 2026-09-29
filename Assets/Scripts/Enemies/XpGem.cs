using UnityEngine;
using GamingApp.Player;

namespace GamingApp.Enemies
{
    /// <summary>Dropped by enemies; flies to the player once inside the pickup radius.</summary>
    public class XpGem : MonoBehaviour
    {
        [SerializeField] float flySpeed = 12f;

        int value = 1;
        static PlayerExperience player;

        public void Init(int xp) => value = xp;

        void Update()
        {
            if (player == null) player = FindFirstObjectByType<PlayerExperience>();
            if (player == null) return;

            Vector3 to = player.transform.position - transform.position;
            if (to.magnitude > player.PickupRadius) return;

            transform.position += to.normalized * (flySpeed * Time.deltaTime);
            if (to.magnitude < 0.4f)
            {
                player.AddXp(value);
                Destroy(gameObject);
            }
        }
    }
}
