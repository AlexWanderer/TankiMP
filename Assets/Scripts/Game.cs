/// <summary>
/// Скрипт с базовой логикой мультиплеера. Создание персонажа, загрузка карты, этц.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;



public class Game : Photon.MonoBehaviour {

    public Image LoadScreen;

    public RectTransform SpawnWindow;

    public RectTransform TeamsWindow;

    public RectTransform SpawnButton;

    public GameObject PlayerPrefab;

    private GameObject player;

    private bool playerSpawned = false;

    public bool PlayerDead = true;

    private PunTeams.Team team = PunTeams.Team.none;

    private GameObject levelSettings;

    private LevelSettings levelConfig;

    private bool levelSynced = false;

    private PhotonView photonView;

    public static float PlayerHP = 100;
    public Text hpGUI;

    public static bool lockControls = false; // свойство для блокировки управления персонажем

   // bool 

    public void Awake()
    {
        LoadScreen.gameObject.SetActive(true); //Пока все не загрузилось, кажем загрузочный экран

        photonView = GetComponent<PhotonView>();

        

        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(Lobby.SceneNameMenu);
            return;
        }

      //  player = PhotonNetwork.Instantiate(this.PlayerPrefab.name, transform.position, Quaternion.identity, 0) as GameObject;

        if (PhotonNetwork.isMasterClient) // Если мы сервер, запускаем процедуру инициализации уровня
        {
            if (levelSettings == null)
            {
                levelSettings = GameObject.Find("LevelSettings");
            }

            LevelSettings s = levelSettings.GetComponent<LevelSettings>();
            levelConfig = s;

           // photonView.RPC("SyncLevelSettings", PhotonTargets.OthersBuffered, s.LevelName, s.HasTeams, s.Bots, s.RoundTime);
            s.LoadLevel();
            levelSynced = true;
        }

    }

    [RPC]
    public void SyncLevelSettings(string lvlName, bool teams, bool bots, float roundTime)
    {
        if (levelSettings == null)
        {
            if (!(GameObject.Find("LevelSettings")))
            {
                levelSettings = new GameObject();
                levelSettings.AddComponent<LevelSettings>();
            }
            else
            {
                levelSettings = GameObject.Find("LevelSettings");
            }
            
        }

        LevelSettings config = levelSettings.GetComponent<LevelSettings>();

        config.LevelName = lvlName;
        config.HasTeams = teams;
        config.Bots = bots;
        config.RoundTime = roundTime;
        config.LoadLevel();
        levelConfig = config;

        levelSynced = true;

    }

    void Update()
    {
        if (levelSynced && !playerSpawned)
        {
            SpawnWindow.gameObject.SetActive(true); //Если не еще не заспавнились, кажем диалог выбора команды и кнопку спавна.
            if (levelConfig.HasTeams)
            {
                LoadScreen.gameObject.SetActive(false);
                TeamsWindow.gameObject.SetActive(true);

            }
        }
        else
        {
            SpawnWindow.gameObject.SetActive(false);
            TeamsWindow.gameObject.SetActive(false);
        }

        hpGUI.text = Mathf.Floor(PlayerHP).ToString();
    }


    public void SpawnPlayer()  //Внешняя функция спавна из соответствующего меню. Выполняет проверку на принадлежность к команде
    {
        if (levelConfig.HasTeams && (team == PunTeams.Team.none))
        {
            return; // Если команда не выбрана, а уровень командный, то спавниться не будем
        }
        else
        {
            CreatePlayer();

            playerSpawned = true;
            PlayerDead = false;

        }
    }

    void CreatePlayer()
    {
        
        bool found = false;
        List<SpawnPoint> points = new List<SpawnPoint>();

        foreach (SpawnPoint p in levelConfig.SpawnPoints)
        {
            if (p.Type == SpawnPoint.PointType.Player)
            {
                if (levelConfig.HasTeams)
                {
                    if (p.Team == PhotonNetwork.player.GetTeam())
                    {
                        if (p.CheckAvailability())
                        {
                            points.Add(p);
                            found = true;
                        }
                       
                    }
                       
                }
            }
            
        }

        if (!found)
        {
            Debug.Log("Error: No such Spawn points");
            return; //Подходящих точек нет, заспанится не вышло.
        }

        SpawnPoint choosenPoint = points[Random.Range(0,points.Count)];

        player = PhotonNetwork.Instantiate(this.PlayerPrefab.name, choosenPoint.transform.position, choosenPoint.transform.rotation, 0) as GameObject;

        player.GetComponent<HealthManager>().photonView.RPC("InitHPAndTeam", PhotonTargets.AllBuffered, 100f, 100f, team, PhotonNetwork.playerName);
        
    }

    public void SetTeamAndSpawn(int teamID)
    {
        SetPlayerTeam(teamID);
        SpawnPlayer();
    }

    public void SetPlayerTeam(int teamID)
    {
        if (teamID == 1)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
            team = PunTeams.Team.blue;
        }
        else if (teamID == 0)
        {
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
            team = PunTeams.Team.red;
        }
        
       
    }

    public void DisconnectToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void PlayerKilled()
    {
        Debug.Log("You are Dead");
        StartCoroutine(DeathDelay(5f));
    }

    IEnumerator DeathDelay(float time)
    {
        yield return new WaitForSeconds(time);
        playerSpawned = false;

    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Exit()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        // back to main menu        
        Application.LoadLevel(Lobby.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
        Application.LoadLevel(Lobby.SceneNameMenu);
    }



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
        // Debug.Log(info.sender.name);
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);

        if (PhotonNetwork.isMasterClient)
        {
            LevelSettings s = levelSettings.GetComponent<LevelSettings>();
            levelConfig = s;

            photonView.RPC("SyncLevelSettings", player, s.LevelName, s.HasTeams, s.Bots, s.RoundTime);
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu        
        Application.LoadLevel(Lobby.SceneNameMenu);
    }

}
