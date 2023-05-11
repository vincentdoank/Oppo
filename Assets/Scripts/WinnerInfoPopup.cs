using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class PlayerFlag
{
    public FootballController.PlayerType playerType;
    public Sprite flag;
}

public class WinnerInfoPopup : MonoBehaviour
{
    public List<PlayerFlag> playerFlagList;
    public Image flagImage;

    public RectTransform contentRectTransform;
    public RectTransform panelRectTransform;

    public static WinnerInfoPopup Instance { get; private set; }

    private void Start()
    {
        Instance = this;   
    }

    public void Show(FootballController.PlayerType winner)
    {
        panelRectTransform.gameObject.SetActive(true);
        Sprite flag = playerFlagList.Where(x => x.playerType == winner).Select(x => x.flag).FirstOrDefault();
        if (flag)
        {
            flagImage.sprite = flag;
        }

        PlayScaleAnimation();
    }

    private void PlayScaleAnimation()
    {
        contentRectTransform.anchoredPosition = new Vector2(-3400, 0);
        panelRectTransform.localScale = new Vector3(1, 0, 1);
        Tweener tween = panelRectTransform.DOScaleY(1f, 0.2f);
        tween.OnComplete(PlayPosition1Animation);
        tween.Play();
    }

    private void PlayPosition1Animation()
    {
        contentRectTransform.anchoredPosition = new Vector2(-3400, 0);
        Tweener tween = contentRectTransform.DOAnchorPos(Vector3.zero, 0.2f);
        tween.OnComplete(PlayPosition2Animation);
        tween.Play();
    }

    private void PlayPosition2Animation()
    {
        Tweener tween = contentRectTransform.DOAnchorPos(new Vector2(3400, 0), 0.2f);
        tween.SetDelay(1.5f);
        tween.OnComplete(Hide);
        tween.Play();
    }

    private void Hide()
    {
        panelRectTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Show(FootballController.PlayerType.Striker);
        }
    }
}
