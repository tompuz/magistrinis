using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.VuforiaApi
{
    public class GetTarget : BaseApi, IGetTarget
    {
        public override string HttpAction
        {
            get { return "GET"; }
        }

        public override string ServiceUri
        {
            get { return url + RequestPath; }
        }

        public override string RequestPath { get; set; }
        public override string RequestBody { get; set; }
        public override string Date { set; get; }

        private string _transactionId = string.Empty;

        private readonly string _targetName;

        private const string md5Empty = "d41d8cd98f00b204e9800998ecf8427e";

        public GetTarget(string name)
        {
            _targetName = name;
        }

        public IEnumerator GetAllTargets(Action<object> successCallback)
        {

            RequestPath = "/targets";
            Date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());

            Debug.Log(Date);

            WWWForm form = new WWWForm();
            var headers = form.headers;
            headers["Host"] = url;
            headers["Date"] = Date;
            headers["Authorization"] = string.Format("VWS {0}:{1}", access_key, GenerateSignature());
            headers["Content-Type"] = string.Empty;

            WWW request = new WWW(ServiceUri, null, headers);
            
            yield return request;

            if (request.error != null)
            {
                Debug.Log("request error: " + request.error);
                Debug.Log("returned error: " + request.text);
            }
            else
            {
                Debug.Log("request success");
                Debug.Log("returned data" + request.text);
                var result = JsonUtility.FromJson<ResultTargets>(request.text);
                Debug.Log("transaction_id: " + result.transaction_id);
                Debug.Log("transaction_id from list: " + result.results.FirstOrDefault());
                successCallback(result.results);
            }
        }

        public class DatabaseObject
        {
            public string Name;
            public string TransactionId;
        }

        [Serializable]
        private class ResultTargets
        {
            public string result_code;
            public string transaction_id;
            public List<string> results;
        }

        [Serializable]
        public class TargetRecord
        {
            public string target_id;
            public bool active_flag;
            public string name;
            public int tracking_rating;
            public double width;
            public string reco_rating;
        }

        [Serializable]
        public class ResultTarget
        {
            public string result_code;
            public string transaction_id;
            public TargetRecord target_record;
        }

        public IEnumerator GetSingleTarget(string transactionId, Action<TargetRecord> successCallback)
        {
            //MNP.ShowPreloader("AR", "Tikrinami objektai");
            RequestPath = "/targets/" + transactionId;
            Date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());
            _transactionId = string.Format("/{0}",transactionId);
            Debug.Log(Date);

            WWWForm form = new WWWForm();
            var headers = form.headers;
            headers["Host"] = url;
            headers["Date"] = Date;
            headers["Authorization"] = string.Format("VWS {0}:{1}", access_key, GenerateSignature());
            headers["Content-Type"] = string.Empty;

            WWW request = new WWW(ServiceUri, null, headers);
            yield return request;

            //MNP.HidePreloader();
            if (request.error != null)
            {
                Debug.Log("request error: " + request.error);
                Debug.Log("returned error: " + request.text);
            }
            else
            {
                Debug.Log("request success");
                Debug.Log("returned data" + request.text);
                var result = JsonUtility.FromJson<ResultTarget>(request.text);
                successCallback(result.target_record);
            }
        }
    }
}
