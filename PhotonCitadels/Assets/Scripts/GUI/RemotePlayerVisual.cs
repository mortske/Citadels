using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RemotePlayerVisual : MonoBehaviour
{
    Character curCharacter;
    GameManager gameManager;

    public GameObject box;
    public Text text_playerName;
    public Image image_KingCross;
    public Image image_TurnYes;
    public Image image_TurnNo;
    public Text text_Character;
    public Text text_Coins;
    public Text text_Cards;
    public CardVisual[] districts;

    void Start()
    {
        gameManager = GameManager.instance;
    }

    public void SetUpRemotePlayer(int playerID)
    {
        box.SetActive(true);
        Reset();
        curCharacter = GameManager.instance.GetRemotePlayer(playerID);
        if (curCharacter == null)
            return;

        text_playerName.text = curCharacter.name;

        if(gameManager.KingID != curCharacter.myID)
            image_KingCross.enabled = true;

        if(gameManager.turnID == curCharacter.myID)
            image_TurnYes.enabled = true;
        else
            image_TurnNo.enabled = true;

        if (curCharacter.hasStartedTurn)
            text_Character.text = curCharacter.character.ToString();
        else
            text_Character.text = "?";

        text_Coins.text = curCharacter.coins.ToString();
        text_Cards.text = curCharacter.PlayerHand.collection.Count.ToString();

        for (int i = 0; i < curCharacter.BuiltDistricts.collection.Count; i++)
        {
            Card c = curCharacter.BuiltDistricts.GetCardAt(i);
            districts[i].card = c;
            districts[i].text_Name.text = c.name;
            districts[i].text_Cost.text = c.cost.ToString();
            Color color = c.GetColor;
            districts[i].image_Color.color = new Color(color.r, color.g, color.b, color.a);
        }
    }

    public void Reset()
    {
        text_playerName.text = "";
        image_KingCross.enabled = false;
        image_TurnYes.enabled = false;
        image_TurnNo.enabled = false;
        text_Character.text = "?";
        text_Coins.text = "0";
        text_Cards.text = "0";

        foreach (CardVisual visual in districts)
        {
            visual.card = null;
            visual.text_Name.text = "";
            visual.text_Cost.text = "";
            visual.image_Color.color = new Color(0, 0, 0, 0);
        }
    }

    public void Hide()
    {
        box.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        //TODO: Add visual for checking other players, remove these buttons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetUpRemotePlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetUpRemotePlayer(2);
        }
    }
}
