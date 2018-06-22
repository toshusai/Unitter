# Unitter
## Twitter API Library for Unity.

## Usage
- Example  
`using Unitter;`  
Inherit the "Client" class and override the Callback method and use it.  
Check "example/ExampleClient" and "example/Example.cs"
```cs
public class ExampleClient : Client
{
    private string oauthToken;

    public string GetOauthToken()
    {
        return oauthToken;
    }

    public override void PostOAuthRequestTokenCallback(bool isSuccess, string response)
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

    public override void GetStatuesHomeTimelineCallback(bool isSuccess, string response)
    {
        base.GetStatuesHomeTimelineCallback(isSuccess, response);
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
```
```cs
public class ExampleScript : MonoBehaviour
{
    [SerializeField]
    public ExampleClient client;
    //Your app Keys
    [SerializeField]
    public string consumerKey = "";
    [SerializeField]
    public string consumerSecret = "";

    // STEP 1
    // Open Authorize URL.
    [ContextMenu("OpenAuthURL")]
    private void OpenAuthURL()
    {
        client.SetConsumerKeySecret(consumerKey, consumerSecret);
        client.PostOAuthRequestToken();
    }

    // STEP 2
    // Input PIN.
    public int pin = -1;

    // STEP 3
    // Set Access Token.
    [ContextMenu("SetAccessToken")]
    public void SetAccessToken()
    {
        client.PostOAuthAccessToken(pin.ToString(), client.GetOauthToken());
    }

    // STEP 4
    // Get timeline to console.
    [ContextMenu("GetTimeline")]
    public void GetTimeline()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>{
            {"count", "200"},
            {"include_entities", "false"}
        };
        client.GetStatuesHomeTimeline(parameters);
    }
}

```

## Extra
- There is a script to set texture, sprite, RawImage from image URL.  
Check "scripts/TextureDownloader, SpriteDownloader, ImageDownloader".  

## Lisense
MIT Lisense.