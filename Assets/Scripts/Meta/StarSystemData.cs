using UnityEngine;

namespace GamingApp.Meta
{
    /// <summary>A level: a star system with its own backdrop and floor. Each next system is 15% harder.</summary>
    [CreateAssetMenu(menuName = "GamingApp/Star System", fileName = "System_")]
    public class StarSystemData : ScriptableObject
    {
        public int index;
        public string displayName;
        public Color starColor = Color.white;
        public Color floorColor = Color.black;
        public Sprite systemSprite;
        public Sprite nebulaA;
        public Sprite nebulaB;
        public Texture2D floor;

        public const float DifficultyStep = 0.15f;
        public float Difficulty => 1f + DifficultyStep * index;
        public int Number => index + 1;
    }
}
