using System;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.Data.SqlClient;
using Assets.Scripts;
using Assets.Scripts.Enums;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

public class ButtonClicked : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(CreatedScene scene)
    {
        SceneLoader.LoadScene(scene);
    }

    public void onClick()
    {
        SceneLoader.LoadScene(CreatedScene.UploadRecomendations);
    }

    public void LoadPreview()
    {
        SceneLoader.LoadScene(CreatedScene.Preview);
    }

    public void onBackClicked()
    {
        SceneLoader.LoadScene(CreatedScene.Menu);
    }

    public void onOpenCameraClicked()
    {
        SceneLoader.LoadScene(CreatedScene.AugmentedCamera);
    }

    public void LoadCoordinates()
    {
        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            new WaitForSeconds(1);
            maxWait--;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            var obj = GameObject.Find("GPSText");
            Text guiText = obj.GetComponent<Text>();
            guiText.text = "Unable to determine device location";
        }
        else
        {
            var obj = GameObject.Find("GPSText");
            Text guiText = obj.GetComponent<Text>();

            guiText.text =
                Input.location.lastData.latitude + " " +
                Input.location.lastData.longitude + " " +
                Input.location.lastData.altitude + " " +
                Input.location.lastData.horizontalAccuracy + " " +
                Input.location.lastData.timestamp;
        }
        Input.location.Stop();
    }

    private void Validate()
    {
        var obj = GameObject.Find("InputField");
        Text[] guiText = obj.GetComponentsInChildren<Text>();
        var decription = guiText[1].text;
        if (string.IsNullOrEmpty(decription))
        {

        }
    }

    public void SubmitRecomendation()
    {
        try
        {
            Validate();
            var obj = GameObject.Find("InputField");
            Text[] guiText = obj.GetComponentsInChildren<Text>();
            var decription = guiText[1].text;
            if (string.IsNullOrEmpty(decription))
            {
               
            }

        string connectionString = "";

        



        if (!string.IsNullOrEmpty(decription))
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.ConnectionString =
                  "Network Library=dbmssocn;" +
                  "Data Source=37.157.149.70,1433;" +
                  "Initial Catalog=Magistrinis;" +
                  "User Id=User;" +
                  "Password=Paslaptis;";
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"insert into dbo.ImageObjects (Photo,Angle,Distance,Description) 
                                    values (@photo, @angle, @distance, @description)";

                cmd.Parameters.Add("@photo", SqlDbType.Image).Value = GlobalItems.PictureBytes;
                cmd.Parameters.Add("@angle", SqlDbType.Int).Value = 10;
                cmd.Parameters.Add("@distance", SqlDbType.Decimal).Value = 10;
                cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = decription;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            }

        }
        catch (Exception exc)
        {
            //MessageBox.Show(exc.Message,"Klaida", MessageBoxButtons.OK);
        }
       // MessageBox.Show("test", "Klaida", MessageBoxButtons.OK);
    }

    public string GetConnectionPath()
    {
        if (Application.platform == RuntimePlatform.Android)
            return "URI=file:" + Application.persistentDataPath + "/" + "Database.db";
        else
            return "URI=file:" + Application.dataPath + "/" + "Database.db";
    }
}