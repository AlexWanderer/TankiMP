/// <summary>
/// Скрипт, отвечающий за управление танком. Мультиплеерная синхронизация вынесена в отдельный скрипт.
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TankControl))]

public class TankMPSync : Photon.MonoBehaviour
{
    private TankControl controls;


    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private Quaternion correctPlayerHeadRot = Quaternion.identity; //We lerp towards this
    private Quaternion correctBarrelRot = Quaternion.identity; //We lerp towards this
    
   // private Transform BarrelRot


    void Awake()
    {
        controls = GetComponent<TankControl>();
    }

    void Update()
    {
        if (!photonView.isMine)
        {

            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            controls.gameObject.transform.position = Vector3.Lerp(controls.gameObject.transform.position, correctPlayerPos, Time.deltaTime * 12f);
            controls.gameObject.transform.rotation = Quaternion.Lerp(controls.gameObject.transform.rotation, correctPlayerRot, Time.deltaTime * 12f);
            controls.Head.transform.rotation = Quaternion.Lerp(controls.Head.transform.rotation, correctPlayerHeadRot, Time.deltaTime * 12);
            controls.Barrel.transform.rotation = Quaternion.Lerp(controls.Barrel.transform.rotation, correctBarrelRot, Time.deltaTime * 14);

        }
        else
        {
            //Ничего не делаем, все перемещения производит локальный скрипт управления
        }

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(controls.gameObject.transform.position);
            stream.SendNext(controls.gameObject.transform.rotation);
            stream.SendNext(controls.Barrel.transform.rotation);
            stream.SendNext(controls.Head.transform.rotation);
        }
        else
        {
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            correctBarrelRot = (Quaternion)stream.ReceiveNext();
            correctPlayerHeadRot = (Quaternion)stream.ReceiveNext();
        }
    }
}