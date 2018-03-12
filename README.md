# Unitter
## Twitter API Library for Unity.

## How to Use
```cs
[RequireComponent(typeof(Client))]
public class Sample : MonoBehaviour
{
    public Client client;
    //Your app Keys
    public string consumerKey = "";
    public string consumerSecret = "";

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
            //Open Authorize URL
            Application.OpenURL("https://api.twitter.com/oauth/authorize?oauth_token=" + oauthToken);
        } else {
            Debug.Log(response);
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
        StartCoroutine(client.Post(accessTokenUrl, parameters, ConfirmPinCallback));
    }

    //STEP 4
    void ConfirmPinCallback(bool isSuccess, string response)
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
    // Excute GetUserTimeLine Method (with button etc.).
    public void GetUserTimeLine()
    {
        string userTimelineUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        //Set parameters.
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("count", "20");
        //GET uset timeline.
        StartCoroutine(client.Get(userTimelineUrl, parameters, GetUserTimeLineCallback));
    }

    //STEP 6
    //You get your timeline.
    void GetUserTimeLineCallback(bool isSuccess, string response)
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
```

## Lisense
Apache License Version 2.0, January 2004 http://www.apache.org/licenses/