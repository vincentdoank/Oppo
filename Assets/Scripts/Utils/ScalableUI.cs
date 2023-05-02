using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScalableUI : MonoBehaviour
{
    public LayoutGroup[] layoutGroups;
    public bool fit = true;
    public bool autoRescale = true;
    public UnityEvent<Vector3> onScaleChanged;

    public void OnEnable()
    {
        Debug.LogWarning("scale before : " + transform.localScale);
        //RectTransformUtility.CalculateRelativeRectTransformBounds(transform);
        //foreach (LayoutGroup lg in layoutGroups)
        //{
        //    lg.CalculateLayoutInputHorizontal();
        //    lg.CalculateLayoutInputVertical();
        //}
        StartCoroutine(WaitForRebuild());
    }

    private IEnumerator WaitForRebuild()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }
        if (fit)
        {
            float height = Screen.height;
            float recthHeight = ((RectTransform)transform).rect.height;

            float scaleRatio = recthHeight / height;
            Debug.LogWarning("scaleratio : " + scaleRatio + " " + height + " " + recthHeight);
            if (scaleRatio > 1)
            {
                Debug.LogWarning("scale before : " + transform.localScale);
                if (autoRescale)
                {
                    transform.localScale = Vector3.one / scaleRatio;
                }
                Debug.LogWarning("scale : " + transform.localScale);
                onScaleChanged?.Invoke(Vector3.one / scaleRatio);
            }
            else
            {
                onScaleChanged?.Invoke(Vector3.one);
            }
        }
    }
}
