using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Enums;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

    public WebCamTexture mCamera = null;
    public GameObject plane;

    // Use this for initialization
    protected void Start()
    {
        Debug.Log("Script has been started");
        plane = GameObject.FindWithTag("Player");

        mCamera = new WebCamTexture();
        plane.GetComponent<Renderer>().material.mainTexture = mCamera;
        mCamera.Play();

    }

    // Update is called once per frame
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mCamera.Stop();
            SceneLoader.LoadScene(CreatedScene.Menu);
        }
    }

    //if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
}
