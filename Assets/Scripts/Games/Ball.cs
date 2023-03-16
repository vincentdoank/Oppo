using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WTI.NetCode;

public class Ball : MonoBehaviour
{
    private Rigidbody rigidBody;
    public Vector3 DefPos { get; private set; }

    private float smoothness = 100f;
    private bool stopUpdate = false;
    public bool isShooting;
    public ParticleSystem puffEffect;

    private Transform leftHand = null;
    private Transform rightHand = null;

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
        if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            stopUpdate = true;
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
        if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            rigidBody.isKinematic = false;
            stopUpdate = false;
            leftHand = null;
            rightHand = null;
            rigidBody.AddForce(FootballController.Instance.goalKeeper.transform.forward * 4f, ForceMode.Impulse);
        }
    }

    public void Shoot(Vector3 shootPosition)
    {
        Debug.LogWarning("Shoot : " + shootPosition);
        //Reset();
        FootballController.Instance.goalKeeper.canCatch = true;
        stopUpdate = false;
        rigidBody.isKinematic = false;
        isShooting = true;
        FootballController.Instance.scoreController.time.Pause(true);
        if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            Vector3 direction = shootPosition - transform.position;
            Vector3 upForce = Vector3.zero;
            if (shootPosition.y > transform.position.y)
            {
                upForce = Vector3.up * 5f;
            }
            rigidBody.AddForce(direction.normalized * 25f + upForce, ForceMode.Impulse);
        }
        //rigidBody.AddTorque(Vector3.down * 10000, ForceMode.Impulse);
        //float upForce = Random.Range(1, 7);
        //float sideForce = Random.Range(-8, 8);
        //rigidBody.AddForce(Vector3.forward * Random.Range(25, 40) + new Vector3(sideForce, upForce, 0), ForceMode.Impulse);
        FootballController.Instance.goal.gameObject.SetActive(true);
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
        FootballController.Instance.goalKeeper.canCatch = true;
        FootballController.Instance.scoreController.time.Pause(true);
        Vector3 upForce = Vector3.up * 5f;
        rigidBody.AddTorque(Vector3.forward * 5f);
        FootballController.Instance.goal.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsServer && FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        //if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
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
            if (centerPoint.y > FootballController.Instance.goalKeeper.transform.position.y + 2.2f)
            {
                centerPoint.y = FootballController.Instance.goalKeeper.transform.position.y + 2.2f;
            }
            centerPoint.z = FootballController.Instance.goalKeeper.transform.position.z + (FootballController.Instance.goalKeeper.transform.forward * 1.2f).z;
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
}
