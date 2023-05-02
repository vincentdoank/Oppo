using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PopupMessage : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text contentText;
    public Button singleButton;
    public Button acceptButton;
    public Button rejectButton;

    public Transform panel;

    private Tweener tween;

    private void Start()
    {
        panel.transform.localScale = Vector3.zero;

        tween = panel.DOScale(Vector3.one, 0.2f);
        //tween.Play();
    }

    public void PlayAnimation(Vector3 toScale)
    {
        tween = panel.DOScale(toScale, 0.2f);
        tween.Play();
    }

    public void ShowMessage(string title = "", string content = "", Action onButtonClicked = null)
    {
        titleText.text = title;
        contentText.text = content;
        singleButton.onClick.RemoveAllListeners();
        singleButton.onClick.AddListener(() => onButtonClicked?.Invoke());
        singleButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        rejectButton.gameObject.SetActive(false);
        panel.localScale = Vector3.zero;
    }

    public void ShowConfirmationMessage(string title = "", string content = "", Action onAcceptButtonClicked = null, Action onRejectButtonClicked = null)
    {
        titleText.text = title;
        contentText.text = content;
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => onAcceptButtonClicked?.Invoke());
        rejectButton.onClick.RemoveAllListeners();
        rejectButton.onClick.AddListener(() => onRejectButtonClicked?.Invoke());
        singleButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(true);
        panel.localScale = Vector3.zero;
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}
