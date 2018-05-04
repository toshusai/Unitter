using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExampleClient))]
public class Example : MonoBehaviour
{
    public ExampleClient client;
    //Your app Keys
    public string consumerKey = "";
    public string consumerSecret = "";

    // STEP 1
    private void Start()
    {
        client.SetConsumerKeySecret(consumerKey, consumerSecret);
        // Open Authorize URL.
        client.PostOAuthRequestToken();
    }

    // STEP 2
    // Enter pin by inspecter.
    public int pin = -1;
    // STEP 3
    // Call by inspecter window.
    [ContextMenu("SetAccessToken")]
    public void SetAccessToken()
    {
        client.PostOAuthAccessToken(pin.ToString(), client.GetOauthToken());
    }

    // STEP 4
    // Call by inspecter window.
    [ContextMenu("GetTimeline")]
    public void GetTimeline()
    {
        client.GetStatuesHomeTimeline(count: 200, include_entities: false);
    }
}
