using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public Loading loading;

    public static LoadingManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLoading(string text, float progress, float minValue = 0f, float maxValue = 1f)
    {
        loading.gameObject.SetActive(true);
        loading.ShowLoading(text, progress, minValue, maxValue);
    }

    public void ShowLoading(string text, float progress, float minValue = 0f, float maxValue = 1f, string progressFormat = "0.0", string progressSuffix = "%")
    {
        loading.gameObject.SetActive(true);
        loading.ShowLoading(text, progress, minValue, maxValue, progressFormat, progressSuffix);
    }

    public void UpdateLoading(float progress)
    {
        loading.UpdateLoading(progress);
    }

    public void HideLoading()
    {
        loading.gameObject.SetActive(false);
    }
}
