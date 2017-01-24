using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataAccess;
using Assets.Scripts.VuforiaApi;
using Assets.Scripts.VuforiaApi.VuforiaData;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Buttons.Preview
{
    public class DeleteButton : MonoBehaviour
    {
        public Dropdown DropDownList;
        public InputField DescriptionField;
        public Image ImageField;
        public List<Sprite> Sprites;
        public Image Image;
        private Button _btn;
        //private AndroidDialog;
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        private readonly IObjectsManager _objectsManager = new ObjectsManager();

        //IDeleteTarget target = new DeleteTarget();
        protected void Start()
        {

            _btn = gameObject.GetComponent<Button>();
            _btn.onClick.AddListener(ConfirmDeleteObject);
        }

        private void ConfirmDeleteObject()
        {
#if UNITY_ANDROID
            var dialog = AndroidDialog.Create("AR", "Ar tikrai norite ištrinti objektą?", "Taip", "Ne");
            dialog.ActionComplete += DeleteObject;
#endif
#if UNITY_EDITOR
            //DeleteObject(AndroidDialogResult.YES);

#endif
        }

        private void DeleteObject(AndroidDialogResult obj)
        {
            if (obj == AndroidDialogResult.YES)
            {
                try
                {
                    var transaction = VuforiaObjects.TransactionIdList.FirstOrDefault(t => t.Key == DropDownList.captionText.text);
                    _objectsManager.DeleteObject(transaction.Value);
                    RefreshObjectsList();
                    MobileMessage.ShowMessage("AR", "Objektas panaikintas");
                }
                catch (Exception exc)
                {
                    MobileMessage.ShowMessage("Klaida", exc.Message);
                    
                }
                //Debug.Log("transacton ID: " + transaction.Value);
                //if (transaction.)
                //{
                //TODO delete object from DB + Vuforia
                //StartCoroutine(target.DeleteSingleTarget(transaction.Value, DeleteFromDatabase));
                //}

            }
        }

        private void DeleteFromDatabase(GetTarget.ResultTarget obj)
        {
            Debug.Log("Deleted: " + obj.result_code);
            MobileMessage.ShowMessage("AR","Object deleted from vuforia");
        }

        public void RefreshObjectsList()
        {
            var objects = _objectsManager.GetActiveObjects();
            DropDownList.ClearOptions();
            VuforiaObjects.TransactionIdList.Clear();
            List<string> optionsList = new List<string>();
            foreach (var obj in objects)
            {
                optionsList.Add(obj.Name);
                VuforiaObjects.TransactionIdList.Add(obj.Name, obj.TransactionId);
            }
            DropDownList.AddOptions(optionsList);
            if (DropDownList.options.Count > 0)
            {
                Debug.Log("dropDownList selected text: " + DropDownList.options[0].text);
                DescriptionField.text = objects.Select(t => t.Description).FirstOrDefault();
                var rating = objects.Select(t => t.Rating).FirstOrDefault();
                ImageField.sprite = rating != -1 ? Sprites[rating] : Sprites[0];
                SetImage(objects.Select(t => t.Photo).FirstOrDefault());
            }
        }

        private void SetImage(byte[] bytes)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, true);
            var img = bytes;
            texture.LoadImage(img);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(5, 5));
            Image.sprite = sprite;
        }
    }
}
