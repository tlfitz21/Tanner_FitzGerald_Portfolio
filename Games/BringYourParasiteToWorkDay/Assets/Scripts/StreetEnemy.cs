using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttackDummy))]
public class StreetEnemy : MonoBehaviour
{
    [SerializeField] float speedRatio = 1f;
    [SerializeField] float attackRangeY = 1.15f;
    [SerializeField] float heavyChance = 0.35f;
    [SerializeField] float lightTelegraphTime = 0.2f;
    [SerializeField] float heavyTelegraphTime = 0.48f;
    [SerializeField] float lightTelegraphPulses = 2.5f;
    [SerializeField] float heavyTelegraphPulses = 6f;
    [SerializeField] float lightActive = 0.18f;
    [SerializeField] float heavyActive = 0.28f;
    [SerializeField] float lightReach = 1.15f;
    [SerializeField] float heavyReach = 1.75f;
    [SerializeField] float lightOverlap = 0.35f;
    [SerializeField] float heavyOverlap = 0.4f;
    [SerializeField] float lightHitboxHeight = 0.9f;
    [SerializeField] float heavyHitboxHeight = 1.05f;
    [SerializeField] float lightCooldown = 0.85f;
    [SerializeField] float heavyCooldown = 1.15f;
    [SerializeField] float staggerTime = 0.7f;
    [SerializeField] float knockdownTime = 1.25f;
    [SerializeField] float launchSpeed = 8f;
    [SerializeField] float launchTime = 0.4f;
    [SerializeField] float launchHop = 0.4f;
    [SerializeField] Color lightTint = new Color(1f, 0.55f, 0.2f, 1f);
    [SerializeField] Color heavyTint = new Color(1f, 0.35f, 0.1f, 1f);
    [SerializeField] Color lightTelegraphColor = new Color(1f, 0.86f, 0.25f, 0.45f);
    [SerializeField] Color heavyTelegraphColor = new Color(1f, 0.22f, 0.08f, 0.7f);

    enum Motion
    {
        Free,
        Stagger,
        Launch,
        Down
    }

    Rigidbody2D body;
    Collider2D bodyCollider;
    AttackDummy dummy;
    Transform visual;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;
    Color baseColor;
    Transform player;
    DogHealth dogHealth;
    Collider2D walkArea;
    bool activated;
    bool swinging;
    float cooldown;
    float moveSpeed = 6f;
    float depthSpeed = 3.5f;
    float laneOffset;
    float blockWait;
    bool gateGuard;
    bool waitForHulk;
    PlayerControllerDog dogMove;
    PlayerCombatDog dogCombat;

    static readonly List<StreetEnemy> roster = new List<StreetEnemy>();
    static int laneSeed;
    Motion motion = Motion.Free;
    float motionTimer;
    float launchVelocity;
    int knockDirection = 1;
    int facing = -1;
    float hopHeight;
    GameObject chargeVisual;
    static Sprite[] civilianFrames;
    static int variantCursor;
    int variant = -1;
    float animTime;
    Vector2 lastAnimPosition;
    const float FootY = -0.42f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        dummy = GetComponent<AttackDummy>();
        body.gravityScale = 0f;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        laneOffset = (laneSeed % 3 - 1) * 0.4f;
        laneSeed++;

        HidePlaceholder();

        Transform existingVisual = transform.Find("Visual");
        if (existingVisual != null)
        {
            visual = existingVisual;
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                baseColor = spriteRenderer.color;
            shadowRenderer = GroundShadow.Attach(transform, FootY, 0.7f, 0.18f);
            AssignCivilian();
            return;
        }

        GameObject visualObject = new GameObject("Visual");
        visual = visualObject.transform;
        visual.SetParent(transform, false);
        spriteRenderer = visualObject.AddComponent<SpriteRenderer>();

        shadowRenderer = GroundShadow.Attach(transform, FootY, 0.7f, 0.18f);
        AssignCivilian();
    }

    void Start()
    {
        if (dummy != null)
            dummy.SetMaxHealth(150f);
        cooldown = Random.Range(0.05f, 0.3f);
    }

    public void SetGateGuard()
    {
        gateGuard = true;
    }

    public void WaitForHulk()
    {
        waitForHulk = true;
    }

    public bool IsGateGuard => gateGuard;

    public bool IsCleared => dummy == null || dummy.IsDefeated;

    void Update()
    {
        BindPlayer();
        UpdateSorting();
        TickMotion();
        AnimateCivilian();

        if (dummy != null && dummy.IsDefeated)
            return;

        if (!activated)
        {
            TryActivate();
            return;
        }

        if (dogHealth != null && dogHealth.IsDead)
            return;
        if (motion != Motion.Free || swinging || player == null)
            return;

        cooldown -= Time.deltaTime;
        Vector2 toPlayer = (Vector2)player.position - body.position;
        Face(toPlayer.x);

        bool inReach = Mathf.Abs(toPlayer.x) <= AttackReach(false) && Mathf.Abs(toPlayer.y) <= attackRangeY;
        bool playerBlocking = dogCombat != null && dogCombat.IsBlocking;
        bool playerOpen = dogMove != null && (dogMove.IsStaggered || (dogCombat != null && dogCombat.IsAttacking && !PlayerFacingMe()));
        blockWait = inReach && playerBlocking ? blockWait + Time.deltaTime : 0f;

        if (inReach && cooldown <= 0f && !(playerBlocking && blockWait < 0.35f))
        {
            bool heavy = playerOpen || blockWait >= 0.7f || Random.value < heavyChance;
            StartCoroutine(Attack(heavy));
            return;
        }

        float side = Mathf.Sign(body.position.x - player.position.x);
        if (side == 0f)
            side = -1f;
        float holdX = player.position.x + side * (AttackReach(false) * 0.45f);
        float holdY = player.position.y + laneOffset;
        if (PlayerFacingMe() && dogCombat != null && dogCombat.IsAttacking && !inReach)
            holdY = player.position.y + Mathf.Sign(laneOffset == 0f ? side : laneOffset) * 1.35f;

        MoveToward((Vector2)player.position + Separation() + new Vector2(holdX - player.position.x, holdY - player.position.y));
    }

    public void SetColor(Color color)
    {
        if (variant >= 0)
            return;

        baseColor = color;
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    public void React(EnemyHitEffect effect, int direction)
    {
        if (effect == EnemyHitEffect.None || (dummy != null && dummy.IsDefeated))
            return;

        knockDirection = direction >= 0 ? 1 : -1;
        swinging = false;
        ClearCharge();
        StopAllCoroutines();
        ApplyTint(Color.white);

        if (effect == EnemyHitEffect.Launch)
        {
            motion = Motion.Launch;
            motionTimer = 0.28f;
            hopHeight = launchHop;
            launchVelocity = knockDirection * launchSpeed;
            if (visual != null && variant < 0)
                visual.localRotation = Quaternion.Euler(0f, 0f, -75f * knockDirection);
            return;
        }

        if (effect == EnemyHitEffect.Knockdown)
        {
            motion = Motion.Down;
            motionTimer = knockdownTime;
            if (visual != null && variant < 0)
                visual.localRotation = Quaternion.Euler(0f, 0f, -75f * knockDirection);
            return;
        }

        motion = Motion.Stagger;
        motionTimer = staggerTime;
    }

    void TickMotion()
    {
        if (motion == Motion.Free)
            return;

        motionTimer -= Time.deltaTime;
        if (motion == Motion.Launch)
        {
            float progress = launchTime <= 0f ? 1f : 1f - Mathf.Clamp01(motionTimer / launchTime);
            body.MovePosition(ClampToStreet(body.position + Vector2.right * launchVelocity * Time.deltaTime));
            if (visual != null)
                visual.localPosition = new Vector3(0f, Mathf.Sin(progress * Mathf.PI) * hopHeight, 0f);

            if (motionTimer > 0f)
                return;

            motion = Motion.Down;
            motionTimer = knockdownTime * 0.65f;
            if (visual != null && variant < 0)
            {
                visual.localPosition = Vector3.zero;
                visual.localRotation = Quaternion.Euler(0f, 0f, -75f * knockDirection);
            }
            return;
        }

        if (motionTimer > 0f)
            return;

        if (dummy != null && dummy.IsDefeated)
            return;

        motion = Motion.Free;
        cooldown = 0.35f;
        if (visual != null)
        {
            visual.localRotation = Quaternion.identity;
            visual.localPosition = Vector3.zero;
        }
        ApplyTint(Color.white);
    }

    IEnumerator Attack(bool heavy)
    {
        swinging = true;
        if (variant < 0)
            ApplyTint(heavy ? heavyTint : lightTint);

        float telegraphTime = heavy ? heavyTelegraphTime : lightTelegraphTime;
        float pulses = heavy ? heavyTelegraphPulses : lightTelegraphPulses;
        float active = heavy ? heavyActive : lightActive;
        Color telegraphColor = heavy ? heavyTelegraphColor : lightTelegraphColor;
        Vector2 center = HitboxCenter(heavy, out Vector2 size);
        Vector2 warningSize = heavy ? size * 1.2f : size;

        if (telegraphTime > 0f)
            yield return StartCoroutine(ShowTelegraph(heavy, center, warningSize, telegraphTime, pulses, telegraphColor));

        if (motion != Motion.Free || dogHealth == null || dogHealth.IsDead)
        {
            EndSwing();
            yield break;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        for (int i = 0; i < hits.Length; i++)
        {
            DogHealth health = hits[i].GetComponent<DogHealth>();
            if (health == null)
                continue;

            health.ReceiveAttack(heavy);
            break;
        }

        yield return StartCoroutine(ShowHitbox(heavy, center, size, active));
        EndSwing(heavy);
    }

    IEnumerator ShowTelegraph(bool heavy, Vector2 center, Vector2 size, float duration, float pulses, Color color)
    {
        ClearCharge();
        GameObject marker = new GameObject(heavy ? "Heavy Telegraph" : "Light Telegraph");
        marker.transform.position = center;
        chargeVisual = marker;
        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDebugSprite();
        renderer.sortingOrder = 29000;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (motion != Motion.Free)
            {
                Destroy(marker);
                yield break;
            }

            float pulse = Mathf.PingPong((elapsed / duration) * pulses, 1f);
            float scale = 0.72f + 0.28f * pulse;
            marker.transform.localScale = new Vector3(size.x * scale, size.y * scale, 1f);
            Color drawn = color;
            drawn.a = color.a * (0.35f + 0.65f * pulse);
            renderer.color = drawn;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (chargeVisual == marker)
            chargeVisual = null;
        Destroy(marker);
    }

    IEnumerator ShowHitbox(bool heavy, Vector2 center, Vector2 size, float lifetime)
    {
        ClearCharge();
        GameObject box = new GameObject(heavy ? "Heavy Hitbox" : "Light Hitbox");
        box.transform.position = center;
        chargeVisual = box;
        box.transform.localScale = new Vector3(size.x, size.y, 1f);
        SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDebugSprite();
        renderer.color = heavy
            ? new Color(1f, 0.35f, 0.1f, 0.5f)
            : new Color(1f, 0.85f, 0.2f, 0.45f);
        renderer.sortingOrder = 30000;

        if (lifetime > 0f)
            yield return new WaitForSeconds(lifetime);

        if (chargeVisual == box)
            chargeVisual = null;
        Destroy(box);
    }

    void ClearCharge()
    {
        if (chargeVisual == null)
            return;

        Destroy(chargeVisual);
        chargeVisual = null;
    }

    void OnEnable()
    {
        if (!roster.Contains(this))
            roster.Add(this);
    }

    void OnDisable()
    {
        roster.Remove(this);
        ClearCharge();
    }

    void MoveToward(Vector2 destination)
    {
        Vector2 aim = destination - body.position;
        if (aim.sqrMagnitude < 0.04f)
            return;

        if (aim.sqrMagnitude > 1f)
            aim.Normalize();

        Vector2 velocity = new Vector2(aim.x * moveSpeed, aim.y * depthSpeed);
        body.MovePosition(ClampToStreet(body.position + velocity * Time.deltaTime));
    }

    Vector2 Separation()
    {
        Vector2 push = Vector2.zero;
        for (int i = 0; i < roster.Count; i++)
        {
            StreetEnemy other = roster[i];
            if (other == this || other.dummy != null && other.dummy.IsDefeated)
                continue;

            Vector2 away = body.position - other.body.position;
            float distance = away.magnitude;
            if (distance < 0.001f || distance > 1.15f)
                continue;

            push += away / distance * (1.15f - distance);
        }

        return push;
    }

    bool FriendAlreadySwinging()
    {
        for (int i = 0; i < roster.Count; i++)
        {
            StreetEnemy other = roster[i];
            if (other == this || !other.swinging)
                continue;
            if (Vector2.Distance(body.position, other.body.position) < 3.5f)
                return true;
        }

        return false;
    }

    bool FriendBetweenMeAndPlayer()
    {
        if (player == null)
            return false;

        float mySide = body.position.x - player.position.x;
        for (int i = 0; i < roster.Count; i++)
        {
            StreetEnemy other = roster[i];
            if (other == this || other.dummy != null && other.dummy.IsDefeated)
                continue;

            float otherSide = other.body.position.x - player.position.x;
            bool between = Mathf.Abs(otherSide) < Mathf.Abs(mySide) && Mathf.Sign(otherSide) == Mathf.Sign(mySide);
            bool sameLane = Mathf.Abs(other.body.position.y - body.position.y) < 0.45f;
            if (between && sameLane)
                return true;
        }

        return false;
    }

    bool PlayerFacingMe()
    {
        if (dogMove == null || player == null)
            return false;

        float dx = body.position.x - player.position.x;
        return dogMove.Facing > 0 ? dx > 0.2f : dx < -0.2f;
    }

    void EndSwing(bool heavy = false)
    {
        swinging = false;
        cooldown = heavy ? heavyCooldown : lightCooldown;
        if (motion == Motion.Free)
            ApplyTint(Color.white);
    }

    float AttackReach(bool heavy)
    {
        float halfWidth = bodyCollider != null ? Mathf.Max(0.05f, bodyCollider.bounds.extents.x) : 0.4f;
        return halfWidth + (heavy ? heavyReach : lightReach);
    }

    Vector2 HitboxCenter(bool heavy, out Vector2 size)
    {
        float halfWidth = bodyCollider != null ? Mathf.Max(0.05f, bodyCollider.bounds.extents.x) : 0.4f;
        float overlap = Mathf.Clamp(heavy ? heavyOverlap : lightOverlap, 0f, halfWidth);
        float reach = Mathf.Max(0.05f, heavy ? heavyReach : lightReach);
        float inner = halfWidth - overlap;
        float outer = halfWidth + reach;
        size = new Vector2(outer - inner, heavy ? heavyHitboxHeight : lightHitboxHeight);
        float mid = (inner + outer) * 0.5f;
        return body.position + new Vector2(facing * mid, 0f);
    }

    void Face(float direction)
    {
        if (Mathf.Abs(direction) < 0.05f || visual == null)
            return;

        facing = direction > 0f ? 1 : -1;
        if (variant >= 0)
            return;

        Vector3 scale = visual.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        visual.localScale = scale;
    }

    void HidePlaceholder()
    {
        SpriteRenderer placeholder = GetComponent<SpriteRenderer>();
        if (placeholder == null)
            return;

        placeholder.enabled = false;
        placeholder.sprite = null;
        Destroy(placeholder);
    }

    void AssignCivilian()
    {
        if (CivilianSprite(0) == null || spriteRenderer == null || visual == null)
            return;

        variant = variantCursor % 4;
        variantCursor++;
        baseColor = Color.white;
        spriteRenderer.color = Color.white;
        lastAnimPosition = transform.position;
        ShowCivilian(variant * 3);
    }

    void AnimateCivilian()
    {
        if (variant < 0 || spriteRenderer == null)
            return;

        if (dummy != null && dummy.IsDefeated)
        {
            if (visual != null)
                visual.localRotation = Quaternion.identity;
            spriteRenderer.color = Color.white;
            ShowCivilian(variant * 3 + 2);
            return;
        }

        Vector2 position = transform.position;
        bool moving = (position - lastAnimPosition).sqrMagnitude > 0.00002f;
        lastAnimPosition = position;
        animTime += Time.deltaTime * (moving ? 1f : 0.4f);
        int step = Mathf.FloorToInt(animTime * 8f) % 2;
        ShowCivilian(variant * 3 + step);
    }

    void ShowCivilian(int index)
    {
        Sprite sprite = CivilianSprite(index);
        Sprite stand = CivilianSprite(variant * 3);
        if (sprite == null || stand == null || visual == null)
            return;

        float parentX = Mathf.Abs(transform.lossyScale.x);
        float parentY = Mathf.Abs(transform.lossyScale.y);
        if (parentX < 0.01f)
            parentX = 1f;
        if (parentY < 0.01f)
            parentY = 1f;

        float world = 1.55f / stand.bounds.size.y;
        float scaleX = world / parentX;
        float scaleY = world / parentY;
        visual.localScale = new Vector3(scaleX * -facing, scaleY, 1f);
        spriteRenderer.sprite = sprite;
        if (motion == Motion.Launch)
            return;

        Vector3 place = visual.localPosition;
        place.y = FootY - sprite.bounds.min.y * scaleY;
        visual.localPosition = place;
    }

    static Sprite CivilianSprite(int index)
    {
        if (civilianFrames == null)
            civilianFrames = new Sprite[12];
        if (index < 0 || index >= civilianFrames.Length)
            return null;
        if (civilianFrames[index] != null)
            return civilianFrames[index];

        Sprite loaded = SpriteLibrary.Load("Assets/Sprites/Civilians/civilian_sprite_" + index.ToString("00") + ".png");
        if (loaded != null)
        {
            loaded.texture.filterMode = FilterMode.Point;
            civilianFrames[index] = loaded;
        }
        return civilianFrames[index];
    }

    void TryActivate()
    {
        if (waitForHulk)
        {
            HulkEnemy hulk = FindAnyObjectByType<HulkEnemy>();
            if (hulk != null && !hulk.IsOutOfFight)
                return;
            waitForHulk = false;
        }

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 view = cam.WorldToViewportPoint(transform.position);
        if (view.z < 0f || view.x < -0.02f || view.x > 1.02f || view.y < 0f || view.y > 1f)
            return;

        activated = true;
    }

    void BindPlayer()
    {
        if (player != null)
            return;

        dogHealth = FindAnyObjectByType<DogHealth>();
        if (dogHealth == null)
            return;

        player = dogHealth.transform;
        dogMove = dogHealth.GetComponent<PlayerControllerDog>();
        dogCombat = dogHealth.GetComponent<PlayerCombatDog>();
        if (dogMove != null)
        {
            walkArea = dogMove.WalkArea;
            moveSpeed = dogMove.MoveSpeed * speedRatio;
            depthSpeed = dogMove.DepthSpeed * speedRatio;
        }
    }

    void UpdateSorting()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100f);
        GroundShadow.Sort(shadowRenderer, transform.position.y, 0);
    }

    void ApplyTint(Color tint)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = new Color(
            baseColor.r * tint.r,
            baseColor.g * tint.g,
            baseColor.b * tint.b,
            baseColor.a);
    }

    Vector2 ClampToStreet(Vector2 position)
    {
        if (walkArea == null)
            return position;

        Bounds bounds = walkArea.bounds;
        position.x = Mathf.Clamp(position.x, bounds.min.x + 0.5f, bounds.max.x - 0.5f);
        position.y = Mathf.Clamp(position.y, bounds.min.y + 0.5f, bounds.max.y - 0.5f);
        return position;
    }

    static Sprite debugSprite;

    static Sprite GetDebugSprite()
    {
        if (debugSprite != null)
            return debugSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        debugSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return debugSprite;
    }
}
