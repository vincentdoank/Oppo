using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public string userid;
    public string password;
}

public class DownloadContent : MonoBehaviour
{
    public const string CD = "1572c6a9-34af-4ce8-978e-742b2bfb20b5";
    public TextAsset jsonFile;
    public static DownloadContent Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    //private IEnumerator Start()
    //{
    //    yield return null;
    //    yield return AddressableManager.Instance.GetDownloadSizeAsync("Ball", null);
    //    //DownloadBall();
    //    RequestContent();

    //    if (ColorUtility.TryParseHtmlString("#09FF0064", out Color benchColor))
    //    {
    //        PlayerSettings.Instance.SetBenchColor(benchColor);
    //    }
    //}

    public void DownloadBall()
    {
        Debug.LogWarning("Download Ball");
        LoadingManager.Instance.ShowLoading("Downloading...", 0f, 0.16f, 1f, "0.00", "%");
        StartCoroutine(AddressableManager.Instance.DownloadAddressableAsync("Ball", LoadingManager.Instance.UpdateLoading, OnDownloadBallCompleted));
    }

    private void OnDownloadBallCompleted()
    {

    }

    public void RequestLogin(string userid, string password, Action onCompleted)
    {
        UserData jsonData = new UserData { userid = userid, password = password };

        StartCoroutine(API.PostRequest<ResponseData>("loadminlogin", JsonUtility.ToJson(jsonData), (data) =>
        {
        try
        {
            if (data != null)
            {
                Debug.LogError("data : " + data.is_error);
                    if (data.is_error != "N")
                    {
                        throw new Exception();
                    }
                    onCompleted?.Invoke();

                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc);
                PopupMessageController.Instance.ShowConfirmationMessage("err", "Info", "Exception Error");
            }
        }, (err) => PopupMessageController.Instance.ShowMessage("err", "Information", "Failed to Download " + ("(" + err + ")"))));
    }

    public void RequestToken(Action onCompleted)
    {
        StartCoroutine(API.PostRequest<ResponseTokenData>("mbDataFeed/generateToken", null, (data) =>
        {
            try
            {
                if (data != null)
                {
                    Debug.LogError("data : " + data.is_error);
                    if (data.is_error != "N")
                    {
                        throw new Exception();
                    }
                    onCompleted?.Invoke();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc.Message);
                PopupMessageController.Instance.ShowConfirmationMessage("err", "Info", "Exception Error");
            }
        }, (err) => PopupMessageController.Instance.ShowMessage("err", "Information", "Failed to Download " + ("(" + err + ")"))));
    }

    public IEnumerator RequestContent(Action onCompleted)
    {
        Debug.LogError("RequestContent");
        ResponseGameData jsonData = new ResponseGameData { cd = CD };
        yield return null;
        StartCoroutine (API.PostRequest<ResponseData>("posts/penalty", JsonUtility.ToJson(jsonData), (data) =>
        {
            if (data != null)
            {
                if (data.is_error == "N")
                {
                    Debug.LogWarning("data : " + JsonUtility.ToJson(data));
                    StartCoroutine(RequestDownloadAllImage(data));

                    PlayerPrefs.SetString(Consts.PLAYER1_NAME, data.data[0].striker_name);
                    PlayerPrefs.SetString(Consts.PLAYER2_NAME, data.data[0].goal_keeper_name);
                    PlayerPrefs.SetString(Consts.BENCH_COLOR, data.data[0].bench_color);
                    PlayerPrefs.Save();

                    onCompleted?.Invoke();
                    data = null;
                }
                //ConvertBase64ToByteArray(Consts.BALL_TEXTURE, data.data.ball_texture);
                //ConvertBase64ToByteArray(Consts.BANNER_TEXTURE, data.data.banner_texture);
                //ConvertBase64ToByteArray(Consts.FIELD1_DIFF_TEXTURE, data.data.field_texture_diffuse);
                //ConvertBase64ToByteArray(Consts.FIELD1_NORM_TEXTURE, data.data.field_texture_normal);
                //ConvertBase64ToByteArray(Consts.FIELD2_DIFF_TEXTURE, data.data.field_texture_2_diffuse);
                //ConvertBase64ToByteArray(Consts.FIELD2_NORM_TEXTURE, data.data.field_texture_2_normal);
                //ConvertBase64ToByteArray(Consts.SCORE_OFF_TEXTURE, data.data.goal_status_off);
                //ConvertBase64ToByteArray(Consts.SCORE_ON_TEXTURE, data.data.goal_status_on);
                //ConvertBase64ToByteArray(Consts.PLAYER_NAME_BG_TEXTURE, data.data.player_name_background_texture);
                //ConvertBase64ToByteArray(Consts.SCORE_CONTAINER_TEXTURE, data.data.score_container_texture);

            }
        }, (err) => PopupMessageController.Instance.ShowMessage("err", "Information", "Failed to Download " + ("(" + err + ")"))));
    }

    private IEnumerator RequestDownloadAllImage(ResponseData data)
    {
        Debug.LogError("RequestDownloadAllImage");
        yield return RequestDownloadImage(Consts.PLAYER_NAME_BG_TEXTURE, data.data[0].player_name_background_texture);
        yield return RequestDownloadImage(Consts.SCORE_CONTAINER_TEXTURE, data.data[0].score_container_texture);
        yield return RequestDownloadImage(Consts.SCORE_OFF_TEXTURE, data.data[0].goal_status_off);
        yield return RequestDownloadImage(Consts.SCORE_ON_TEXTURE, data.data[0].goal_status_on);
        yield return RequestDownloadImage(Consts.BALL_TEXTURE, data.data[0].ball_texture);
        yield return RequestDownloadImage(Consts.FIELD1_DIFF_TEXTURE, data.data[0].field_texture_diffuse);
        yield return RequestDownloadImage(Consts.FIELD1_NORM_TEXTURE, data.data[0].field_texture_normal);
        yield return RequestDownloadImage(Consts.FIELD2_DIFF_TEXTURE, data.data[0].field_texture_2_diffuse);
        yield return RequestDownloadImage(Consts.FIELD2_NORM_TEXTURE, data.data[0].field_texture_2_normal);
        yield return RequestDownloadImage(Consts.STRIKER_TEXTURE, data.data[0].texture_striker);
        yield return RequestDownloadImage(Consts.KEEPER_TEXTURE, data.data[0].texture_keeper);
        yield return RequestDownloadImage(Consts.BANNER_TEXTURE, data.data[0].banner_texture);
    }

    private IEnumerator RequestDownloadImage(string path, string fileName)
    {
        ImageFileData jsonData = new ImageFileData { cd = CD, name = fileName };
        yield return (API.PostImageRequest("mbDataFeed/mblrtnpltimage", JsonUtility.ToJson(jsonData), (data) =>
        {
            Debug.LogWarning("img data : " + data.Length);
            if (data != null)
            {
                SaveImageToFile(path, data);
            }
        }, (err) => PopupMessageController.Instance.ShowMessage("err", "Information", "Failed to Download " + ("(" + err + ")"))));
    }

    public void LoadFromTextAsset(Action onCompleted)
    {
        ResponseData data = JsonUtility.FromJson<ResponseData>(jsonFile.text);
        if (data != null)
        {
            try
            {
                ConvertBase64ToByteArray(Consts.BALL_TEXTURE, data.data[0].ball_texture);
                ConvertBase64ToByteArray(Consts.BANNER_TEXTURE, data.data[0].banner_texture);
                ConvertBase64ToByteArray(Consts.FIELD1_DIFF_TEXTURE, data.data[0].field_texture_diffuse);
                ConvertBase64ToByteArray(Consts.FIELD1_NORM_TEXTURE, data.data[0].field_texture_normal);
                ConvertBase64ToByteArray(Consts.FIELD2_DIFF_TEXTURE, data.data[0].field_texture_2_diffuse);
                ConvertBase64ToByteArray(Consts.FIELD2_NORM_TEXTURE, data.data[0].field_texture_2_normal);
                ConvertBase64ToByteArray(Consts.SCORE_OFF_TEXTURE, data.data[0].goal_status_off);
                ConvertBase64ToByteArray(Consts.SCORE_ON_TEXTURE, data.data[0].goal_status_on);
                ConvertBase64ToByteArray(Consts.PLAYER_NAME_BG_TEXTURE, data.data[0].player_name_background_texture);
                ConvertBase64ToByteArray(Consts.SCORE_CONTAINER_TEXTURE, data.data[0].score_container_texture);

                PlayerPrefs.SetString(Consts.PLAYER1_NAME, data.data[0].striker_name);
                PlayerPrefs.SetString(Consts.PLAYER2_NAME, data.data[0].goal_keeper_name);
                PlayerPrefs.SetString(Consts.BENCH_COLOR, data.data[0].bench_color);
                PlayerPrefs.Save();

                onCompleted?.Invoke();

            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc);
                PopupMessageController.Instance.ShowConfirmationMessage("err", "Info", ErrorMessageTranslator.Translate(ErrorMessage.DATA_NOT_FOUND));
            }
        }
    }

    public void ConvertBase64ToByteArray(string name, string base64String)
    {
        byte[] data = Convert.FromBase64String(base64String);
        SaveImageToFile(name, data);
    }

    public void SaveImageToFile(string name, byte[] data)
    {
        Debug.LogWarning("save image to file : " + name);
        string path = Application.persistentDataPath + "/" + name;
        if (File.Exists(path))
        {
            File.Delete(path);
            File.WriteAllBytes(path, data);
        }
        else
        {
            File.WriteAllBytes(path, data);
        }
    }
}
