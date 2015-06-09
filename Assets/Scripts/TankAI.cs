using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TankControl))]
[RequireComponent(typeof(TankShoot))]

public class TankAI : Photon.MonoBehaviour {
   // public GameObject Marker;
    public LayerMask DetectMask;
    public LayerMask ObstacleDetectMask;

    public enum AIState
    {
        Idle,
        Moving,
        Wandering,
        Attacking
    }

    public AIState State;
    public string EnemyTag = "Player";
    public bool TargetPlayer = true;
    public GameObject EnemyObject;
    public Transform VisHelper;

    bool hasTask = false;

    TankControl controls;
    TankShoot gun;

    bool avoiding = false;
    float avoidDistance = 2f;


    public float MaxTimeOutOfSight = 5;
    float timeOutOfSight;

	void Start () 
    {
       // GameObject marker2 = (GameObject)Instantiate(Marker);
        //marker2.transform.parent = GameObject.Find("Canvas").transform;
        //marker2.GetComponent<EnemyMarker>().obj = gameObject;
        controls = GetComponent<TankControl>();
        gun = GetComponent<TankShoot>();


        if (TargetPlayer)
        {
            FindEnemyByTag();
        }
	}
	

	void FixedUpdate () 
    {
        if (photonView.isMine)
        {
            if (!hasTask)
            {
                if (State == AIState.Attacking)
                {
                    hasTask = true;
                    StartCoroutine(AttackMode());

                }
                else if (State == AIState.Wandering)
                {
                    hasTask = true;
                    StartCoroutine(WanderingMode());
                }
            }
        }

	}

    IEnumerator AttackMode()
    {

        while(true) 
        {
             if (CheckVisibility())
             {
                timeOutOfSight = Time.time; // Мы видели врага последний раз - сейчас
                if (!avoiding)
                {
                    if (FrontSensor() < avoidDistance)
                    {
                        avoiding = true;
                        StartCoroutine(ObstacleAvoid());
                    }
                    else
                    {
                        MoveTowardsEnemy();
                    }

                    
                }

               
                controls.LookAtPoint(EnemyObject.transform.position);
                if (controls.TargettingComplete())
                {
                    gun.Shoot();
                }

             } else {
                 if(Time.time > (timeOutOfSight + MaxTimeOutOfSight)) {
                     State = AIState.Wandering;
                     hasTask = false;
                     break;
                 }
             }

             yield return new WaitForFixedUpdate();
        }        
       
    }

    IEnumerator WanderingMode()
    {
        float wanderTime = 0; ;
        float maxWanderTime = 5f;
        float turnTime = 0f;
        float curTurnTime = 0f;
        float maxTurnTime = 1.5f;
        float turnDir = 0;
        bool turning = false;

        while (true)
        {
            if (CheckVisibility()) // Оп-оп, оппа нишутя! враг! Идем его херачить
            {
                State = AIState.Attacking;
                hasTask = false;
                break;
            }

            wanderTime += Time.deltaTime;
            if (!avoiding)
            {
                if (FrontSensor() < avoidDistance)
                {
                    avoiding = true;
                    StartCoroutine(ObstacleAvoid());
                }

                controls.Move(1);
                if (wanderTime > maxWanderTime)
                {
                    if (!turning)
                    {
                        turnDir = Mathf.Sign(Random.Range(-1, 1));
                        turnTime = Random.Range(0.5f, maxTurnTime);
                        turning = true;
                        curTurnTime = 0f;
                    }
                    else
                    {
                        if (curTurnTime < turnTime)
                        {
                            curTurnTime += Time.deltaTime;
                            controls.Turn(turnDir);
                        }
                        else
                        {
                            turning = false;
                            wanderTime = 0f;
                            turnTime = 0f;
                        }
                    }
                }

            }
 

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ObstacleAvoid()
    {
        float timer = 0f;
        float avoidTime = Random.Range(0.35f,0.6f);
        float avoidDir = Mathf.Sign(Random.Range(-1, 1));

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > avoidTime)
            {
                avoiding = false;
               // Debug.Log("Avoid finished");
                break;
            }
            controls.Turn(1f * 2f);
            controls.Move(0.5f);
            yield return new WaitForFixedUpdate();
        }
        
    }

    void FindEnemyByTag()
    {
        EnemyObject = GameObject.FindGameObjectWithTag(EnemyTag);
    }

    void FindEnemyPlayer()
    {

    }

    bool CheckVisibility()
    {
        Vector3 startPos = VisHelper.position; 
        Vector3 dir = EnemyObject.transform.position - startPos + Vector3.up * 0.15f;
        dir.Normalize();
        Ray ray = new Ray(startPos, dir);
      //  Debug.DrawRay(startPos, dir,Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15f, DetectMask))
        {
            if (hit.collider.transform.root.gameObject == EnemyObject)
            {
                return true; 
            }  
        }

        return false;

    }


    float FrontSensor()
    {
        Vector3 startPos = VisHelper.position - Vector3.up * 0.15f ;
        Vector3 dir = transform.TransformDirection(-Vector3.right);
       // Debug.DrawRay(startPos, dir, Color.green);
        dir.Normalize();
        Ray ray = new Ray(startPos, dir);
         RaycastHit hit;
         if (Physics.Raycast(ray, out hit, 4f, ObstacleDetectMask))
         {

             return hit.distance;
         }
        return 999f;
    }

    void TurnDeg(float deg)
    {

    }

    void MoveTowardsEnemy()
    {
       // FrontSensor();
        Vector3 ourPos = transform.position;
        Vector3 enemyPos = EnemyObject.transform.position;
        Vector3 moveDir = enemyPos - ourPos;
        float dist = moveDir.magnitude;

        Vector3 fwdDir = transform.TransformDirection(-Vector3.right);
        
        Debug.DrawRay(ourPos, fwdDir, Color.yellow);
        moveDir.Normalize();

        float TurnDir = Vector3.Cross(fwdDir, moveDir).y;

        moveDir.y = 0;
        fwdDir.y = 0;
        float angle = Vector3.Angle(fwdDir, moveDir);

        if (angle > 5)
        {
            controls.Turn(Mathf.Sign(TurnDir));
        }

        if (dist > 2)
        {
            controls.Move(1f);
        }

    }

}
