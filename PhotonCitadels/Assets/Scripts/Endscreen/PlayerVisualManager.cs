using UnityEngine;
using System.Collections;

public class PlayerVisualManager : MonoBehaviour 
{
    public PlayerVisual[] playerVisuals;
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
            playerVisuals[i].SetScores(playerScore.players[i].name, playerScore.score[i], 
                                       playerScore.players[i].BuiltDistricts.collection.Count, 
                                       playerScore.players[i].PlayerHand.collection.Count, 
                                       playerScore.players[i].coins);
        }
        for (int i = playerScore.players.Length; i < 7; i++)
        {
            playerVisuals[i].GrayOut();
        }
    }
}
