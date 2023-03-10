using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catch : MonoBehaviour
{
    public GoalKeeper goalKeeper;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            if (GameManager.Instance.IsServer)
            {
                BallCatch();
            }
        }
    }

    private void BallCatch()
    {
        goalKeeper.Catch();
    }
}
