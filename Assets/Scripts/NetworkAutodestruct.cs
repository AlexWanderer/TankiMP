using UnityEngine;
using System.Collections;

public class NetworkAutodestruct : Photon.MonoBehaviour {
    [Range(0.02f, 10f)]
    public float Delay = 2f;


    void Start()
    {
        if (photonView.isMine)
        {
            Invoke("Kill", Delay);

        }

    }

    void Kill()
    {
        PhotonNetwork.Destroy(this.gameObject);

    }


}
