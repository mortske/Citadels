using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Character : MonoBehaviour 
{
    public int myID;
    public bool isLocal { get; set; }
    Hand hand;
    CardCollection builtDistricts;
    public bool hasTakenTurn = false;
    public bool murdered = false;

    public Hand PlayerHand
    {
        get { return hand; }
        set { hand = value; }
    }
    public CardCollection BuiltDistricts
    {
        get { return builtDistricts; }
        set { builtDistricts = value; }
    }

    public int coins;
    public CharacterCard character;

    public void Initialize()
    {
        hand = GetComponent<Hand>();
        hand.Collection = new List<Card>();
        builtDistricts = GetComponents<CardCollection>()[1];
        builtDistricts.collection = new List<Card>();
        character = CharacterCard.Nothing;
        coins = 2;
        isLocal = true;
    }

    public void Reset()
    {
        SetCharacter(9);
        hasTakenTurn = false;
        murdered = false;
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

    public void TakePlayerTurn()
    {
        if (!murdered)
        {
            if (character == CharacterCard.Assassin)
            {
                int[] tmp = { 1, 2, 3, 4, 5, 6, 7, 8 };
                GameManager.instance.gameGUI.ShowCharacterSelection(tmp, "Pick a character to murder!");
            }
            else
                GameManager.instance.gameGUI.ShowTakeAnAction();
        }
        else
        {
            GameManager.instance.SendTurnToNextCharacter();
        }
    }

    public void SelectedVictim(int character)
    {
        foreach (Character remotePlayer in GameManager.instance.remotePlayers)
        {
            if (remotePlayer.character == (CharacterCard)character)
            {
                Hashtable table = new Hashtable();
                table[(byte)1] = remotePlayer.myID;
                GameManager.instance.gameClient.SendEvent(13, table, true, false);
            }
        }

        GameManager.instance.gameGUI.ShowTakeAnAction();
    }

    public void Murder()
    {
        murdered = true;
    }

    public void BuildDistrict()
    {
        CardVisual selectedCard = GameManager.instance.gameGUI.selectedCard;
        if (selectedCard != null)
        {
            if (coins >= selectedCard.card.cost)
            {
                AdjustCoins(-selectedCard.card.cost);
                Card c = hand.RemoveCardWithID(selectedCard.card.id);
                GameManager.instance.gameGUI.RemoveCard(c);
                builtDistricts.AddCard(c);
                GameManager.instance.gameGUI.RecalculateCardPositions();
                GameManager.instance.gameGUI.AddDistrict(c, builtDistricts.collection.Count - 1);
                GameManager.instance.gameGUI.turnButtons[0].interactable = false;

                Hashtable table = new Hashtable();
                table[(byte)1] = myID;
                table[(byte)2] = c.id;
                GameManager.instance.gameClient.SendEvent(11, table, true, false);
            }
        }
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
            if (c.color == color)
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
