using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loading : MonoBehaviour
{
    public TMP_Text loadingText;
    public TMP_Text progressText;
    public Slider loadingSlider;

    private float minValue;
    private float maxValue;
    private string progressFormat;
    private string progressSuffix;

    public void ShowLoading(string text, float progress, float minValue = 0f, float maxValue = 1f)
    {
        progress = CheckProgressValue(progress);

        this.minValue = minValue;
        this.maxValue = maxValue;

        loadingText.text = text;
        loadingSlider.value = progress;
        progressText.gameObject.SetActive(false);
    }

    public void ShowLoading(string text, float progress, float minValue = 0f, float maxValue = 1f, string progressFormat = "0.0", string progressSuffix = "%")
    {
        progress = CheckProgressValue(progress);

        this.minValue = minValue;
        this.maxValue = maxValue;

        loadingText.text = text;
        loadingSlider.value = progress;
        progressText.gameObject.SetActive(true);

        this.progressFormat = progressFormat;
        this.progressSuffix = progressSuffix;

        progressText.text = progress.ToString(progressFormat) + progressSuffix;
    }

    public void UpdateLoading(float progress)
    {
        Debug.LogWarning("progress : " + progress);
        progressText.text = (progress == maxValue ? progress * (progressSuffix == "%" ? 100 : 1) : (progress * (progressSuffix == "%" ? 100 : 1)).ToString(progressFormat)) + progressSuffix;
        progress = CheckProgressValue(progress);
        loadingSlider.value = progress;
    }

    private float CheckProgressValue(float progress)
    {
        if (progress < minValue)
        {
            progress = minValue;
        }
        else if (progress > maxValue)
        {
            progress = maxValue;
        }
        Debug.LogWarning("updated progress : " + progress);
        return progress;
    }
}
