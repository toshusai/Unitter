using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Unitter
{
    [RequireComponent(typeof(RawImage))]
    public class ImageDownloader : MonoBehaviour
    {
        /// <summary>
        /// Set RawImage.texture from image URL
        /// </summary>
        public IEnumerator SetImage(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            RawImage rawImage = GetComponent<RawImage>();
            rawImage.texture = www.texture;
            rawImage.SetNativeSize();
        }
    }
}