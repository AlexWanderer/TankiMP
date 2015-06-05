using UnityEngine;
using System.Collections;

public class DamageInfo  
{
    public float Damage = 0f;

    public int Team = -1;

    public string Owner = "null";

    public GameObject OwnerObj;

    public DamageInfo(float dmg, int team, string owner, GameObject ownObj)
    {
        Damage = dmg;

        Team = team;

        Owner = owner;

        OwnerObj = ownObj;
    }
}
