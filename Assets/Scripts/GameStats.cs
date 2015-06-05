using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : Photon.MonoBehaviour {
    public List<string> Players = new List<string>();
    public float MaxRoundTime = 60f;
    public float RoundTime = 0f;


    void Update()
    {
        RoundTime += Time.deltaTime;
    }

}
