using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WTI.NetCode;

public class Ball : MonoBehaviour
{
    private Rigidbody rigidBody;
    public Vector3 DefPos { get; private set; }

    public int bounce;
    public int maxBounce;

    private float smoothness = 100f;
    public bool isShooting;
    public ParticleSystem puffEffect;

    private Transform leftHand = null;
    private Transform rightHand = null;

    public bool isEligilble = false;

    public LayerMask goalLayerMask;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        DefPos = transform.position;
        isShooting = false;
    }

    public void Reset()
    {
        Debug.Log("reset");
        PlayPuffParticle();
        StartCoroutine(WaitForReset());
        EventManager.onBallPositionResetted(GameManager.Instance.GetClientId());
        isEligilble = false;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void PlayPuffParticle()
    {
        puffEffect.Play();
    }

    private IEnumerator WaitForReset()
    {
        yield return new WaitForSeconds(0.1f);
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        transform.position = DefPos;
    }

    public void Catch(Transform leftHand, Transform rightHand)
    {
        if (GameManager.Instance.IsServer)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.isKinematic = true;
            this.leftHand = leftHand;
            this.rightHand = rightHand;

            //transform.position = catchPoint;
        }
    }

    public void Release()
    {
        if (GameManager.Instance.IsServer)
        {
            rigidBody.isKinematic = false;
            leftHand = null;
            rightHand = null;
            rigidBody.AddForce(((FootballController)GameMatchController.Instance).goalKeeper.transform.forward * 3f, ForceMode.Impulse);
        }
    }

    public void Shoot(Vector3 shootPosition)
    {
        Debug.LogWarning("Shoot : " + shootPosition);
        //Reset();
        //
        //.goalKeeper.canCatch = true;
        //stopUpdate = false;
        //rigidBody.isKinematic = false;
        //isShooting = true;
        //((FootballController)GameMatchController.Instance).scoreController.time.Pause(true);
        if (((FootballController)GameMatchController.Instance).playerType == FootballController.PlayerType.Striker)
        {
            EventManager.onBallShot?.Invoke(GameManager.Instance.GetClientId(), shootPosition);
            //SendShootPosition(shootPosition);
            //Vector3 direction = shootPosition - transform.position;
            //Vector3 upForce = Vector3.zero;
            //if (shootPosition.y > transform.position.y)
            //{
            //    upForce = Vector3.up * 6f;
            //}
            //rigidBody.AddForce(direction.normalized * 12f + upForce, ForceMode.Impulse);
        }
        //rigidBody.AddTorque(Vector3.down * 10000, ForceMode.Impulse);
        //float upForce = Random.Range(1, 7);
        //float sideForce = Random.Range(-8, 8);
        //rigidBody.AddForce(Vector3.forward * Random.Range(25, 40) + new Vector3(sideForce, upForce, 0), ForceMode.Impulse);
        //((FootballController)GameMatchController.Instance).goal.gameObject.SetActive(true);
    }

    //SERVER
    public void SendShootPosition(Vector3 shootPosition)
    {
        ((FootballController)GameMatchController.Instance).goalKeeper.canCatch = true;
        rigidBody.isKinematic = false;
        isShooting = true;
        Vector3 direction = shootPosition - transform.position;
        Vector3 upForce = Vector3.zero;
        if (shootPosition.y > transform.position.y)
        {
            upForce = Vector3.up * 6f;
        }
        rigidBody.AddForce(direction.normalized * 12f + upForce, ForceMode.Impulse);
        ((FootballController)GameMatchController.Instance).ShowGoalArea(true);
        ((FootballController)GameMatchController.Instance).ShowMissArea(true);
    }

    public void AddForce(Vector3 force)
    {
        //rigidBody.velocity = Vector3.zero;
        //force.x *= 2f;
        //rigidBody.AddForce(force, ForceMode.VelocityChange);        
    }

    public void Shoot()
    {
        //Reset();
        isShooting = true;
        rigidBody.AddTorque(Vector3.forward * 3f);
        ((FootballController)GameMatchController.Instance).goalKeeper.canCatch = true;
        ((FootballController)GameMatchController.Instance).scoreController.time.Pause(true);
        ((FootballController)GameMatchController.Instance).ShowGoalArea(true);
        ((FootballController)GameMatchController.Instance).ShowMissArea(true);
    }

    public void Shoot(Vector3 force, float randomize)
    {
        rigidBody.AddForce(new Vector3(force.x + randomize, force.y, force.z), ForceMode.Impulse);
    }

    private void Update()
    {
        //if (!GameManager.Instance.IsServer && ((FootballController)GameMatchController.Instance).playerType == FootballController.PlayerType.Striker)
        //if (((FootballController)GameMatchController.Instance).playerType == FootballController.PlayerType.Striker)
        if(GameManager.Instance.IsServer)
        {
            //rigidBody.isKinematic = false;
            EventManager.onFootballUpdated?.Invoke(GameManager.Instance.GetClientId(), transform.position, transform.localEulerAngles);
        }
        else
        {
            rigidBody.isKinematic = true;
        }

        if (rigidBody.isKinematic && leftHand != null && rightHand != null)
        {
            Vector3 centerPoint = (leftHand.position + rightHand.position) / 2;
            if (centerPoint.y > ((FootballController)GameMatchController.Instance).goalKeeper.transform.position.y + 2.2f)
            {
                centerPoint.y = ((FootballController)GameMatchController.Instance).goalKeeper.transform.position.y + 2.2f;
            }
            centerPoint.z = ((FootballController)GameMatchController.Instance).goalKeeper.transform.position.z + (((FootballController)GameMatchController.Instance).goalKeeper.transform.forward * 1.2f).z;
            //transform.position = centerPoint;
            transform.position = Vector3.Lerp(transform.position, centerPoint, Time.deltaTime * 10f);
        }
    }

    public void UpdatePosition(Vector3 position, Vector3 eulerAngle)
    {
        if (isShooting)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothness);
        }
        else
        {
            transform.position = position;
        }
        transform.localEulerAngles = eulerAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == goalLayerMask)
        {
            if (bounce == 0)
            {
                isEligilble = true;
            }
        }
        
        bounce += 1;

        if (bounce == 2 && isEligilble)
        {
            if (!((FootballController)GameMatchController.Instance).isBallSaved)
            {
                ((FootballController)GameMatchController.Instance).matchDataList.Add(new MatchData { match = ((FootballController)GameMatchController.Instance).scoreController.GetRound(), winnerId = (int)ScoreController.Player.player1 });
                ((FootballController)GameMatchController.Instance).scoreController.AddScore(ScoreController.Player.player1);
                //((FootballController)GameMatchController.Instance).NextMatch();
                Debug.Log("clientdId : " + GameManager.Instance.GetClientId());
                Debug.Log("get player 1 score : " + ((FootballController)GameMatchController.Instance).scoreController.GetPlayer1Score());

                FootballPlayerData data = new FootballPlayerData
                {
                    isStrikerSelected = GameMatchController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.Striker),
                    isGoalKeeperSelected = GameMatchController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.GoalKeeper),
                    p1Score = GameMatchController.Instance.scoreController.GetPlayer1Score(),
                    p2Score = GameMatchController.Instance.scoreController.GetPlayer2Score(),
                    matchDataList = ((FootballController)GameMatchController.Instance).matchDataList
                };
                EventManager.onFootballDataSent?.Invoke(data);

                //EventManager.onScoreUpdated?.Invoke(GameManager.Instance.GetClientId(), ((FootballController)GameMatchController.Instance).scoreController.GetPlayer1Score(), ((FootballController)GameMatchController.Instance).scoreController.GetPlayer2Score());
            }
        }
    }
}
