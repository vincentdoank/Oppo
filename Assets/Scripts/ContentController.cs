using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum TextureType
{
    JPG,
    PNG
}

public class ContentController : MonoBehaviour
{
    public Button downloadButton;
    public Button loadLocalButton;

    private void Start()
    {
        downloadButton?.onClick.AddListener(DownloadAssets);
        loadLocalButton?.onClick.AddListener(LoadLocalAssets);
    }

    public void LoadLocalAssets()
    {
        Debug.Log("LoadLocalAssets");
        try
        {
            if (PlayerPrefs.HasKey(Consts.BENCH_COLOR))
            {
                string hexColor = PlayerPrefs.GetString(Consts.BENCH_COLOR);
                Debug.Log("hexColor : " + hexColor);
                if (ColorUtility.TryParseHtmlString(hexColor, out Color benchColor))
                {
                    PlayerSettings.Instance.SetBenchColor(benchColor);
                }
                else
                {
                    Debug.Log("failed to parse hexColor");
                }
            }

            if (PlayerPrefs.HasKey(Consts.PLAYER1_NAME) && PlayerPrefs.HasKey(Consts.PLAYER2_NAME))
            {
                string player1Name = PlayerPrefs.GetString(Consts.PLAYER1_NAME);
                string player2Name = PlayerPrefs.GetString(Consts.PLAYER2_NAME);
                Debug.Log("load player name " + player1Name + " " + player2Name);
                PlayerSettings.Instance.SetPlayerName(player1Name, player2Name);
            }

            Texture2D texture = LoadTextureFile(Consts.BALL_TEXTURE, 512, 512);
            if (texture)
            {
                PlayerSettings.Instance.SetBallTexture(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.BANNER_TEXTURE, 512, 256, TextureType.PNG);
            if (texture)
            {
                PlayerSettings.Instance.SetBannerTexture(texture, texture.width, texture.height);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.FIELD1_DIFF_TEXTURE, 512, 512);
            if (texture)
            {
                PlayerSettings.Instance.SetField1Texture(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.FIELD2_DIFF_TEXTURE, 512, 512);
            if (texture)
            {
                PlayerSettings.Instance.SetField2Texture(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.PLAYER_NAME_BG_TEXTURE, 512, 512, TextureType.PNG);
            if (texture)
            {
                PlayerSettings.Instance.SetPlayerNameSprite(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.SCORE_CONTAINER_TEXTURE, 512, 512, TextureType.PNG);
            if (texture)
            {
                PlayerSettings.Instance.SetScoreContainerSprite(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.SCORE_ON_TEXTURE, 512, 512, TextureType.PNG);
            if (texture)
            {
                PlayerSettings.Instance.SetScoreOnSprite(texture);
            }
            else
            {
                throw new Exception();
            }
            texture = LoadTextureFile(Consts.SCORE_OFF_TEXTURE, 512, 512, TextureType.PNG);
            if (texture)
            {
                PlayerSettings.Instance.SetScoreOffSprite(texture);
            }
            else
            {
                throw new Exception();
            }

            texture = null;
        }
        catch (Exception exc)
        {
            Debug.LogError("err : " + exc.Message);
            PopupMessageController.Instance.ShowMessage("err", "Info", ErrorMessageTranslator.Translate(ErrorMessage.DATA_NOT_FOUND));
        }
    }

    public void DownloadAssets()
    {
        DownloadContent.Instance.RequestLogin("ssssss111", "ssssssaaaaaaaaasssaa", () => DownloadContent.Instance.RequestToken(() => StartCoroutine(DownloadContent.Instance.RequestContent(LoadLocalAssets))));
        //DownloadContent.Instance.LoadFromTextAsset(LoadLocalAssets);
    }

    private Texture2D LoadTextureFile(string name, int width, int height, TextureType textureType = TextureType.JPG)
    {
        switch (textureType)
        {
            case TextureType.JPG:
                return LoadTextureFile(name, width, height, 
                    (texture) => 
                    {
                        return LoadTextureFileAsJPG(texture);
                    });
            case TextureType.PNG:
                return LoadTextureFile(name, width, height,
                    (texture) =>
                    {
                        return LoadTextureFileAsPNG(texture);
                    });
        }
        return null;
    }

    /// <summary>
    /// Load Texture after the format changed
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    private Texture2D LoadTextureFile(string name, int width, int height, Func<Texture2D, Texture2D> onCompleted)
    {
        Debug.LogError("loadtexturefile : " + name);
        try
        {
            string path = Application.persistentDataPath + "/" + name;
            byte[] data = File.ReadAllBytes(path);
            Debug.LogError("size : " + data.Length);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, true, false);
            if (texture.LoadImage(data))
            {
                texture = onCompleted?.Invoke(texture);
                return texture;
            }
        }
        catch
        {
            return null;
        }
        return null;
    }

    private Texture2D LoadTextureFileAsJPG(Texture2D texture)
    {
        texture.LoadImage(texture.EncodeToJPG(100));
        return texture;
    }

    private Texture2D LoadTextureFileAsPNG(Texture2D texture)
    {
        texture.LoadImage(texture.EncodeToPNG());
        return texture;
    }


    //private Texture2D LoadNormalMapFile(string name, int width, int height)
    //{
    //    Debug.Log("loadtexturefile : " + name);
    //    try
    //    {
    //        string path = Application.persistentDataPath + "/" + name;
    //        byte[] data = File.ReadAllBytes(path);
    //        Texture2D texture = new Texture2D(width, height, TextureFormat.DXT5, true, true, false); 
    //        if (texture.LoadImage(data))
    //        {
    //            return texture;
    //        }
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //    return null;
    //}
}
