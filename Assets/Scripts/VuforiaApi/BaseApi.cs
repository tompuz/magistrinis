using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.VuforiaApi
{
    public abstract class BaseApi : MonoBehaviour
    {
        protected string access_key = "4d06c8b111c1a70ce0220e5843df2e10eaaa998f";
        protected string secret_key = "fd7713cd111ef4d783ff58bbed119ecaeb967af8";
        protected string url = @"https://vws.vuforia.com";

        protected string ClientAccessKey = "c870ce1ed4fe95725373c03e01b6cb02e408c521";
        protected string ClientSecretKey = "9180ce6b014c6a706c902f30934afe35be165c3e";

        #region Request parameters

        public abstract string HttpAction { get; }
        public abstract string ServiceUri { get; }
        public abstract string RequestPath { get; set; }
        public abstract string RequestBody { set; get; }
        public abstract string Date { set; get; }

        #endregion

        public class PostNewTrackableRequest
        {
            public string name;
            public float width;
            public string image;
            public string application_metadata;
        }

        protected string CreateMd5RequestBody(string body)
        {
            MD5 md5 = MD5.Create();
            var contentMd5Bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(body));
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < contentMd5Bytes.Length; i++)
            {
                sb.Append(contentMd5Bytes[i].ToString("x2"));
            }
            string contentMd5 = sb.ToString();
            return contentMd5;
        }

        protected string GenerateSignature()
        {
            string contentType = string.Empty;
            string hexDigest = "d41d8cd98f00b204e9800998ecf8427e"; // Hex digest of an empty string

            if (HttpAction.Equals("GET") || HttpAction.Equals("DELETE"))
            {
                // Do nothing because the strings are already set correctly
            }
            else if (HttpAction.Equals("POST") || HttpAction.Equals("PUT"))
            {
                contentType = "application/json";
                // If this is a POST or PUT the request should have a request body
                hexDigest = RequestBody;
            }
            //var test = CreateMd5RequestBody(string.Empty);
            //Debug.Log(string.Format("digest: {0}", test));

            string toDigest = string.Format("{0}\n{1}\n{2}\n{3}\n{4}", HttpAction, hexDigest, contentType, Date, RequestPath);
            string shaHashed = "";
            try
            {
                Debug.Log(string.Format("string to sign: {0}", toDigest));
                shaHashed = calculateRFC2104HMAC(secret_key, toDigest);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to sign");
            }
            return shaHashed;
        }

        protected string GenerateSignature(WWWForm form, string secretKey)
        {
            string contentType = string.Empty;
            string hexDigest = "d41d8cd98f00b204e9800998ecf8427e"; // Hex digest of an empty string
            string requestPath = "/targets";
            string serviceURI = url + requestPath;
            if (HttpAction.Equals("GET") || HttpAction.Equals("DELETE"))
            {
                // Do nothing because the strings are already set correctly
            }
            else if (HttpAction.Equals("POST") || HttpAction.Equals("PUT"))
            {
                contentType = "application/json";
                // If this is a POST or PUT the request should have a request body
                //hexDigest = contentMD5((HttpEntityEnclosingRequestBase)request);
            }

            string date = (string)form.headers.FirstOrDefault(p => p.Key == "Date").Value;
            string toDigest = "GET" + "\n" + hexDigest + "\n" + contentType + "\n" + date + "\n" + serviceURI;
            string shaHashed = "";
            try
            {
                Debug.Log(string.Format("string to sign: {0}", toDigest));
                shaHashed = calculateRFC2104HMAC(secretKey, toDigest);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to sign");
            }
            return shaHashed;
        }

        public static string calculateRFC2104HMAC(string key, string data)
        {
            string result = "";
            try
            {
                // get an hmac_sha1 key from the raw key bytes
                HMACSHA1 sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(key));
                byte[] sha1Bytes = System.Text.Encoding.ASCII.GetBytes(data);
                MemoryStream stream = new MemoryStream(sha1Bytes);
                byte[] sha1Hash = sha1.ComputeHash(stream);
                result = System.Convert.ToBase64String(sha1Hash);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return result;
        }
    }
}
