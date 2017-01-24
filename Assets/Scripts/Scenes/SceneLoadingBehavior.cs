using UnityEngine;

namespace Assets.Scripts.Scenes
{
    internal class SceneLoadingBehavior : MonoBehaviour
    {
        protected void Start()
        {
            AndroidNativeUtility.HidePreloader();
        }
    }
}
