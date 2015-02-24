using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Hand : CardCollection 
{
    Character myOwner;

    void Start()
    {
        myOwner = GetComponent<Character>();
    }

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        if (myOwner.isLocal)
        {
            GameManager.instance.gameGUI.AddCard(card);
            Hashtable table = new Hashtable();
            table[(byte)1] = myOwner.myID;
            table[(byte)2] = card.id;
            GameManager.instance.gameClient.SendEvent(8, table, true, false);
        }
    }
}
