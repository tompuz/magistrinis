using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Upload
{
    public class DeselectPhotoButton : MonoBehaviour
    {
        private Button _btn;

        protected void Start()
        {
            _btn = gameObject.GetComponent<Button>();
            if (GlobalItems.PictureBytes == null)
            {
                _btn.enabled = false;
            }
            else
            {
                _btn.onClick.AddListener(DeselectImage);
            }
        }

        private void DeselectImage()
        {
            GlobalItems.PictureBytes = null;
            _btn.enabled = false;
        }
    }
}
