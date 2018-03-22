using System.Text;
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

        /// <summary>
        /// Convert List&lt;T&gt; to array root JSON.
        /// </summary>
        public static string ListToJson<T>(List<T> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            foreach (var t in list) {
                stringBuilder.Append(JsonUtility.ToJson(t) + ",");
            }
            stringBuilder.Length--;
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}