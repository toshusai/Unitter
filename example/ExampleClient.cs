using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unitter;

public class ExampleClient : Client
{
    //Your app Keys
    public string consumerKey_ = "";
    public string consumerSecret_ = "";
    public InputField pinInputField;

    private string oauthToken;

    public void OpenAuthURL()
    {
        SetConsumerKeySecret(consumerKey_, consumerSecret_);
        PostOAuthRequestToken(PostOAuthRequestTokenCallback);
    }

    private void PostOAuthRequestTokenCallback(bool isSuccess, string response)
    {
        if (isSuccess)
        {
            Dictionary<string, string> parameters = GetParametersFromResponse(response);
            oauthToken = parameters["oauth_token"];
            string authorizeUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken;
            Application.OpenURL(authorizeUrl);
        }
        else
        {
            Debug.LogError("HTTP error :" + response);
        }
    }

    public void SetAccessToken()
    {
        PostOAuthAccessToken(pinInputField.text, oauthToken, PostOAuthAccessTokenCallback);
    }

    protected override void PostOAuthAccessTokenCallback(bool isSuccess, string response)
    {
        if (isSuccess)
        {
            base.PostOAuthAccessTokenCallback(isSuccess, response);
            GetTimeline();
        }else{
            Debug.LogError("HTTP error :" + response);
        }
    }

    private void GetTimeline()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>{
            {"count", "200"},
            {"include_entities", "false"}
        };
        GetStatuesHomeTimeline(parameters, GetStatuesHomeTimelineCallback);
    }

    private void GetStatuesHomeTimelineCallback(bool isSuccess, string response)
    {
        if (isSuccess)
        {
            JSONNode tweets = JSON.Parse(response);
            for (int i = 0; i < tweets.Count; i++)
            {
                Debug.Log(tweets[i]["user"]["name"] + " : " + tweets[i]["text"]);
            }
        }
        else
        {
            Debug.LogError("HTTP error :" + response);
        }
    }
}
