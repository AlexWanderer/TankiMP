using UnityEngine;
using System.Collections;

public class ShellExplosion : Photon.MonoBehaviour {
    public GameObject Explosion;
    public float Damage = 10f;
    public int Team = 0;
    public float SplashRadius = 0f;
    public float SplashDamage = 0f;
    public bool HasSplashDamage = false;
    Rigidbody body;

    Material mat;

    string owner;

	void Awake () 
    {

       // Debug.Log(Team);
        

        body = GetComponent<Rigidbody>();
        if (!photonView.isMine)
        {
            body.isKinematic = true;
        }


	}

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            body.AddForce(Vector3.up * body.mass * 7f);
        }
    }

    void OnCollisionEnter(Collision col) //Обрабатываем коллизии только на главном снаряде, остальные с ним просто синхронизируются
    {
        if (photonView.isMine)
        {
            //col.collider.transform.root.gameObject.SendMessage("DoDamage", Damage, SendMessageOptions.DontRequireReceiver);
            col.collider.transform.root.gameObject.SendMessage("DoDamage", new DamageInfo(Damage,Team, PhotonNetwork.playerName), SendMessageOptions.DontRequireReceiver);
            photonView.RPC("Explode", PhotonTargets.All);

            if (HasSplashDamage)
            {
                Collider[] cols = Physics.OverlapSphere(this.transform.position, SplashRadius);
                foreach (Collider c in cols)
                {
                    c.SendMessageUpwards("DoDamage", new DamageInfo(SplashDamage, Team, PhotonNetwork.playerName), SendMessageOptions.DontRequireReceiver);
                }
            }

        }
        
    }

    [RPC]
    void Explode()
    {
       // Explosion.SetActive(true);
       // Explosion.AddComponent<Autodestruct>();
       // Explosion.GetComponent<Autodestruct>().Delay = 1.5f;
       GameObject expl = Instantiate(Explosion, this.transform.position, Quaternion.identity) as GameObject;
       expl.GetComponent<ExplosionEffect>().Explode();

       // Explosion.transform.rotation = Quaternion.FromToRotation(Vector3.up, col.contacts[0].normal);
        Explosion.transform.parent = null;
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(this.photonView);
        }
        //Destroy(this.gameObject);
    }

    [RPC]
	public void SetSettings(float dmg, int team, bool hasSplashDmg, float splashRad, float splashDmg, string own)
    {
        owner = own;

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


        mat = Instantiate(GetComponent<Renderer>().material) as Material;
        if (Team == 0)
        {
            mat.color = Color.red;
            GetComponent<TrailRenderer>().material.color = Color.red;
        }
        else
        {
            mat.color = Color.blue;
            GetComponent<TrailRenderer>().material.color = Color.blue;
        }


        GetComponent<Renderer>().material = mat;
        

    }
}
