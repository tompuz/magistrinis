using Assets.Scripts.Wrappers;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class MenuScene : MonoBehaviour
    {
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        protected void Start()
        {
//            if (!AndroidNativePlugin.IsMobileConnected())
//            {
//                MobileMessage.ShowMessage("AR", "Prašome įjungti internetą");
//            }
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.Exit();
        }
    }
}
