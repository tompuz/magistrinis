using System;
using Vuforia;

namespace Assets.Scripts
{
    using UnityEngine;
    using System.IO;

    public class TakePicture : MonoBehaviour, ITrackerEventHandler
    {
        private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;
        private bool m_RegisteredFormat = false;
        private bool m_LogInfo = true;

        static bool hideButton = false;


        void Start()
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

        void TakeScreenshot()
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

                byte[] bytes = tex.EncodeToPNG();
                Destroy(tex);

                // For testing purposes, also write to a file in the project folder
                //File.WriteAllBytes(Application.persistentDataPath + "/Screenshot.png", bytes);


                // now use Unity's built in
                Application.CaptureScreenshot(string.Format("UnityScreenshot_{0}.png",DateTime.Now));
                
            }

        }

        void OnGUI()
        {

            if (!TakePicture.hideButton)
            {
                int positionX = Screen.width/3;
                int positionY = Screen.height - 40;

                if (GUI.Button(new Rect(positionX, positionY, 160, 40), "Take Picture"))
                {
                    TakePicture.hideButton = true;
                    TakeScreenshot();
                }
            }
        }
    }
}
