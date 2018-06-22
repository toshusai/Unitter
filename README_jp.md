# Unitter
## Unity用のTwitter APIライブラリ

## 使い方  
名前空間 "Unitter" の "Client" を継承した独自クラスを作成し、使いたいAPIののコールバックメソッドをオーバーライドして、APIをリクエストするメソッドを使います.  
### 独自クラスの例
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
### APIリクエストのメソッドを使うスクリプトの例
```cs
public class ExampleScript : MonoBehaviour
{
    [SerializeField]
    public ExampleClient client;
    // Twitter の App のキー
    [SerializeField]
    public string consumerKey = "";
    [SerializeField]
    public string consumerSecret = "";

    // STEP 1
    // 認証ページを開く
    [ContextMenu("OpenAuthURL")]
    private void OpenAuthURL()
    {
        client.SetConsumerKeySecret(consumerKey, consumerSecret);
        client.PostOAuthRequestToken();
    }

    // STEP 2
    // 認証ページに表示されるPINを入れる（インスペクター等で）
    public int pin = -1;

    // STEP 3
    // アクセストークンを設定する
    [ContextMenu("SetAccessToken")]
    public void SetAccessToken()
    {
        client.PostOAuthAccessToken(pin.ToString(), client.GetOauthToken());
    }

    // STEP 4
    // タイムラインをデバッグログとして出力する
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

## おまけ
- 画像のURLからSprite Texture RawImageに変換するスクリプト.  
  - "scripts/TextureDownloader"
  - "scripts/SpriteDownloader"  
  - "scripts/ImageDownloader"  

SimpleJSON(https://github.com/Bunny83/SimpleJSON)を使っています.  
`tweets[i]["user"]["name"]`のようにJSONをオブジェクトに変換して扱えます.

## ライセンス
MIT Lisense.