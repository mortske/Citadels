﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameGUIHandler : MonoBehaviour 
{
    public GameManager gameManager;

    public Canvas GameUI;
    public Canvas PlayerUI;
    public GameObject CardsUI;
    public GameObject characterSelectionUI;
    public GameObject ActionSelection;
    public GameObject cardSelection;
    public Text text_deckAmnt;
    public Text text_king;
    public Text text_turn;
    public Text text_coinAmnt;
    public Text text_curChar;
    public Button[] characterButtons;
    public Button[] cardsSelections;
    public GameObject cardPrefab;

    List<CardVisual> allCards;

    void Start()
    {
        allCards = new List<CardVisual>();
    }

    void Update()
    {
        if (gameManager.gameClient.GameStarted)
        {
            if (!GameUI.enabled)
                GameUI.enabled = true;
            if (!PlayerUI.enabled)
                PlayerUI.enabled = true;

            text_deckAmnt.text = "Cards in deck: " + gameManager.deck.Collection.Count;
            text_coinAmnt.text = "Coins: " + gameManager.myPlayer.coins;
            text_curChar.text = "Current Character: " + gameManager.myPlayer.character;
        }
        else
        {
            if (GameUI.enabled)
                GameUI.enabled = false;
            if (PlayerUI.enabled)
                PlayerUI.enabled = false;
        }
    }

    public void AddCard(Card card)
    {
        GameObject g = (GameObject)Instantiate(cardPrefab);
        RectTransform cardtransform = g.GetComponent<RectTransform>();
        g.transform.SetParent(CardsUI.transform);
        RectTransform rtransform = CardsUI.GetComponentsInChildren<RectTransform>()[1];
        g.transform.localPosition = new Vector3(rtransform.localPosition.x -(allCards.Count * cardtransform.rect.width), rtransform.localPosition.y);
        
        CardVisual visual = g.GetComponent<CardVisual>();
        visual.card = card;
        visual.text_Name.text = card.name;
        visual.text_Cost.text = card.cost.ToString();
        visual.image_Color.color = card.GetColor;
        allCards.Add(visual);
        RecalculateCardPositions();
    }

    public void RecalculateCardPositions()
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            RectTransform cardtransform = allCards[i].GetComponent<RectTransform>();
            RectTransform rtransform = CardsUI.GetComponentsInChildren<RectTransform>()[1];
            allCards[i].transform.localPosition = new Vector3(rtransform.localPosition.x - (i * (cardtransform.rect.width)), rtransform.localPosition.y);
        }
    }

    public void RemoveCard(Card card)
    {

    }

    public void SetKingText()
    {
        if (gameManager.gameClient.LocalPlayer.ID == gameManager.KingID)
        {
            text_king.text = "You are the king!";
        }
        else
        {
            text_king.text = "You are not the king..";
        }
    }
    public void SetTurnText()
    {
        if (gameManager.IsMyTurn)
        {
            text_turn.text = "It's your turn!";
        }
        else
        {
            text_turn.text = "It's not your turn yet..";
        }
    }

    public void ShowCharacterSelection(int[] characters)
    {
        characterSelectionUI.SetActive(true);

        for (int i = 0; i < characterButtons.Length; i++)
            characterButtons[i].interactable = false;

        for (int i = 0; i < characters.Length; i++)
            characterButtons[characters[i]].interactable = true;
    }
    public void SetCharacterSelection(int character)
    {
        gameManager.myPlayer.SetCharacter(character);
        characterSelectionUI.SetActive(false);
        gameManager.RemoveCharacterFromSelection(character);

        Hashtable table = new Hashtable();
        table[(byte)1] = gameManager.charsInGame.ToArray();
        gameManager.gameClient.SendEvent(6, table, true, false);

        gameManager.SendOverTurn();
    }

    public void ShowTakeAnAction()
    {
        ActionSelection.SetActive(true);
    }

    public void HideTakeAnAction()
    {
        ActionSelection.SetActive(false);
        //gameManager.SendTurnToNextCharacter();
    }

    public void ShowCardSelection()
    {
        HideTakeAnAction();
        cardSelection.SetActive(true);
        for (int i = 0; i < cardsSelections.Length; i++)
        {
            Deck deck = gameManager.deck;
            Card card = deck.GetCardAt(i);
            CardVisual visual = cardsSelections[i].GetComponent<CardVisual>();
            
            visual.card = card;
            visual.text_Name.text = card.name;
            visual.text_Cost.text = card.cost.ToString();
            visual.image_Color.color = card.GetColor;
            allCards.Add(visual);
        }
    }

    public void SelectCard(int i)
    {
        Card c = gameManager.deck.RemoveTopCard();
        gameManager.myPlayer.PlayerHand.AddCard(c);
        cardSelection.SetActive(false);
    }
}