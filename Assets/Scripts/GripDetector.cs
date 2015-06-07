using UnityEngine;
using System.Collections;

public class GripDetector : MonoBehaviour {
    public bool DebugDraw = false;

    public Collider RightTrack;
    public Collider LeftTrack;
    GameObject colLeft;
    GameObject colRight;


    public int ValidCols = 0;

    bool newCycle = false;
    bool hasCols = false;


    public bool HasContact()
    {
        if ((ValidCols > 1)&&(Vector3.Angle(Vector3.up,transform.TransformDirection(Vector3.up)) < 50))
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        Collider[] cols = GetComponents<CapsuleCollider>();
        RightTrack = cols[0];
        LeftTrack = cols[1];
    }

    void FixedUpdate()
    {
       // Debug.Log(HasContact());

        if (hasCols)
        {
            hasCols = false;
        }
        else
        {
            ValidCols = 0;
        }
        newCycle = true;
    }


    void OnCollisionStay(Collision col)
    {
        if (DebugDraw)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.blue);
        }
        
        int i = 0;
       // validCols = 0;
        Color clr = Color.white;
        if (newCycle)
        {
            ValidCols = 0;
            newCycle = false;
        }    
            foreach (ContactPoint p in col.contacts)
            {
               if((p.thisCollider == RightTrack)||(p.thisCollider == LeftTrack))
               {
                   hasCols = true;
                   if (Mathf.Abs(Vector3.Angle(transform.TransformDirection(Vector3.up), col.contacts[i].normal)) < 50f)
                   {
                       clr = Color.green;
                       ValidCols++;
                   }
                   else
                   {
                       clr = Color.red;
                   }
                   if (DebugDraw)
                   {
                       Debug.DrawRay(col.contacts[i].point, col.contacts[i].normal, clr);
                   }
                   
               }
               i++;
                
            }

    }
}
