# Unitter
## Twitter API Library for Unity.

## Usage
- Example 1 (Simple method).  
Inherit the "Client" class and override the Callback method and use it.  
Check "example/ExampleClient" and "example/Example.cs"
```cs
public class ExampleClient : Client {

    public override void PostOAuthRequestTokenCallback(bool isSuccess, string response)
    {
        Debug.Log(response);
    }
}
```
```cs
public class Example : MonoBehaviour
{
    public ExampleClient client;
    //Your app Keys
    public string consumerKey = "";
    public string consumerSecret = "";

    private void Start()
    {
        client.SetConsumerKeySecret(consumerKey, consumerSecret);
        client.PostOAuthRequestToken();
    }
}
```


- Example 2  
Use "Client" class "Post" method directly.  
Check "example/Example2.cs".
```cs
void Start()
{
    //Set Consumer Key and Consumer Secret.
    client.SetConsumerKeySecret(consumerKey, consumerSecret);
    //POST request_token URL.
    StartCoroutine(client.Post(requestTokenUrl, new Dictionary<string, string>(), OpenAuthorizeUrl));
}

string oauthToken;
void OpenAuthorizeUrl(bool isSuccess, string response)
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
```

## Extra
- There is a script to set texture, sprite, RawImage from image URL.  
Check "scripts/helper/TextureDownloader, SpriteDownloader, ImageDownloader".  
- There is a JSON List<t> converter.  
Check "scripts/helper/JsonHelper".

## Lisense
MIT Lisense.