using System;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Upload
{
    public class OpenNormalCameraButton : MonoBehaviour
    {
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        public Button BtnCamera;
        public Button BtnPreviewPhoto;

        protected void Start()
        {
            AndroidCamera.Instance.OnImagePicked += ImagePicked;
            BtnCamera.onClick.AddListener(() =>
            {
                try
                {
                    AndroidCamera.Instance.GetImageFromCamera();
                }
                catch (Exception exc)
                {
                    MobileMessage.ShowMessage("Klaida",exc.Message);
                }
            });
        }

        protected void OnDestroy()
        {
            AndroidCamera.Instance.OnImagePicked -= ImagePicked;
        }

        private void ImagePicked(AndroidImagePickResult result)
        {
            
            try
            {
                if (result.IsSucceeded)
                {
                    BtnPreviewPhoto.gameObject.SetActive(true);
                    GlobalItems.PictureBytes = result.Image.EncodeToJPG(100);
                    MobileMessage.ShowMessage("AR", "Nuotrauka įkelta!");
                }
                else
                    MobileMessage.ShowMessage("Klaida", "Nepavyko passirinkti nuotraukos!");
                }
            catch (Exception exc)
            {
                MobileMessage.ShowMessage("Klaida", exc.Message);
            }
        }
    }
}
