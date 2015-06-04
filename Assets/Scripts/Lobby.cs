/// <summary>
/// Скрипт, отвечающий за создание игры, подсоединение к уже существующей и все такое.
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Lobby : Photon.MonoBehaviour {

    private bool connectedToPUN = false;

    public string RoomName 
    {
        get { return roomName; }
        set
        {
            if (value != "")
            {
                roomName = value;
            }
        }
    }

    private string roomName;

    private GameObject levelSettings;

    public string[] Levels; //Список уровней

    public string LevelName = "TestLevel";

    private bool connectFailed = false;

    private bool connected = false;

    public static readonly string SceneNameMenu = "Lobby";

    public static readonly string SceneNameGame = "Game";

    public static readonly string GameVersion = "0.1";

    public string PlayerName
    {
        get { return PhotonNetwork.playerName; }

        set
        {
            if (value != "")
            {
                PhotonNetwork.playerName = value;
                PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
            }
        }
    }

    public void Awake()
    {
        levelSettings = GameObject.Find("LevelSettings");

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings(GameVersion);
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Mingebag" + Random.Range(1, 255);
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }


    public void Update()
    {
        connected = PhotonNetwork.connected;

        if (!connected)
        {
            if (PhotonNetwork.connecting)
            {
                Debug.Log("Connecting to: " + PhotonNetwork.ServerAddress);
            }
            else
            {
                Debug.Log("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
            }

            if (this.connectFailed)
            {

                Debug.Log("Connection failed.");
                Debug.Log(String.Format("Server: {0}", new object[] { PhotonNetwork.ServerAddress }));
                Debug.Log("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);

            }

            return;
        }


    }

    public void SetLevelSettings(string levelName, bool teams, bool bots, float roundTime)
    {
        LevelSettings config = levelSettings.GetComponent<LevelSettings>();
        config.LevelName = LevelName;
        config.HasTeams = teams;
        config.Bots = bots;
        config.RoundTime = roundTime;
    }

    public void CreateRoom()
    {
        //Placeholder:
        SetLevelSettings("TestLevel", true, false, 240f);
        if (true) //Добавить проверку на присоединение к мастер-серверу
        {
            PhotonNetwork.CreateRoom(this.roomName, new RoomOptions() { maxPlayers = 10 }, null);
        }
        
       
    }

    public void JoinRandomRoom()
    {
        if (true) //Добавить проверку на присоединение к мастер-серверу
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        PhotonNetwork.LoadLevel(SceneNameGame);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.networkingPeer.ServerAddress);
    }

    public void OnConnectedToLobby()
    {
        connectedToPUN = true;
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
	
}
