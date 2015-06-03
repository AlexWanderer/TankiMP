using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour {
    public float shakeAmp = 0.2f;
    float curAmp;
    bool shakeOn = false;

    Transform tr;
	
	void Start ()
    {
        tr = transform;
        //StartShake(0.3f);
	}
	
	
	void LateUpdate ()
    {
        if (shakeOn)
        {
            Shake();
        }
	}

    public void StartShake(float amp)
    {
        shakeOn = true;
        shakeAmp = amp;
        curAmp = amp;
    }

    void Shake()
    {
        tr.position += Random.insideUnitSphere*curAmp;
        curAmp -= Time.deltaTime * 0.13f;
        if (curAmp < 0)
        {
            shakeOn = false;
        }
        //shakeOn = true;

    }
}
