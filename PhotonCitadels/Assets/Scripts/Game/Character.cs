using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Character : MonoBehaviour 
{
    public int myID;
    public bool isLocal { get; set; }
    Hand hand;
    public Hand PlayerHand
    {
        get { return hand; }
        set { hand = value; }
    }
    public int coins;
    public CharacterCard character;

    public void Initialize()
    {
        hand = GetComponent<Hand>();
        hand.Collection = new List<Card>();
        character = CharacterCard.Nothing;
        coins = 2;
        isLocal = true;
    }

    public void AdjustCoins(int amnt)
    {
        coins += amnt;
        Hashtable table = new Hashtable();
        table[(byte)1] = myID;
        table[(byte)2] = coins;
        GameManager.instance.gameClient.SendEvent(9, table, true, false);
    }

    public void SetCharacter(int character)
    {
        this.character = (CharacterCard)character;
        Hashtable table = new Hashtable();
        table[(byte)1] = myID;
        table[(byte)2] = character;
        GameManager.instance.gameClient.SendEvent(10, table, true, false);
    }

    public void UseCharacterAbility()
    {
        //TODO: implement all characters one by one so i dont miss shit
        switch (character)
        {
            case CharacterCard.Magician:
                //TODO: implement mage
                break;
            case CharacterCard.King:
                AddCoinsForCardColors(CardColor.Gold);
                //TODO: implement king
                break;
            case CharacterCard.Bishop:
                AddCoinsForCardColors(CardColor.Blue);
                break;
            case CharacterCard.Merchant:
                AddCoinsForCardColors(CardColor.Green);
                break;
            case CharacterCard.Architect:
                break;
            case CharacterCard.Warlord:
                AddCoinsForCardColors(CardColor.Red);
                break;
            case CharacterCard.Queen:
                break;
        }
    }

    void AddCoinsForCardColors(CardColor color)
    {
        int coins = 0;
        foreach (Card c in hand.collection)
        {
            if (c.color == CardColor.Blue)
            {
                coins++;
            }
        }
        AdjustCoins(coins);
    }
}

public enum CharacterCard
{
    Assassin,
    Thief,
    Magician,
    King,
    Bishop,
    Merchant,
    Architect,
    Warlord,
    Queen,
    Nothing
}
