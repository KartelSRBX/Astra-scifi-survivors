using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamingApp.UI
{
    /// <summary>Scene names as listed in Build Settings.</summary>
    public static class SceneLoader
    {
        public const string Menu = "Menu";
        public const string Game = "Main";

        public static void Load(string scene)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
        }
    }
}
