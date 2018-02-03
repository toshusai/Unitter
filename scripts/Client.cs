using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Security.Cryptography;

namespace Unitter
{
    public class Client : MonoBehaviour
    {
        private string consumerKey = "";
        private string consumerSecret = "";
        private string accessToken = "";
        private string accessTokenSecret = "";

        //Callback method
        public delegate void response(bool isSuccess, string response);

        #region Set methods

        /// <summary>
        /// Set Consumer Key, Consumer Secret, Access Token and Access Token Secret.
        /// </summary>
        public void SetAll(string consumerKey_, string consumerSecret_, string accessToken_, string accessTokenSecret_)
        {
            consumerKey = consumerKey_;
            consumerSecret = consumerSecret_;
            accessToken = accessToken_;
            accessTokenSecret = accessTokenSecret_;
        }

        /// <summary>
        /// Set Consumer Key and Consumer Secret.
        /// </summary>
        public void SetConsumerKeySecret(string consumerKey_, string consumerSecret_)
        {
            consumerKey = consumerKey_;
            consumerSecret = consumerSecret_;
        }

        /// <summary>
        /// Set Access Token.
        /// </summary>
        public void SetAccessToken(string accessToken_)
        {
            accessToken = accessToken_;
        }

        /// <summary>
        /// Set Access Token Secret.
        /// </summary>
        public void SetAccessTokenSecret(string accessTokenSecret_)
        {
            accessTokenSecret = accessTokenSecret_;
        }

        #endregion

        #region Request base

        /// <summary>
        /// POST request.
        /// </summary>
        public IEnumerator Post(string requestURl, Dictionary<string, string> parameters, response callback)
        {
            WWWForm form = new WWWForm();
            UnityWebRequest request = UnityWebRequest.Post(requestURl, form);
            yield return StartCoroutine(SendRequest(request, requestURl, "POST", parameters, callback));
        }

        /// <summary>
        /// Get request.
        /// </summary>
        public IEnumerator Get(string requestURl, Dictionary<string, string> parameters, response callback)
        {
            parameters = SortDictionary(parameters);

            string requestParameterUrl = requestURl + "?";
            foreach (KeyValuePair<string, string> pair in parameters) {
                requestParameterUrl += pair.Key + "=" + pair.Value + "&";
            }
            requestParameterUrl = requestParameterUrl.Remove(requestParameterUrl.Length - 1);

            UnityWebRequest request = UnityWebRequest.Get(requestParameterUrl);
            yield return SendRequest(request, requestURl, "GET", parameters, callback);
        }

        /// <summary>
        /// Web request.
        /// </summary>
        private IEnumerator SendRequest(UnityWebRequest request, string requestURl, string requestMethod, Dictionary<string, string> parameters, response callback)
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", GenerateHeaderAuthorization(requestURl, requestMethod, parameters));
            yield return request.SendWebRequest();
            if (request.responseCode == 200 || request.responseCode == 201) {
                callback(true, request.downloadHandler.text);
            } else {
                callback(false, request.downloadHandler.text);
            }
        }

        #endregion

        #region Oveload methods

        /// <summary>
        /// No require parameters POST request.
        /// </summary>
        public IEnumerator Post(string requestURl, response callback)
        {
            WWWForm form = new WWWForm();
            UnityWebRequest request = UnityWebRequest.Post(requestURl, form);
            yield return StartCoroutine(SendRequest(request, requestURl, "POST", new Dictionary<string, string>(), callback));
        }

        #endregion

        #region Helper of request method

        public static Dictionary<string, string> GetParametersFromResponse(string response)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string[] split = response.Split(new char[] { '&', '=' });
            for (int i = 0; i < split.Length / 2; i++) {
                parameters.Add(split[i * 2], split[i * 2 + 1]);
            }
            return parameters;
        }

        #endregion

        #region Helper of auth

        /// <summary>
        /// Generate request header for Twitter API Authorization.
        /// </summary>
        public string GenerateHeaderAuthorization(string requestUrl, string requestMethod, Dictionary<string, string> parameters)
        {
            //Set default parameters.
            parameters.Add("oauth_callback", "oob");
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_nonce", RandomString(8));
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", GetUnixTimeStamp());
            parameters.Add("oauth_version", "1.0");
            if (parameters.ContainsKey("oauth_token") == false) {
                parameters.Add("oauth_token", accessToken);
            }

            //Sort parameters.
            parameters = SortDictionary(parameters);
            //Add signature to parameters.
            parameters.Add("oauth_signature", GenerateSignature(requestUrl, requestMethod, parameters));

            //Generate header "OAuth key=value,key=value,...".
            string authorization = "OAuth ";
            foreach (KeyValuePair<string, string> pair in parameters) {
                authorization += String.Format("{0}=\"{1}\",", Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(pair.Value));
            }
            //Remove last ",".
            authorization = RemoveLastString(authorization);
            return authorization;
        }

        /// <summary>
        /// Generate signature for Authorization header.
        /// </summary>
        private string GenerateSignature(string requestUrl, string requestMethod, Dictionary<string, string> parameters)
        {
            //Convert to "Key=Value&Key=Value&..." format.
            string parametersString = "";
            foreach (KeyValuePair<string, string> pair in parameters) {
                parametersString += Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value) + "&";
            }

            //Remove last "&".
            parametersString = RemoveLastString(parametersString);

            //Generate key and data for signature.
            string signatureKey = Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(accessTokenSecret);
            string signatureData = Uri.EscapeDataString(requestMethod) + "&" + Uri.EscapeDataString(requestUrl) + "&" + Uri.EscapeDataString(parametersString);

            //Generate HMACSHA1 hash value.
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureData));

            //BASE64 encode.
            string signature = Convert.ToBase64String(signatureBytes);

            return signature;
        }

        #endregion

        #region Private helper

        /// <summary>
        /// Remove a char of last from string.
        /// </summary>
        private string RemoveLastString(string str)
        {
            return str.Remove(str.Length - 1);
        }

        /// <summary>
        /// Return sorted Dictionary&lt;string, string&gt; (Not SortedDictionary&lt;string, string&gt;).
        /// </summary>
        private Dictionary<string, string> SortDictionary(Dictionary<string, string> dictionary)
        {
            SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(dictionary);
            return new Dictionary<string, string>(sortedDictionary);
        }

        /// <summary>
        /// Generate random string.
        /// </summary>
        private string RandomString(int length = 8)
        {
            string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder randomString = new StringBuilder();
            for (int i = 0; i < length; i++) {
                randomString.Append(str[UnityEngine.Random.Range(0, str.Length)]);
            }
            return randomString.ToString();
        }

        /// <summary>
        /// Gets the unix time stamp.
        /// </summary>
        private static string GetUnixTimeStamp()
        {
            return Convert.ToInt32((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
        }

        #endregion
    }
}