using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class RecomendationsScene : MonoBehaviour
    {
        // Update is called once per frame
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.LoadScene(CreatedScene.Menu);
        }
    }
}
