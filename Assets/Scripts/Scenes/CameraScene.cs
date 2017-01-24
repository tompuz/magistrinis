using System;
using System.Collections;
using Assets.Scripts.Enums;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
    public class CameraScene : MonoBehaviour
    {
        public GUIContent ButtonContent;

        IMobileMessageWrapper message = new MobileMessageWrapper();
        private Rect _rect;

        protected void Start()
        {
            int imageWidth = Screen.width/5;
            int imageHeight = Screen.height/15;
            _rect = new Rect(Screen.width / 2 - imageWidth/2, Screen.height - imageHeight, imageWidth, imageHeight);
        }

        // Update is called once per frame
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.LoadScene(CreatedScene.Menu);
        }

        protected void OnDestroy()
        {
            //Destroy(gameObject.GetComponent<Camera>());
        }

        protected void OnGUI()
        {
            if (GUI.Button(_rect, ButtonContent))
            {
                try
                {
                    AndroidCamera.Instance.SaveScreenshotToGallery(string.Format("{0}", DateTime.Now.ToString("yyyyyMMddHHmmss")));
                }
                catch (Exception exc)
                {
                    message.ShowMessage("Klaida", exc.Message);
                }
            }
        }
//
//        public void DestroyAllGameObjects()
//        {
//            GameObject[] GameObjects = FindObjectsOfType<GameObject>();
//
//            foreach (GameObject t in GameObjects)
//            {
//                Destroy(t);
//            }
//        }
    }
}
