using UnityEngine;
using GamingApp.Meta;
using GamingApp.UI;
using GamingApp.Weapons;

namespace GamingApp.Core
{
    /// <summary>Dresses the battle scene for the selected star system and announces it.</summary>
    public class SystemApplier : MonoBehaviour
    {
        [SerializeField] Renderer ground;
        [SerializeField] Camera cam;
        [SerializeField] float floorTileWorldSize = 16f;
        [SerializeField] float groundSize = 400f;

        void Awake()
        {
            var sys = RunSetup.System;
            if (sys == null) return;
            if (sys.floor != null)
            {
                var mat = ground.material;
                mat.mainTexture = sys.floor;
                mat.color = Color.white;
                float tiles = groundSize / floorTileWorldSize;
                mat.mainTextureScale = new Vector2(tiles, tiles);
            }
            cam.backgroundColor = sys.floorColor;
        }

        void Start()
        {
            var sys = RunSetup.System;
            if (sys != null) Banner.Show($"SYSTEM {sys.Number}", sys.displayName.ToUpper(), sys.starColor);
        }
    }
}
