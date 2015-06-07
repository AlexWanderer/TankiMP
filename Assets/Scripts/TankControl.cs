/// <summary>
/// Скрипт, отвечающий за управление танком. Мультиплеерная синхронизация вынесена в отдельный скрипт.
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GripDetector))]

public class TankControl : Photon.MonoBehaviour
{
    public bool PlayerControlled = false;

    public GameObject Head;
    public GameObject Barrel;
    public GameObject Muzzle;

    [Range(0f, 45f)]
    public float MaxBarrelAngle = 25f;

    [Range(-20f, 0f)]
    public float MinBarrelAngle = -15f;


    [Range(0.0f, 6.0f)]
    public float MaxVel = 3f;
    public float VelMod = 0f;

    public AnimationCurve EngPower;
    public float EngMaxHP = 10;
    public float PowMod = 0f;

    public AnimationCurve TurnForce;
    public float MaxTurnSpeed = 2f;
    public float MaxTurnMomentum = 5f;


    Rigidbody body;
    Vector3 velocity;

    float angleToTurn;
    public float HeadTurnRate = 90f;
    public float BarrelTurnRate = 15f;
    float barAngleToTurn;

    public float CorrShootDelta = 0.2f;


    float curPower;
    float curMomentum;

    GripDetector grDetect;
    bool hasGrip;

    private ShootController marker;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = transform.Find("MassCenter").transform.localPosition;
        grDetect = GetComponent<GripDetector>();

        if (photonView.isMine)
        {
            Camera.main.gameObject.GetComponent<FollowTarget>().Target = this.transform;
            marker = Camera.main.gameObject.GetComponent<ShootController>();
        }
    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
          //  Debug.Log("mine");
            body.isKinematic = false;

            body.WakeUp(); // Танк не должен никогда засыпать, он всегда бдит
            CalcMovement();
            RotateHead();
            RotateBarrel();



            if ((PlayerControlled)&&(!Game.lockControls))
            {

                Move(Input.GetAxis("Vertical"));
                Turn(Input.GetAxis("Horizontal"));
                LookAtPoint(marker.TargetPosition);
                if (Input.GetKeyDown("r") && (!hasGrip))
                {
                  //  Recover(); // Пока эту функцию использовать не будем, пока не решу, как ее реализовать получше
                }

            }
        }
        else
        {
            body.isKinematic = true; // Блокируем взаимодействие с танком, он не наш, мы его можем только бить и об него спотыкаться.
        }
       
    }


    public void Move(float dir)
    {
        if (hasGrip)
        {
           
            if (dir > 0)
            {

                body.AddRelativeForce(-curPower * dir, 0, 0);
            }
            else
            {
                body.AddRelativeForce(curPower * (-dir), 0, 0);
            }
        }
    }

    public void Turn(float dir)
    {
        if (dir > 0)
        {
            body.AddRelativeTorque(0, curMomentum * dir, 0);
        }
        else
        {
            body.AddRelativeTorque(0, curMomentum * dir, 0);
        }
    }

    void CalcMovement()
    {
        hasGrip = grDetect.HasContact();
        velocity = body.velocity;
        float velM = velocity.magnitude;

        float engForce = EngPower.Evaluate(velM / (MaxVel + VelMod)) * (EngMaxHP + PowMod);
        curPower = engForce;

        float turnMom = TurnForce.Evaluate(Mathf.Abs(body.angularVelocity.y) / MaxTurnSpeed) * MaxTurnMomentum;
        curMomentum = turnMom;
    }

    public void LookAtPoint(Vector3 point)
    {
        Vector3 fwd = -Head.transform.right;
       // Debug.DrawLine(Head.transform.position, Head.transform.position + fwd);
        Vector3 trgDir = point - Barrel.transform.position;
       // Debug.DrawLine(Barrel.transform.position, point, Color.red);

        Vector2 fwd2d = new Vector2(fwd.x, fwd.z);
        Vector2 trg2d = new Vector2(trgDir.x, trgDir.z);

        float deltaHeight = point.y - Head.transform.position.y - CorrShootDelta;

        float elevation = Mathf.Acos(((trgDir.magnitude) * (trgDir.magnitude) + (trg2d.magnitude) * (trg2d.magnitude) - Mathf.Pow(deltaHeight, 2f)) / (2f * trgDir.magnitude * trg2d.magnitude));
        elevation *= Mathf.Rad2Deg;
        elevation *= Mathf.Sign(deltaHeight);
        float desAng = Vector2.Angle(fwd2d, trg2d) * Mathf.Sign(Vector3.Cross(fwd, trgDir).y);
        // Debug.Log(elevation);     
        TurnHead(desAng);
        TurnBarrel(elevation);
    }

    public void TurnHead(float angle)
    {
        angleToTurn = angle;

    }


    void TurnBarrel(float angle)
    {
        angle = Mathf.Clamp(angle, MinBarrelAngle, MaxBarrelAngle);

        barAngleToTurn = Mathf.DeltaAngle(-Barrel.transform.localRotation.eulerAngles.z, angle);
    }

    void RotateBarrel()
    {

        float barTurnTol = BarrelTurnRate * Time.deltaTime;
        if (Mathf.Abs(barAngleToTurn) > barTurnTol)
        {
            float angle = -Mathf.Sign(barAngleToTurn) * barTurnTol;
            barAngleToTurn += angle;
            Barrel.transform.rotation *= Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Barrel.transform.rotation *= Quaternion.Euler(0, 0, barAngleToTurn);
            // barAngleToTurn = 0;
        }

    }

    void RotateHead()
    {
        float headTurnTol = HeadTurnRate * Time.deltaTime;
        if (Mathf.Abs(angleToTurn) > headTurnTol)
        {
            float angle = Mathf.Sign(angleToTurn) * headTurnTol;
            angleToTurn -= angle;
            Head.transform.rotation *= Quaternion.Euler(0, angle, 0);
            // 
        }
        else
        {
            Head.transform.rotation *= Quaternion.Euler(0, angleToTurn, 0);
            angleToTurn = 0;
        }
    }

    public bool TargettingComplete()
    {
        if ((Mathf.Abs(angleToTurn) + Mathf.Abs(barAngleToTurn)) < .8f)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}