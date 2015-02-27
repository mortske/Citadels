using UnityEngine;
using System;
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
    public int stolenFrom = -1;

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
        stolenFrom = -1;
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
            if (stolenFrom != -1)
            {
                Hashtable table = new Hashtable();
                table[(byte)1] = stolenFrom;
                table[(byte)2] = coins;
                GameManager.instance.gameClient.SendEvent(15, table, true, false);
                AdjustCoins(-coins);
            }
            if (character == CharacterCard.Assassin)
                GameManager.instance.gameGUI.ShowCharacterSelection(FindSelectableCharacters(), "Pick a character to murder!");
            else if(character == CharacterCard.Thief)
                GameManager.instance.gameGUI.ShowCharacterSelection(FindSelectableCharacters(), "Pick a character to steal from!");
            else
                GameManager.instance.gameGUI.ShowTakeAnAction();
        }
        else
        {
            GameManager.instance.SendTurnToNextCharacter();
        }
    }

    int[] FindSelectableCharacters()
    {
        List<int> tmp = new List<int>();
        for (int i = 0; i < Enum.GetValues(typeof(CharacterCard)).Length - 1; i++)
        {
            bool cont = false;
            if (i == 0)
                cont = true;

            if (i == (int)character)
                cont = true;
            
            for (int j = 0; j < GameManager.instance.removedChars.Count; j++)
            {
                if (i == GameManager.instance.removedChars[j])
                    cont = true;
            }
            for (int j = 0; j < GameManager.instance.remotePlayers.Length; j++)
                if (GameManager.instance.remotePlayers[j].murdered)
                    if(i == (int)GameManager.instance.remotePlayers[j].character)
                        cont = true;
            if (cont)
                continue;
            tmp.Add(i);
        }
        return tmp.ToArray();
    }

    public void SelectedVictim(int character)
    {
        if (this.character == CharacterCard.Assassin)
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
        }
        else if (this.character == CharacterCard.Thief)
        {
            foreach (Character remotePlayer in GameManager.instance.remotePlayers)
            {
                if (remotePlayer.character == (CharacterCard)character)
                {
                    Hashtable table = new Hashtable();
                    table[(byte)1] = remotePlayer.myID;
                    table[(byte)2] = myID;
                    GameManager.instance.gameClient.SendEvent(14, table, true, false);
                }
            }
        }

        GameManager.instance.gameGUI.ShowTakeAnAction();
    }

    public void Murder()
    {
        murdered = true;
    }

    public void StealFrom(int playerID)
    {
        stolenFrom = playerID;
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
                GameManager.instance.gameGUI.ShowMagicianSelection();
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

    public void TakeNewHand(int player)
    {
        if (player == -1)
        {
            int cards = hand.collection.Count;
            for (int i = 0; i < cards; i++)
            {
                Card c = hand.RemoveCardAt(0);
                GameManager.instance.discard.AddCard(c);
                GameManager.instance.gameGUI.RemoveCard(c);
            }
            for (int i = 0; i < cards; i++)
            {
                Card c = GameManager.instance.deck.RemoveTopCard();
                hand.AddCard(c);
            }
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
