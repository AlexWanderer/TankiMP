using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour {
    public AnimationCurve Scale;
    public float MaxScale = 5f;
    bool started = false;
    float time = 0f;
    public float maxTime;


    void Update()
    {
        
        if (started)
        {
            time += Time.deltaTime;
            transform.localScale = new Vector3(Scale.Evaluate(time/maxTime) * MaxScale, Scale.Evaluate(time/maxTime) * MaxScale, Scale.Evaluate(time/maxTime) * MaxScale);
        }
        
    }

    public void Explode()
    {
        started = true;
        Destroy(this.gameObject, maxTime);
    }

}
