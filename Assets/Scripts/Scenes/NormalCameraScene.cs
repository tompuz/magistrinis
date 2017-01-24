using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Wrappers;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    class NormalCameraScene : MonoBehaviour
    {
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        protected void Start()
        {
            AndroidCamera.Instance.OnImagePicked += ImageChoosen;
            AndroidCamera.Instance.GetImageFromCamera();
        }

        private void ImageChoosen(AndroidImagePickResult result)
        {
            if (result.IsSucceeded)
            {
                GlobalItems.PictureBytes = result.Image.EncodeToPNG();
                SceneLoader.LoadScene(CreatedScene.UploadRecomendations);
            }
            else
                MobileMessage.ShowMessage("Klaida","Nepavyko passirinkti nuotraukos!");
        }

        // Update is called once per frame
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.LoadScene(CreatedScene.UploadRecomendations);
        }
    }
}
