using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : Player
{
    public Ball ball;
    public Transform ballPlaceholder;
    public SwipeController swipeController;

    private Vector3 shootPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        swipeController.onSwipeCompleted += PlayShootAnimation;
    }

    private void OnDestroy()
    {
        swipeController.onSwipeCompleted -= PlayShootAnimation;
    }

    public void PlayShootAnimation(Vector3 shootPosition)
    {
        if (((FootballController)GameMatchController.Instance).playerType == FootballController.PlayerType.Striker)
        {
            EventManager.onShootAnimationPlayed?.Invoke(GameManager.Instance.GetClientId());
        }
        ((FootballController)GameMatchController.Instance).swipeController.CanSwipe(false);
        ((FootballController)GameMatchController.Instance).scoreController.time.Pause(true);
        this.shootPosition = shootPosition;
        animator.SetTrigger("Shoot");
        Debug.LogWarning("play shoot animation : " + shootPosition);
    }

    public void PlayShootAnimation()
    {
        Debug.Log("animator : " + animator);
        ((FootballController)GameMatchController.Instance).scoreController.time.Pause(true);
        animator.SetTrigger("Shoot");
        Debug.LogWarning("play shoot animation");
    }

    //protected override void Update()
    //{
    //    base.Update();
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        PlayShootAnimation(shootPosition);
    //    }
    //}

    protected override void DoAction()
    {
        base.DoAction();
        if (((FootballController)GameMatchController.Instance).playerType == FootballController.PlayerType.Striker)
        {
            //Debug.Log("striker do action");
            Vector2 offset = Random.insideUnitCircle * 6f;
            shootPosition = ((FootballController)GameMatchController.Instance).goal.position + (Vector3)offset;
            PlayShootAnimation(shootPosition);
        }
    }

    protected override bool CheckIdle()
    {
        return base.CheckIdle();
    }


    public void Shoot()
    {
        ball.Shoot(shootPosition);
    }

    public void Serve()
    {
        ball.gameObject.SetActive(true);
        ball.transform.position = ballPlaceholder.position;
        ball.Shoot(shootPosition);
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.normal.textColor = Color.blue;
    //    GUI.Label(new Rect(60, 300, 200, 60), ((FootballController)GameMatchController.Instance).playerType.ToString(), style);
    //}
}
