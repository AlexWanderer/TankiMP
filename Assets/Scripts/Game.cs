/// <summary>
/// Скрипт с базовой логикой мультиплеера. Создание персонажа, загрузка карты, этц.
/// </summary>

using UnityEngine;
using System.Collections;



public class Game : MonoBehaviour {

    public GameObject PlayerPrefab;

    private GameObject player;


    public void Awake()
    {

        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(Lobby.SceneNameMenu);
            return;
        }

        player = PhotonNetwork.Instantiate(this.PlayerPrefab.name, transform.position, Quaternion.identity, 0) as GameObject;

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
