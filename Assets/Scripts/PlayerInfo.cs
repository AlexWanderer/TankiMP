﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfo : Photon.MonoBehaviour {

    public GameObject Player;

    public Text PlayerName;

    public Text PlayerHp;

    TankControl controls;

    HealthManager hp;

    void Awake() 
    {
        hp = Player.GetComponent<HealthManager>();

        controls = Player.GetComponent<TankControl>();

        PlayerName.text = hp.Name;

        if (hp.Team == 0)
        {
            PlayerName.color = Color.red;
            PlayerHp.color = Color.red;
        }
        else
        {
            PlayerName.color = Color.blue;
            PlayerHp.color = Color.blue;
        }


        
    }

	void Update () 
    {

        if (hp.Team == 0)
        {
            PlayerName.color = Color.red;
            PlayerHp.color = Color.red;
        }
        else
        {
            PlayerName.color = Color.blue;
            PlayerHp.color = Color.blue;
        }

        PlayerHp.text = Mathf.Floor(hp.Health).ToString();
        PlayerName.text = hp.Name;

        this.transform.rotation = Quaternion.Euler(26f, 0, 0);


	}
}
