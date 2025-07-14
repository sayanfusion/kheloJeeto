using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DevCommon.SaveSystem;

namespace DevCommon.Utils
{
    public sealed class ImageLoader : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Image image;
        private Coroutine imageRequest;

        // Load image from URL or string data
        public void LoadImage(string a_ImageData, bool a_IsUrl, Action<Texture2D> a_OnComplete = null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            if (a_IsUrl)
            {
                if (imageRequest != null)
                    StopCoroutine(imageRequest);
                imageRequest = StartCoroutine(loadImageFromURL(a_ImageData, a_OnComplete));
            }
            else
            {
                Texture2D t_Tex = getSpriteFromString(a_ImageData);
                a_OnComplete?.Invoke(t_Tex);
            }
        }

        // Load image from byte array
        public void LoadImage(byte[] imageData, Action<Texture2D> a_OnComplete = null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            Texture2D t_Tex = getSpriteFromByteArray(imageData);
            a_OnComplete?.Invoke(t_Tex);
        }

        #region ImageFromURL
        private IEnumerator loadImageFromURL(string a_Url, Action<Texture2D> a_OnComplete)
        {
            UnityWebRequest t_Www = UnityWebRequestTexture.GetTexture(a_Url);
            yield return t_Www.SendWebRequest();

            if (t_Www.result == UnityWebRequest.Result.ConnectionError || t_Www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(t_Www.error);
            }
            else
            {
                Texture2D t_Tex = ((DownloadHandlerTexture)t_Www.downloadHandler).texture;
                reloadImageWithTexture(t_Tex);
                a_OnComplete?.Invoke(t_Tex);
            }
            imageRequest = null;
        }

        private void reloadImageWithTexture(Texture2D a_Texture)
        {
            Rect t_Rec = new Rect(0, 0, a_Texture.width, a_Texture.height);
            if (spriteRenderer != null) 
            {
                spriteRenderer.sprite = Sprite.Create(a_Texture, t_Rec, new Vector2(0.5f, 0.5f));

                //MaterialPropertyBlock block = new MaterialPropertyBlock();
                //block.SetTexture("_MainTex", texture);
                //m_SpriteRenderer.SetPropertyBlock(block);
            }
            else if (image != null)
                image.overrideSprite = Sprite.Create(a_Texture, t_Rec, new Vector2(0.5f, 0.5f));
        }
        #endregion

        #region ImageFromString
        private Texture2D getSpriteFromString(string a_ImageData)
        {
            Texture2D t_Tex = ImageHandler.StringToTexture2D(a_ImageData);
            Sprite t_Sprite = Sprite.Create(t_Tex, new Rect(0.0f, 0.0f, t_Tex.width, t_Tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = t_Sprite;
            return t_Tex;
        }
        #endregion

        #region ImageFromByteArray
        private Texture2D getSpriteFromByteArray(byte[] a_ImageData)
        {
            Texture2D t_Tex = ImageHandler.BytesToTexture2D(a_ImageData);
            Sprite t_Sprite = Sprite.Create(t_Tex, new Rect(0.0f, 0.0f, t_Tex.width, t_Tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = t_Sprite;
            return t_Tex;
        }
        #endregion
    }
}
