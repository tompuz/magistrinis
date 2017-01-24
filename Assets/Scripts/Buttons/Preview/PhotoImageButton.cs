using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Preview
{
    public class PhotoImageButton : MonoBehaviour
    {
        private Vector2 startingVector;
        private int width = 100;
        private int height = 100;
        private bool fullPreview = false;

        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        private Button _btn;
        private RectTransform _transform;

        public void Start()
        {
            _btn = gameObject.GetComponent<Button>();
            _transform = gameObject.GetComponent<RectTransform>();
            _btn.onClick.AddListener(FullPreview);
            startingVector = _transform.position;
        }

        public void FullPreview()
        {
            if (!fullPreview)
            {
                _transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
                fullPreview = true;
                _transform.localScale = new Vector3(5,8,1);
            }
            else
            {
                _transform.position = startingVector;
                fullPreview = false;
                _transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
