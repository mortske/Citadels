using UnityEngine;
using System.Collections;

public class PlayerVisualManager : MonoBehaviour 
{
    PlayerVisual[] playerVisuals;
    PlayerScore playerScore;

    void Start()
    {
        playerScore = GameObject.Find("PlayerScore").GetComponent<PlayerScore>();
        setVisuals();
    }

    public void setVisuals()
    {
        for (int i = 0; i < playerScore.players.Length; i++)
        {
            playerVisuals[i].SetScores(playerScore.players[i], playerScore.score[i], 0, 0);
        }
    }
}
