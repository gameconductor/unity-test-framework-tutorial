using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{

    public Transform hitCenter;
    public LayerMask hitMask;
    public Collider[] hits;

    void Update()
    {
        hits = Physics.OverlapBox(hitCenter.position, new Vector3(0.359f, 0.138f, 2.0f), transform.rotation, hitMask);
    }
}
