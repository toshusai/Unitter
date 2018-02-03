using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unitter;

public class Sample : MonoBehaviour
{
    public Client client;
    //Your app Keys
    private string consumerKey = "";
    private string consumerSecret = "";

    private const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
    private const string accessTokenUrl = "https://api.twitter.com/oauth/access_token";

    //STEP 1
    void Start()
    {
        //Set Consumer Key and Consumer Secret.
        client.SetConsumerKeySecret(consumerKey, consumerSecret);
        //POST request_token URL.
        StartCoroutine(client.Post(requestTokenUrl, new Dictionary<string, string>(), LogAuthUrl));
    }

    //STEP 2
    string oauthToken;
    void LogAuthUrl(bool isSuccess, string response)
    {
        if (isSuccess) {
            //Get parameters from response text.
            Dictionary<string, string> parameters = Client.GetParametersFromResponse(response);
            oauthToken = parameters["oauth_token"];
            //Output log Authorization URL.
            Debug.Log("https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken);
        }
    }

    //SETP 3
    // 3.1.Get PIN code from Authorization URL.
    // 3.2.Input PIN InputField.
    // 3.3.Excute ConfirmPin Method (with button etc.).
    public InputField pinInput;
    public void ConfirmPin()
    {
        //Set parameters
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            { "oauth_verifier", pinInput.text},
            { "oauth_token", oauthToken}
        };
        //POST access_token URL
        StartCoroutine(client.Post(accessTokenUrl, parameters, SetAccessToken));
    }

    //STEP 4
    void SetAccessToken(bool isSuccess, string response)
    {
        if (isSuccess) {
            //Get parameters from response text.
            Dictionary<string, string> parameters = Client.GetParametersFromResponse(response);
            //Set Access Token and Access Token Secret.
            client.SetAccessToken(parameters["oauth_token"]);
            client.SetAccessTokenSecret(parameters["oauth_token_secret"]);
        } else {
            Debug.Log("HTTP error :" + response);
        }
    }

    //SETP 5
    // Excute GEtUserTimeLine Method (with button etc.).
    public void GetUserTimeLine()
    {
        string userTimelineUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        //Set parameters.
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("count", "20");
        //GET uset timeline.
        StartCoroutine(client.Get(userTimelineUrl, parameters, LogTweet));
    }

    //STEP 6
    //You get your timeline.
    void LogTweet(bool isSuccess, string response)
    {
        if (isSuccess) {
            List<Tweet> tweets = JsonHelper.ListFromJson<Tweet>(response);
            for (int i = 0; i < tweets.Count; i++) {
                Debug.Log(tweets[i].user.name + " : " + tweets[i].text);
            }
        } else {
            Debug.Log("HTTP error :" + response);
        }
    }
}