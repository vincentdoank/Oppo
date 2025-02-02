using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using WTI.NetCode;

public class FootballPlayerData
{
    public bool isStrikerSelected;
    public bool isGoalKeeperSelected;
    public float shootTime;
    public int p1Score; //STRIKER
    public int p2Score; //GK
    public List<MatchData> matchDataList;
}

public class MatchData
{
    public int match;
    public int winnerId; //based on ScoreController.Player ( -1 for none )
}

public class FootballController : GameMatchController
{

    public Camera strikerCamera;
    public Camera goalKeeperCamera;

    public GoalKeeper goalKeeper;
    public Striker striker;
    public Transform goal;
    public GameObject[] missAreas;
    public SwipeController swipeController;
    public Image goalImage;
    public Image missImage;
    public Image saveImage;

    public ParticleSystem touchEffect;
    public ScoreController screenScoreController;
    public TweenFade transitionAnim;
    public List<MatchData> matchDataList = new List<MatchData>();

    public enum PlayerType
    {
        None,
        Striker,
        GoalKeeper,
        Draw
    }

    public PlayerType playerType = PlayerType.None;
    public bool isBallSaved = false;

    private float shootTimer = 4f;
    private float elapsedTime;

    protected override void Start()
    {
        base.Start();
        Debug.LogWarning("runtime : " + Application.platform);
        if (Application.platform != RuntimePlatform.Android)
        {
            scoreController.gameObject.SetActive(false);
            screenScoreController.gameObject.SetActive(true);
            scoreController = screenScoreController;
        }
        else
        {
            screenScoreController.gameObject.SetActive(false);
            scoreController.gameObject.SetActive(true);
        }
        Debug.LogWarning("screen");
        elapsedTime = 0;
        //scoreController.time.SetTime(10, ForwardShoot);

        EventManager.onStrikerSelected += OnStrikerSelected;
        EventManager.onGoalKeeperSelected += OnGoalKeeperSelected;
        EventManager.onNetworkConnected += ApplyRole;
        //if (NetworkController.Instance.GetClientId() == 1)
        //{
        //    StartCoroutine(GetWeatherData());
        //}

        //playerType = PlayerType.GoalKeeper;
        //OnGoalKeeperSelected();
    }

    public void ApplyRole()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            screenCamera.gameObject.SetActive(true);
            return;
        }

#if !UNITY_EDITOR
        Debug.Log("ApplyRole");
        OnGoalKeeperSelected();
        //OnStrikerSelected();
        //OnDrawLineSelected();
#else
        screenCamera.gameObject.SetActive(true);
#endif
        Debug.Log("player role : " + playerType.ToString());

        CheckWeather();
    }

    private void OnDestroy()
    {
        EventManager.onStrikerSelected -= OnStrikerSelected;
        EventManager.onGoalKeeperSelected -= OnGoalKeeperSelected;
    }

    //public void StartMatch()
    //{
    //    isStarted = true;
    //    CheckWeather();
    //    if (GameManager.Instance.IsServer)
    //    {
    //        Debug.Log("Start Match");
    //        scoreController.time.SetTime(10, () =>
    //        {
    //            ForwardShoot();
    //            EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
    //        });
    //        EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
    //    }
    //}

    public void PlayGoalAnimation()
    {
        goalImage.gameObject.SetActive(true);
        CrowdManager.Instance.CrowdClap();
    }

    public void PlayMissAnimation()
    {
        if (!isBallSaved)
        {
            missImage.gameObject.SetActive(true);
        }
    }

    public void PlaySaveAnimation()
    {
        isBallSaved = true;
        saveImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GameManager.Instance.IsServer && ball.isShooting)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= shootTimer)
            {
                //Debug.Log("check current match : " + CheckCurrentMatch());
                if (!CheckCurrentMatch())
                {
                    PlayMissAnimation();
                    scoreController.AddScore(ScoreController.Player.player2);
                    matchDataList.Add(new MatchData { match = scoreController.GetRound(), winnerId = (int)ScoreController.Player.player2 });
                    FootballPlayerData data = new FootballPlayerData
                    {
                        isStrikerSelected = clientsRoleDict.ContainsValue((int)PlayerType.Striker),
                        isGoalKeeperSelected = clientsRoleDict.ContainsValue((int)PlayerType.GoalKeeper),
                        p1Score = scoreController.GetPlayer1Score(),
                        p2Score = scoreController.GetPlayer2Score(),
                        matchDataList = matchDataList
                    };
                    EventManager.onFootballDataSent?.Invoke(data);
                    //EventManager.onScoreUpdated?.Invoke(GameManager.Instance.GetClientId(), scoreController.GetPlayer1Score(), scoreController.GetPlayer2Score());
                }
                if (scoreController.GetPlayer1Score() + scoreController.GetPlayer2Score() < 5 && CheckCurrentMatch())
                {
                    NextRound();
                }
                ball.isShooting = false;
            }
        }
    }

    private ulong GetClientIdByRole(PlayerType playerType)
    {
        if (clientsRoleDict.ContainsValue((int)playerType))
        {
            return clientsRoleDict.Where(x => x.Value == (int)playerType).FirstOrDefault().Key;
        }
        return 0;
    }

    public void CheckState(bool isStrikerSelected, bool isGoalKeeperSelected, float shootTime, int p1Score, int p2Score)
    {
        OnStrikerSelected(GetClientIdByRole(PlayerType.Striker), isStrikerSelected);
        OnGoalKeeperSelected(GetClientIdByRole(PlayerType.GoalKeeper), isGoalKeeperSelected);
    }

    private void OnStrikerSelected()
    {
        playerType = PlayerType.Striker;
        strikerCamera.gameObject.SetActive(true);
        goalKeeperCamera.gameObject.SetActive(false);
        screenCamera.gameObject.SetActive(false);

        EventManager.onStrikerSelected?.Invoke(GameManager.Instance.GetClientId(), true);
    }

    private void OnGoalKeeperSelected()
    {
        playerType = PlayerType.GoalKeeper;
        strikerCamera.gameObject.SetActive(false);
        goalKeeperCamera.gameObject.SetActive(true);
        screenCamera.gameObject.SetActive(false);

        EventManager.onGoalKeeperSelected?.Invoke(GameManager.Instance.GetClientId(), true);
    }

    private void OnDrawLineSelected()
    {
        playerType = PlayerType.Draw;
        GameManager.Instance.controlType = GameManager.ControlType.SHAKEDRAW;
        strikerCamera.gameObject.SetActive(false);
        goalKeeperCamera.gameObject.SetActive(false);
        screenCamera.gameObject.SetActive(true);

        OnDrawLineSelected(GameManager.Instance.GetClientId(), true);
    }

    public void UpdateGoalKeeperPosition(Vector3 position, Vector3 handPosition)
    {
        goalKeeper.UpdatePosition(position, handPosition);
    }

    public void UpdateFootballPosition(Vector3 position, Vector3 eulerAngle)
    {
        ball.UpdatePosition(position, eulerAngle);
    }

    public void Shoot()
    {
        striker.PlayShootAnimation();
    }


    protected override void AutoShoot()
    {
        ForwardShoot();
    }

    public void ForwardShoot()
    {
        if (!GameManager.Instance.IsServer)
        {
            scoreController.time.SetElapsedTime(0);
        }
        if (playerType == PlayerType.Striker)
        {
            striker.PlayShootAnimation(Vector3.forward * 25);
        }
    }

    public void StartShootTimer()
    {
        //Debug.Log("StartShootTimer");
        scoreController.time.SetTime(10, null);
    }

    public void NextRound()
    {
        Debug.LogWarning("NextRound");
        NetworkController.Instance.errorMessage = "NextRound";
        elapsedTime = 0;
        isBallSaved = false;
        PlayTransitionAnim();
        scoreController.NextRound();
        if (playerType == PlayerType.Striker)
        {
            //goalKeeper.Release();
            //ball.Reset();
            swipeController.CanSwipe(true);
            swipeController.ClearLine();
        }
        ball.Reset();
        if (GameManager.Instance.IsServer)
        {
            scoreController.time.SetTime(10, () =>
            {
                EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
            });
            EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
            EventManager.onNextRoundStarted?.Invoke(GameManager.Instance.GetClientId());
        }
        CrowdManager.Instance.CrowdRandom();
    }

    public void PlayWinAnimation()
    {
        if (scoreController.GetPlayer1Score() > scoreController.GetPlayer2Score())
        {
            striker.PlayWinAnimation();
            goalKeeper.PlayLoseAnimation();
            //WinnerInfoPopup.Instance.Show(PlayerType.Striker);
        }
        else
        {
            striker.PlayLoseAnimation();
            goalKeeper.PlayWinAnimation();
            //WinnerInfoPopup.Instance.Show(PlayerType.GoalKeeper);
        }
    }

    public void ResetMatch()
    {
        striker.PlayIdleAnimation();
        goalKeeper.PlayIdleAnimation();
        Debug.LogWarning("Reset Match");
        CheckWeather();
        elapsedTime = 0;
        isBallSaved = false;
        PlayTransitionAnim();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.isShooting = false;
        if (playerType == PlayerType.Striker)
        {
            //goalKeeper.Release();
            //ball.Reset();
            swipeController.CanSwipe(true);
            swipeController.ClearLine();
        }
        ball.Reset();
        //CrowdManager.Instance.CrowdRandom();
    }

    public IEnumerator WaitForResetMatch()
    {
        //Debug.Log("Reset Match");
        PlayWinAnimation();
        swipeController.CanSwipe(false);
        elapsedTime = 0;
        isBallSaved = false;
        ball.isShooting = false;
        yield return new WaitForSeconds(5f);
        striker.PlayIdleAnimation();
        goalKeeper.PlayIdleAnimation();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.Reset();
        //CrowdManager.Instance.CrowdRandom();
        scoreController.time.SetTime(10, () =>
        {
            EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
        });
        EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
        EventManager.onResetMatchStarted?.Invoke(GameManager.Instance.GetClientId());
    }

    public void PlayTransitionAnim()
    {
        //Debug.Log("PlayTransitionAnim : " + transitionAnim);
        //transitionAnim.Play();
    }

    public bool CheckCurrentMatch()
    {
        Debug.Log("match count : " + matchDataList.Count + " " + scoreController.GetRound());
        if (matchDataList.Count - 1 == scoreController.GetRound())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StopMatch()
    {
        ball.isShooting = false;
    }

    public void OnStrikerSelected(ulong clientId, bool isSelected)
    {
        if (isSelected)
        {
            clientsRoleDict.Add(clientId, (int)PlayerType.Striker);
        }
        else
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public void OnGoalKeeperSelected(ulong clientId, bool isSelected)
    {
        if (isSelected)
        {
            clientsRoleDict.Add(clientId, (int)PlayerType.GoalKeeper);
        }
        else
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public void OnDrawLineSelected(ulong clientId, bool isSelected)
    {
        if (isSelected)
        {
            clientsRoleDict.Add(clientId, (int)PlayerType.Draw);
        }
        else
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public void OnPartOfDayChanged(int part)
    {
        skyController.OnPartOfDayChanged(part);
    }

    public void OnWeatherChanged(int weather)
    {
        skyController.OnWeatherChanged(weather);
    }

    public void OnBallResetted()
    {
        ball.PlayPuffParticle();
    }

    public void OnBallCaught()
    {
        goalKeeper.Catch();
    }

    public void OnBallShot(Vector3 position)
    {
        ball.SendShootPosition(position);
    }

    public override void OnDisconnected(ulong clientId)
    {
        if (clientsRoleDict.ContainsKey(clientId))
        {
            if (clientsRoleDict[clientId] == (int)PlayerType.Striker)
            {
                EventManager.onStrikerSelected.Invoke(clientId, false);
            }
            else if (clientsRoleDict[clientId] == (int)PlayerType.GoalKeeper)
            {
                EventManager.onGoalKeeperSelected.Invoke(clientId, false);
            }
            else if (clientsRoleDict[clientId] == (int)PlayerType.Draw)
            {
                EventManager.onClearLine.Invoke();
            }

            clientsRoleDict.Remove(clientId);
        }
    }

    public override void SendPlayerData()
    {
        EventManager.onFootballDataSent?.Invoke(new FootballPlayerData
        {
            shootTime = scoreController.time.GetCurrentTime(),
            p1Score = scoreController.GetPlayer1Score(),
            p2Score = scoreController.GetPlayer2Score(),
            matchDataList = new List<MatchData>()
        });
    }

    public void UpdatePlayerData(FootballPlayerData data)
    {
        scoreController.SetScore(data.matchDataList, data.p1Score, data.p2Score);
        if (data.p1Score + data.p2Score >= 5)
        {
            PlayWinAnimation();
        }
    }

    public void ShowGoalArea(bool value)
    {
        goal.gameObject.SetActive(value);
    }

    public void ShowMissArea(bool value)
    {
        foreach (GameObject missArea in missAreas)
        {
            missArea.SetActive(value);
        }
    }
}
