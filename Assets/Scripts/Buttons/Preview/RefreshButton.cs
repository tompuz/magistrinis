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
    public class RefreshButton : MonoBehaviour
    {
        public Dropdown DropDownList;
        public InputField DescriptionField;
        public Image ImageField;
        public List<Sprite> Sprites;

        public Button _btn;
        public IGetTarget _target;
        public IObjectsManager _objectsManager = new ObjectsManager();
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();

        public void Start()
        {
            _target = new GetTarget("Knyga");
            _btn = gameObject.GetComponent<Button>();
            _btn.onClick.AddListener(RefreshData);
        }

        private void RefreshData()
        {
            foreach (var vuforiaId in UnprocessedTransactions.TransactionIdList)
            {
                StartCoroutine(_target.GetSingleTarget(vuforiaId, UpdateSingleTarget));
                AndroidNativeUtility.ShowPreloader("AR","Atnaujinami duomenys");
            }
            MobileMessage.ShowMessage("AR", "Duomenys jau visi užkrauti");
        }

        public void UpdateSingleTarget(GetTarget.TargetRecord target)
        {
            AndroidNativeUtility.HidePreloader();
            Debug.Log(string.Format("Updating record transactionId({0}), name({1}), width({2}), rating({3}) ", target.target_id, target.name, target.width, target.tracking_rating));
            if (target.active_flag)
            {
                _objectsManager.UpdateObject(target.name, target.target_id, target.tracking_rating);
                RefreshObjectsList();
                if (UnprocessedTransactions.TransactionIdList.Contains(target.target_id))
                    UnprocessedTransactions.TransactionIdList.Remove(target.target_id);
                MobileMessage.ShowMessage("AR","Pridėtas naujas apdorotas objektas");
            }
            else
            {
                if (!UnprocessedTransactions.TransactionIdList.Contains(target.target_id))
                    UnprocessedTransactions.TransactionIdList.Add(target.target_id);
                MobileMessage.ShowMessage("AR", "Nuotrauka vis dar apdorojama");
            }
            
        }

        private void RefreshObjectsList()
        {
            var objects = _objectsManager.GetActiveObjects();
            DropDownList.ClearOptions();
            List<string> optionsList = new List<string>();
            foreach (var obj in objects)
            {
                optionsList.Add(obj.Name);
            }
            DropDownList.AddOptions(optionsList);
            Debug.Log("dropDownList selected text: " + DropDownList.options[0].text);
            DescriptionField.text = objects.Select(t => t.Description).FirstOrDefault();
            ImageField.sprite = Sprites[objects.Select(t => t.Rating).FirstOrDefault()];
        }
    }
}
