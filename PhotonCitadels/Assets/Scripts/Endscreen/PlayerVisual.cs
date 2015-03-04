using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerVisual : MonoBehaviour 
{
    public Text text_name;
    public Text text_score;
    public Text text_districts;
    public Text text_cards;
    public Text text_coins;

    public void SetScores(string name, int score, int districts, int cards, int coins)
    {
        text_name.text = name + ":";
        text_score.text = score.ToString();
        text_districts.text = districts.ToString();
        text_cards.text = cards.ToString();
        text_coins.text = coins.ToString();
    }

    public void GrayOut()
    {
        text_name.color = Color.gray;
        text_score.color = Color.gray;
        text_districts.color = Color.gray;
        text_cards.color = Color.gray;
        text_coins.color = Color.gray;
    }
}
