using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class API
{
    public const string url = "https://oppo.dickyri.net/";

    public static IEnumerator GetRequest<T>(string sub, Action<T> onRequestCompleted, Action<string> onRequestFailed)
    {
        string fullUrl = url + sub;
        using (UnityWebRequest uwr = new UnityWebRequest(fullUrl, "GET"))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    string json = uwr.downloadHandler.text;
                    onRequestCompleted?.Invoke(JsonUtility.FromJson<T>(json));
                }
                else
                {
                    onRequestFailed?.Invoke(uwr.responseCode.ToString());
                    throw new Exception(uwr.result.ToString());
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc.Message + " " + uwr.responseCode);
            }
        }
    }

    public static IEnumerator PostRequest<T>(string sub, string json, Action<T> onRequestCompleted, Action<string> onRequestFailed)
    {
        string fullUrl = url + sub;
        Debug.LogError("json : " + json);
        using (UnityWebRequest uwr = UnityWebRequest.Post(fullUrl, json, "application/json"))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            try
            {
                Debug.LogError("err : " + sub + " " + uwr.result);
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = uwr.downloadHandler.text;
                    Debug.LogError("response : " + jsonResponse);
                    onRequestCompleted?.Invoke(JsonUtility.FromJson<T>(jsonResponse));
                }
                else
                {
                    onRequestFailed?.Invoke(uwr.responseCode.ToString());
                    throw new Exception(uwr.responseCode.ToString());
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc.Message + " " + uwr.responseCode);
            }
        }
    }

    public static IEnumerator PostImageRequest(string sub, string json, Action<byte[]> onRequestCompleted, Action<string> onRequestFailed)
    {
        string fullUrl = url + sub;
        Debug.LogError("json : " + json);
        using (UnityWebRequest uwr = UnityWebRequest.Post(fullUrl, json, "application/json"))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            try
            {
                Debug.LogError("err : " + sub + " " + uwr.result);
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    onRequestCompleted?.Invoke(uwr.downloadHandler.data);
                }
                else
                {
                    onRequestFailed?.Invoke(uwr.responseCode.ToString());
                    throw new Exception(uwr.responseCode.ToString());
                }
            }
            catch (Exception exc)
            {
                Debug.LogError("err : " + exc.Message + " " + uwr.responseCode);
            }
        }
    }

    //public IEnumerator RequestGetFbx(string fbxName)
    //{
    //    string fullUrl = url + fbxName;
    //    UnityWebRequest uwr = new UnityWebRequest(fullUrl, "GET");
    //    uwr.downloadHandler = new DownloadHandlerBuffer();
    //    yield return uwr.SendWebRequest();
    //    byte[] data = uwr.downloadHandler.data;

    //    SaveToFile(data, "Test.fbx");
    //}

    //private void SaveToFile(byte[] data, string fileName)
    //{
    //    File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, data);
    //    Debug.Log("Written");

    //    TestLoad();
    //}

    //private void TestLoad()
    //{
    //    //File.ReadAllBytes(Application.persistentDataPath + "/" + "Test.fbx");
    //    FBXFile fbx = FbxImporter.ParseFile(Application.persistentDataPath + "/" + "Test.fbx");
    //    Debug.Log(fbx.Name);

    //}
}
