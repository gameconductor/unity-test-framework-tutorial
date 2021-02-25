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
        // hit = Physics.CheckBox(hitCenter.position, new Vector3(0.359f, 0.138f, 2.0f), transform.rotation, hitMask);
        hits = Physics.OverlapBox(hitCenter.position, new Vector3(0.359f, 0.138f, 2.0f), transform.rotation, hitMask);
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        // bool hit = Physics.CheckBox(hitCenter.position, new Vector3(0.2175f, 0.082f, 0.805f), transform.rotation, hitMask);
        if (hits.Length > 0)
        {
            Gizmos.matrix = Matrix4x4.TRS(hitCenter.position, transform.rotation, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.2175f, 0.082f, 0.805f));
        }
    }
}
