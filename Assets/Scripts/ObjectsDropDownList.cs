using System.Collections.Generic;
using Assets.Scripts.DataAccess;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ObjectsDropDownList : MonoBehaviour
    {
        public Dropdown DropDownList;
        public InputField DescriptionField;
        public Image ImageField;
        public List<Sprite> Sprites;
        public Image Image;
        protected void Start()
        {
            DropDownList.onValueChanged.AddListener(ValueChanged);
        }

        private void ValueChanged(int arg0)
        {
            Debug.Log("dropDownList selected text: " + DropDownList.options[arg0].text);
            var obj = ObjectsManager.GetObjectByName(DropDownList.options[arg0].text);
            DescriptionField.text = obj.Description;
            var sprites = Resources.LoadAll("rating", typeof(Sprite));
            ImageField.sprite = obj.Rating != -1 ? Sprites[obj.Rating] : Sprites[0];
            SetImage(obj.Photo);
        }

        private void SetImage(byte[] bytes)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, true);
            var img = bytes;
            texture.LoadImage(img);
            //Image.texture = texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            Image.sprite = sprite;
        }
    }
}
