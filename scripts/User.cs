namespace Unitter
{
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
        public long followers_count;
        public long friends_count;
        public long listed_count;
        public string created_at;
        public long favourites_count;
        public long utc_offset;
        public string time_zone;
        public bool geo_enabled;
        public bool verified;
        public long statuses_count;
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
}