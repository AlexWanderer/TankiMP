using UnityEngine;
using System.Collections;

public class GripDetector : MonoBehaviour {
    public Collider rightTrack;
    public Collider leftTrack;
    GameObject colLeft;
    GameObject colRight;


    //public int colsNum = 0;
    public int validCols = 0;

    bool newCycle = false;
    bool hasCols = false;


    public bool HasContact()
    {
        if ((validCols > 1)&&(Vector3.Angle(Vector3.up,transform.TransformDirection(Vector3.up)) < 50))
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        Collider[] cols = GetComponentsInChildren<CapsuleCollider>();
        rightTrack = cols[0];
        leftTrack = cols[1];
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
            validCols = 0;
        }
        newCycle = true;
    }




    void OnCollisionEnter(Collision col)
    {
        if (col.contacts[0].thisCollider == leftTrack)
        {
           // colLeft = col.gameObject;

        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject == colLeft)
        {
           // Debug.Log("left col missed");
        }
        //Debug.Log(col.contacts[0].thisCollider.gameObject.name);
        //col.
    }

    void OnCollisionStay(Collision col)
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.blue);
        int i = 0;
       // validCols = 0;
        Color clr = Color.white;
        if (newCycle)
        {
            validCols = 0;
            newCycle = false;
        }    
            foreach (ContactPoint p in col.contacts)
            {
               if((p.thisCollider == rightTrack)||(p.thisCollider == leftTrack))
               {
                   hasCols = true;
                   if (Mathf.Abs(Vector3.Angle(transform.TransformDirection(Vector3.up), col.contacts[i].normal)) < 50f)
                   {
                       clr = Color.green;
                       validCols++;
                   }
                   else
                   {
                       clr = Color.red;
                   }
                   Debug.DrawRay(col.contacts[i].point, col.contacts[i].normal, clr);
                   
               }
               i++;
                
            }
          //  Debug.Log(validCols);
        
        
    }
}
