/// <summary>
/// Скрипт, удаляющий объекты через заданное время. Полезен для всяких временных клиентских эффектов
/// </summary>

using UnityEngine;
using System.Collections;

public class Autodestruct : MonoBehaviour {
    [Range(0.02f, 10f)]
    public float Delay = 2f;
    public bool Fade = false;
    float opacity = 1.2f;
    Material mat;

	void Start () 
    {
        Invoke("Kill", Delay);
        if (Fade)
        {
            mat = Instantiate(GetComponent<Renderer>().material) as Material;
            GetComponent<Renderer>().material = mat;
            
        }
	}

    void Kill()
    {
        Destroy(this.gameObject);

    }

    void Update()
    {
        if (Fade)
        {
            opacity -= (Time.deltaTime * opacity) / Delay;
            mat.color = new Color(mat.color.r,mat.color.g,mat.color.b,opacity);
        }
    }

}
