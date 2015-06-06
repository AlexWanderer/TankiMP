using UnityEngine;
using System.Collections;

public class DamageInfo  
{
    public float Damage = 0f;

    public int Team = -1;

    public int Owner = 0;

    public GameObject OwnerObj;

    public DamageInfo(float dmg, int team, int owner)
    {
        Damage = dmg;

        Team = team;

        Owner = owner;

        //OwnerObj = ownObj;
    }
}
