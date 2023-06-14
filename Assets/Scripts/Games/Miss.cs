using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball" && GameManager.Instance.IsServer)
        {
            Out();            
        }
    }

    private void Out()
    {
        if (!((FootballController)GameMatchController.Instance).CheckCurrentMatch())
        {
            if (GameManager.Instance.IsServer)
            {
                ((FootballController)GameMatchController.Instance).PlayMissAnimation();
            }
            ((FootballController)GameMatchController.Instance).matchDataList.Add(new MatchData { match = ((FootballController)GameMatchController.Instance).scoreController.GetRound(), winnerId = (int)ScoreController.Player.player2 });
            ((FootballController)GameMatchController.Instance).scoreController.AddScore(ScoreController.Player.player2);
            FootballPlayerData data = new FootballPlayerData
            {
                isStrikerSelected = ((FootballController)GameMatchController.Instance).clientsRoleDict.ContainsValue((int)FootballController.PlayerType.Striker),
                isGoalKeeperSelected = ((FootballController)GameMatchController.Instance).clientsRoleDict.ContainsValue((int)FootballController.PlayerType.GoalKeeper),
                p1Score = ((FootballController)GameMatchController.Instance).scoreController.GetPlayer1Score(),
                p2Score = ((FootballController)GameMatchController.Instance).scoreController.GetPlayer2Score(),
                matchDataList = ((FootballController)GameMatchController.Instance).matchDataList  
            };
            EventManager.onFootballDataSent?.Invoke(data);
            gameObject.SetActive(false);
        }
    }
}
