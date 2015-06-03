//Можно использовать и для мобов. Универсальный скрипт
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(TankControl))]

public class TankShoot : Photon.MonoBehaviour {
   // public bool PlayerControlled = false;

    public GameObject Projectile;
    public Transform Muzzle;
    public Transform Head;

    public bool shootingEnabled = true;
    public AudioClip shotSnd;

    private TankControl controls;

    public bool overrideShotParameters = false;
    public float damage = 10f;
    public Color shotColor = Color.red;
    public bool hasSplash = false;
    public float splashRad = 0.5f;
    public float splashDmg = 5f;
    

    [Range(0.02f, 1.0f)]
    public float shootDelay = 0.3f;

    [Range(1f, 30f)]
    public float muzzleSpeed = 20f;

    [Range(0f, 50f)]
    public float recoil = 3f;

    float lastShootT = 0f;
    bool shooting = false;

    Rigidbody tBody;

	void Awake () 
    {
        tBody = GetComponent<Rigidbody>();
        controls = GetComponent<TankControl>();
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

   public void Shoot()
    {
        if (shootingEnabled)
        {
            shooting = true;
            Muzzle.gameObject.SetActive(true);

            if (Time.time > (lastShootT + shootDelay))
            {

                GameObject proj = Instantiate(Projectile) as GameObject;
                if (overrideShotParameters)
                {
                 //   proj.GetComponent<ShellExplosion>().SetSettings(damage, shotColor, hasSplash, splashRad, splashDmg);
                }

                GetComponent<AudioSource>().PlayOneShot(shotSnd);
                proj.transform.position = Muzzle.position;
                proj.transform.rotation = Head.rotation;
                proj.transform.rotation *= Quaternion.Euler(0, 0, 90);
                Rigidbody body = proj.GetComponent<Rigidbody>();
                body.velocity = GetComponent<Rigidbody>().velocity + Muzzle.transform.TransformDirection(-Vector3.right) * muzzleSpeed + Muzzle.transform.TransformDirection(Vector3.up)*.3f;
                tBody.AddForceAtPosition(Muzzle.transform.TransformDirection(Vector3.right) * recoil, Muzzle.transform.position);
                lastShootT = Time.time;
            }

        }
        
      
  }
}
