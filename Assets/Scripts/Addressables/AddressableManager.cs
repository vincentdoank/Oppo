using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Threading.Tasks;

public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance { get; private set; }

    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        yield return (Addressables.InitializeAsync());

        //Addressables.ClearDependencyCacheAsync("Ball");
        //Addressables.ClearResourceLocators();
        //yield return new WaitForSeconds(3f);

        //yield return GetDownloadSizeAsync("Ball", null);
        //yield return DownloadAddressableAsync("Ball", null);
        //yield return LoadGroupAssetAsync<Material>("Ball", null);
    }

    public IEnumerator DownloadAddressableAsync(string key, Action onCompleted)
    {
#if UNITY_EDITOR
        //Addressables.ClearDependencyCacheAsync(key);
#endif
        AsyncOperationHandle ao = Addressables.DownloadDependenciesAsync(key);
        while (!ao.IsDone)
        {
            yield return null;
            EventManager.onProgressLoadingUpdated?.Invoke(ao.PercentComplete);
            Debug.Log("value : " + ao.PercentComplete);
        }
        onCompleted?.Invoke();
    }

    public IEnumerator DownloadAddressableAsync(string key, Action<float> onDownloading, Action onCompleted)
    {
#if UNITY_EDITOR
        //Addressables.ClearDependencyCacheAsync(key);
#endif
        AsyncOperationHandle ao = Addressables.DownloadDependenciesAsync(key);
        while (!ao.IsDone)
        {
            yield return null;
            onDownloading?.Invoke(ao.PercentComplete);
            Debug.LogWarning("value : " + ao.PercentComplete);
        }
        onCompleted?.Invoke();
    }

    public IEnumerator DownloadAddressableAsync(string key, UnityEngine.UI.Slider slider, TMPro.TMP_Text progressText, Action onCompleted)
    {
#if UNITY_EDITOR
        //Addressables.ClearDependencyCacheAsync(key);
#endif
        AsyncOperationHandle ao = Addressables.DownloadDependenciesAsync(key);
        while (!ao.IsDone)
        {
            yield return null;
            slider.value = ao.PercentComplete;
            progressText.text = ao.PercentComplete.ToString("0.00") + "%";
        }
        onCompleted?.Invoke();
    }

    /// <summary>
    /// Get Asset Size
    /// </summary>
    /// <param name="key"></param>
    /// <param name="onCompleted"></param>
    /// <returns></returns>
    public IEnumerator GetDownloadSizeAsync(string key, Action<long> onCompleted)
    {
        Debug.Log("GetDownloadSizeAsync");
        AsyncOperationHandle<long> ao = Addressables.GetDownloadSizeAsync(key); 
        ao.Completed += OnCompleted;
        yield return ao;
        Debug.LogWarning("GetDownloadSizeAsync Completed : " + ao.Result);
        if(ao.OperationException != null)
        Debug.LogError(ao.OperationException.Message);
        onCompleted?.Invoke(ao.Result);
    }

    private void OnCompleted(AsyncOperationHandle<long> size)
    {
        Debug.LogWarning("Completed : " + size.Result);
    }

    public IEnumerator LoadAssetAsync(string key)
    {
        AsyncOperationHandle<IList<IResourceLocation>> ao = Addressables.LoadResourceLocationsAsync(key);
        yield return ao;
        Debug.Log("count : " + ao.Result.Count);
        for (int i = 0; i < ao.Result.Count; i++)
        {
            Debug.Log("loaded : " + ao.Result[i].PrimaryKey);
        }
    }

    #region TASK

    public async Task<T> LoadAssetAsync<T>(string key)
    {
        AsyncOperationHandle ao = Addressables.LoadAssetAsync<T>(key);
        await ao.Task;
        return (T)ao.Task.Result;
    }

    public async Task<List<T>> LoadGroupAssetAsync<T>(string key)
    {
        AsyncOperationHandle<IList<IResourceLocation>> ao = Addressables.LoadResourceLocationsAsync(key, typeof(T));
        await ao.Task;
        Debug.Log("count : " + ao.Result.Count);
        List<T> objList = new List<T>();
        for (int i = 0; i < ao.Result.Count; i++)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(ao.Result[i].PrimaryKey);
            await handle.Task;
            objList.Add((T)ao.Result[i].Data);
            Debug.Log("downloaded : " + handle.Result.ToString());
        }
        return objList;
    }

    #endregion


    public IEnumerator LoadAssetAsync<T>(string key, Action<T> onCompleted)
    {
#if UNITY_EDITOR
        //Addressables.ClearDependencyCacheAsync(key);
#endif
        AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(key);
        yield return ao;
        onCompleted?.Invoke(ao.Result);
    }

    public IEnumerator LoadGroupAssetAsync<T>(string key, Action<List<T>> onCompleted)
    {
        AsyncOperationHandle<IList<IResourceLocation>> ao = Addressables.LoadResourceLocationsAsync(key, typeof(T));
        yield return ao;
        Debug.Log("count : " + ao.Result.Count);
        List<T> objList = new List<T>();
        for (int i = 0; i < ao.Result.Count; i++)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(ao.Result[i].PrimaryKey);
            yield return handle;
            objList.Add((T)ao.Result[i].Data);
            Debug.Log("downloaded : " + handle.Result.ToString());
        }
        onCompleted?.Invoke(objList);
    }

    /// <summary>
    /// Load from addressable and load its scene
    /// </summary>
    /// <param name="key"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public IEnumerator LoadSceneAsync(string key, Action<SceneInstance> onComplete)
    {
        Addressables.ClearDependencyCacheAsync(key);

        Debug.Log("Loading Scene " + key);
        AsyncOperationHandle<SceneInstance> ao = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Single, false);
        while (!ao.IsDone)
        {
            Debug.Log("status : " + ao.Status.ToString());
            yield return null;
            EventManager.onProgressLoadingUpdated?.Invoke(ao.PercentComplete);
            Debug.Log("value : " + ao.PercentComplete);
        }

        if (ao.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("status : " + ao.Status.ToString());
            EventManager.onProgressLoadingUpdated?.Invoke(1f);
            onComplete?.Invoke(ao.Result);
        }
    }

    public IEnumerator LoadSceneAsync(string key, UnityEngine.UI.Slider slider, TMPro.TMP_Text progressText, Action<SceneInstance> onComplete)
    {
        Debug.Log("Loading Scene " + key);
        AsyncOperationHandle<SceneInstance> ao = Addressables.LoadSceneAsync(key);
        while (!ao.IsDone)
        {
            yield return null;
            slider.value = ao.PercentComplete;
            progressText.text = ao.PercentComplete.ToString("0.00") + "%";
        }
        slider.value = 1f;
        onComplete?.Invoke(ao.Result);
    }

    public void LoadScene(string key)
    {
        Addressables.ClearDependencyCacheAsync(key);

        Debug.Log("Loading Scene " + key);
        Addressables.LoadSceneAsync(key).WaitForCompletion();
    }

}
