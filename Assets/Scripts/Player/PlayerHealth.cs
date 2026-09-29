using UnityEngine;
using GamingApp.Core;

namespace GamingApp.Player
{
    /// <summary>Bridges the player's Health to the run state.</summary>
    [RequireComponent(typeof(Health))]
    public class PlayerHealth : MonoBehaviour
    {
        void Start() => GetComponent<Health>().Died += _ => GameManager.Instance.PlayerDied();
    }
}
