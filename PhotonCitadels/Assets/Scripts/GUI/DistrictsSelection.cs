using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistrictsSelection : MonoBehaviour 
{
    public Button[] playerButtons;
    public CardVisual[] districtCards;
    public CardVisual mainCard;
    public Button activation;
    public Text coinsText;

    Character curRemotePlayer;
    Card selectedCard;

    public void Reset()
    {
        selectedCard = null;
        curRemotePlayer = null;

        for (int i = 0; i < districtCards.Length; i++)
        {
            districtCards[i].text_Name.text = "";
            districtCards[i].text_Cost.text = "";
            districtCards[i].image_Color.color = Color.white;
            districtCards[i].card = null;
        }
        mainCard.text_Name.text = "";
        mainCard.text_Cost.text = "";
        mainCard.image_Color.color = Color.white;
        mainCard.card = null;
        coinsText.text = "Coins: " + GameManager.instance.myPlayer.coins;
    }

    public void SetSelectableButtons()
    {
        for (int i = 0; i < playerButtons.Length; i++)
        {
            playerButtons[i].interactable = false;
            if (i >= GameManager.instance.gameClient.CurrentRoom.PlayerCount)
                break;
            int cardlength = GameManager.instance.GetRemotePlayer(i + 1).PlayerHand.collection.Count;
            playerButtons[i].interactable = true;
        }
    }

    public void SetDistrictsTo(int player)
    {
        curRemotePlayer = GameManager.instance.GetRemotePlayer(player);
        for (int i = 0; i < curRemotePlayer.BuiltDistricts.collection.Count; i++)
        {
            Card card = curRemotePlayer.BuiltDistricts.GetCardAt(i);
            CardVisual visual = districtCards[i];
            visual.card = card;
            visual.text_Name.text = card.name;
            visual.text_Cost.text = card.cost.ToString();
            visual.image_Color.color = card.GetColor;
        }
    }

    public void SelectCard(int card)
    {
        if (curRemotePlayer != null)
        {
            if (card < curRemotePlayer.BuiltDistricts.collection.Count)
            {
                Card c = curRemotePlayer.BuiltDistricts.GetCardAt(card);
                mainCard.card = c;
                mainCard.text_Name.text = c.name;
                mainCard.text_Cost.text = c.cost.ToString();
                mainCard.image_Color.color = c.GetColor;
                selectedCard = c;
            }
        }
    }

    public void PickCard()
    {
        if (curRemotePlayer != null && selectedCard != null)
        {
            if (curRemotePlayer.BuiltDistricts.collection.Count < 8)
            {
                if (GameManager.instance.myPlayer.coins >= selectedCard.cost - 1)
                {
                    GameManager.instance.myPlayer.DestroyOthersDistrict(curRemotePlayer, selectedCard);
                    activation.interactable = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
