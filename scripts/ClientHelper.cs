using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Unitter
{
    static public class ClientHelper
    {
        /// <summary>
        /// Remove a char of last from string.
        /// </summary>
        static public string RemoveLastString(string str)
        {
            return str.Remove(str.Length - 1);
        }

        /// <summary>
        /// Return sorted Dictionary&lt;string, string&gt; (Not SortedDictionary&lt;string, string&gt;).
        /// </summary>
        static public Dictionary<string, string> SortDictionary(Dictionary<string, string> dictionary)
        {
            SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(dictionary);
            return new Dictionary<string, string>(sortedDictionary);
        }

        /// <summary>
        /// Generate random string.
        /// </summary>
        static public string RandomString(int length = 8)
        {
            string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder randomString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(str[UnityEngine.Random.Range(0, str.Length)]);
            }
            return randomString.ToString();
        }

        /// <summary>
        /// Gets the unix time stamp.
        /// </summary>
        static public string GetUnixTimeStamp()
        {
            return Convert.ToInt32((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
        }


        static public bool ConfirmCach(string cachPath, Action<bool, string> callback, int interval){
            if (File.Exists(cachPath))
            {
                DateTime lastTime = File.GetLastWriteTime(cachPath);
                if ((DateTime.Now - lastTime).Seconds < interval)
                {
                    string response = File.ReadAllText(cachPath);
                    callback(true, response);
                    Debug.Log("load cach");
                    return true;
                }
            }
            return false;
        } 
    }
}
