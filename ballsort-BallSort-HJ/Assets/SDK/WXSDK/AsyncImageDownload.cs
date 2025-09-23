using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class AsyncImageDownload : MonoSingleton<AsyncImageDownload>
{
    Dictionary<int, Sprite> m_dicHeadSpr = new Dictionary<int, Sprite>();

    private static AsyncImageDownload _instance = null;
    public static AsyncImageDownload GetInstance() { return Instance; }
    
    public bool Init()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
        }

        return true;
    }

    public void StartUp()
    {

    }

    public void SetAsyncImage(string url, Image image)
    {
        //开始下载图片前，将UITexture的主图片设置为占位图
        int code = url.GetHashCode();
        if (m_dicHeadSpr.ContainsKey(code))
        {
            image.sprite = m_dicHeadSpr[code];
        }
        else       //如果之前不存在缓存中  就用WWW类下载
        {
            //判断是否是第一次加载这张图片
            if (!File.Exists(path + code.ToString() + ".txt"))
            {
                //如果之前不存在缓存文件
                StartCoroutine(DownloadImage(url, image, code));
            }
            else
            {
                StartCoroutine(LoadLocalImage(url, image, code));
            }
        }
    }

    IEnumerator DownloadImage(string url, Image image, int code)
    {
        WWW www = new WWW(url);
        yield return www;

        Texture2D tex2d = www.texture;

        if (tex2d != null)
        {
            Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
            image.sprite = sprite;

            byte[] pngData = tex2d.EncodeToPNG();
            File.WriteAllBytes(path + code.ToString() + ".txt", pngData);

            m_dicHeadSpr[code] = sprite;
        }else
        {
            Debug.Log("load image from url error:" + url);
        }

        
    }

    IEnumerator LoadLocalImage(string url, Image image, int code)
    {
        string filePath = "file:///" + path + code.ToString() + ".txt";
        //     Debug.Log("getting local image:" + filePath);
        WWW www = new WWW(filePath);
        yield return www;

        Texture2D texture = www.texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        image.sprite = sprite;
        m_dicHeadSpr[code] = sprite;

    }

    public string path
    {
        get
        {
            //pc,ios //android :jar:file//
            return Application.persistentDataPath + "/ImageCache/";

        }
    }
}