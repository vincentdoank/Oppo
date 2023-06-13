using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WTI.NetCode;

public class TennisPlayer : Player
{
    public TennisGameController.PlayerType controlType;
    public float minX, maxX;
    public float moveSpeed;
    private Vector3 acceleration;
    private new Animator animator;

    private Vector3 calibrationOffset = Vector3.zero;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            acceleration.x = Input.acceleration.x - calibrationOffset.x;
            acceleration.y = Input.acceleration.y - calibrationOffset.y;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            acceleration.x = -Input.acceleration.y - calibrationOffset.x;
            acceleration.y = Input.acceleration.z - calibrationOffset.z;
        }

        animator.SetFloat("Acceleration", acceleration.x);
        
        //if (playerType == PlayerType.AI)
        //{
        //    if (acceleration.x > -0.1f && acceleration.x < 0.1f)
        //    {
        //        ResetCheckIdle();
        //    }
        //}
    }

    private void LateUpdate()
    {
        if ((TennisGameController.Instance.playerType == TennisGameController.PlayerType.Player1 && controlType == TennisGameController.PlayerType.Player1 && TennisGameController.Instance.player1 == this) ||
            (TennisGameController.Instance.playerType == TennisGameController.PlayerType.Player2 && controlType == TennisGameController.PlayerType.Player2 && TennisGameController.Instance.player2 == this))
        {
            Vector2 offset = new Vector2((maxX + minX) / 2, 0);
            Vector3 targetPos = transform.position;

            targetPos.x = offset.x + acceleration.x;
            if (targetPos.x > maxX) targetPos.x = maxX;
            else if (targetPos.x < minX) targetPos.x = minX;

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
            if (controlType == TennisGameController.PlayerType.Player1)
            {
                EventManager.onPlayer1PositionUpdated?.Invoke(NetworkController.Instance.GetClientId(), transform.position);
            }
            else
            {
                EventManager.onPlayer2PositionUpdated?.Invoke(NetworkController.Instance.GetClientId(), transform.position);
            }
        }
    }

    private void PlayAnimation(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    public void UpdateAnimation(string animationName)
    {
        PlayAnimation(animationName);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
        //if (TennisGameController.Instance.playerType == TennisGameController.PlayerType.Player1)
        //{
        //    EventManager.onPlayer1PositionUpdated?.Invoke(GameManager.Instance.GetClientId(), position);
        //    //Debug.LogWarning("Send GK pos");
        //}
        //else
        //{
        //    EventManager.onPlayer2PositionUpdated?.Invoke(GameManager.Instance.GetClientId(), position);
        //}
    }
}
