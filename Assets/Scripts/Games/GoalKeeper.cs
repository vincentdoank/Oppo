using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GoalKeeper : Player
{
    private Vector3 acceleration;

    public Transform target;
    public Transform goalKeeper;
    public Transform hands;
    public Transform leftHand;
    public Transform rightHand;
    public Vector3 leftHandOffset;
    public Vector3 rightHandOffset;
    public float minX, maxX, minY, maxY;
    public float minAngle = -60f;
    public float maxAngle = 60f;
    public float smoothness = 5f;
    public float handSpeed = 2f;
    public Rig rig;

    private Vector3 calibrationOffset;

    private void Start()
    {
        Debug.LogWarning("Start");
        animator = GetComponent<Animator>();
        EventManager.onCalibrate += Calibrate;
    }

    private void OnDestroy()
    {
        EventManager.onCalibrate -= Calibrate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper && !pauseAi)
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
        }
        if (playerType == PlayerType.AI)
        {
            if (acceleration.x < -0.3f || acceleration.x > 0.3f)
            {
                ResetCheckIdle();
            }
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.IsServer && FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper && playerType == PlayerType.Human)
        {
            UpdateTargetPointer();
            UpdateGoalKeeper();
        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 50;
    //    style.normal.textColor = Color.white;
    //    GUI.Label(new Rect(50, 1000, 600, 60), "acc : " + acceleration, style);
    //}

    private void UpdateTargetPointer()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
        {
            Vector2 offset = new Vector2((maxX + minX) / 2, (maxY + minY) / 2);

            Vector3 targetPos = target.position;

            targetPos.x = offset.x + -acceleration.x * maxX * 4;
            targetPos.y = offset.y + acceleration.y * maxY * 4;
            if (targetPos.x > maxX) targetPos.x = maxX;
            else if (targetPos.x < minX) targetPos.x = minX;
            if (targetPos.y > maxY) targetPos.y = maxY;
            else if (targetPos.y < minY) targetPos.y = minY;

            target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * handSpeed);

            //float x = Mathf.Lerp(target.position.x, targetPos.x, Time.deltaTime * handSpeed);
            //float y = Mathf.Lerp(target.position.y, targetPos.y, Time.deltaTime * handSpeed);
            //target.position = new Vector3(x, y, target.position.z);
        }
#endif
    }

    private void UpdateGoalKeeper()
    {
        Vector3 pos = goalKeeper.position;
        pos.x = target.position.x;
        goalKeeper.position = pos;

        Vector3 handPos = hands.position;
        handPos.y = target.position.y;
        hands.position = handPos;

        if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
        {
            EventManager.onGoalKeeperPositionUpdated?.Invoke(GameManager.Instance.GetClientId(), pos, target.position);
            Debug.LogWarning("Send GK pos");
        }
        //float angle = acceleration.x * 60f * 2f;
        //if (angle > maxAngle) angle = maxAngle;
        //if (angle < minAngle) _ = minAngle;

        //float finalAngle = Mathf.LerpAngle(transform.localEulerAngles.z, -angle, Time.deltaTime * smoothness);

        ////goalKeeper.localEulerAngles = Vector3.Lerp(goalKeeper.localEulerAngles, new Vector3(0, 0, finalAngle), Time.deltaTime * smoothness);
        //goalKeeper.localEulerAngles = new Vector3(0, 0, finalAngle);
    }

    public void UpdatePosition(Vector3 position, Vector3 handPosition)
    {
        //goalKeeper.position = position;
        if (position.x > goalKeeper.position.x + 0.15f)
        {
            animator.SetFloat("Acceleration", 0.2f);
        }
        else if (position.x < goalKeeper.position.x - 0.15f)
        {
            animator.SetFloat("Acceleration", -0.2f);
        }
        else
        {
            animator.SetFloat("Acceleration", 0f);
        }
        goalKeeper.position = Vector3.Lerp(goalKeeper.position, position, Time.deltaTime * 30f);
        hands.position = handPosition;

    }

    public void Calibrate()
    {
        calibrationOffset = Input.acceleration;
    }

    protected override void DoAction()
    {
        Debug.LogWarning("isPauseAI");
        if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper && !pauseAi)
        {
            Debug.LogWarning("goalkeeper AI");
            Vector3 targetPos = FootballController.Instance.ball.transform.position;
            Vector3 pos = goalKeeper.position;

            if (targetPos.x > goalKeeper.position.x + 0.15f)
            {
                animator.SetFloat("Acceleration", 0.2f);
            }
            else if (targetPos.x < goalKeeper.position.x - 0.15f)
            {
                animator.SetFloat("Acceleration", -0.2f);
            }
            else
            {
                animator.SetFloat("Acceleration", 0f);
            }

            targetPos.y = pos.y;
            goalKeeper.position = Vector3.Lerp(goalKeeper.position, targetPos, Time.deltaTime * 5f);
        }
    }

    protected override bool CheckIdle()
    {
        Vector3 acc = acceleration;
        if (Mathf.Abs(acc.x - accelerometerTolerance) > 0 || Mathf.Abs(acc.y - accelerometerTolerance) > 0 || Mathf.Abs(acc.z - accelerometerTolerance) > 0)
        {
            ResetCheckIdle();
            return false;
        }
        return base.CheckIdle();
    }

    public override void PlayIdleAnimation()
    {
        //rig.weight = 1f;
        base.PlayIdleAnimation();
    }

    public override void PlayLoseAnimation()
    {
        //rig.weight = 0f;
        Release();
        base.PlayLoseAnimation();
    }

    public override void PlayWinAnimation()
    {
        //rig.weight = 0f;
        Release();
        base.PlayWinAnimation();
    }

    public void Catch()
    {
        rig.weight = 1f;
        FootballController.Instance.ball.Stop();
        leftHand.localPosition = leftHand.InverseTransformPoint(FootballController.Instance.ball.transform.position) + leftHandOffset;
        rightHand.localPosition = rightHand.InverseTransformPoint(FootballController.Instance.ball.transform.position) + rightHandOffset;
        if (GameManager.Instance.IsServer)
        {
            EventManager.onBallCaught?.Invoke(GameManager.Instance.GetClientId());
        }
    }

    public void Release()
    {
        leftHand.localPosition = leftHandOffset;
        rightHand.localPosition = rightHandOffset;
        rig.weight = 0f;
    }

    //protected override bool CheckIdle()
    //{
    //    Vector3 acc = acceleration;
    //    if (Mathf.Abs(acc.x - accelerometerTolerance) > 0 || Mathf.Abs(acc.y - accelerometerTolerance) > 0 || Mathf.Abs(acc.z - accelerometerTolerance) > 0)
    //    {
    //        ResetCheckIdle();
    //        return false;
    //    }
    //    return base.CheckIdle();
    //}
}
