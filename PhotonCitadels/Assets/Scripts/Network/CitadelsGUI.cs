using System;
using System.Collections.Generic;
using System.Threading;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CitadelsGUI : MonoBehaviour 
{
    public string AppId;
    public CitadelsGameClient gameClient { get; set; }
    GameManager gameManager;
    public InputField nameField;

    int maxPlayers = 1;
    string roomName = "";

    public void Awake()
    {
        if (string.IsNullOrEmpty(this.AppId))
        {
            Debug.LogError("AppId not set!");
            Debug.Break();
        }

        Application.runInBackground = true;
        CustomTypes.Register();
        gameManager = GetComponent<GameManager>();

        this.gameClient = new CitadelsGameClient();
        this.gameClient.MasterServerAddress = "app-eu.exitgamescloud.com:5055";
        this.gameClient.AppId = this.AppId;   // edited in Inspector!
        this.gameClient.AppVersion = "1.0";
        this.gameClient.PlayerName = "unityPlayer";
        this.gameClient.citadelsGUI = this;
        this.gameClient.gameManager = gameManager;
        this.gameClient.Connect();

        nameField.text = PlayerPrefs.GetString("PlayerName", "");
    }

    void Update()
    {
        this.gameClient.Service();
    }

    void OnApplicationQuit()
    {
        this.gameClient.Disconnect();     // let's try to do a regular disconnect on app quit

        LoadBalancingPeer lbPeer = this.gameClient.loadBalancingPeer;
        lbPeer.StopThread();   // for the Editor it's better stop any connection immediately
    }

    public void SetPlayerName(Text playerText)
    {
        gameClient.LocalPlayer.Name = playerText.text;
        PlayerPrefs.SetString("PlayerName", playerText.text);
    }

    void OnGUI()
    {
        if (!gameClient.GameStarted)
            GUILayout.Label("State: " + gameClient.State.ToString());

        switch (gameClient.State)
        {
            case ClientState.JoinedLobby:
                this.OnGUILobby();
                break;
            case ClientState.Joined:
                this.OnGUIJoined();
                break;
        }
    }

    private void OnGUILobby()
    {
        if (gameClient.LocalPlayer.Name != "unityPlayer")
        {
            GUILayout.Label("Lobby Screen");
            GUILayout.Label(string.Format("Players in rooms: {0} looking for rooms: {1}  rooms: {2}", this.gameClient.PlayersInRoomsCount, this.gameClient.PlayersOnMasterCount, this.gameClient.RoomsCount));

            #region CreateRoomGUI
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
            {
                maxPlayers--;
                if (maxPlayers < 2)
                    maxPlayers = 2;
            }
            GUILayout.Label(maxPlayers.ToString(), GUILayout.Width(10));
            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
            {
                maxPlayers++;
                if (maxPlayers > 6)
                    maxPlayers = 6;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Room name: ", GUILayout.Width(80));
            roomName = GUILayout.TextArea(roomName);
            GUILayout.EndHorizontal();
            #endregion

            if (GUILayout.Button("Create", GUILayout.Width(150)))
            {
                this.gameClient.OpCreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)this.maxPlayers }, TypedLobby.Default);
                Debug.Log("creating room");
            }

            GUILayout.Label("Rooms to choose from: " + this.gameClient.RoomInfoList.Count);
            foreach (RoomInfo roomInfo in this.gameClient.RoomInfoList.Values)
            {
                string playerCount = "*Full*";
                if (roomInfo.PlayerCount < roomInfo.MaxPlayers)
                    playerCount = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
                string roomname = roomInfo.Name + " " + playerCount;
                if (GUILayout.Button(roomname))
                {
                    this.gameClient.OpJoinRoom(roomInfo.Name);
                }
            }
        }
    }

    private void OnGUIJoined()
    {
        if (!gameClient.GameStarted)
        {
            Room currentRoom = this.gameClient.CurrentRoom;
            GUILayout.Label("Players: " + currentRoom.Players.Count + "/" + currentRoom.MaxPlayers);

            if (gameClient.CanStartGame && gameClient.LocalPlayer.IsMasterClient)
            {
                if (GUILayout.Button("Start Game", GUILayout.Width(150)))
                {
                    StartGame();
                }
            }
            else
                GUILayout.Space(30);


            if (GUILayout.Button("Leave", GUILayout.Width(150)))
                this.gameClient.OpLeaveRoom();
        }
    }

    void StartGame()
    {
        //Start game
        this.gameClient.SendEvent(1, null, true, RaiseEventOptions.Default, true);

        //set deckorder
        Hashtable table = new Hashtable();
        table[(byte)1] = gameManager.deck.GetIDArray();
        this.gameClient.SendEvent(2, table, true, RaiseEventOptions.Default, false);

        //request all players to add cards to their hands
        this.gameClient.SendEvent(4, null, true, RaiseEventOptions.Default, true);

        gameManager.SetUpCharactersInGame(3);
        gameManager.gameGUI.ShowCharacterSelection(gameManager.charsInGame.ToArray(), "Pick a character");
    }
}