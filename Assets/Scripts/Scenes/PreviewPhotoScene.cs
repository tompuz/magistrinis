using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Assets.Scripts.Enums;
using Assets.Scripts.VuforiaApi;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes
{
    public class PreviewPhotoScene : MonoBehaviour
    {
        protected void Start()
        {
            LoadImage();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.LoadScene(CreatedScene.UploadRecomendations);
        }

        private void LoadImage()
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, true  );
            //var img = TestLoad();
            var img = GlobalItems.PictureBytes;
            //img.
            texture.LoadImage(img);
            GetComponent<RawImage>().texture = texture;
            //var trasnform = GetComponent<RectTransform>();
            //trasnform.Rotate(0,180,270);
            //trasnform.localScale = new Vector3(1.3f, 0.5f);
            //texture.LoadImage(GlobalItems.PictureBytes);
            //GetComponent<Renderer>().material.mainTexture = texture;
//
//            UploadImage upload = new UploadImage("test", TestLoad());
//            StartCoroutine(upload.PostNewTarget());
            //CallPostTarget();
        }

        private byte[] TestLoad()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"select Photo from dbo.ImageObjects where ImageObjectNr = 10";

                var result = cmd.ExecuteScalar();

                conn.Close();

                //File.WriteAllBytes("C:/test.png", (byte[]) result);

                return (byte[]) result;
            }
        }

        public class PostNewTrackableRequest
        {
            public string name;
            public float width;
            public string image;
            public string application_metadata;
        }
    }
}
