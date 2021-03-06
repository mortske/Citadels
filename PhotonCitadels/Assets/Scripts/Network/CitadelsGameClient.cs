﻿using System.Collections;
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
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
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
            case (byte)12:
                RunEventCode(photonEvent.Code, null);
                break;
            case (byte)13:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)14:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)15:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)16:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)17:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)18:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
            case (byte)19:
                content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                RunEventCode(photonEvent.Code, content);
                break;
        }
    }

    public void RunEventCode(byte code, Hashtable data)
    {
        Character player;
        Card c;
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
                    c = gameManager.deck.RemoveTopCard();
                    gameManager.myPlayer.PlayerHand.AddCard(c);
                }
                break;
            case (byte)5: //change turn
                gameManager.SetTurn((int)data[(byte)1]);
                break;
            case (byte)6: //set character list
                gameManager.SetCharactersInGame((int[])data[(byte)1], (int[])data[(byte)2]);
                break;
            case (byte)7: //set GameMode
                gameManager.curGameState = (GameState)((int)data[(byte)1]);
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
            case (byte)12: //start new round
                gameManager.StartNewRound(gameManager.KingID);
                break;
            case (byte)13: //murder character
                if ((int)data[(byte)1] == gameManager.myPlayer.myID)
                    gameManager.myPlayer.Murder();
                else
                    gameManager.GetRemotePlayer((int)data[(byte)1]).murdered = true;
                break;
            case (byte)14: //steal from character
                if ((int)data[(byte)1] == gameManager.myPlayer.myID)
                    gameManager.myPlayer.StealFrom((int)data[(byte)2]);
                break;
            case (byte)15: //send money to player
                if ((int)data[(byte)1] == gameManager.myPlayer.myID)
                    gameManager.myPlayer.AdjustCoins((int)data[(byte)2]);
                break;
            case (byte)16: //add card from remote player to discard
                player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                c = player.PlayerHand.RemoveCardAt(0);
                GameManager.instance.discard.AddCard(c);
                break;
            case (byte)17: //switch hands with players
                int first = (int)data[(byte)1];
                int other = (int)data[(byte)2];
                if (other == gameManager.myPlayer.myID)
                    gameManager.myPlayer.SwitchHandsWith(first);
                else
                    gameManager.GetRemotePlayer(other).SwitchHandsWith(first);
                break;
            case (byte)18: //set king id
                gameManager.SetKingID((int)data[(byte)1]);
                break;
            case (byte)19: //move card from district to discard
                if ((int)data[(byte)1] == gameManager.myPlayer.myID)
                    player = gameManager.myPlayer;
                else
                    player = gameManager.GetRemotePlayer((int)data[(byte)1]);
                player.DestroyMyDistrict((int)data[(byte)2]);
                break;
        }
    }
}
