using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Enums;
using Assets.Scripts.VuforiaApi;
using Assets.Scripts.VuforiaApi.VuforiaData;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.Scenes
{
    public class PreviewScene : MonoBehaviour
    {
        public Dropdown dropDownList;
        public InputField DescriptionField;
        public Image ImageField;
        public List<Sprite> Sprites;
        public Image Image;
        public IObjectsManager _objectsManager = new ObjectsManager();
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        public IGetTarget target;

        protected void Start()
        {
            target = new GetTarget("Knyga");
            //var objects = FindObjectsOfType<Image>();
            //target.UnityReq();
            //var coroutine = target.GetAllTargets(UpdateTargets);
            //coroutine.MoveNext();
            //coroutine.MoveNext()
            
            StartCoroutine(target.GetAllTargets(UpdateTargets));
            //StartCoroutine(target.GetSingleTarget("d4276b7fb6de4100b34032fe2e3a1f24"));

        }

        public void UpdateTargets(object parameter)
        {
            AndroidNativeUtility.ShowPreloader("AR", "Kraunasi");
            try
            {
                List<string> vuforiaList = (List<string>) parameter;
                var dbObjects = _objectsManager.GetDatabaseObjects();
                foreach (var vuforiaId in vuforiaList)
                {
                    if (!dbObjects.Select(o => o.TransactionId).Contains(vuforiaId))
                    {
                        Debug.Log("vuforia id to be processed: " + vuforiaId);
                        StartCoroutine(target.GetSingleTarget(vuforiaId, UpdateSingleTarget));
                    }
                }
                RefreshObjectsList();
            }
            catch (Exception exc)
            {
                Debug.Log(exc);
                MobileMessage.ShowMessage("Klaida", exc.Message);
            }
            finally
            {
                AndroidNativeUtility.HidePreloader();
            }
        }

        public void UpdateSingleTarget(GetTarget.TargetRecord target)
        {
            Debug.Log(string.Format("Updating record transactionId({0}), name({1}), width({2}), rating({3}) ",target.target_id ,target.name, target.width, target.tracking_rating));
            if (target.active_flag)
            {
                _objectsManager.UpdateObject(target.name, target.target_id, target.tracking_rating);
                RefreshObjectsList();
                if (UnprocessedTransactions.TransactionIdList.Contains(target.target_id))
                    UnprocessedTransactions.TransactionIdList.Remove(target.target_id);
            }
            else
            {
                if (!UnprocessedTransactions.TransactionIdList.Contains(target.target_id))
                    UnprocessedTransactions.TransactionIdList.Add(target.target_id);
            }
        }

        public void RefreshObjectsList()
        {
            var objects = _objectsManager.GetActiveObjects();
            dropDownList.ClearOptions();
            VuforiaObjects.TransactionIdList.Clear();
            List<string> optionsList = new List<string>();
            foreach (var obj in objects)
            {
                optionsList.Add(obj.Name);
                VuforiaObjects.TransactionIdList.Add(obj.Name, obj.TransactionId);
            }
            dropDownList.AddOptions(optionsList);
            if (dropDownList.options.Count > 0)
            {
                Debug.Log("dropDownList selected text: " + dropDownList.options[0].text);
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

        // Update is called once per frame
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneLoader.LoadScene(CreatedScene.Menu);
        }
    }
}
