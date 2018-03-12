using System.Collections.Generic;
using UnityEngine;

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
        public string id_str;
        public string name;
        public string screen_name;
        public string location;
        //public Location profile_location;
        public string description;
        public string url;
        public Entitiy entities;
        public bool @protected;
        public int followers_count;
        public int friends_count;
        public int listed_count;
        public string created_at;
        public int favourites_count;
        public int utc_offset;
        public string time_zone;
        public bool geo_enabled;
        public bool verified;
        public int statuses_count;
        public string lang;
        // If you add this member, deserialization will be performed recursively. 
        //public Tweet status;
        public bool contributors_enabled;
        public bool is_translator;
        public bool is_translation_enabled;
        public string profile_background_color;
        public string profile_background_image_url;
        public string profile_background_image_url_https;
        public bool profile_background_tile;
        public string profile_image_url;
        public string profile_image_url_https;
        public string profile_banner_url;
        public string profile_link_color;
        //public string profile_sidebar_border_color;
        //public string profile_sidebar_fill_color;
        //public string profile_text_color;
        //public bool profile_use_background_image;
        //public bool has_extended_profile;
        public bool default_profile;
        public bool default_profile_image;
        public bool following;
        public bool follow_request_sent;
        public bool notifications;
        //public string translator_type;
        public bool suspended;
        public bool needs_phone_verification;
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

    [System.Serializable]
    public class Id
    {
        public long[] ids;
    }



}