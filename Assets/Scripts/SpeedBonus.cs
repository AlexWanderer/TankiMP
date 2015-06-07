using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]

public class SpeedBonus : Photon.MonoBehaviour {
    public Vector3 RotVector = new Vector3 (0, 30, 0);

    void Update()
    {
        transform.rotation *= Quaternion.Euler(RotVector * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        GameObject obj = col.transform.root.gameObject;
        if (obj.tag == "NetworkObject")
        {
            if (obj.GetPhotonView().isMine) // Если наш персонаж на него наехал
            {
                if (obj.GetComponent<TankControl>())
                {
                    if (obj.GetComponent<SpeedBonusEffect>())
                    {
                        // Не делаем ничего
                        return;
                    }
                    else
                    {
                        obj.AddComponent<SpeedBonusEffect>();
                        photonView.RPC("Die", PhotonNetwork.masterClient);
                        photonView.RPC("AddBonus", PhotonTargets.OthersBuffered, obj.GetComponent<PhotonView>().viewID);
                        Destroy(this.gameObject);
                    }
                }

            }
            else
            {
                return;
            }
        }

    }

    [RPC]
    public void Die()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    [RPC]
    public void AddBonus(int ID)
    {
        PhotonView view = PhotonView.Find(ID);
        view.gameObject.AddComponent<SpeedBonusEffect>();
    }
}
