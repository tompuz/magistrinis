using System.Collections;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class SceneLoader
    {
        public static void Exit()
        {
            UnityEngine.Application.Quit();
        }

        public static void LoadScene(CreatedScene scene)
        {
            AndroidNativeUtility.ShowPreloader("AR", "Kraunasi");
            SceneManager.LoadScene(scene.ToString());
        }
    }
}
