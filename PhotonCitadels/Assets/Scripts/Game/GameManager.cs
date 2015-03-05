using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;
    CitadelsGUI citadelsGUI;
    public CitadelsGameClient gameClient { get; private set; }
    public GameGUIHandler gameGUI;
    public GameObject PlayerPrefab;
    public Character[] remotePlayers;
    public PlayerScore playerScore;

    public Deck deck { get; set; }
    public Deck discard { get; set; }
    public Character myPlayer;

    public int KingID;
    public int turnID;
    public GameState curGameState;
    public List<int> charsInGame;
    public List<int> removedChars;
    

    public bool IsMyTurn
    {
        get { return turnID == gameClient.LocalPlayer.ID; }
    }

    public bool IAmKing
    {
        get { return KingID == gameClient.LocalPlayer.ID; }
    }

    public Character NextIDPlayer
    {
        get{
            int myID = myPlayer.myID;
            if (myID == gameClient.CurrentRoom.PlayerCount)
                return GetRemotePlayer(1);
            return GetRemotePlayer(myID + 1);
        }
    }
    public Character PrevIDPlayer
    {
        get
        {
            int myID = myPlayer.myID;
            if (myID == 1)
                return GetRemotePlayer(gameClient.CurrentRoom.PlayerCount);
            return GetRemotePlayer(myID - 1);
        }
    }

    public bool SomeOneHasWon
    {
        get
        {
            if (myPlayer.BuiltDistricts.collection.Count > 7)
            {
                return true;
            }
            for (int i = 0; i < remotePlayers.Length; i++)
            {
                if (remotePlayers[i].BuiltDistricts.collection.Count > 7)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public Character GetRemotePlayer(int id)
    {
        for (int i = 0; i < remotePlayers.Length; i++)
        {
            if (id == remotePlayers[i].myID)
                return remotePlayers[i];
        }
        Debug.LogError("Could not find a remote player with id: " + id);
        return null;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        citadelsGUI = GetComponent<CitadelsGUI>();
        gameClient = citadelsGUI.gameClient;

        deck = GetComponents<Deck>()[0];
        discard = GetComponents<Deck>()[1];
        myPlayer.Initialize();
        KingID = 1;
        turnID = KingID;
        curGameState = GameState.CharacterSelection;
    }

    public void StartGame()
    {
        Debug.Log("Started Game");
        SetupRemotePlayers();
        gameClient.GameStarted = true;

        for (int i = 0; i < deck.Collection.Count; i++)
        {
            deck.Collection[i].id = i;
        }
        deck.Shuffle<Card>(deck.Collection);

        gameGUI.SetKingText();
        gameGUI.SetTurnText();

        myPlayer.myID = gameClient.LocalPlayer.ID;
        myPlayer.name = gameClient.LocalPlayer.Name;
    }

    void SetupRemotePlayers()
    {
        remotePlayers = new Character[gameClient.CurrentRoom.PlayerCount - 1];

        int i = 0;
        foreach (KeyValuePair<int, Player> p in gameClient.CurrentRoom.Players)
        {
            if (p.Key != gameClient.LocalPlayer.ID)
            {
                GameObject newPlayer = (GameObject)Instantiate(PlayerPrefab);
                newPlayer.name = p.Value.Name;
                remotePlayers[i] = newPlayer.GetComponent<Character>();
                remotePlayers[i].Initialize();
                remotePlayers[i].isLocal = false;
                remotePlayers[i].myID = p.Key;
                remotePlayers[i].name = newPlayer.name;
                i++;
            }
        }
    }

    public void SetKingID(int kingID)
    {
        this.KingID = kingID;
        gameGUI.SetKingText();
    }

    public void SendOverTurn()
    {
        if (turnID == gameClient.CurrentRoom.PlayerCount)
            turnID = 1;
        else
            turnID++;
        
        gameGUI.SetTurnText();

        Hashtable table = new Hashtable();
        table[(byte)1] = turnID;
        this.gameClient.SendEvent(5, table, true, RaiseEventOptions.Default, false);
    }
    public void SetTurn(int id)
    {
        if (curGameState == GameState.PlayerTurns && id != myPlayer.myID)
            GetRemotePlayer(id).hasStartedTurn = true;

        turnID = id;
        gameGUI.SetTurnText();

        if (IsMyTurn)
        {
            if (curGameState == GameState.PlayerTurns)
            {
                myPlayer.hasStartedTurn = true;
                myPlayer.TakePlayerTurn();
            }
            else if (IAmKing && curGameState == GameState.CharacterSelection)
            {
                SetGameState(GameState.PlayerTurns);

                int firstPlayerID = GetFirstPlayer();
                SetTurn(firstPlayerID);
                Hashtable table = new Hashtable();
                table[(byte)1] = turnID;
                this.gameClient.SendEvent(5, table, true, RaiseEventOptions.Default, false);
            }
            else
            {
                gameGUI.ShowCharacterSelection(charsInGame.ToArray(), "Pick a character");
            }
        }
    }

    public void SendTurnToNextCharacter()
    {
        myPlayer.FinishTurn();
        myPlayer.hasTakenTurn = true;
        int nextPlayerID = GetNextPlayer();
        if (nextPlayerID != 0)
        {
            SetTurn(nextPlayerID);
            Hashtable table = new Hashtable();
            table[(byte)1] = turnID;
            this.gameClient.SendEvent(5, table, true, RaiseEventOptions.Default, false);
        }
        else
        {
            gameClient.SendEvent(12, null, true, true);
        }
    }

    public void StartNewRound(int kingID)
    {
        if (!SomeOneHasWon)
        {
            turnID = kingID;
            gameGUI.SetTurnText();
            curGameState = GameState.CharacterSelection;

            if (IsMyTurn)
            {
                SetUpCharactersInGame(3);
                gameGUI.ShowCharacterSelection(charsInGame.ToArray(), "Pick a character");
            }
            myPlayer.Reset();
            gameGUI.Reset();
        }
        else
        {
            CalculatePlayersScore();
            Application.LoadLevel(Application.loadedLevel + 1);
        }
    }

    public void SetGameState(GameState state)
    {
        curGameState = state;
        Hashtable table = new Hashtable();
        table[(byte)1] = (int)GameState.PlayerTurns;
        this.gameClient.SendEvent(7, table, true, false);
    }

    public int GetFirstPlayer()
    {
        int lowestcharacterID = myPlayer.myID;
        int lowestid = (int)myPlayer.character;
        for (int i = 0; i < remotePlayers.Length; i++)
        {
            if ((int)remotePlayers[i].character < lowestid)
            {
                lowestid = (int)remotePlayers[i].character;
                lowestcharacterID = (int)remotePlayers[i].myID;
            }
        }
        return lowestcharacterID;
    }

    public int GetNextPlayer()
    {
        int nextPlayerID = 0;
        int myCharsID = (int)myPlayer.character;
        int nextPlayerCharID = 10;
        for (int i = 0; i < remotePlayers.Length; i++)
        {
            if ((int)remotePlayers[i].character > myCharsID && (int)remotePlayers[i].character < nextPlayerCharID)
            {
                nextPlayerCharID = (int)remotePlayers[i].character;
                nextPlayerID = remotePlayers[i].myID;
            }
        }
        return nextPlayerID;
    }

    public void SetUpCharactersInGame(int amountToRemove)
    {
        charsInGame = new List<int>();
        removedChars = new List<int>();

        foreach (CharacterCard item in Enum.GetValues(typeof(CharacterCard)))
        {
            if(item != CharacterCard.Nothing)
                charsInGame.Add((int)item);
        }
        for (int i = 0; i < amountToRemove; i++)
        {
            int charPos = UnityEngine.Random.Range(0, charsInGame.Count - 1);
            removedChars.Add(charPos);
            charsInGame.RemoveAt(charPos);
        }
        removedChars.RemoveAt(0);
    }
    public void RemoveCharacterFromSelection(int character)
    {
        charsInGame.Remove(character);
    }

    public void SetCharactersInGame(int[] characters, int[] removedCharacters)
    {
        charsInGame = characters.ToList();
        removedChars = removedCharacters.ToList();
    }

    public void CalculatePlayersScore()
    {
        List<Character> players = new List<Character>();
        List<int> scores = new List<int>();

        for (int i = -1; i < remotePlayers.Length; i++)
        {
            int score = 0;
            Character player = myPlayer;
            if (i != -1)
                player = remotePlayers[i];
            for (int j = 0; j < player.BuiltDistricts.collection.Count; j++)
            {
                score += player.BuiltDistricts.GetCardAt(j).cost;
            }
            if (player.BuiltDistricts.collection.Count > 7)
            {
                //TODO: add 4 points to first player to build 8 districts
                score += 2;
            }
            if (CheckForAllColors(player))
                score += 3;
            players.Add(player);
            scores.Add(score);
        }

        playerScore.SetScores(players.ToArray(), scores.ToArray());
    }

    public bool CheckForAllColors(Character player)
    {
        bool[] colors = new bool[Enum.GetValues(typeof(CardColor)).Length];
        for (int i = 0; i < player.PlayerHand.collection.Count; i++)
        {
            colors[(int)player.PlayerHand.GetCardAt(i).color] = true;
        }
        bool alldone = false;
        for (int i = 0; i < colors.Length; i++)
        {
            alldone = colors[i];
        }
        return alldone;
    }
}

public enum GameState
{
    CharacterSelection,
    PlayerTurns
}
