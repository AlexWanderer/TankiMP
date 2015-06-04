using UnityEngine;
using System.Collections;

public class TankExplosion : MonoBehaviour {
    public float time = 5f;
    public bool timedDestruct = true;
    Autodestruct destructor;

    public float explosiveForce = 100f;

    Rigidbody[] parts;

	void Start ()
    {
        if (timedDestruct)
        {
            destructor = gameObject.AddComponent<Autodestruct>();
            destructor.Delay = time;
            destructor.Fade = false;
        }

        Camera.main.GetComponent<CamShake>().StartShake(0.3f);

        parts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in parts)
        {
           // body.gameObject.GetComponent<Renderer>().material.color = Color.black;
            
            body.AddExplosionForce(explosiveForce, this.transform.position , 5f);
            body.AddTorque(new Vector3(Random.Range(20f, 200f), Random.Range(20f, 200f), Random.Range(20f, 200f)));
        }
	}
	
	
}
