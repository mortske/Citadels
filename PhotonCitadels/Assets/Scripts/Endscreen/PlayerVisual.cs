using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerVisual : MonoBehaviour 
{
    public Text text_name;
    public Text text_score;
    public Text text_districts;
    public Text text_cards;

    public void SetScores(string name, int score, int districts, int cards)
    {
        text_name.text = name + ":";
        text_score.text = score.ToString();
        text_districts.text = districts.ToString();
        text_cards.text = cards.ToString();
    }
}
