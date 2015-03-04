using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour 
{
    public Character[] players;
    public int[] score;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetScores(Character[] characters, int[] scores)
    {
        players = characters;
        score = scores;
    }
}
