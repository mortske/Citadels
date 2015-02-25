using System.Collections;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CitadelsGameClient : LoadBalancingClient
{
    public CitadelsGUI citadelsGUI;
    public GameManager gameManager;

    private bool gameStarted = false;

    public bool CanStartGame
    {
        get { return CurrentRoom.Players.Count == CurrentRoom.MaxPlayers; }
    }

    public bool GameStarted
    {
        get { return gameStarted; }
        set { gameStarted = value; }
    }

    public void SendEvent(byte evCode, Hashtable evData, bool sendSafe, bool runOnSelf)
    {
        if (runOnSelf)
            RunEventCode(evCode, evData);
        this.loadBalancingPeer.OpRaiseEvent(evCode, evData, sendSafe, RaiseEventOptions.Default);

    }
    public void SendEvent(byte evCode, Hashtable evData, bool sendSafe, RaiseEventOptions options, bool runOnSelf)
    {
        if (runOnSelf)
            RunEventCode(evCode, evData);
        this.loadBalancingPeer.OpRaiseEvent(evCode, evData, sendSafe, options);
        
    }

    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);

        switch (photonEvent.Code)
        {    
            case (byte)1:
                RunEventCode(photonEvent.Code, null);
                break;
            case (byte)2:
                Hashtable content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)3:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)4:
                RunEventCode(photonEvent.Code, null);
                break;
            case (byte)5:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)6:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)7:
                RunEventCode(photonEvent.Code, null);
                break;
            case (byte)8:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)9:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)10:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)11:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            //    case (byte)0:
            //      Hashtable content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
            //      RunEventCode(photonEvent.Code, content);
            //      break;
        }
    }

    public void RunEventCode(byte code, Hashtable data)
    {
        Character player;
        switch (code)
        {
            case (byte)1: //start game
                gameManager.StartGame();
                break;
            case (byte)2: //set deck order
                gameManager.deck.SetOrderWithName(((int[])data[(byte)1]));
                break;
            case (byte)3: //destroy card in deck
                gameManager.deck.DestroyCardWithID((int)data[(byte)1]);
                break;
            case (byte)4: //add startercards to hand
                for (int i = 0; i < 4; i++)
                {
                    Card c = gameManager.deck.RemoveTopCard();
                    gameManager.myPlayer.PlayerHand.AddCard(c);
                }
                break;
            case (byte)5: //change turn
                gameManager.SetTurn((int)data[(byte)1]);
                break;
            case (byte)6: //set character list
                gameManager.SetCharactersInGame((int[])data[(byte)1]);
                break;
            case (byte)7: //set to playerturns GameMode
                gameManager.curGameState = GameState.PlayerTurns;
                break;
            case (byte)8: //add card to remote players hand
                player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                player.PlayerHand.AddCard(gameManager.deck.RemoveCardWithID((int)data[(byte)2]));
                break;
            case (byte)9: //set coins to remote player
                player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                player.coins = (int)data[(byte)2];
                break;
            case (byte)10: //set character to remote player
                player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                player.character = (CharacterCard)((int)data[(byte)2]);
                break;
            case (byte)11: //add card from hand to district
                player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                player.BuiltDistricts.AddCard(player.PlayerHand.RemoveCardWithID((int)data[(byte)2]));
                break;
        }
    }
}
