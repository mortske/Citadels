using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Character : MonoBehaviour 
{
    public GameManager gameManager;
    public int myID;
    public bool isLocal { get; set; }
    Hand hand;
    CardCollection builtDistricts;
    public bool hasTakenTurn { get; set; }
    public bool murdered { get; set; }
    public int stolenFrom { get; set; }
    public int districtstoBuild { get; set; }

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
        gameManager = GameObject.Find("Game").GetComponent<GameManager>();
        hand = GetComponent<Hand>();
        hand.Collection = new List<Card>();
        builtDistricts = GetComponents<CardCollection>()[1];
        builtDistricts.collection = new List<Card>();
        character = CharacterCard.Nothing;
        coins = 2;
        isLocal = true;
        hasTakenTurn = false;
        murdered = false;
        stolenFrom = -1;
        districtstoBuild = 1;
    }

    public void Reset()
    {
        SetCharacter(9);
        hasTakenTurn = false;
        murdered = false;
        stolenFrom = -1;
        districtstoBuild = 1;
    }

    public void AdjustCoins(int amnt)
    {
        coins += amnt;
        Hashtable table = new Hashtable();
        table[(byte)1] = myID;
        table[(byte)2] = coins;
        gameManager.gameClient.SendEvent(9, table, true, false);
    }

    public void SetCharacter(int character)
    {
        this.character = (CharacterCard)character;
        Hashtable table = new Hashtable();
        table[(byte)1] = myID;
        table[(byte)2] = character;
        gameManager.gameClient.SendEvent(10, table, true, false);
    }

    public void TakePlayerTurn()
    {
        if (character == CharacterCard.King)
        {
            gameManager.SetKingID(myID);
            Hashtable table = new Hashtable();
            table[(byte)1] = stolenFrom;
            gameManager.gameClient.SendEvent(18, table, true, false);
        }
        if (!murdered)
        {
            if (stolenFrom != -1)
            {
                Hashtable table = new Hashtable();
                table[(byte)1] = stolenFrom;
                table[(byte)2] = coins;
                gameManager.gameClient.SendEvent(15, table, true, false);
                AdjustCoins(-coins);
            }

            if (character == CharacterCard.Architect)
            {
                gameManager.gameGUI.turnButtons[1].interactable = false;
                districtstoBuild = 3;
            }
            if (character == CharacterCard.Queen)
            {
                //TODO: Change to king character and not marker
                //TODO: Change to get coins after turn is over if king was murdered
                //TODO: make queen available only if 5 or more players
                gameManager.gameGUI.turnButtons[1].interactable = false;
                int kingID = gameManager.KingID;
                if (kingID == gameManager.PrevID || kingID == gameManager.NextID)
                {
                    AdjustCoins(3);
                }
            }

            if (character == CharacterCard.Assassin)
            {
                gameManager.gameGUI.turnButtons[1].interactable = false;
                gameManager.gameGUI.ShowCharacterSelection(FindSelectableCharacters(), "Pick a character to murder!");
            }
            else if (character == CharacterCard.Thief)
            {
                gameManager.gameGUI.turnButtons[1].interactable = false;
                gameManager.gameGUI.ShowCharacterSelection(FindSelectableCharacters(), "Pick a character to steal from!");
            }
            else
                gameManager.gameGUI.ShowTakeAnAction();
        }
        else
        {
            gameManager.SendTurnToNextCharacter();
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
            
            for (int j = 0; j < gameManager.removedChars.Count; j++)
            {
                if (i == gameManager.removedChars[j])
                    cont = true;
            }
            for (int j = 0; j < gameManager.remotePlayers.Length; j++)
                if (gameManager.remotePlayers[j].murdered)
                    if(i == (int)gameManager.remotePlayers[j].character)
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
            foreach (Character remotePlayer in gameManager.remotePlayers)
            {
                if (remotePlayer.character == (CharacterCard)character)
                {
                    Hashtable table = new Hashtable();
                    table[(byte)1] = remotePlayer.myID;
                    gameManager.gameClient.SendEvent(13, table, true, false);
                }
            }
        }
        else if (this.character == CharacterCard.Thief)
        {
            foreach (Character remotePlayer in gameManager.remotePlayers)
            {
                if (remotePlayer.character == (CharacterCard)character)
                {
                    Hashtable table = new Hashtable();
                    table[(byte)1] = remotePlayer.myID;
                    table[(byte)2] = myID;
                    gameManager.gameClient.SendEvent(14, table, true, false);
                }
            }
        }

        gameManager.gameGUI.ShowTakeAnAction();
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
        CardVisual selectedCard = gameManager.gameGUI.selectedCard;
        if (selectedCard != null)
        {
            if (coins >= selectedCard.card.cost)
            {
                districtstoBuild--;
                AdjustCoins(-selectedCard.card.cost);
                Card c = hand.RemoveCardWithID(selectedCard.card.id);
                gameManager.gameGUI.RemoveCard(c);
                builtDistricts.AddCard(c);
                gameManager.gameGUI.RecalculateCardPositions();
                gameManager.gameGUI.AddDistrict(c, builtDistricts.collection.Count - 1);

                if(districtstoBuild == 0)
                    gameManager.gameGUI.turnButtons[0].interactable = false;

                Hashtable table = new Hashtable();
                table[(byte)1] = myID;
                table[(byte)2] = c.id;
                gameManager.gameClient.SendEvent(11, table, true, false);
            }
        }
    }

    public void UseCharacterAbility()
    {
        switch (character)
        {
            case CharacterCard.Magician:
                gameManager.gameGUI.ShowMagicianSelection();
                //TODO: change "new hand" to specific amount of cards
                break;
            case CharacterCard.King:
                AddCoinsForCardColors(CardColor.Gold);
                gameManager.gameGUI.turnButtons[1].interactable = false;
                break;
            case CharacterCard.Bishop:
                AddCoinsForCardColors(CardColor.Blue);
                gameManager.gameGUI.turnButtons[1].interactable = false;
                break;
            case CharacterCard.Merchant:
                AddCoinsForCardColors(CardColor.Green);
                gameManager.gameGUI.turnButtons[1].interactable = false;
                break;
            case CharacterCard.Warlord:
                gameManager.gameGUI.ShowWarlordSelection();
                
                break;
        }
    }

    public void TakeNewHand(int player)
    {
        Hashtable table = new Hashtable();
        if (player == -1)
        {
            
            int cards = hand.collection.Count;
            for (int i = 0; i < cards; i++)
            {
                Card c = hand.RemoveCardAt(0);
                gameManager.discard.AddCard(c);
                gameManager.gameGUI.RemoveCard(c);
                
                table[(byte)1] = myID;
                gameManager.gameClient.SendEvent(16, table, true, false);
                
            }
            for (int i = 0; i < cards; i++)
            {
                Card c = gameManager.deck.RemoveTopCard();
                hand.AddCard(c);
            }
        }
        else
        {
            SwitchHandsWith(player);
            table[(byte)1] = myID;
            table[(byte)2] = player;
            gameManager.gameClient.SendEvent(17, table, true, false);
        }
    }

    public void SwitchHandsWith(int other)
    {
        Character playerToTakeFrom = gameManager.GetRemotePlayer(other);
        List<Card> myhand = hand.collection;
        hand.collection = playerToTakeFrom.hand.collection;
        playerToTakeFrom.hand.collection = myhand;
        gameManager.gameGUI.UpdateHandVisuals();
    }

    public void AddMerchantCoin()
    {
        if (character == CharacterCard.Merchant)
            AdjustCoins(1);
    }

    public void AddArchitectCards()
    {
        if (character == CharacterCard.Architect)
        {
            for (int i = 0; i < 2; i++)
            {
                Card c = gameManager.deck.RemoveTopCard();
                gameManager.myPlayer.PlayerHand.AddCard(c);
            }
        }
    }

    public void AddWarlordCoins()
    {
        AddCoinsForCardColors(CardColor.Red);
        gameManager.gameGUI.warlordSelection.SetActive(false);
    }

    public void AddCoinsForCardColors(CardColor color)
    {
        int coins = 0;
        foreach (Card c in builtDistricts.collection)
        {
            if (c.color == color)
            {
                coins++;
            }
        }
        AdjustCoins(coins);
    }

    public void DestroyOthersDistrict(Character player, Card card)
    {
        AdjustCoins(-(card.cost - 1));
        gameManager.discard.AddCard(gameManager.GetRemotePlayer(player.myID).builtDistricts.RemoveCardWithID(card.id));
        Hashtable table = new Hashtable();
        table[(byte)1] = player.myID;
        table[(byte)2] = card.id;
        gameManager.gameClient.SendEvent(19, table, true, false);
    }
    public void DestroyMyDistrict(int cardID)
    {
        Card card = builtDistricts.RemoveCardWithID(cardID);
        if (isLocal)
        {
            gameManager.gameGUI.RemoveDistrict(card);
        }
        gameManager.discard.AddCard(card);
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
