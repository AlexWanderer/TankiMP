using UnityEngine;
using System.Collections;

public class ShellExplosion : Photon.MonoBehaviour {
    public GameObject Explosion;
    public float Damage = 10f;
    public int Team = 0;
    public float SplashRadius = 0f;
    public float SplashDamage = 0f;
    public bool HasSplashDamage = false;

	void Awake () 
    {
	
	}

    void OnCollisionEnter(Collision col) //Обрабатываем коллизии только на главном снаряде, остальные с ним просто синхронизируются
    {
        if (photonView.isMine)
        {
            col.collider.transform.root.gameObject.SendMessage("DoDamage", Damage, SendMessageOptions.DontRequireReceiver);

            photonView.RPC("Explode", PhotonTargets.All);

            if (HasSplashDamage)
            {
                Collider[] cols = Physics.OverlapSphere(this.transform.position, SplashRadius);
                foreach (Collider c in cols)
                {
                    c.SendMessageUpwards("DoDamage", SplashDamage, SendMessageOptions.DontRequireReceiver);
                }
            }

        }
        
    }

    [RPC]
    void Explode()
    {
        Explosion.SetActive(true);
        Explosion.AddComponent<Autodestruct>();
        Explosion.GetComponent<Autodestruct>().Delay = 1.5f;
       // Explosion.transform.rotation = Quaternion.FromToRotation(Vector3.up, col.contacts[0].normal);
        Explosion.transform.parent = null;
        Destroy(this.gameObject);
    }

    [RPC]
	public void SetSettings(float dmg, int team, bool hasSplashDmg, float splashRad, float splashDmg)
    {
        Damage = dmg;
        if (team == 0) //RED
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        if (team == 1) //BLU
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        Team = team;
        //GetComponent<Light>().color = col;
        HasSplashDamage = hasSplashDmg;
        SplashRadius = splashRad;
        SplashDamage = splashDmg;

    }
}
