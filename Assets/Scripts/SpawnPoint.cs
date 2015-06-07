using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
    public enum PointType
    {
        Player,
        Bot,
        Bonus,
        Other
    }

    public PointType Type = PointType.Other;
    public PunTeams.Team Team = PunTeams.Team.none;

    private float PlayerSearchRadius = 3f;
    private float OtherSearchradius = 1f;
    private int dynLayer;

    void Awake()
    {
        dynLayer = LayerMask.NameToLayer("Dynamic Objects");
    }

    public bool CheckAvailability()
    {
        float rad;
        if (Type == PointType.Player)
        {
            rad = PlayerSearchRadius;
        }
        else
        {
            rad = OtherSearchradius;
        }

        Collider[] cols = Physics.OverlapSphere(this.transform.position, rad);
        foreach (Collider c in cols)
        {
            if (c.gameObject.layer == dynLayer)
            {
                return false; // Нашли динамический объект, спавнится не стоит. Или внутри танка заспавнишься, или снаряд сразу прилетит.
            }
        }
        return true;
    }

}
