using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Unitter
{
    [RequireComponent(typeof(Renderer))]
    public class TextureDownloader : MonoBehaviour
    {
        /// <summary>
        /// Set Renderer.material.mainTexture from image URL
        /// </summary>
        public IEnumerator SetTexture(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = www.texture;
        }
    }
}