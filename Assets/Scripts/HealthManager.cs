using UnityEngine;
using System.Collections;

public class HealthManager : Photon.MonoBehaviour {
    public string Name = "Mingebag";
    public float Health = 100f;
    public GameObject DeadPrefab;
    public PunTeams.Team Team = PunTeams.Team.none;

    private Game gm;
    float maxHealth;
    bool dead = false;
    private Material plyMat;

    private int killedBy;

    public bool isBot = false;

	void Awake () 
    {
        plyMat.color = Color.green;


        gm = GameObject.Find("WorldOrigin").GetComponent<Game>();

        maxHealth = Health;

        if (!isBot)
        {
            gm.PlayerChars.Add(this.gameObject);
        }

        if ((photonView.isMine)&(!isBot))
        {
            Game.PlayerHP = Health;
        }
	}

    [RPC]
    public void InitHPAndTeam(float hp, float maxHP, PunTeams.Team team, string n)
    {
        Team = team;
        Health = hp;
        maxHealth = maxHP;
        Name = n;

        if ((photonView.isMine) & (!isBot))
        {
            Game.PlayerHP = Health;

        }

        plyMat = Instantiate(GetComponent<Renderer>().material) as Material;
        if (team == PunTeams.Team.red)
        {
            plyMat.color = Color.red;

        }
        else if (team == PunTeams.Team.blue)
        {
            plyMat.color = Color.blue;
        }

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Renderer>())
            {
                child.GetComponent<Renderer>().material = plyMat;
            }
            
        }

        GetComponent<Renderer>().material = plyMat;

    }

    public void DoDamage(DamageInfo dmgInfo)
    {


        if (!dead)
        {
            photonView.RPC("BroadcastDamage", PhotonTargets.AllBuffered, dmgInfo.Damage, dmgInfo.Owner);   
        }
    }

    [RPC]
    void BroadcastDamage(float damage, int owner)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
        }

        if (!isBot)
        {
            gm.PlayerChars.Remove(this.gameObject); // выписываемся из списка игроков
        }


        if (photonView.isMine)
        {
            if ((photonView.isMine) & (!isBot))
            {
                Game.PlayerHP = Health;
            }


            if ((Health <= 0)&&(!dead))
            {
                dead = true;
                if (!isBot)
                {
                    gm.PlayerKilled();
                }
                
                killedBy = owner;
                photonView.RPC("Die",PhotonTargets.All, killedBy);
            }
        }
    }

    [RPC]
    public void heal(float amount)
    {
        if (!dead)
        {
            if (photonView.isMine)
            {
                Game.PlayerHP = Health;
            }

            Health += amount;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    }

    [RPC]
    public void Die(int whoKilled)
    {
        if (!isBot)
        {
            Debug.Log(Name + " killed by " + PhotonNetwork.player.Get(whoKilled).name);
            killedBy = whoKilled;
        }
        
        dead = true;
        Health = 0;
        GameObject ded = Instantiate(DeadPrefab, this.transform.position, transform.rotation) as GameObject; //Не синхронизируем, так как физики они не имеют и не влияют на игру.
        if (photonView.isMine)
        {
            PhotonNetwork.player.Get(whoKilled).AddScore(1);
            if (!isBot)
            {
                ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();
                PlayerProperties["Deaths"] = (int)PhotonNetwork.player.customProperties["Deaths"] + 1;
                PhotonNetwork.player.SetCustomProperties(PlayerProperties);
            }
            

            PhotonNetwork.Destroy(this.photonView);
        }
       // Destroy(this.gameObject);
    }


    public float GetHealthInPercent()
    {
        return Health / maxHealth;
    }
}
