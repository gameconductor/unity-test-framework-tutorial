using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Telegrab : MonoBehaviour
{
    public Camera cam;
    public GameObject arms;
    public Material telegrabMat;
    public Material bucketMat;

    string state = "idle";
    RaycastHit hit;
    bool grabbing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "detect")
        {
            DetectObject();
        } else if (state == "grabbing")
        {
            AttractObject();
            Animator animator = arms.GetComponent<Animator>();
            animator.SetBool("telegrab", true);
        } else if (state == "throw")
        {
            ThrowObject();
            Animator animator = arms.GetComponent<Animator>();
            animator.SetBool("telegrab", false);
        }
    }


    void OnDrawGizmos()
    {
        if (grabbing)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cam.transform.position, hit.transform.position);
        }
    }

    void DetectObject()
    {
        Vector3 origin = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);

        if (Physics.SphereCast(origin, 0.20f, cam.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Props"))) {
            grabbing = true;
            Debug.Log("grabbing");
            state = "grabbing";
        }
    }

    void AttractObject()
    {
        Animator animator = arms.GetComponent<Animator>();
        Transform objectHit = hit.transform;
        Vector3 target = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);
        target += cam.transform.TransformDirection(new Vector3(-0.5f, 0.3f, 1.5f));
        Vector3 direction = target - objectHit.position;

        if (direction.magnitude > 1f)
        {
            objectHit.GetComponent<Rigidbody>().AddTorque(new Vector3(1f, 1f, 1f));
            // objectHit.GetComponent<Rigidbody>().AddForce(direction.normalized * 50f);
            objectHit.GetComponent<Rigidbody>().velocity = direction.normalized * (direction.magnitude + 2f) * 2f;
            animator.SetBool("telegrab", true);
        } else
        {
            objectHit.GetComponent<Rigidbody>().velocity = direction * 2f;
        }
        TelegrabVFXOn();
    }

    void ThrowObject()
    {
        Vector3 origin = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);
        Ray ray = new Ray(origin, cam.transform.forward);
        RaycastHit rayhit;
        // TODO: have a default distance if the ray fails
        if (Physics.Raycast(ray, out rayhit))
        {
            Vector3 target = origin + (cam.transform.forward * rayhit.distance);
            target.y += 0.5f;
            Transform objectHit = hit.transform;
            Vector3 direction = target - objectHit.position;
            objectHit.GetComponent<Rigidbody>().AddForce(direction.normalized * 1500f);
        }
        TelegrabVFXOff();

        Animator animator = arms.GetComponent<Animator>();
        grabbing = false;
        hit = new RaycastHit();
        state = "thrown";
    }

    public void Grab()
    {
        if (state == "idle")
        {
            state = "detect";
        }
    }

    public void Stop()
    {
        Animator animator = arms.GetComponent<Animator>();
        animator.SetBool("telegrab", false);
        animator.SetBool("telegrabAttack", false);
        TelegrabVFXOff();
        grabbing = false;
        hit = new RaycastHit();
        state = "idle";
        // hit = null;
    }

    public void Throw()
    {
        Animator animator = arms.GetComponent<Animator>();        
        animator.SetBool("telegrab", false);
        animator.SetBool("telegrabAttack", true);
        state = "throw";
    }
 
    void TelegrabVFXOn()
    {
        MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;
        materials[1] = telegrabMat;
        meshRenderer.materials = materials;
    }

    void TelegrabVFXOff()
    {
        if (grabbing) {
            MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            materials[1] = bucketMat;
            meshRenderer.materials = materials;
        }
    }

}
