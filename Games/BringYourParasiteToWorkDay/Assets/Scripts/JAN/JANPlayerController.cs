using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JANPlayerController : MonoBehaviour, IJANDamageable
{
    [Header("Move")]
    [SerializeField] float moveSpeed = 7.2f;
    [SerializeField] float sprintSpeed = 15.5f;
    [SerializeField] float crouchSpeed = 3.6f;
    [SerializeField] float groundAccel = 70f;
    [SerializeField] float airAccel = 18f;
    [SerializeField] float stopFriction = 28f;
    [SerializeField] float hopBoost = 1.1f;
    [SerializeField] float maxHopSpeed = 12.5f;
    [SerializeField] float jumpSpeed = 7.4f;
    [SerializeField] float gravity = -24f;

    [Header("Body")]
    [SerializeField] float standHeight = 1.8f;
    [SerializeField] float crouchHeight = 1.05f;
    [SerializeField] float lookSensitivity = 0.09f;

    [Header("Vitals")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float maxArmor = 100f;
    [SerializeField] float maxStamina = 180f;
    [SerializeField] float sprintDrain = 24f;
    [SerializeField] float staminaRegen = 36f;

    CharacterController controller;
    JANWeaponSystem weapons;
    Camera view;
    Vector3 velocity;
    Vector3 shove;
    float health;
    float armor;
    float stamina;
    float staminaDelay;
    bool crouching;
    bool grounded;

    public bool IsDead { get; private set; }
    public float Health => health;
    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float MaxArmor => maxArmor;
    public float Stamina => stamina;
    public float MaxStamina => maxStamina;
    public Camera View => view;
    public JANWeaponSystem Weapons
    {
        get
        {
            if (weapons == null)
                weapons = GetComponent<JANWeaponSystem>();
            return weapons;
        }
    }

    public static JANPlayerController Create(Vector3 feetPosition)
    {
        GameObject body = new GameObject("Janitor");
        body.transform.position = feetPosition;
        CharacterController capsule = body.AddComponent<CharacterController>();
        capsule.height = 1.8f;
        capsule.radius = 0.36f;
        capsule.center = new Vector3(0f, 0.9f, 0f);
        capsule.stepOffset = 0.36f;
        capsule.slopeLimit = 52f;
        JANPlayerController player = body.AddComponent<JANPlayerController>();
        body.AddComponent<JANWeaponSystem>();
        return player;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        weapons = GetComponent<JANWeaponSystem>();
        health = maxHealth;
        armor = 0f;
        stamina = maxStamina;
        AttachCamera();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void AttachCamera()
    {
        view = Camera.main;
        if (view == null)
            return;

        view.transform.SetParent(transform, false);
        view.transform.localPosition = new Vector3(0f, 1.58f, 0f);
        view.transform.localRotation = Quaternion.identity;
        view.fieldOfView = 90f;
        view.nearClipPlane = 0.04f;
        view.clearFlags = CameraClearFlags.SolidColor;
        view.backgroundColor = new Color(0.08f, 0.07f, 0.06f);
        view.allowMSAA = false;
    }

    void Update()
    {
        JANGameManager game = JANGameManager.Instance;
        if (IsDead || (game != null && game.Ended))
            return;

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null || mouse == null)
            return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }

        Look(mouse);
        Crouch(keyboard);
        Move(keyboard);
        CollectPickups();
    }

    void Look(Mouse mouse)
    {
        if (Cursor.lockState != CursorLockMode.Locked || view == null)
            return;

        float yaw = mouse.delta.ReadValue().x * lookSensitivity;
        transform.Rotate(0f, yaw, 0f);
        view.transform.localRotation = Quaternion.identity;
    }

    void Crouch(Keyboard keyboard)
    {
        crouching = keyboard.leftCtrlKey.isPressed || keyboard.cKey.isPressed;
        float height = crouching ? crouchHeight : standHeight;
        controller.height = height;
        controller.center = new Vector3(0f, height * 0.5f, 0f);
        if (view != null)
        {
            float eye = crouching ? 0.82f : 1.58f;
            Vector3 local = view.transform.localPosition;
            local.y = Mathf.Lerp(local.y, eye, 12f * Time.deltaTime);
            view.transform.localPosition = local;
        }
    }

    void Move(Keyboard keyboard)
    {
        float inputX = 0f;
        float inputZ = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) inputX -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) inputX += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) inputZ -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) inputZ += 1f;

        Vector3 wish = transform.right * inputX + transform.forward * inputZ;
        if (wish.sqrMagnitude > 1f)
            wish.Normalize();

        bool wantsSprint = keyboard.leftShiftKey.isPressed && !crouching && wish.sqrMagnitude > 0.01f;
        bool sprinting = wantsSprint && stamina > 0f;
        if (sprinting)
        {
            stamina = Mathf.Max(0f, stamina - sprintDrain * Time.deltaTime);
            staminaDelay = 0.55f;
        }
        else
        {
            staminaDelay -= Time.deltaTime;
            if (staminaDelay <= 0f)
                stamina = Mathf.Min(maxStamina, stamina + staminaRegen * Time.deltaTime);
        }

        float speed = crouching ? crouchSpeed : (sprinting ? sprintSpeed : moveSpeed);
        bool jumpPressed = keyboard.spaceKey.wasPressedThisFrame;

        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        if (controller.isGrounded)
        {
            if (wish.sqrMagnitude < 0.01f)
                horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, stopFriction * Time.deltaTime);
            else
                horizontal = Vector3.MoveTowards(horizontal, wish * speed, groundAccel * Time.deltaTime);

            if (jumpPressed)
            {
                if (horizontal.magnitude > moveSpeed * 0.75f)
                    horizontal += wish * hopBoost;
                if (horizontal.magnitude > maxHopSpeed)
                    horizontal = horizontal.normalized * maxHopSpeed;
                velocity.y = jumpSpeed;
            }
            else if (velocity.y < 0f)
                velocity.y = -2.2f;
        }
        else
        {
            horizontal = Vector3.MoveTowards(horizontal, wish * speed, airAccel * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
        }

        horizontal += shove;
        shove = Vector3.MoveTowards(shove, Vector3.zero, 18f * Time.deltaTime);
        velocity.x = horizontal.x;
        velocity.z = horizontal.z;
        controller.Move(velocity * Time.deltaTime);
        grounded = controller.isGrounded;
    }

    void CollectPickups()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 0.85f, 0.9f, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < hits.Length; i++)
        {
            JANPickup pickup = hits[i].GetComponent<JANPickup>();
            if (pickup != null)
                pickup.Collect(this);
        }
    }

    public void AddShove(Vector3 worldVelocity)
    {
        shove += worldVelocity;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 knockDirection)
    {
        if (IsDead)
            return;

        float absorbed = 0f;
        if (armor > 0f)
        {
            absorbed = Mathf.Min(armor, amount * 0.5f);
            armor -= absorbed;
        }

        health = Mathf.Max(0f, health - (amount - absorbed));
        if (knockDirection.sqrMagnitude > 0.01f)
            AddShove(knockDirection.normalized * 1.1f);
        JANGameManager.Instance?.PlayerHurt();
        if (health <= 0f)
        {
            IsDead = true;
            JANGameManager.Instance?.PlayerDied();
        }
    }

    public bool TryPickup(JANPickup.Kind kind, float amount)
    {
        if (IsDead)
            return false;

        if (kind == JANPickup.Kind.Health)
        {
            if (health >= maxHealth)
                return false;
            health = Mathf.Min(maxHealth, health + amount);
            return true;
        }

        if (kind == JANPickup.Kind.Armor)
        {
            if (armor >= maxArmor)
                return false;
            armor = Mathf.Min(maxArmor, armor + amount);
            return true;
        }

        if (Weapons == null)
            return false;
        return Weapons.AddAmmo(Mathf.RoundToInt(amount));
    }
}
