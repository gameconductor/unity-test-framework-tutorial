using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public event Action<float> OnHPChanged;
    public CharacterController controller;
    public float speed = 1f;
    public float gravity = -9.81f;
    public float mass = 1f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3;
    public Camera cam;
    public float mouseSensitivity = 20f;
    public bool dead = false;
    public GameObject arms;
    public PlayerSword playerSword;
    public Shockwave shockwave;
    public float health;
    public bool activeCam = true;

    Vector3 velocity;
    bool isGrounded = true;
    Animator animator;
    float xRotation = -10f;
    bool attacking = false;
    InputActionAsset playerInput;
    HashSet<Collider> damagedEnemies = new HashSet<Collider>();


    float GetDeltaFactor(float delta)
    {
        return delta / (1/16f);
    }

    void Start()
    {
        OnHPChanged += OnHPTest;
        animator = GetComponent<Animator>();
        if (Application.platform != RuntimePlatform.LinuxEditor &&
            Application.platform != RuntimePlatform.LinuxPlayer) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!activeCam) {
            cam.GetComponent<AudioListener>().enabled = false;
            cam.enabled = false;
        }

        playerInput = GetComponent<PlayerInput>().actions;
    }

    void OnHPTest(float damage) {
        Debug.Log("Damage: " + damage);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Movement();
        PlayerInput();
        CheckAttackCollision();
        if (health <= 0.0f)
        {
            dead = true;
            animator.enabled = true;
            arms.SetActive(false);
        }

        InputAction secondaryAttack = playerInput.FindActionMap("Player").FindAction("SecondaryAttack");
    }

    void Movement()
    {
        if (dead)
        {
            return;
        }

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        InputAction moveAction = playerInput.FindActionMap("Player").FindAction("Move");
        Vector2 direction = moveAction.ReadValue<Vector2>();

        Vector3 movement = transform.right * direction.x + transform.forward * direction.y;
        controller.Move(movement * (speed * 0.1f) * GetDeltaFactor(Time.deltaTime));

        velocity.y += (mass * 0.01f) * gravity;
        controller.Move(velocity * Time.deltaTime);
    }

    void PlayerInput() {
        Vector2 lookDelta = Mouse.current.delta.ReadValue();
        MouseLook(lookDelta.x, lookDelta.y);

        InputAction primaryAction   = playerInput.FindActionMap("Player").FindAction("PrimaryAttack");
        InputAction secondaryAction = playerInput.FindActionMap("Player").FindAction("SecondaryAttack");
        InputAction tertiaryAction  = playerInput.FindActionMap("Player").FindAction("TertiaryAttack");
        InputAction jumpAction      = playerInput.FindActionMap("Player").FindAction("Jump");
        InputAction resetAction     = playerInput.FindActionMap("Player").FindAction("Reset");

        bool primaryAttack   = primaryAction.phase == InputActionPhase.Started;
        bool secondaryAttack = secondaryAction.phase == InputActionPhase.Started;
        bool tertiaryAttack  = tertiaryAction.phase == InputActionPhase.Started;
        bool jump            = jumpAction.phase == InputActionPhase.Started;
        bool reset           = resetAction.phase == InputActionPhase.Started;

        if (primaryAttack) {
            if (secondaryAttack) { // if true, we're using telekinesis
                Animator animator = arms.GetComponent<Animator>();        
                animator.SetBool("telegrabAttack", true);
            } else { // else it's a sword attack
                OnAttackAnimStart();
            }
        }

        if (secondaryAttack) {
            Debug.Log("Grab");
            GetComponent<Telegrab>().Grab();
        } else {
            GetComponent<Telegrab>().Stop();
        }

        if (tertiaryAttack)
        {
            OnShockwaveAnimStart();
        } else {
            OnShockwaveAnimStop();
        }

        if (jump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (reset)
        {
            SceneManager.LoadScene("Scenes/Main");
        }
    }

    void MouseLook(float x, float y) {
        float deltaFactor = GetDeltaFactor(Time.deltaTime);
        float mouseX = x * (mouseSensitivity / 100f) * deltaFactor;
        float mouseY = y * (mouseSensitivity / 100f) * deltaFactor;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70, 70f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void CheckAttackCollision()
    {
        if (playerSword.hits.Length > 0 && attacking)
        {
            OnAttackCollision(playerSword.hits);
        }
    }

    void OnAttackCollision(Collider[] enemies)
    {
        for (int i = 0; i < enemies.Length; i += 1)
        {
            if (!damagedEnemies.Contains(enemies[i])) {
                enemies[i].GetComponent<Skeleton>().ApplyDamage(20f);
                Debug.Log(enemies[i].name + " receives 20 damage");
                damagedEnemies.Add(enemies[i]);
            }
        }
    }

    void OnAttackAnimStart()
    {
        attacking = true;
        Animator animator = arms.GetComponent<Animator>();
        animator.SetBool("attacking", true);
        damagedEnemies = new HashSet<Collider>();
    }

    public void OnAttackAnimEnd()
    {
        attacking = false;
        Animator animator = arms.GetComponent<Animator>();
        animator.SetBool("attacking", false);
    }

    void OnShockwaveAnimStart()
    {
        Animator animator = arms.GetComponent<Animator>();
        animator.SetBool("shockwave", true);
    }

    public void OnShockwaveAnimStop()
    {
        Animator animator = arms.GetComponent<Animator>();
        animator.SetBool("shockwave", false);
    }

    public void OnShockwaveAnimBlast()
    {
        shockwave.OnBlastAnimStart();
    }

    public void applyDamage(float damage)
    {
        health -= damage;
        OnHPChanged?.Invoke(-damage);
    }
}
