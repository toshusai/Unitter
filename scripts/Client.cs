using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Unitter
{
    public class Client : MonoBehaviour
    {
        const string POST = "POST";
        const string GET = "GET";

        const string BASE_URL = "https://api.twitter.com/1.1/";

        const string STATUS_HOME_TIMELINE_CACH = "/status_home_timeline.json";
        const string USERS_SHOW_CACH = "/users_show.json";
        const string USERS_LOOKUP_CACH = "/users_lookup.json";
        const string FRIENDS_IDS_CACH = "/friends_ids.json";
        const string FOLLOWERS_IDS_CACH = "/followes_ids.json";
        const string SEARCH_TEWWTS_CACH = "/search_tweets.json";

        protected string consumerKey = "";
        protected string consumerSecret = "";
        protected string accessToken = "";
        protected string accessTokenSecret = "";
        protected string userId = "";
        protected string scrrenName = "";

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

        public string GetUserID()
        {
            return userId;
        }

        public string GetScreenName()
        {
            return scrrenName;
        }

        public void SaveToken()
        {
            string token = "{" +
                String.Format("access_token: \"{0}\",", accessToken) +
                      String.Format("access_token_secret: \"{0}\",", accessTokenSecret) +
                      String.Format("screen_name: \"{0}\",", scrrenName) +
                      String.Format("user_id: \"{0}\"", userId) +
                "}";
            File.WriteAllText(Application.persistentDataPath + "/token.json", token);
        }

        public bool LoadToken()
        {
            string path = Application.persistentDataPath + "/token.json";
            if (File.Exists(path))
            {
                JSONNode token = JSON.Parse(File.ReadAllText(path));
                accessToken = token["access_token"];
                accessTokenSecret = token["access_token_secret"];
                scrrenName = token["screen_name"];
                userId = token["user_id"];
                return true;
            }else{
                return false;
            }
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
            yield return SendRequest(request, requestURl, POST, parameters, callback);
        }

        /// <summary>
        /// Get request.
        /// </summary>
        public IEnumerator Get(string requestURl, Dictionary<string, string> parameters, response callback)
        {
            parameters = ClientHelper.SortDictionary(parameters);

            string requestParameterUrl = requestURl + "?";
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                requestParameterUrl += pair.Key + "=" + pair.Value + "&";
            }
            requestParameterUrl = requestParameterUrl.Remove(requestParameterUrl.Length - 1);

            UnityWebRequest request = UnityWebRequest.Get(requestParameterUrl);
            yield return SendRequest(request, requestURl, GET, parameters, callback);
        }

        /// <summary>
        /// Web request.
        /// </summary>
        private IEnumerator SendRequest(UnityWebRequest request, string requestURl, string requestMethod, Dictionary<string, string> parameters, response callback)
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", GenerateHeaderAuthorization(requestURl, requestMethod, parameters));
            yield return request.SendWebRequest();
            if (request.responseCode == 200 || request.responseCode == 201)
            {
                callback(true, request.downloadHandler.text);
            }
            else
            {
                callback(false, request.downloadHandler.text);
            }
        }

        #endregion

        #region Overload methods

        /// <summary>
        /// No require parameters POST request.
        /// </summary>
        public IEnumerator Post(string requestURl, response callback)
        {
            WWWForm form = new WWWForm();
            UnityWebRequest request = UnityWebRequest.Post(requestURl, form);
            yield return StartCoroutine(SendRequest(request, requestURl, POST, new Dictionary<string, string>(), callback));
        }

        #endregion

        #region Helper of auth

        /// <summary>
        /// Generate request header for Twitter API Authorization.
        /// </summary>
        private string GenerateHeaderAuthorization(string requestUrl, string requestMethod, Dictionary<string, string> parameters)
        {
            //Set default parameters.
            if (!parameters.ContainsKey("oauth_callback"))
            {
                parameters.Add("oauth_callback", "oob");
            }
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_nonce", ClientHelper.RandomString(8));
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", ClientHelper.GetUnixTimeStamp());
            parameters.Add("oauth_version", "1.0");
            if (parameters.ContainsKey("oauth_token") == false)
            {
                parameters.Add("oauth_token", accessToken);
            }

            // Sort parameters.
            parameters = ClientHelper.SortDictionary(parameters);
            // Add signature to parameters.
            parameters.Add("oauth_signature", GenerateSignature(requestUrl, requestMethod, parameters));

            // Generate header "OAuth key=value,key=value,...".
            string authorization = "OAuth ";
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                authorization += String.Format("{0}=\"{1}\",", Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(pair.Value));
            }
            // Remove last ",".
            authorization = ClientHelper.RemoveLastString(authorization);
            return authorization;
        }

        /// <summary>
        /// Generate signature for Authorization header.
        /// </summary>
        private string GenerateSignature(string requestUrl, string requestMethod, Dictionary<string, string> parameters)
        {
            // Convert to "Key=Value&Key=Value&..." format.
            string parametersString = "";
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                parametersString += Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value) + "&";
            }

            // Remove last "&".
            parametersString = ClientHelper.RemoveLastString(parametersString);

            // Generate key and data for signature.
            string signatureKey = Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(accessTokenSecret);
            string signatureData = Uri.EscapeDataString(requestMethod) + "&" + Uri.EscapeDataString(requestUrl) + "&" + Uri.EscapeDataString(parametersString);

            // Generate HMACSHA1 hash value.
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureData));

            // BASE64 encode.
            string signature = Convert.ToBase64String(signatureBytes);

            return signature;
        }

        #endregion

        #region Helper of request method
        /// <summary>
        /// Gets the query from response.
        /// </summary>
        public static Dictionary<string, string> GetParametersFromResponse(string response)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string[] split = response.Split(new char[] { '&', '=' });
            for (int i = 0; i < split.Length / 2; i++)
            {
                parameters.Add(split[i * 2], split[i * 2 + 1]);
            }
            return parameters;
        }

        #endregion


        #region virtual Rest API

        /// <summary>
        /// 
        /// </summary>
        public void PostOAuthRequestToken()
        {
            string endpointUrl = "https://api.twitter.com/oauth/request_token";
            StartCoroutine(Post(endpointUrl, new Dictionary<string, string>(), PostOAuthRequestTokenCallback));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void PostOAuthRequestTokenCallback(bool isSuccess, string response)
        {
            // Example
            /*
            if (isSuccess) {
                Dictionary<string, string> parameters = GetParametersFromResponse(response);
                string oauthToken = parameters["oauth_token"];
                string authorizeUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken;
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        public void PostOAuthAccessToken(string oauthVerifier, string oauthToken)
        {
            string endpointUrl = "https://api.twitter.com/oauth/access_token";
            Dictionary<string, string> parameters = new Dictionary<string, string>() {
                { "oauth_verifier", oauthVerifier},
                { "oauth_token", oauthToken}
            };
            StartCoroutine(Post(endpointUrl, parameters, PostOAuthAccessTokenCallback));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void PostOAuthAccessTokenCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                Dictionary<string, string> parameters = Client.GetParametersFromResponse(response);
                // Set Access Token and Access Token Secret.
                SetAccessToken(parameters["oauth_token"]);
                SetAccessTokenSecret(parameters["oauth_token_secret"]);
                userId = parameters["user_id"];
                scrrenName = parameters["screen_name"];
            }
            else
            {
                Debug.LogError("HTTP ERROR : " + response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetStatuesHomeTimeline(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + STATUS_HOME_TIMELINE_CACH, GetStatuesHomeTimelineCallback, 60))
            {
                return;
            }
            string endpointUrl = BASE_URL + "statuses/home_timeline.json";
            StartCoroutine(Get(endpointUrl, parameters, GetStatuesHomeTimelineCallback));
        }

        public virtual void GetStatuesHomeTimelineCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + STATUS_HOME_TIMELINE_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        /// <summary>
        /// Requests / 15-min window (user auth) 900
        /// </summary>
        public void GetUsersShow(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + USERS_SHOW_CACH, GetUsersShowCallback, 1))
            {
                return;
            }
            string endpointUrl = BASE_URL + "users/show.json";
            StartCoroutine(Get(endpointUrl, parameters, GetUsersShowCallback));
        }

        public virtual void GetUsersShowCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + USERS_SHOW_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        /// <summary>
        /// Requests / 15-min window (user auth) 15
        /// </summary>
        public void GetFriendsIds(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + FRIENDS_IDS_CACH, GetFriendsIdsCallback, 60))
            {
                return;
            }
            string endpointUrl = BASE_URL + "friends/ids.json";
            StartCoroutine(Get(endpointUrl, parameters, GetFriendsIdsCallback));
        }

        public virtual void GetFriendsIdsCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + FRIENDS_IDS_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        /// <summary>
        /// Requests / 15-min window (user auth) 15
        /// </summary>
        public void GetFollowersIds(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + FOLLOWERS_IDS_CACH, GetFollowersIdsCallback, 60))
            {
                return;
            }
            string endpointUrl = BASE_URL + "followers/ids.json";
            StartCoroutine(Get(endpointUrl, parameters, GetFollowersIdsCallback));
        }

        public virtual void GetFollowersIdsCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + FOLLOWERS_IDS_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        /// <summary>
        /// Requests / 15-min window (user auth) 900
        /// </summary>
        public void GetUsersLookup(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + USERS_LOOKUP_CACH, GetUsersLookupCallback, 1))
            {
                return;
            }
            string requestUrl = BASE_URL + "users/lookup.json";
            StartCoroutine(Get(requestUrl, parameters, GetUsersLookupCallback));
        }

        public virtual void GetUsersLookupCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + USERS_LOOKUP_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        /// <summary>
        /// Gets the search tweets.
        /// </summary>
        public void GetSearchTweets(Dictionary<string, string> parameters)
        {
            if (ClientHelper.SaveCach(Application.temporaryCachePath + SEARCH_TEWWTS_CACH, GetSearchTweetsCallback, 1))
            {
                return;
            }
            string requestUrl = BASE_URL + "search/tweets.json";
            StartCoroutine(Get(requestUrl, parameters, GetSearchTweetsCallback));
        }

        public virtual void GetSearchTweetsCallback(bool isSuccess, string response)
        {
            if (isSuccess)
            {
                File.WriteAllText(Application.temporaryCachePath + USERS_LOOKUP_CACH, response);
            }
            else
            {
                Debug.LogError("HTTP error : " + response);
            }
        }

        #endregion
    }
}