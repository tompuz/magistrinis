using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Validators;
using Assets.Scripts.VuforiaApi;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Upload
{
    public class SubmitButton : MonoBehaviour
    {
        public InputField[] InputFields;
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        public IObjectsManager ObjectsManager = new ObjectsManager();
        public IUploadTarget TargetManager = new UploadImage();

        public void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(Submit);
        }

        private void Submit()
        {
            //GlobalItems.PictureBytes = File.ReadAllBytes("C:/test.png");
            bool valid = InputFields.Select(inputField => inputField.GetComponent<InputFieldValidator>()).All(validator => validator.Valid);
            Debug.Log(valid);
            if (valid)
            {
                if (GlobalItems.PictureBytes == null)
                {
                    MobileMessage.ShowMessage("Klaida", "Trūksta įkeltos nuotraukоs!");
                    return;
                }
                try
                {
                    var description = GameObject.Find("InputField").GetComponentsInChildren<Text>().First(t => t.name == "Text").text;
                    StartCoroutine(TargetManager.PostNewTarget(description, GlobalItems.PictureBytes, InsertTarget));
                }
                catch (Exception exc)
                {
                    MobileMessage.ShowMessage("Klaida", exc.Message);
                }
            }
            else
            {
                MobileMessage.ShowMessage("Klaida", "Neužpildyti visi privalomi laukai");
            }
        }

        public void InsertTarget()
        {
            var description = GameObject.Find("InputField").GetComponentsInChildren<Text>().First(t => t.name == "Text").text;
            var angle =
                int.Parse(
                    GameObject.Find("InputAngle")
                        .GetComponentsInChildren<Text>()
                        .First(t => t.name == "Text")
                        .text);
            var distance =
                decimal.Parse(
                    GameObject.Find("InputDistance")
                        .GetComponentsInChildren<Text>()
                        .First(t => t.name == "Text")
                        .text);
            try
            {
                ObjectsManager.InsertObject(angle, distance, description);
                MobileMessage.ShowMessage("AR", "Objektas sukurtas!");
            }
            catch (Exception exc)
            {
                MobileMessage.ShowMessage("Klaida", exc.Message);
            }
        }
    }
}
