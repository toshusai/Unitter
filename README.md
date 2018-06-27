# Unitter
## Twitter API Library for Unity.

## Usage
- Example  
`using Unitter;`  
Inherit the "Client" class and override the Callback method and use it.  
Check "example/ExampleClient".
```cs
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
```

## Extra
- There is a script to set texture, sprite, RawImage from image URL.  
Check "scripts/TextureDownloader, SpriteDownloader, ImageDownloader".  

## Lisense
MIT Lisense.