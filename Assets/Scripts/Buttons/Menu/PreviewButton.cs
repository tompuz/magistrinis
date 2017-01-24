using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Menu
{
    public class PreviewButton : MonoBehaviour
    {
        private Button _btn;

        protected void Start()
        {
            _btn = gameObject.GetComponent<Button>();
            _btn.onClick.AddListener(() =>
            {
                SceneLoader.LoadScene(CreatedScene.Preview);
            });
        }
    }
}
