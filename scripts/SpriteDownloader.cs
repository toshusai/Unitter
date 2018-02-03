using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Unitter
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDownloader : MonoBehaviour
    {
        /// <summary>
        /// Set SpriteRenderer.sprite from image URL
        /// </summary>
        public IEnumerator SetSprite(string url, FilterMode filterMode = FilterMode.Bilinear, Vector4? border_ = null)
        {
            Vector4 border = border_ ?? Vector4.zero;
            WWW www = new WWW(url);
            yield return www;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Texture2D texture = www.texture;
            texture.filterMode = filterMode;
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 48, 1, SpriteMeshType.FullRect, border);
        }

    }
}