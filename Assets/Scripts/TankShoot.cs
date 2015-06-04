//Можно использовать и для мобов. Универсальный скрипт
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(TankControl))]

public class TankShoot : Photon.MonoBehaviour {

    public GameObject Projectile;
    public Transform Muzzle;
    public Transform Head;

    public bool ShootingEnabled = true;
    public AudioClip ShotSnd;

    private TankControl controls;
    private HealthManager hpManager;

    public bool OverrideShotParameters = false;
    public float Damage = 10f;
    public Color ShotColor = Color.red;
    public bool HasSplash = false;
    public float SplashRad = 0.5f;
    public float SplashDmg = 5f;
    

    [Range(0.02f, 1.0f)]
    public float ShootDelay = 0.3f;

    [Range(1f, 30f)]
    public float MuzzleSpeed = 20f;

    [Range(0f, 50f)]
    public float Recoil = 3f;

    float lastShootT = 0f;
    bool shooting = false;

    Rigidbody tBody;

	void Awake () 
    {
        tBody = GetComponent<Rigidbody>();
        controls = GetComponent<TankControl>();
        hpManager = GetComponent<HealthManager>();
	}
	
	
	void Update () 
    {

        if (controls.PlayerControlled)
        {
            if (photonView.isMine)
            {
                if (Input.GetMouseButton(0))
                {
                    Shoot();
                }
            }
           
        }
       
        if(!shooting) 
        {
            Muzzle.gameObject.SetActive(false);
        }
        else
        {
            shooting = false;
        }
	
    }

    [RPC]
    public void ShootEffects()
    {
        
        GetComponent<AudioSource>().PlayOneShot(ShotSnd);
        Muzzle.gameObject.SetActive(true);

    }

   public void Shoot()
    {
        if (ShootingEnabled)
        {
            shooting = true;

           // Debug.Log(PhotonNetwork.GetPing());

            if (Time.time > (lastShootT + ShootDelay))
            {
                //Создаем снаряд, обвязку к нему, настраиваем
                GameObject proj = PhotonNetwork.Instantiate(Projectile.name, Muzzle.position, Head.rotation * Quaternion.Euler(0, 0, 90), 0);

                photonView.RPC("ShootEffects", PhotonTargets.All);
                proj.GetComponent<ShellExplosion>().photonView.RPC("SetSettings", PhotonTargets.All, Damage, hpManager.Team, HasSplash, SplashRad, SplashDmg );
                
                //А сами тем временем даем пенделя снаряду
                Rigidbody body = proj.GetComponent<Rigidbody>();
               // body.velocity = GetComponent<Rigidbody>().velocity + Vector3.forward;
                body.velocity = GetComponent<Rigidbody>().velocity + Muzzle.transform.TransformDirection(-Vector3.right) * MuzzleSpeed + Muzzle.transform.TransformDirection(Vector3.up)*0.1f;
                tBody.AddForceAtPosition(Muzzle.transform.TransformDirection(Vector3.right) * Recoil, Muzzle.transform.position);
               
                lastShootT = Time.time;
            }

        }
        
      
  }
}
