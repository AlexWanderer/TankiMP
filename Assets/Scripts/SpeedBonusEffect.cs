using UnityEngine;
using System.Collections;

public class SpeedBonusEffect : Photon.MonoBehaviour {
    public float Duration = 10f;
    float elapsedTime = 0f;

    public float AdditionalHP = 2f;
    float stockHP;
    
    public float AdditionalSpeed = 3f;
    float stockSpeed;

    TankControl controls;

    void Awake()
    {
        ApplyBonus();
        controls = GetComponent<TankControl>();
    }

    public void ApplyBonus()
    {
        controls = GetComponent<TankControl>();
        stockSpeed = controls.MaxVel;
        stockHP = controls.EngMaxHP;

        controls.MaxVel += AdditionalSpeed;
        controls.EngMaxHP += AdditionalHP;

    }

    public void ClearBonus()
    {
        controls.MaxVel = stockSpeed;
        controls.EngMaxHP = stockHP;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > Duration)
        {
            ClearBonus();
            if (photonView.isMine)
            {
                photonView.RPC("DestroyBonus", PhotonTargets.Others);
            }
            Destroy(this.GetComponent<SpeedBonusEffect>());
        }
    }

    [RPC]
    public void DestroyBonus()
    {
        Destroy(this.GetComponent<SpeedBonusEffect>());
    }
}
