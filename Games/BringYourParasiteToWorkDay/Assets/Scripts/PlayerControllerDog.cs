using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerDog : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float depthSpeed = 3.5f;
    [SerializeField] float blockSpeedMultiplier = 0.4f;
    [SerializeField] float attackMoveMultiplier = 0.22f;
    [SerializeField] float staggerTime = 0.16f;

    [Header("Walkable street")]
    [SerializeField] BoxCollider2D walkArea;
    [SerializeField] float edgePadding = 0.15f;

    [Header("Jump (visual only)")]
    [SerializeField] Transform visual;
    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float jumpDuration = 0.45f;

    [Header("Depth sorting")]
    [SerializeField] int sortingOffset;
    [SerializeField] float sortingScale = 100f;
    [SerializeField] bool spriteFacesRight;

    [Header("Animation")]
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite idleBreath;
    [SerializeField] Sprite[] runSprites = new Sprite[8];
    [SerializeField] float runFrameRate = 12f;
    [SerializeField] float idleFrameRate = 2f;
    [SerializeField] float spriteWidth = 1.45f;

    Rigidbody2D rb;
    Collider2D bodyCollider;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;
    PlayerCombatDog combat;
    DogHealth health;
    Vector2 moveInput;
    float jumpTimeRemaining;
    float staggerTimer;
    int facing = 1;
    bool scripted;
    bool scriptedMoving;
    bool wasRunning;
    float runTime;
    float idleTime;
    Vector2 scriptedPosition;
    float scriptedHop;

    public bool IsStaggered => staggerTimer > 0f;
    public bool IsScripted => scripted;
    public float MoveSpeed => moveSpeed;
    public float DepthSpeed => depthSpeed;

    public int Facing => facing;
    public bool IsAirborne => jumpTimeRemaining > 0f;
    public float VisualHeight => visual != null ? visual.localPosition.y : 0f;
    public Vector2 BodyPosition => rb != null ? rb.position : (Vector2)transform.position;
    public Vector2 BodyExtents => bodyCollider != null ? (Vector2)bodyCollider.bounds.extents : new Vector2(0.5f, 0.5f);
    public Collider2D WalkArea => walkArea;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        combat = GetComponent<PlayerCombatDog>();
        health = GetComponent<DogHealth>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (visual == null)
        {
            Transform child = transform.Find("Visual");
            if (child != null)
                visual = child;
        }

        if (visual != null)
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (visual == null || visual == transform)
            Debug.LogWarning($"{name}: put the SpriteRenderer on a child named Visual. Jumping the root would move the collider.", this);

        LoadDogSprites();
        FitVisual();
        ApplyFacing();
        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
            spriteRenderer.color = Color.white;
        }
        SpriteMaterialFix.ApplyAll();

        float foot = spriteWidth * 0.28f;
        shadowRenderer = GroundShadow.Attach(transform, -foot, spriteWidth * 0.72f, 0.2f);
    }

    public void BeginScripted(Vector2 position)
    {
        scripted = true;
        scriptedMoving = false;
        scriptedPosition = position;
        scriptedHop = 0f;
        jumpTimeRemaining = 0f;
        staggerTimer = 0f;
        if (combat != null)
            combat.StopAttack();
    }

    public void SetScriptedMoving(bool moving)
    {
        scriptedMoving = moving;
    }

    public void SetScriptedPose(Vector2 position, float hop)
    {
        scriptedPosition = position;
        scriptedHop = hop;
    }

    public void EndScripted()
    {
        scripted = false;
        scriptedMoving = false;
        scriptedHop = 0f;
        if (visual != null)
        {
            Vector3 local = visual.localPosition;
            local.y = 0f;
            visual.localPosition = local;
        }
    }

    void Update()
    {
        if (scripted)
        {
            if (visual != null)
            {
                Vector3 local = visual.localPosition;
                local.y = scriptedHop;
                visual.localPosition = local;
            }

            FaceDirection(scriptedPosition.x - rb.position.x);
            UpdateSorting();
            Animate();
            return;
        }

        if (health != null && (health.IsDead || health.InCutscene))
        {
            Animate();
            return;
        }

        if (staggerTimer > 0f)
            staggerTimer -= Time.deltaTime;

        moveInput = ReadMove();
        UpdateFacing();

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            TryJump();

        UpdateJump(Time.deltaTime);
        UpdateSorting();
        Animate();
    }

    void FixedUpdate()
    {
        if (scripted)
        {
            rb.linearVelocity = Vector2.zero;
            rb.MovePosition(scriptedPosition);
            return;
        }

        if (health != null && health.IsDead)
            return;
        if (health != null && health.InCutscene)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speedMultiplier = 1f;
        if (staggerTimer > 0f)
            speedMultiplier = 0f;
        else if (combat != null && combat.IsAttacking)
            speedMultiplier = attackMoveMultiplier;
        else if (combat != null && combat.IsBlocking)
            speedMultiplier = blockSpeedMultiplier;
        Vector2 input = moveInput;
        if (IsAirborne)
            input.y = 0f;

        Vector2 velocity = new Vector2(input.x * moveSpeed, input.y * depthSpeed) * speedMultiplier;
        Vector2 next = ClampToWalkArea(rb.position + velocity * Time.fixedDeltaTime);
        next = StreetObstacle.ResolveMovement(rb.position, next, BodyExtents);
        rb.MovePosition(next);
    }

    Vector2 ReadMove()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float x = 0f;
        float y = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f)
            input.Normalize();
        return input;
    }

    void UpdateFacing()
    {
        if (staggerTimer > 0f)
            return;
        if (combat != null && combat.FacingLocked)
            return;
        if (Mathf.Abs(moveInput.x) < 0.01f || visual == null)
            return;

        FaceDirection(moveInput.x);
    }

    public void FaceDirection(float x)
    {
        if (Mathf.Abs(x) < 0.01f || visual == null)
            return;

        facing = x > 0f ? 1 : -1;
        ApplyFacing();
    }

    void ApplyFacing()
    {
        if (visual == null)
            return;

        Vector3 scale = visual.localScale;
        float sign = spriteFacesRight ? facing : -facing;
        scale.x = Mathf.Abs(scale.x) * sign;
        visual.localScale = scale;
    }

    void TryJump()
    {
        if (IsAirborne || staggerTimer > 0f)
            return;
        if (combat != null && (combat.IsAttacking || combat.IsBlocking))
            return;

        jumpTimeRemaining = jumpDuration;
    }

    void UpdateJump(float deltaTime)
    {
        if (visual == null)
            return;

        Vector3 local = visual.localPosition;
        if (jumpTimeRemaining <= 0f)
        {
            local.y = 0f;
            visual.localPosition = local;
            return;
        }

        jumpTimeRemaining -= deltaTime;
        float t = 1f - Mathf.Clamp01(jumpTimeRemaining / jumpDuration);
        local.y = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        visual.localPosition = local;
    }

    void LoadDogSprites()
    {
        if (runSprites == null || runSprites.Length < 8)
        {
            Sprite[] sized = new Sprite[8];
            if (runSprites != null)
            {
                for (int i = 0; i < runSprites.Length && i < sized.Length; i++)
                    sized[i] = runSprites[i];
            }

            runSprites = sized;
        }

        Sprite loadedIdle = LoadDogSprite(0);
        if (loadedIdle != null)
            idleSprite = loadedIdle;
        Sprite loadedBreath = LoadDogSprite(9);
        if (loadedBreath != null)
            idleBreath = loadedBreath;
        for (int i = 0; i < runSprites.Length; i++)
        {
            Sprite frame = LoadDogSprite(i + 1);
            if (frame != null)
                runSprites[i] = frame;
        }
    }

    static Sprite LoadDogSprite(int index)
    {
        return SpriteLibrary.Load("Assets/Sprites/Dog/dog_sprite_color" + index + ".png");
    }

    void FitVisual()
    {
        if (visual == null || idleSprite == null || idleSprite.rect.width < 1f)
            return;

        float nativeWidth = idleSprite.rect.width / idleSprite.pixelsPerUnit;
        if (nativeWidth < 0.001f)
            return;

        float scale = spriteWidth / nativeWidth;
        float sign = visual.localScale.x < 0f ? -1f : 1f;
        visual.localScale = new Vector3(scale * sign, scale, 1f);
        ApplyFacing();
    }

    void Animate()
    {
        if (spriteRenderer == null || idleSprite == null)
            return;

        bool running = IsRunning();
        if (!running || runSprites == null || runSprites.Length == 0 || runSprites[0] == null)
        {
            wasRunning = false;
            runTime = 0f;
            spriteRenderer.sprite = IdlePose();
            return;
        }

        if (!wasRunning)
            runTime = 0f;
        wasRunning = true;
        runTime += Time.deltaTime;
        int frame = Mathf.FloorToInt(runTime * runFrameRate) % runSprites.Length;
        if (runSprites[frame] != null)
            spriteRenderer.sprite = runSprites[frame];
    }

    Sprite IdlePose()
    {
        if (idleBreath == null || (health != null && health.IsDead))
            return idleSprite;

        idleTime += Time.deltaTime;
        int frame = Mathf.FloorToInt(idleTime * idleFrameRate) % 2;
        return frame == 0 ? idleSprite : idleBreath;
    }

    bool IsRunning()
    {
        if (health != null && (health.IsDead || health.InCutscene))
            return false;
        if (staggerTimer > 0f)
            return false;
        if (scripted)
            return scriptedMoving;
        return moveInput.sqrMagnitude > 0.01f;
    }

    void UpdateSorting()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder = sortingOffset - Mathf.RoundToInt(transform.position.y * sortingScale);
        GroundShadow.Sort(shadowRenderer, transform.position.y, sortingOffset);
    }

    Vector2 ClampToWalkArea(Vector2 position)
    {
        if (walkArea == null)
            return position;

        Bounds bounds = walkArea.bounds;
        Vector2 extents = bodyCollider != null ? (Vector2)bodyCollider.bounds.extents : Vector2.zero;
        float padX = extents.x + edgePadding;
        float padY = extents.y + edgePadding;

        position.x = Mathf.Clamp(position.x, bounds.min.x + padX, bounds.max.x - padX);
        position.y = Mathf.Clamp(position.y, bounds.min.y + padY, bounds.max.y - padY);
        return position;
    }

    public void Stagger(float duration)
    {
        if (staggerTimer > 0f)
            return;

        staggerTimer = Mathf.Max(duration, staggerTime);
        if (combat != null)
            combat.StopAttack();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }
}
