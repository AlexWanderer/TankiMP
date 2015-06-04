using UnityEngine;
using System.Collections;

public class HealthManager : Photon.MonoBehaviour {
    public float Health = 100f;
    public GameObject DeadPrefab;
    public int Team = -1;

    float maxHealth;
    bool dead = false;
    private Material plyMat;

	void Awake () 
    {
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

        GetComponent<Renderer>().material = plyMat;

    }

    public void DoDamage(float amount)
    {
        if (!dead)
        {
            photonView.RPC("BroadcastDamage", PhotonTargets.All, amount);   
        }
    }

    [RPC]
    void BroadcastDamage(float num)
    {
        Health -= num;

        if (photonView.isMine)
        {
            if (Health <= 0)
            {
                photonView.RPC("Die",PhotonTargets.All);
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
    public void Die()
    {
        dead = true;
        Health = 0;
        GameObject ded = Instantiate(DeadPrefab, transform.position, transform.rotation) as GameObject; //Не синхронизируем, так как физики они не имеют и не влияют на игру.

        Destroy(this.gameObject);
    }


    public float GetHealthInPercent()
    {
        return Health / maxHealth;
    }
}
