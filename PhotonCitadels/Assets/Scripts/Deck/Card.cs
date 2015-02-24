using UnityEngine;
using System.Collections;

[System.Serializable]
public class Card 
{
    public string name;
    public int id { get; set; }
    public int cost;
    public CardColor color;

    public Color GetColor
    {
        get
        {
            switch (color)
            {
                case CardColor.Gold:
                    return new Color(255, 215, 0);
                case CardColor.Red:
                    return new Color(255, 0, 0);
                case CardColor.Green:
                    return Color.green;
                case CardColor.Blue:
                    return new Color(0, 0, 255);
                case CardColor.Purple:
                    return new Color(148, 0, 211);
                default:
                    return new Color(255, 255, 255);
            }
        }
    }

    public Card(string _name)
    {
        name = _name;
    }
}

public enum CardColor
{
    Gold,
    Red,
    Green,
    Blue,
    Purple
}
