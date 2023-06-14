using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using WTI.NetCode;

public class TennisPlayerData
{
    public float shootTime;
    public int p1Score; //STRIKER
    public int p2Score; //GK
    public int round;
    public int maxRound;
}

public class TennisGameController : GameMatchController
{
    public Camera player1Camera;
    public Camera player2Camera;

    public TennisPlayer player1;
    public TennisPlayer player2;
    public Transform goal;
    public GameObject missArea;
    public SwipeController swipeController;
    public Image scoreImage;
    public Image outImage;

    public ParticleSystem touchEffect;
    public TweenFade transitionAnim;
    public List<MatchData> matchDataList = new List<MatchData>();

    public enum PlayerType
    {
        None,
        Player1,
        Player2
    }

    public PlayerType playerType = PlayerType.None;
    public bool isBallSaved = false;

    private float shootTimer = 4f;
    private float elapsedTime;

    private int round;
    private int maxRound;

    protected override void Start()
    {
        base.Start();
        Debug.LogWarning("runtime : " + Application.platform);

        elapsedTime = 0;
        //scoreController.time.SetTime(10, ForwardShoot);


        //if (NetworkController.Instance.GetClientId() == 1)
        //{
        //    StartCoroutine(GetWeatherData());
        //}

        //playerType = PlayerType.GoalKeeper;
        //OnGoalKeeperSelected();
        StartCoroutine(WaitForApplyRole());
    }

    private IEnumerator WaitForApplyRole()
    {
        yield return null;
        ApplyRole();
    }

    public void ApplyRole()
    {
        if (GameManager.Instance.IsServer)
        {
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);
            screenCamera.gameObject.SetActive(true);
            return;
        }
#if !UNITY_EDITOR
        Debug.Log("AplyRole");

        OnPlayer1Selected();
        //OnPlayer2Selected();
        //OnDrawLineSelected();
#endif
        Debug.Log("player role : " + playerType.ToString());

        CheckWeather();
    }

    public override void StartMatch()
    {
        isStarted = true;
        CheckWeather();
        if (GameManager.Instance.IsServer)
        {
            Debug.Log("Start Match");
            scoreController.time.SetTime(10, () =>
            {
                EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
            });
            EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
        }
    }

    public void PlayScoreAnimation()
    {
        CrowdManager.Instance.CrowdClap();
    }

    public void PlayOutAnimation()
    {
        outImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GameManager.Instance.IsServer)
        {
            
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

    //public void CheckState(float shootTime, int p1Score, int p2Score)
    //{
    //    OnPlayer1Selected(GetClientIdByRole(PlayerType.Player1));
    //    OnPlayer2Selected(GetClientIdByRole(PlayerType.Player2));
    //}

    public void CheckState()
    {
        if (GameManager.Instance.IsServer)
        {
            //CheckState(scoreController.time.GetCurrentTime(), scoreController.GetPlayer1Score(), scoreController.GetPlayer2Score());
        }
    }

    private void OnPlayer1Selected()
    {
        playerType = PlayerType.Player1;
        player1Camera.gameObject.SetActive(true);
        player2Camera.gameObject.SetActive(false);

        EventManager.onStrikerSelected?.Invoke(GameManager.Instance.GetClientId(), true);
    }

    private void OnPlayer2Selected()
    {
        playerType = PlayerType.Player2;
        player1Camera.gameObject.SetActive(false);
        player2Camera.gameObject.SetActive(true);

        EventManager.onGoalKeeperSelected?.Invoke(GameManager.Instance.GetClientId(), true);
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
        CrowdManager.Instance.CrowdRandom();
    }

    public void PlayWinAnimation()
    {
        if (scoreController.GetPlayer1Score() > scoreController.GetPlayer2Score())
        {
            player1.PlayWinAnimation();
            player2.PlayLoseAnimation();
        }
        else
        {
            player1.PlayLoseAnimation();
            player2.PlayWinAnimation();
        }
    }

    public void ResetMatch()
    {
        player1.PlayIdleAnimation();
        player2.PlayIdleAnimation();
        Debug.LogWarning("Reset Match");
        CheckWeather();
        elapsedTime = 0;
        isBallSaved = false;
        PlayTransitionAnim();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.isShooting = false;
        if (GameManager.Instance.IsServer)
        {
            ball.Reset();
        }
        CrowdManager.Instance.CrowdRandom();
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
        player1.PlayIdleAnimation();
        player2.PlayIdleAnimation();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.Reset();
        CrowdManager.Instance.CrowdRandom();
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

    public void OnBallShot(Vector3 position)
    {
        ball.SendShootPosition(position);
    }

    public override void OnDisconnected(ulong clientId)
    {
        if (clientsRoleDict.ContainsKey(clientId))
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public override void SendPlayerData()
    {
        EventManager.onTennisDataSent?.Invoke(new TennisPlayerData
        {
            shootTime = scoreController.time.GetCurrentTime(),
            p1Score = scoreController.GetPlayer1Score(),
            p2Score = scoreController.GetPlayer2Score(),
            round = round,
            maxRound = maxRound
        });
    }

    public void UpdatePlayerData(TennisPlayerData data)
    {
        scoreController.SetScore(data.p1Score, data.p2Score);
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
        missArea.SetActive(value);
    }

    public void OnPlayer1PositionUpdated(Vector3 position)
    {
        player1.UpdatePosition(position);
    }

    public void OnPlayer2PositionUpdated(Vector3 position)
    {
        player2.UpdatePosition(position);
    }

    public void OnPlayer1AnimationUpdated(string animationName)
    {
        player1.UpdateAnimation(animationName);
    }

    public void OnPlayer2AnimationUpdated(string animationName)
    {
        player2.UpdateAnimation(animationName);
    }
}
