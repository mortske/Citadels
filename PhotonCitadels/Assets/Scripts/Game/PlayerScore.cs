using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour 
{
    public string[] players;
    public int[] score;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetScores(string[] characters, int[] scores)
    {
        players = characters;
        score = scores;
    }
}
