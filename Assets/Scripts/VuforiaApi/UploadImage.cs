using System;
using System.Collections;
using System.Text;
using Assets.Scripts.Scenes;
using Assets.Scripts.Wrappers;
using UnityEngine;

namespace Assets.Scripts.VuforiaApi
{
    public class UploadImage : BaseApi, IUploadTarget
    {
        public IMobileMessageWrapper MobileMessage = new MobileMessageWrapper();
        public IEnumerator PostNewTarget(string targetName, byte[] image, Action successCallback)
        {
            RequestPath = "/targets";
            string contentType = "application/json";
            Date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());
            Debug.Log(Date);

            string metadataStr = "Vuforia metadata";//May use for key,name...in game
            byte[] metadata = Encoding.ASCII.GetBytes(metadataStr);
            PreviewPhotoScene.PostNewTrackableRequest model = new PreviewPhotoScene.PostNewTrackableRequest
            {
                name = targetName,
                width = 64.0f,
                image = Convert.ToBase64String(image),
                application_metadata = Convert.ToBase64String(metadata)
            };
            // don't need same as width of texture

            string requestBody = JsonUtility.ToJson(model);

            WWWForm form = new WWWForm();
            var headers = form.headers;
            headers["Host"] = url;
            headers["Date"] = Date;
            headers["Content-Type"] = contentType;

            RequestBody = CreateMd5RequestBody(requestBody);
            var signature = GenerateSignature();

            headers["Authorization"] = string.Format("VWS {0}:{1}", access_key, signature);

            Debug.Log("<color=green>Signature: " + signature + "</color>");

            WWW request = new WWW(ServiceUri, System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(model)), headers);
            yield return request;

            if (request.error != null)
            {
                Debug.Log("request error: " + request.error);
                MobileMessage.ShowMessage("Klaida", request.error + " " + request.text);
            }
            else
            {
                Debug.Log("request success");
                Debug.Log("returned data" + request.data);
                successCallback();
            }
        }

        public override string HttpAction {get { return "POST"; }}
        public override string ServiceUri { get { return url + RequestPath; } }
        public override string RequestPath { get; set; }
        public override string RequestBody { get; set; }
        public override string Date { set; get; }
    }
}
