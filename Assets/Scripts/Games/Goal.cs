using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball" && GameManager.Instance.IsServer)
        {
            ScoreGoal();
        }
    }

    private void ScoreGoal()
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
