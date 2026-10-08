using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JANEnemyAI : MonoBehaviour, IJANDamageable
{
    public enum Kind
    {
        Melee,
        Ranged
    }

    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hurt,
        Death
    }

    [SerializeField] Kind kind = Kind.Melee;
    [SerializeField] float maxHealth = 70f;
    [SerializeField] float moveSpeed = 5.1f;
    [SerializeField] float attackRange = 1.55f;
    [SerializeField] float attackDamage = 16f;
    [SerializeField] float attackCooldown = 0.9f;
    [SerializeField] float sightRange = 22f;

    CharacterController controller;
    JANBillboardSprite billboard;
    Renderer bodyRenderer;
    Renderer headRenderer;
    Color bodyColor;
    Color headColor;
    float flashUntil;
    Vector3 home;
    Vector3 velocity;
    Vector3 shove;
    float health;
    float nextAttack;
    float hurtTimer;
    float strafeTimer;
    float strafe = 1f;
    const float SwingFrameTime = 0.09f;
    float walkTime;
    float swingTime;
    bool alert;
    bool swinging;
    bool swingHit;
    bool falling;
    float fall;
    Quaternion fallFrom;
    int shownFrame;
    Material spriteMat;
    static Sprite[] meleeFrames;
    static Sprite[] gunnerFrames;
    State state = State.Idle;

    public bool IsDead { get; private set; }
    public Kind AgentKind => kind;
    public State CurrentState => state;

    public static JANEnemyAI Create(Kind agentKind, Vector3 feet)
    {
        bool melee = agentKind == Kind.Melee;
        GameObject body = new GameObject(melee ? "SS Guard" : "SS Agent");
        body.transform.position = feet;
        CharacterController capsule = body.AddComponent<CharacterController>();
        capsule.height = melee ? 1.85f : 1.75f;
        capsule.radius = 0.38f;
        capsule.center = new Vector3(0f, capsule.height * 0.5f, 0f);
        capsule.stepOffset = 0.35f;
        capsule.slopeLimit = 50f;

        JANEnemyAI agent = body.AddComponent<JANEnemyAI>();
        agent.kind = agentKind;
        agent.maxHealth = melee ? 70f : 48f;
        agent.moveSpeed = melee ? 5.3f : 4.2f;
        agent.attackRange = melee ? 1.6f : 16f;
        agent.attackDamage = melee ? 16f : 8f;
        agent.attackCooldown = melee ? 0.85f : 1.15f;

        Color color = melee
            ? new Color(0.22f, 0.42f, 0.95f, 1f)
            : new Color(0.82f, 0.84f, 0.9f, 1f);
        bool sheet = melee ? agent.MeleeSprite(0) != null : agent.GunnerSprite(0) != null;
        GameObject visual = JANArt.Quad("Billboard", sheet ? Color.white : color, body.transform);
        visual.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        visual.transform.localScale = new Vector3(1.15f, 2.05f, 1f);
        agent.billboard = visual.AddComponent<JANBillboardSprite>();
        agent.bodyRenderer = visual.GetComponent<Renderer>();
        agent.bodyColor = color;
        if (sheet)
            agent.Present(0);
        else
        {
            Color face = new Color(0.93f, 0.75f, 0.58f, 1f);
            GameObject head = JANArt.Quad("Head", face, visual.transform);
            head.transform.localPosition = new Vector3(0f, 0.32f, -0.02f);
            head.transform.localScale = new Vector3(0.34f, 0.16f, 1f);
            agent.headRenderer = head.GetComponent<Renderer>();
            agent.headColor = face;
        }
        agent.home = feet;
        agent.health = agent.maxHealth;
        JANGameManager.Instance?.RegisterAgent();
        return agent;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (health <= 0f)
            health = maxHealth;
        home = transform.position;
    }

    void Update()
    {
        if (IsDead)
        {
            TickFall();
            return;
        }

        if (flashUntil > 0f && Time.time >= flashUntil)
            EndFlash();

        JANGameManager game = JANGameManager.Instance;
        JANPlayerController player = game != null ? game.Player : null;
        if (player == null || player.IsDead || (game != null && game.Ended))
        {
            state = State.Idle;
            ApplyGravity();
            return;
        }

        if (Time.time < game.PeaceUntil)
        {
            state = State.Idle;
            ApplyGravity();
            return;
        }

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;
        float notice = kind == Kind.Melee ? 22f : 11f;
        if (!alert && distance < notice && HasLineOfSight(player))
        {
            alert = true;
            nextAttack = Time.time + 1.1f;
        }

        if (hurtTimer > 0f)
        {
            state = State.Hurt;
            hurtTimer -= Time.deltaTime;
            if (swinging)
                AdvanceSwing(player);
            else
                ApplyGravity();
            return;
        }

        if (!alert)
        {
            state = State.Patrol;
            Patrol();
            return;
        }

        bool sight = HasLineOfSight(player);
        if (kind == Kind.Melee)
            TickMelee(player, toPlayer, distance);
        else
            TickRanged(player, toPlayer, distance, sight);
    }

    void TickMelee(JANPlayerController player, Vector3 toPlayer, float distance)
    {
        if (swinging)
        {
            state = State.Attack;
            Face(toPlayer);
            AdvanceSwing(player);
            return;
        }

        if (distance <= attackRange + 0.85f && HasLineOfSight(player) && Time.time >= nextAttack)
        {
            BeginSwing();
            state = State.Attack;
            Face(toPlayer);
            ApplyGravity();
            return;
        }

        if (distance <= attackRange && HasLineOfSight(player))
        {
            state = State.Attack;
            Face(toPlayer);
            ApplyGravity();
            return;
        }

        state = State.Chase;
        MoveToward(toPlayer);
    }

    void TickRanged(JANPlayerController player, Vector3 toPlayer, float distance, bool sight)
    {
        if (sight && distance < attackRange && distance > 3.2f)
        {
            state = State.Attack;
            Face(toPlayer);
            if (Time.time >= nextAttack)
                StartCoroutine(Burst(player));
            ApplyGravity();
            return;
        }

        if (!sight)
        {
            strafeTimer -= Time.deltaTime;
            if (strafeTimer <= 0f)
            {
                strafe = Random.value < 0.5f ? -1f : 1f;
                strafeTimer = Random.Range(0.45f, 0.9f);
            }

            Vector3 side = Vector3.Cross(Vector3.up, toPlayer.sqrMagnitude > 0.01f ? toPlayer.normalized : transform.forward);
            state = State.Chase;
            MoveToward(toPlayer.normalized + side * strafe);
            return;
        }

        state = State.Chase;
        MoveToward(toPlayer);
    }

    IEnumerator Burst(JANPlayerController player)
    {
        nextAttack = Time.time + attackCooldown;
        for (int i = 0; i < 3; i++)
        {
            if (IsDead || player == null || player.IsDead || player.View == null)
                yield break;

            Vector3 origin = transform.position + Vector3.up * 1.35f;
            Vector3 aim = (player.View.transform.position - origin).normalized;
            aim = Quaternion.Euler(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f) * aim;
            JANWeaponSystem.SpawnPellet(origin, aim, attackDamage, 16f, false, new Color(1f, 0.85f, 0.25f));
            yield return new WaitForSeconds(0.12f);
        }
    }

    void Patrol()
    {
        Vector3 target = home + transform.right * Mathf.Sin(Time.time * 0.7f + home.x) * 1.6f;
        MoveToward(target - transform.position);
    }

    void MoveToward(Vector3 direction)
    {
        direction.y = 0f;
        Collider[] nearby = Physics.OverlapSphere(transform.position + Vector3.up, 0.9f, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < nearby.Length; i++)
        {
            JANEnemyAI other = nearby[i].GetComponentInParent<JANEnemyAI>();
            if (other == null || other == this)
                continue;

            Vector3 away = transform.position - other.transform.position;
            away.y = 0f;
            if (away.sqrMagnitude > 0.0001f)
                direction += away.normalized * 1.1f;
        }

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            Face(direction);
        }

        Vector3 motion = direction * moveSpeed + shove;
        shove = Vector3.MoveTowards(shove, Vector3.zero, 16f * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += -24f * Time.deltaTime;
        motion.y = velocity.y;
        controller.Move(motion * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += -24f * Time.deltaTime;
        controller.Move(new Vector3(shove.x, velocity.y, shove.z) * Time.deltaTime);
        shove = Vector3.MoveTowards(shove, Vector3.zero, 16f * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (IsDead || FrameSprite(0) == null)
            return;
        if (flashUntil > Time.time)
            return;

        if (kind == Kind.Melee && swinging)
        {
            Present(SwingSpriteIndex());
            return;
        }

        bool moving = state == State.Patrol || state == State.Chase;
        if (!moving)
        {
            Present(0);
            return;
        }

        walkTime += Time.deltaTime;
        int walkFrames = kind == Kind.Melee ? 2 : 3;
        Present(Mathf.FloorToInt(walkTime * 8f) % walkFrames);
    }

    void BeginSwing()
    {
        swinging = true;
        swingHit = false;
        swingTime = 0f;
        nextAttack = Time.time + attackCooldown;
    }

    int SwingSpriteIndex()
    {
        int frame = Mathf.Clamp(Mathf.FloorToInt(swingTime / SwingFrameTime), 0, 4);
        return frame + 1;
    }

    void AdvanceSwing(JANPlayerController player)
    {
        swingTime += Time.deltaTime;
        int frame = Mathf.Clamp(Mathf.FloorToInt(swingTime / SwingFrameTime), 0, 4);

        if (!swingHit && frame >= 3 && player != null && !player.IsDead)
        {
            swingHit = true;
            Vector3 toPlayer = player.transform.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.magnitude <= attackRange + 0.35f)
                player.TakeDamage(attackDamage, player.transform.position + Vector3.up, toPlayer.normalized);
        }

        if (swingTime < SwingFrameTime * 5f)
        {
            ApplyGravity();
            return;
        }

        swinging = false;
        ApplyGravity();
    }

    void Present(int index)
    {
        shownFrame = index;
        if (flashUntil > Time.time)
            return;

        ApplyFrame(index);
    }

    void ApplyFrame(int index)
    {
        Sprite sprite = FrameSprite(index);
        Sprite stand = FrameSprite(0);
        if (sprite == null || stand == null || bodyRenderer == null)
            return;

        if (spriteMat == null)
        {
            spriteMat = JANArt.SpriteMaterial();
            bodyRenderer.material = spriteMat;
        }

        JANArt.PaintSprite(spriteMat, sprite);

        float unit = stand.pixelsPerUnit;
        float standHeight = stand.rect.height / unit;
        float fit = standHeight > 0.001f ? 2.05f / standHeight : 1f;
        float worldW = sprite.rect.width / sprite.pixelsPerUnit * fit;
        float worldH = sprite.rect.height / sprite.pixelsPerUnit * fit;
        float bodyW = stand.rect.width / unit * fit;
        Transform view = bodyRenderer.transform;
        view.localScale = new Vector3(worldW, worldH, 1f);
        view.localPosition = new Vector3((worldW - bodyW) * 0.5f, worldH * 0.5f, 0f);
    }

    Sprite MeleeSprite(int index)
    {
        if (meleeFrames == null)
            meleeFrames = new Sprite[7];
        if (index < 0 || index >= meleeFrames.Length)
            return null;
        if (meleeFrames[index] != null)
            return meleeFrames[index];

        meleeFrames[index] = SpriteLibrary.Load("Assets/Sprites/JAN/AgentM/agent_m_sprite_" + index + ".png");
        return meleeFrames[index];
    }

    Sprite GunnerSprite(int index)
    {
        if (gunnerFrames == null)
            gunnerFrames = new Sprite[4];
        if (index < 0 || index >= gunnerFrames.Length)
            return null;
        if (gunnerFrames[index] != null)
            return gunnerFrames[index];

        gunnerFrames[index] = SpriteLibrary.Load("Assets/Sprites/JAN/AgentG/agent_g_sprite" + index + ".png");
        return gunnerFrames[index];
    }

    Sprite FrameSprite(int index)
    {
        return kind == Kind.Melee ? MeleeSprite(index) : GunnerSprite(index);
    }

    void Face(Vector3 flatDirection)
    {
        if (flatDirection.sqrMagnitude < 0.01f)
            return;
        transform.rotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
    }

    bool HasLineOfSight(JANPlayerController player)
    {
        Vector3 origin = transform.position + Vector3.up * 1.35f;
        Vector3 target = player.View != null ? player.View.transform.position : player.transform.position + Vector3.up * 1.4f;
        Vector3 to = target - origin;
        if (to.magnitude > sightRange)
            return false;
        if (Physics.Raycast(origin, to.normalized, out RaycastHit hit, to.magnitude, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.GetComponentInParent<JANPlayerController>() != null;
        return true;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 knockDirection)
    {
        if (IsDead)
            return;

        alert = true;
        health -= amount;
        shove += new Vector3(knockDirection.x, 0f, knockDirection.z);
        hurtTimer = 0.16f;
        FlashWhite();

        if (health <= 0f)
            Die();
    }

    void FlashWhite()
    {
        flashUntil = Time.time + 0.14f;
        if (bodyRenderer != null)
            bodyRenderer.sharedMaterial = JANArt.Flat(Color.white);
        if (headRenderer != null)
            headRenderer.sharedMaterial = JANArt.Flat(Color.white);
    }

    void EndFlash()
    {
        flashUntil = 0f;
        if (FrameSprite(0) != null)
        {
            if (spriteMat != null && bodyRenderer != null)
                bodyRenderer.material = spriteMat;
            ApplyFrame(shownFrame);
            return;
        }

        if (bodyRenderer != null)
            bodyRenderer.sharedMaterial = JANArt.Flat(bodyColor);
        if (headRenderer != null)
            headRenderer.sharedMaterial = JANArt.Flat(headColor);
    }

    void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        state = State.Death;
        flashUntil = 0f;
        swinging = false;
        if (controller != null)
            controller.enabled = false;
        int deathFrame = kind == Kind.Melee ? 6 : 3;
        if (FrameSprite(deathFrame) != null)
        {
            if (billboard != null)
                billboard.Freeze();
            if (spriteMat != null && bodyRenderer != null)
                bodyRenderer.material = spriteMat;
            ApplyFrame(deathFrame);
            fallFrom = transform.rotation;
            fall = 0f;
            falling = true;
        }
        else
        {
            if (billboard != null)
                billboard.Freeze();
            transform.Rotate(80f, 0f, 0f);
            if (bodyRenderer != null)
                bodyRenderer.sharedMaterial = JANArt.Flat(bodyColor * 0.35f);
            if (headRenderer != null)
                headRenderer.sharedMaterial = JANArt.Flat(headColor * 0.35f);
        }
        JANGameManager.Instance?.AgentDown();
    }

    void TickFall()
    {
        if (!falling)
            return;

        fall = Mathf.MoveTowards(fall, 1f, Time.deltaTime / 0.34f);
        float angle = Mathf.SmoothStep(0f, 80f, fall);
        transform.rotation = fallFrom * Quaternion.Euler(angle, 0f, 0f);
        if (fall >= 1f)
            falling = false;
    }
}
