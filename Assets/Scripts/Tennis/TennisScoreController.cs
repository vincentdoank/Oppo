using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class TennisPlayerHUD
{
    public int maxRound = 5;

    public TMP_Text nameText;
    public TMP_Text scoreText;
    public Image playerNamePanelImage;
    public List<Image> scoreImageValueList;

    public void Reset(TennisScoreController controller)
    {
        scoreText.text = "0";
        for (int i = 0; i < scoreImageValueList.Count; i++)
        {
            scoreImageValueList[i].sprite = controller.defSprite;
        }
    }

    public void SetScore(TennisScoreController controller, bool success, int score, int round, Action onMatchEnd)
    {
        //Debug.Log("match : " + round + " " + maxRound);
        scoreText.text = score.ToString();
        scoreImageValueList[round].sprite = success ? controller.scoreSprite : controller.missSprite;
        if (round >= maxRound - 1)
        {
            onMatchEnd?.Invoke();
        }
    }
}


public class TennisScoreController : MonoBehaviour
{
    public TennisPlayerHUD player1Hud;
    public TennisPlayerHUD player2Hud;
    public Sprite scoreSprite;
    public Sprite missSprite;
    public Sprite defSprite;

    public Image scorePanelImage;

    public TimeController time;

    public enum Player
    {
        player1 = 1,
        player2 = 2
    }
    public Player player;

    private int player1Score;
    private int player2Score;
    private int round = 0;

    private void Start()
    {
        ResetMatch();
    }

    public int GetPlayer1Score()
    {
        return player1Score;
    }

    public int GetPlayer2Score()
    {
        return player2Score;
    }

    public void ResetMatch()
    {
        round = 0;
        ResetScore();
    }

    public void ResetScore()
    {
        player1Score = player2Score = 0;

        player1Hud.Reset(this);
        player2Hud.Reset(this);
    }

    public void NextRound()
    {
        round += 1;
    }

    public void AddScore(Player player)
    {
        if (GameManager.Instance.IsServer)
        {
            switch (player)
            {
                case Player.player1:
                    player1Score += 1;
                    player1Hud.SetScore(this, true, player1Score, round, null);
                    ((TennisGameController)(GameMatchController.Instance)).ShowGoalArea(false);
                    ((TennisGameController)(GameMatchController.Instance)).ShowMissArea(false);
                    player2Hud.SetScore(this, false, player2Score, round, () => StartCoroutine(((FootballController)GameMatchController.Instance).WaitForResetMatch()));
                    break;
                case Player.player2:
                    player2Score += 1;
                    player1Hud.SetScore(this, false, player1Score, round, null);
                    ((TennisGameController)(GameMatchController.Instance)).ShowGoalArea(false);
                    ((FootballController)GameMatchController.Instance).ShowMissArea(false);
                    player2Hud.SetScore(this, true, player2Score, round, () => StartCoroutine(((FootballController)GameMatchController.Instance).WaitForResetMatch()));
                    break;
            }
        }
    }

    public void SetScore(int p1Score, int p2Score)
    {
        //Debug.Log("SetScore : " + p1Score + " " + p2Score);
        player1Score = p1Score;
        player2Score = p2Score;
    }

    public void ChangePlayer1Name(string name)
    {
        player1Hud.nameText.text = name;
    }

    public void ChangePlayer2Name(string name)
    {
        player2Hud.nameText.text = name;
    }

    public void ChangePlayer1NameBackground(Sprite sprite)
    {
        player1Hud.playerNamePanelImage.sprite = sprite;
        player1Hud.playerNamePanelImage.type = Image.Type.Sliced;
    }

    public void ChangePlayer2NameBackground(Sprite sprite)
    {
        player2Hud.playerNamePanelImage.sprite = sprite;
        player2Hud.playerNamePanelImage.type = Image.Type.Sliced;
    }

    public void ChangeScoreBackground(Sprite sprite)
    {
        scorePanelImage.sprite = sprite;
        scorePanelImage.type = Image.Type.Sliced;
    }

    public int GetRound()
    {
        return round;
    }
}
