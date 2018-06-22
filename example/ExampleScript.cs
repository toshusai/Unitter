using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
