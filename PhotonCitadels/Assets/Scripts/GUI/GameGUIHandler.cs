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
    public GameObject turnSelection;
    public GameObject mageSelection;
    public GameObject warlordSelection;
    public GameObject playerSelection;
    public Image image_Cross;
    public Text text_turn;
    public Text text_coinAmnt;
    public Text text_curChar;
    public Text text_characterSelection;
    public Button[] characterButtons;
    public Button[] playerButtons;
    public Text[] playerButtons_text;
    public Button[] cardsSelections;
    public Button[] turnButtons;
    public GameObject cardPrefab;
    public CardVisual[] districtCards;
    public DistrictsSelection districtSelection;

    List<CardVisual> allCards;
    public CardVisual selectedCard { get; private set; }
    
    void Start()
    {
        allCards = new List<CardVisual>();
    }

    public void Reset()
    {
        turnButtons[0].interactable = true;
        turnButtons[1].interactable = true;
        Button[] warlordButtons = warlordSelection.GetComponentsInChildren<Button>();
        foreach (Button b in warlordButtons)
        {
            b.interactable = true;
        }
    }

    void Update()
    {
        if (gameManager.gameClient.GameStarted)
        {
            if (!GameUI.enabled)
                GameUI.enabled = true;
            if (!PlayerUI.enabled)
                PlayerUI.enabled = true;

            text_coinAmnt.text = gameManager.myPlayer.coins.ToString();
            text_curChar.text = "Character: " + gameManager.myPlayer.character;
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

    public void UpdateHandVisuals()
    {
        foreach (CardVisual visual in allCards)
        {
            Destroy(visual.gameObject);
        }
        allCards = new List<CardVisual>();
        foreach (Card card in gameManager.myPlayer.PlayerHand.collection)
        {
            AddCard(card);
        }
    }

    public void RemoveCard(Card card)
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            if (card.id == allCards[i].card.id)
            {
                Destroy(allCards[i].gameObject);
                allCards.RemoveAt(i);
            }
        }
    }

    public void AddDistrict(Card card, int pos)
    {
        districtCards[pos].card = card;
        districtCards[pos].text_Name.text = card.name;
        districtCards[pos].text_Cost.text = card.cost.ToString();
        districtCards[pos].image_Color.color = card.GetColor;
    }

    public void RemoveDistrict(Card card)
    {
        for (int i = 0; i < districtCards.Length; i++)
        {
            if (districtCards[i].card.id == card.id)
            {
                districtCards[i].card = null;
                districtCards[i].text_Name.text = "";
                districtCards[i].text_Cost.text = "";
                districtCards[i].image_Color.color = Color.white;
                break;
            }
        }
        RecalculateDistrictPositions();
    }

    void ResetDistricts()
    {
        for (int i = 0; i < districtCards.Length; i++)
        {
            districtCards[i].card = null;
            districtCards[i].text_Name.text = "";
            districtCards[i].text_Cost.text = "";
            districtCards[i].image_Color.color = Color.white;
        }
    }

    void RecalculateDistrictPositions()
    {
        ResetDistricts();
        int districtAmnt = gameManager.myPlayer.BuiltDistricts.collection.Count;
        for (int i = 0; i < districtAmnt; i++)
        {
            AddDistrict(gameManager.myPlayer.BuiltDistricts.collection[i], i);
        }
    }

    public void SetKingText()
    {
        if (gameManager.gameClient.LocalPlayer.ID == gameManager.KingID)
        {
            image_Cross.enabled = false;
        }
        else
        {
             image_Cross.enabled = true;
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
            text_turn.text = "It's " + gameManager.GetRemotePlayer(gameManager.turnID).name + "'s turn!";
        }
    }

    public void ShowCharacterSelection(int[] characters, string titleText)
    {
        characterSelectionUI.SetActive(true);
        text_characterSelection.text = titleText;

        for (int i = 0; i < characterButtons.Length; i++)
            characterButtons[i].interactable = false;

        for (int i = 0; i < characters.Length; i++)
            characterButtons[characters[i]].interactable = true;
    }
    public void SetCharacterSelection(int character)
    {
        if (gameManager.curGameState == GameState.CharacterSelection)
        {
            gameManager.myPlayer.SetCharacter(character);
            characterSelectionUI.SetActive(false);
            gameManager.RemoveCharacterFromSelection(character);

            Hashtable table = new Hashtable();
            table[(byte)1] = gameManager.charsInGame.ToArray();
            table[(byte)2] = gameManager.removedChars.ToArray();
            gameManager.gameClient.SendEvent(6, table, true, false);

            gameManager.SendOverTurn();
        }
        else
        {
            gameManager.myPlayer.SelectedVictim(character);
            characterSelectionUI.SetActive(false);
        }
    }

    public void ShowPlayerSelection()
    {
        playerSelection.SetActive(true);
        for (int i = 0; i < playerButtons.Length; i++)
        {
            playerButtons[i].interactable = false;
            if (i == gameManager.myPlayer.myID - 1)
                continue;
            if (i >= gameManager.gameClient.CurrentRoom.PlayerCount)
                break;
            int cardlength = gameManager.GetRemotePlayer(i + 1).PlayerHand.collection.Count;
            playerButtons_text[i].text = "Player" + (i + 1) + " | " + cardlength + " Cards";
            playerButtons[i].interactable = true;
        }
    }

    public void HandlePlayerSelection(int character)
    {
        if (gameManager.myPlayer.character == CharacterCard.Magician)
        {
            gameManager.myPlayer.TakeNewHand(character);
            mageSelection.SetActive(false);
            turnButtons[1].interactable = false;
        }
        playerSelection.SetActive(false);
    }

    public void ShowTakeAnAction()
    {
        ActionSelection.SetActive(true);
    }

    public void HideTakeAnAction()
    {
        ActionSelection.SetActive(false);
        ShowTurnButtons();
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
        Card c = gameManager.deck.RemoveCardAt(i);
        gameManager.myPlayer.PlayerHand.AddCard(c);
        cardSelection.SetActive(false);
    }

    public void ShowTurnButtons()
    {
        turnSelection.SetActive(true);
    }

    public void SetSelectedCard(CardVisual c)
    {
        if (selectedCard != null)
        {
            selectedCard.bg.color = selectedCard.baseColor;
        }
        selectedCard = c;
        selectedCard.bg.color = Color.white;
    }

    public void ShowMagicianSelection()
    {
        mageSelection.SetActive(true);
    }

    public void ShowWarlordSelection()
    {
        warlordSelection.SetActive(true);
    }

    public void ShowDistrictsSelection()
    {
        districtSelection.gameObject.SetActive(true);
        districtSelection.Reset();
        districtSelection.SetSelectableButtons();
    }
}
