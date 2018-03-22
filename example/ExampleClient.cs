using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unitter;

public class ExampleClient : Client
{
    private string oauthToken;
    public string GetOauthToken()
    {
        return oauthToken;
    }

    public override void PostOAuthRequestTokenCallback(bool isSuccess, string response)
    {
        if (isSuccess) {
            Dictionary<string, string> parameters = GetParametersFromResponse(response);
            oauthToken = parameters["oauth_token"];
            string authorizeUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken;
            Application.OpenURL(authorizeUrl);
        } else {
            Debug.Log("HTTP error :" + response);
        }
    }

    public override void GetStatuesHomeTimelineCallback(bool isSuccess, string response)
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
