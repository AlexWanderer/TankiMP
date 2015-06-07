using UnityEngine;
using System.Collections;

public class DamageInfo  
{
    public float Damage = 0f;

    public PunTeams.Team Team = PunTeams.Team.none;

    public int Owner = 0;

    public GameObject OwnerObj;

    public DamageInfo(float dmg, PunTeams.Team team, int owner)
    {
        Damage = dmg;

        Team = team;

        Owner = owner;

        //OwnerObj = ownObj;
    }
}
