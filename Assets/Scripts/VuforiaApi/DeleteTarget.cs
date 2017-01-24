using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.VuforiaApi
{
    public class DeleteTarget : BaseApi, IDeleteTarget
    {
        public override string HttpAction { get { return "DELETE"; } }

        public override string ServiceUri { get { return url + RequestPath; } }

        public override string RequestPath { get; set; }
        public override string RequestBody { get; set; }
        public override string Date { set; get; }

        public IEnumerator DeleteSingleTarget(string transactionId, Action<GetTarget.ResultTarget> successCallback)
        {
            RequestPath = "/targets/" + transactionId;
            Date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());
            Debug.Log(Date);

            WWWForm form = new WWWForm();
            var headers = form.headers;
            headers["X-HTTP-Method-Override"] = "DELETE";
            headers["Host"] = url;
            headers["Date"] = Date;
            headers["Authorization"] = string.Format("VWS {0}:{1}", access_key, GenerateSignature());
            headers["Content-Type"] = string.Empty;
            //var request = UnityWebRequest.Delete(ServiceUri);
            //request.SetRequestHeader("Host", url);
            //request.SetRequestHeader("Date", Date);
            //request.SetRequestHeader("Authorization", string.Format("VWS {0}:{1}", access_key, GenerateSignature()));
            //request.SetRequestHeader("Content-Type", string.Empty);
            //var response = request.Send();
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
                var result = JsonUtility.FromJson<GetTarget.ResultTarget>(request.text);
                successCallback(result);
            }
        }
    }
}
