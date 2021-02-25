using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Transform
// Position 0.083481 0.14 0.146
// Rotation -0.198551 -2.49274 88.5106

// Box Transform
// Position 0 0 0.478
// Scale 0.2175 0.082 0.805

public class OldSword : MonoBehaviour
{

    public Transform hitCenter;
    public LayerMask hitMask;
    public bool hit = false;

    void Update()
    {
        hit = Physics.CheckBox(hitCenter.position, new Vector3(0.2175f, 0.082f, 0.805f), transform.rotation, hitMask);
    }

    void OnDrawGizmos()
    {
        return;
        bool hit = Physics.CheckBox(hitCenter.position, new Vector3(0.2175f, 0.082f, 0.805f), transform.rotation, hitMask);
        if (hit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitCenter.position, new Vector3(0.2175f, 0.082f, 0.805f));
        }
    }
}
