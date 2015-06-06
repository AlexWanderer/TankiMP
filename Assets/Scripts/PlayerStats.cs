using UnityEngine;
using System.Collections;

public class PlayerStats : Photon.MonoBehaviour 
{
    public string Name;
    public int Deaths;
    public int Kills;

    void Awake()
    {
        Name = PhotonNetwork.playerName;
        Kills = PhotonNetwork.player.GetScore();

    }
	
}
