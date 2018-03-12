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
        public IEnumerator SetTexture(string url, FilterMode filterMode = FilterMode.Bilinear, Vector4? border_ = null)
        {
            WWW www = new WWW(url);
            yield return www;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = www.texture;
        }
    }
}