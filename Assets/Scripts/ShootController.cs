using UnityEngine;
using System.Collections;

public class ShootController : MonoBehaviour {
    public Transform Marker;
    public Vector3 TargetPosition;
   


    Camera cam;
	void Start () 
    {
        cam = GetComponent<Camera>();

	}
	
	
	void Update () {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f))
        {
            Marker.position = hit.point;
            TargetPosition = hit.point;
            //playerTank.LookAtPoint(hit.point);
        }

	}
}
