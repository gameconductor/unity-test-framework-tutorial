using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public Player player;
    public CharacterController controller;
    public float speed = 1f;
    public float gravity = -9.81f;
    public float mass = 1f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    // public float jumpHeight = 3;
    public float attackDistance = 2.0f;
    public Transform hpBar;
    public OldSword oldSword;
    public float health = 100f;

    Vector3 velocity;
    bool isGrounded = true;

    private Animator animator;
    private bool attacking = false;
    private bool attackColliding = false;
    private bool dead = false;

    float GetDeltaFactor(float delta)
    {
        return delta / (1/16f);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        // health = Random.Range(.0f, 1.0f) * 100f;
        // speed  = speed * Random.Range(0.5f, 1.0f);
    }

    public void Update()
    {
        LookAtPlayer();
        Movement();
        TransitionAnimation();
        CheckAttackCollision();
        UpdateHPBar();
        CheckDeath();
    }

    void UpdateHPBar()
    {
        GameObject hpBar = transform.Find("HPBar").gameObject;
        MeshRenderer rend = hpBar.GetComponent<MeshRenderer>();
        rend.material.SetFloat("HP", health / 100f);
    }

    void CheckDeath()
    {
        if (health <= 0f)
        {
            dead = true;
            animator.SetBool("dead", true);
            // TODO: disable collisions
        }
    }

    void CheckAttackCollision()
    {
        if (oldSword.hit && !attackColliding)
        {
            attackColliding = true;
            OnAttackCollisionEnter();
        } else if (!oldSword.hit)
        {
            attackColliding = false;
            OnAttackCollisionExit();
        }
    }

    void LookAtPlayer() {
        if (dead)
        {
            return;
        }
        transform.LookAt(player.transform.position);
        transform.localRotation = ClampRotation(transform.localRotation, 20f);
    }

    void Movement() {
        if (dead)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        // Move torward player
        if (GetDistanceFromPlayer() >= attackDistance && animationName == "rig|SkeletonRun")
        {
            controller.Move(transform.forward * (speed * 0.1f) * GetDeltaFactor(Time.deltaTime));
        }

        // Gravity
        velocity.y += (mass * 0.01f) * gravity;
        controller.Move(velocity * Time.deltaTime);
    }
    Quaternion ClampRotation(Quaternion rotation, float range) {
        Vector3 rot = rotation.eulerAngles;
        rot.x = 0f;
        return Quaternion.Euler(rot.x, rot.y, rot.z);
    }

    void TransitionAnimation() {
        if (GetDistanceFromPlayer() >= attackDistance)
        {
            animator.SetBool("running", true);
            animator.SetBool("attacking", false);
        } else {
            animator.SetBool("running", false);
            animator.SetBool("attacking", true);
        }
    }

    float GetDistanceFromPlayer() {
        return (player.transform.position - transform.position).magnitude;
    }   

    void OnAttackStart() {
        attacking = true;
    }

    void OnAttackEnd() {
        attacking = false;
    }

   public void OnAttackCollisionEnter() {
        if (attacking)
        {
            player.applyDamage(20f);
        }
    }

    void OnAttackCollisionExit() {
    }

    public void ApplyDamage(float damage) {
        health -= damage;
        UpdateHPBar();
    }

    public void PushAway(Vector3 direction) {
        controller.Move(direction * Time.deltaTime * 5f);
    }
}
