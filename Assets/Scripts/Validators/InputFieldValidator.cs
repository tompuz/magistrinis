using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Validators
{
    public class InputFieldValidator : MonoBehaviour
    {
        public InputField.CharacterValidation Validation;
        public TouchScreenKeyboardType KeyboardType;
        public Text RequiredText;
        public static Dictionary<string, string> Text = new Dictionary<string, string>();
        public bool Valid;

        // Use this for initialization
        public void Start ()
        {
            if(!Text.ContainsKey(name))
                Text.Add(name, string.Empty);

            var obj = GetComponent<InputField>();
            obj.characterValidation = Validation;
            obj.onValueChanged.AddListener(TextChanged);
            obj.keyboardType = KeyboardType;
            obj.text = Text[name];
            Valid = obj.text != string.Empty;
            if(!Valid)
                RequiredText.enabled = true;
        }

        private void TextChanged(string text)
        {
            RequiredText.enabled = string.IsNullOrEmpty(text);
            Text[name] = text;
            Valid = Text[name] != string.Empty;
        }


//        // Update is called once per frame
//        protected void Update ()
//        {
//	
//        }
    }
}
