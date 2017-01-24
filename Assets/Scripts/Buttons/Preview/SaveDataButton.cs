using System;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Preview
{
    internal class SaveDataButton : MonoBehaviour
    {
        public Dropdown dropDownList;
        public InputField DescriptionField;
        private Button _btn;
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();

        protected void Start()
        {
            _btn = gameObject.GetComponent<Button>();
            _btn.onClick.AddListener(SaveData);
        }

        private void SaveData()
        {
            try
            {
                var objName = dropDownList.options[dropDownList.value].text;
                var description = DescriptionField.text;
                Debug.Log(string.Format("Name = {0}, Description = {1}",objName, description));
                ObjectsManager.UpdateObjectDescription(objName, description);
                MobileMessage.ShowMessage("AR","Pakeitimai išsaugoti");
            }
            catch (Exception exc)
            {
                MobileMessage.ShowMessage("Klaida", exc.Message);
            }
        }
    }
}
