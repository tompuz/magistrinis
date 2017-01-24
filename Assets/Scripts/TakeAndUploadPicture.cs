using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Enums;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Image = Vuforia.Image;

namespace Assets.Scripts
{
    public class TakeAndUploadPicture : MonoBehaviour, ITrackerEventHandler
    {
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;
        private bool m_RegisteredFormat = false;
        private bool m_LogInfo = true;

        static bool hideButton = false;

        public Texture _buttonImage;

        protected void Start()
        {
            //VuforiaBehaviour.Instance.RegisterTrackerEventHandler(this);
        }

        public void OnInitialized()
        {
            //throw new NotImplementedException();
        }

        public void OnTrackablesUpdated()
        {
            if (!m_RegisteredFormat)
            {
                CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
                m_RegisteredFormat = true;
            }
            if (m_LogInfo)
            {
                CameraDevice cam = CameraDevice.Instance;
                Image image = cam.GetCameraImage(m_PixelFormat);
                if (image == null)
                {
                    Debug.Log(m_PixelFormat + " image is not available yet");
                }
                else
                {
                    string s = m_PixelFormat + " image: \n";
                    s += "  size: " + image.Width + "x" + image.Height + "\n";
                    s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
                    s += "  stride: " + image.Stride;
                    Debug.Log(s);
                    m_LogInfo = false;
                }
            }
        }



        private void TakeScreenshot()
        {
            try
            {
                CameraDevice cam = CameraDevice.Instance;
                Image image = cam.GetCameraImage(m_PixelFormat);
                if (image == null)
                {
                    Debug.Log(m_PixelFormat + " image is not available yet");
                }
                else
                {
                    string s = m_PixelFormat + " image: \n";
                    s += "  size: " + image.Width + "x" + image.Height + "\n";
                    s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
                    s += "  stride: " + image.Stride;
                    Debug.Log(s);

                    Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
                    image.CopyToTexture(tex);
                    // tex.ReadPixels (new Rect(0, 0, image.Width, image.Height), 0, 0);
                    tex.Apply();

                    GlobalItems.PictureBytes = tex.EncodeToPNG();
                    Destroy(tex);

//                    if (!Directory.Exists(Application.persistentDataPath + "/Screenshots"))
//                    {
//                        Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots");
//                    }

                    //File.WriteAllBytes(string.Format(Application.persistentDataPath + "/Screenshots/{0}.png", DateTime.Now.ToString("yyyyMMddss")), GlobalItems.PictureBytes);

                    //AndroidJavaObject java = new AndroidJavaObject("com.myapp.commu.Manager");

                   // java.Call("saveImageToGallery", GlobalItems.PictureBytes, string.Format("screenshot_{0}.png", DateTime.Now), "test");


//                    using (var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass))
//                    {
//                        using (var cr = Activity.Call<AndroidJavaObject>("getContentResolver"))
//                        {
//                            var image = Texture2DToAndroidBitmap(texture2D);
//                            var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, image, title, description);
//                            return imageUrl;
//                        }
//                    }

                    SceneLoader.LoadScene(CreatedScene.UploadRecomendations);
                    // For testing purposes, also write to a file in the project folder
                    //File.WriteAllBytes(Application.persistentDataPath + "/Screenshot.png", bytes);

                    // now use Unity's built in
                    //Application.CaptureScreenshot(string.Format("UnityScreenshot_{0}.png", DateTime.Now));
                }
            }
            catch(Exception exc)
            {
                MobileMessage.ShowMessage("Klaida", exc.Message);
            }
        }

        private static AndroidJavaObject _activity;

        public static AndroidJavaObject Activity
        {
            get
            {
                if (_activity == null)
                {
                    var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _activity;
            }
        }

        public static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
        {
            byte[] encoded = texture2D.EncodeToPNG();
            using (var bf = new AndroidJavaClass("android.graphics.BitmapFactory"))
            {
                return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
            }
        }

        protected void OnGUI()
        {
            if (!hideButton)
            {
                int positionX = Screen.width / 3;
                int positionY = Screen.height - positionX / 2;

                if (GUI.Button(new Rect(positionX, positionY, positionX / 2, positionX / 2), _buttonImage))
                {
                    TakeScreenshot();
                }
            }
        }
    }
}
