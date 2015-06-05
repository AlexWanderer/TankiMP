using UnityEngine;
using System.Collections;

public class HealthManager : Photon.MonoBehaviour {
    public float Health = 100f;
    public GameObject DeadPrefab;
    public int Team = -1;

    private Game gm;
    float maxHealth;
    bool dead = false;
    private Material plyMat;

    private string killedBy;

	void Awake () 
    {
        gm = GameObject.Find("WorldOrigin").GetComponent<Game>();

        maxHealth = Health;
	}

    [RPC]
    public void InitHPAndTeam(float hp, float maxHP, int team)
    {
        Team = team;
        Health = hp;
        maxHealth = maxHP;


        plyMat = Instantiate(GetComponent<Renderer>().material) as Material;
        if (team == 0)
        {
            plyMat.color = Color.red;

        }
        else
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
            photonView.RPC("BroadcastDamage", PhotonTargets.AllBuffered, dmgInfo);   
        }
    }

    [RPC]
    void BroadcastDamage(DamageInfo dmgInfo)
    {
        Health -= dmgInfo.Damage;
        

        if (photonView.isMine)
        {
            if ((Health <= 0)&&(!dead))
            {
                dead = true;
                gm.PlayerKilled();
                killedBy = dmgInfo.Owner;
                photonView.RPC("Die",PhotonTargets.All, killedBy);
            }
        }
    }

    [RPC]
    public void heal(float amount)
    {
        if (!dead)
        {
            Health += amount;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    }

    [RPC]
    public void Die(string whoKilled)
    {
        Debug.Log(whoKilled + "became a frag");
        killedBy = whoKilled;
        dead = true;
        Health = 0;
        GameObject ded = Instantiate(DeadPrefab, this.transform.position, transform.rotation) as GameObject; //Не синхронизируем, так как физики они не имеют и не влияют на игру.
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(this.photonView);
        }
       // Destroy(this.gameObject);
    }


    public float GetHealthInPercent()
    {
        return Health / maxHealth;
    }
}
