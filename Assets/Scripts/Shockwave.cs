﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public LayerMask enemiesMask;
    public LayerMask propsMask;
    private float size = 1f;
    private bool blasting = false;

    void Start()
    {
        size = transform.localScale.x;
    }

    void Update()
    {
        size = transform.localScale.x;
        if (blasting)
        {
            CheckBlastCollision();
        }
    }

    public void CheckBlastCollision()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, size, enemiesMask);
        for (int i = 0; i < enemies.Length; i += 1)
        {
            Vector3 direction = enemies[i].transform.position - transform.position;
            enemies[i].GetComponent<Skeleton>().PushAway(direction);
            enemies[i].GetComponent<Skeleton>().ApplyDamage(1f);
        }

        Collider[] props = Physics.OverlapSphere(transform.position, size, propsMask);
        for (int i = 0; i < props.Length; i += 1)
        {
            Vector3 direction = props[i].transform.position - transform.position;
            if (direction.y < 0f)
            {
                direction.y = 0f;
            }
            Rigidbody rigidBody = props[i].GetComponent<Rigidbody>();
            if (rigidBody)
            {
                rigidBody.AddForce(direction.normalized * 30f);
            }
        }
    }

    public void OnBlastAnimStart()
    {
        blasting = true;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("blast", true);
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;
    }

    public void OnBlastAnimEnd()
    {
        blasting = false;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("blast", false);
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;
    }
}
