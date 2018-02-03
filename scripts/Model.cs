using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unitter
{
    public class JsonHelper
    {
        /// <summary>
        /// Convert array root to one root and return List&lt;T&gt;.
        /// </summary>
        public static List<T> ListFromJson<T>(string json)
        {
            var listJson = "{ \"list\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(listJson);
            return wrapper.list;
        }

        [System.Serializable]
        class Wrapper<T>
        {
            public List<T> list = null;
        }
    }

    [System.Serializable]
    public class User
    {
        public long id;
        public string name;
        public string screen_name;
        public string profile_image_url;
    }

    [System.Serializable]
    public class Tweet
    {
        public string created_at;
        public string text;
        public User user;
    }

    [System.Serializable]
    public class Entitiy
    {
        public float id;
    }



}