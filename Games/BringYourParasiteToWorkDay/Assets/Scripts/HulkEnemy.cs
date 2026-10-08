using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttackDummy))]
public class HulkEnemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 500f;
    [SerializeField] float walkSpeed = 2.4f;
    [SerializeField] float depthSpeed = 1.6f;
    [SerializeField] float chargeSpeedScale = 1.3f;
    [SerializeField] float rushSpeedScale = 2.25f;

    Rigidbody2D body;
    AttackDummy dummy;
    Transform visual;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;
    Transform player;
    DogHealth dogHealth;
    Collider2D walkArea;
    PlayerControllerDog dogMove;
    bool activated;
    bool acting;
    float cooldown;
    float chargeSpeed = 7.8f;
    float rushSpeed = 13.5f;
    Vector2 physicsVelocity;
    bool scriptedMove;
    Vector2 scriptedPosition;
    int facing = -1;
    GameObject telegraph;
    TextMesh nameLabel;
    bool held = true;
    bool fleeing;
    bool wallDropped;
    bool brokeExit;
    StreetObstacle exitGate;
    bool boxedIn;
    float boxLeft;
    float boxRight;
    Pose pose;
    float animTime;
    const float FootY = -1.2f;
    float portraitScale = 1f;
    static Sprite[] baronFrames;

    enum Pose
    {
        Idle,
        Walk,
        Charge,
        Sweep,
        Strike,
        Flee
    }

    public bool IsOutOfFight => fleeing;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        dummy = GetComponent<AttackDummy>();
        body.gravityScale = 0f;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        BuildBody();
    }

    void Start()
    {
        if (dummy != null)
            dummy.SetMaxHealth(maxHealth);
    }

    public void ChargeIn(StreetObstacle entrance, float stopX, float dropWallX, System.Action dropWall, Vector2 throwTarget, System.Action onThrown)
    {
        held = false;
        activated = true;
        StartCoroutine(Entrance(entrance, stopX, dropWallX, dropWall, throwTarget, onThrown));
    }

    public void SealBehind(StreetObstacle gate)
    {
        exitGate = gate;
    }

    public void LockArena(float left, float right)
    {
        boxedIn = true;
        boxLeft = left;
        boxRight = right;
    }

    public void BeginFlee()
    {
        fleeing = true;
        acting = false;
        held = false;
        scriptedMove = false;
        ClearTelegraph();
        StopAllCoroutines();
        Face(1f);
        if (nameLabel != null)
            nameLabel.text = "WAH";
        SetPose(Pose.Flee);
        if (spriteRenderer != null && BaronSprite(0) == null)
            spriteRenderer.color = new Color(0.45f, 0.55f, 0.62f, 1f);
        if (dogHealth == null)
            dogHealth = FindAnyObjectByType<DogHealth>();
        if (dogHealth != null)
            dogHealth.RestorePortion(0.5f);
    }

    void Update()
    {
        physicsVelocity = Vector2.zero;
        BindPlayer();
        if (fleeing)
        {
            ApplySpeeds();
            physicsVelocity = Vector2.right * rushSpeed;
            UpdateSorting();
            return;
        }

        UpdateSorting();

        if (held)
        {
            SetPose(Pose.Idle);
            return;
        }
        if (dummy != null && dummy.IsDefeated)
            return;
        if (!activated)
        {
            TryActivate();
            return;
        }
        if (dogHealth != null && dogHealth.IsDead)
            return;
        if (acting || player == null)
            return;

        cooldown -= Time.deltaTime;
        Vector2 toPlayer = (Vector2)player.position - body.position;
        Face(toPlayer.x);

        float distanceX = Mathf.Abs(toPlayer.x);
        float distanceY = Mathf.Abs(toPlayer.y);
        if (cooldown <= 0f && distanceX > 3.4f && distanceX < 10f && distanceY < 0.9f)
        {
            StartCoroutine(Charge());
            return;
        }

        if (cooldown <= 0f && distanceX < 2.5f && distanceY < 1.7f)
        {
            StartCoroutine(distanceY > 0.55f ? Sweep() : Strike());
            return;
        }

        Vector2 aim = toPlayer;
        if (aim.sqrMagnitude > 1f)
            aim.Normalize();
        physicsVelocity = new Vector2(aim.x * walkSpeed, aim.y * depthSpeed);
        SetPose(Pose.Walk);
    }

    void LateUpdate()
    {
        Animate();
    }

    void FixedUpdate()
    {
        if (body == null)
            return;

        if (scriptedMove)
        {
            body.MovePosition(scriptedPosition);
            return;
        }

        if (physicsVelocity.sqrMagnitude < 0.0001f)
            return;

        Vector2 next = body.position + physicsVelocity * Time.fixedDeltaTime;
        if (fleeing)
        {
            if (!brokeExit && exitGate != null && next.x >= exitGate.transform.position.x - 1.2f)
            {
                brokeExit = true;
                exitGate.Break();
                exitGate = null;
            }

            body.MovePosition(next);
            if (next.x > 50f)
                Destroy(gameObject);
            return;
        }

        body.MovePosition(ClampToStreet(next));
    }

    public void React(EnemyHitEffect effect, int direction)
    {
    }

    IEnumerator Charge()
    {
        acting = true;
        SetPose(Pose.Charge);
        ApplySpeeds();
        float laneY = body.position.y;
        int dir = facing;
        float endX = body.position.x + dir * 9f;
        if (walkArea != null)
        {
            Bounds street = walkArea.bounds;
            endX = Mathf.Clamp(endX, street.min.x + 1.1f, street.max.x - 1.1f);
        }

        float travel = Mathf.Abs(endX - body.position.x);
        Vector2 size = new Vector2(Mathf.Max(1.6f, travel), 1.55f);
        Vector2 center = new Vector2((body.position.x + endX) * 0.5f, laneY);
        yield return StartCoroutine(ShowTelegraph("CHARGE", center, size, 0.8f, new Color(1f, 0.22f, 0.12f, 0.5f), 3.2f));
        if (!CanFinish())
            yield break;

        scriptedMove = true;
        scriptedPosition = body.position;
        bool connected = false;
        while ((endX - scriptedPosition.x) * dir > 0.08f)
        {
            float step = chargeSpeed * Time.fixedDeltaTime;
            float remaining = Mathf.Abs(endX - scriptedPosition.x);
            scriptedPosition.x += dir * Mathf.Min(step, remaining);
            scriptedPosition.y = laneY;
            if (!connected)
                connected = TryHit(scriptedPosition, new Vector2(1.9f, 2.2f), true);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();
        if (boxedIn)
        {
            boxLeft = Mathf.Min(boxLeft, scriptedPosition.x);
            boxRight = Mathf.Max(boxRight, scriptedPosition.x);
        }

        EndAct(1.5f);
    }

    IEnumerator Sweep()
    {
        acting = true;
        SetPose(Pose.Sweep);
        Vector2 size = new Vector2(4.6f, 2.3f);
        Vector2 center = body.position + new Vector2(facing * 1.7f, 0f);
        yield return StartCoroutine(ShowTelegraph("SWEEP", center, size, 0.62f, new Color(0.95f, 0.55f, 0.12f, 0.48f), 2.4f));
        if (!CanFinish())
            yield break;

        TryHit(center, size, false);
        yield return new WaitForSeconds(0.18f);
        EndAct(1.25f);
    }

    IEnumerator Strike()
    {
        acting = true;
        SetPose(Pose.Strike);
        Vector2 size = new Vector2(2.15f, 2.05f);
        Vector2 center = body.position + new Vector2(facing * 1.35f, 0.15f);
        yield return StartCoroutine(ShowTelegraph("STRIKE", center, size, 0.9f, new Color(0.85f, 0.12f, 0.08f, 0.62f), 5f));
        if (!CanFinish())
            yield break;

        TryHit(center, size, true);
        yield return new WaitForSeconds(0.22f);
        EndAct(1.45f);
    }

    bool TryHit(Vector2 center, Vector2 size, bool heavy)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        for (int i = 0; i < hits.Length; i++)
        {
            DogHealth health = hits[i].GetComponent<DogHealth>();
            if (health == null)
                continue;

            health.ReceiveAttack(heavy);
            return true;
        }

        return false;
    }

    IEnumerator ShowTelegraph(string attackName, Vector2 center, Vector2 size, float duration, Color color, float pulses)
    {
        ClearTelegraph();
        GameObject marker = new GameObject(attackName);
        marker.transform.position = center;
        telegraph = marker;
        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = StreetObstacle.Square;
        renderer.sortingOrder = 29000;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(marker.transform, false);
        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = attackName;
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 32;
        label.characterSize = 0.12f;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        MeshRenderer mesh = labelObject.GetComponent<MeshRenderer>();
        mesh.sortingOrder = 29001;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float pulse = Mathf.PingPong((elapsed / duration) * pulses, 1f);
            float scale = 0.78f + 0.22f * pulse;
            marker.transform.localScale = new Vector3(size.x * scale, size.y * scale, 1f);
            labelObject.transform.localScale = new Vector3(1f / Mathf.Max(0.01f, size.x * scale), 1f / Mathf.Max(0.01f, size.y * scale), 1f);
            Color drawn = color;
            drawn.a = color.a * (0.4f + 0.6f * pulse);
            renderer.color = drawn;
            elapsed += Time.deltaTime;
            yield return null;
        }

        ClearTelegraph();
    }

    bool CanFinish()
    {
        if (dogHealth == null || dogHealth.IsDead || (dummy != null && dummy.IsDefeated))
        {
            EndAct();
            return false;
        }

        return true;
    }

    void EndAct(float nextCooldown = 0.4f)
    {
        acting = false;
        scriptedMove = false;
        ClearTelegraph();
        cooldown = nextCooldown;
        SetPose(Pose.Idle);
        if (spriteRenderer != null && BaronSprite(0) == null && (dummy == null || !dummy.IsDefeated))
            spriteRenderer.color = new Color(0.28f, 0.33f, 0.2f, 1f);
    }

    void BuildBody()
    {
        visual = new GameObject("Visual").transform;
        visual.SetParent(transform, false);
        visual.localScale = new Vector3(1.8f, 2.7f, 1f);
        spriteRenderer = visual.gameObject.AddComponent<SpriteRenderer>();
        if (BaronSprite(0) != null)
        {
            spriteRenderer.color = Color.white;
            FitPortrait(BaronSprite(0));
        }
        else
        {
            spriteRenderer.sprite = StreetObstacle.Square;
            spriteRenderer.color = new Color(0.28f, 0.33f, 0.2f, 1f);
        }

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = new Vector3(0f, 1.85f, 0f);
        nameLabel = labelObject.AddComponent<TextMesh>();
        TextMesh label = nameLabel;
        label.text = "The Baron\nPoundmaster";
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 32;
        label.characterSize = 0.07f;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelObject.GetComponent<MeshRenderer>().sortingOrder = 20;
        shadowRenderer = GroundShadow.Attach(transform, FootY, 1.25f, 0.28f);
    }

    void TryActivate()
    {
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
        if (dogMove != null)
        {
            walkArea = dogMove.WalkArea;
            ApplySpeeds();
        }
    }

    void ApplySpeeds()
    {
        float top = dogMove != null ? dogMove.MoveSpeed : 6f;
        chargeSpeed = top * chargeSpeedScale;
        rushSpeed = top * rushSpeedScale;
    }

    void Face(float direction)
    {
        if (Mathf.Abs(direction) < 0.05f || visual == null)
            return;

        facing = direction > 0f ? 1 : -1;
        if (BaronSprite(0) == null)
        {
            Vector3 scale = visual.localScale;
            scale.x = Mathf.Abs(scale.x) * -facing;
            visual.localScale = scale;
        }
    }

    void SetPose(Pose next)
    {
        if (pose == next)
            return;

        pose = next;
        animTime = 0f;
    }

    void Animate()
    {
        if (BaronSprite(0) == null || spriteRenderer == null)
            return;

        animTime += Time.deltaTime;
        int index = 0;
        float rate = 8f;
        switch (pose)
        {
            case Pose.Walk:
                index = physicsVelocity.sqrMagnitude > 0.05f || scriptedMove
                    ? Mathf.FloorToInt(animTime * rate) % 3
                    : 0;
                break;
            case Pose.Charge:
                index = 3 + Mathf.FloorToInt(animTime * 12f) % 3;
                break;
            case Pose.Sweep:
                index = 6;
                break;
            case Pose.Strike:
                index = 7;
                break;
            case Pose.Flee:
                index = 8 + Mathf.FloorToInt(animTime * 10f) % 3;
                break;
        }

        ShowBaron(index);
    }

    void FitPortrait(Sprite sprite)
    {
        sprite.texture.filterMode = FilterMode.Point;
        float native = sprite.rect.height / sprite.pixelsPerUnit;
        portraitScale = native > 0.01f ? 2.7f / native : 1f;
        ShowBaron(0);
    }

    void ShowBaron(int index)
    {
        Sprite sprite = BaronSprite(index);
        Sprite stand = BaronSprite(0);
        if (sprite == null || stand == null || visual == null || spriteRenderer == null)
            return;

        spriteRenderer.sprite = sprite;
        float bodyW = stand.bounds.size.x;
        float extra = (sprite.bounds.size.x - bodyW) * portraitScale * 0.5f;
        visual.localScale = new Vector3(portraitScale * -facing, portraitScale, 1f);
        visual.localPosition = new Vector3(extra * -facing, FootY - sprite.bounds.min.y * portraitScale, 0f);
    }

    static Sprite BaronSprite(int index)
    {
        if (baronFrames == null)
            baronFrames = new Sprite[11];
        if (index < 0 || index >= baronFrames.Length)
            return null;
        if (baronFrames[index] != null)
            return baronFrames[index];

        baronFrames[index] = SpriteLibrary.Load("Assets/Sprites/Baron/baron_sprite_" + index.ToString("00") + ".png");
        return baronFrames[index];
    }

    void UpdateSorting()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100f);
        GroundShadow.Sort(shadowRenderer, transform.position.y, 0);
    }

    Vector2 ClampToStreet(Vector2 position)
    {
        if (walkArea == null)
            return position;

        Bounds bounds = walkArea.bounds;
        float minX = bounds.min.x + 1.1f;
        float maxX = bounds.max.x - 1.1f;
        if (boxedIn && !fleeing)
        {
            minX = Mathf.Max(minX, boxLeft);
            maxX = Mathf.Min(maxX, boxRight);
        }

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, bounds.min.y + 1.2f, bounds.max.y - 1.2f);
        return position;
    }

    IEnumerator Entrance(StreetObstacle entrance, float stopX, float dropWallX, System.Action dropWall, Vector2 throwTarget, System.Action onThrown)
    {
        acting = true;
        SetPose(Pose.Charge);
        Face(-1f);
        ApplySpeeds();
        BindPlayer();
        scriptedMove = true;
        scriptedPosition = body.position;
        bool smashed = false;

        while (true)
        {
            if (!wallDropped && scriptedPosition.x < dropWallX)
            {
                wallDropped = true;
                dropWall?.Invoke();
            }

            if (!smashed && entrance != null && scriptedPosition.x <= entrance.transform.position.x + 1.6f)
            {
                entrance.Break();
                smashed = true;
            }

            if (player == null)
            {
                if (scriptedPosition.x <= stopX)
                    break;

                scriptedPosition.x -= rushSpeed * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
                continue;
            }

            Vector2 dog = player.position;
            Vector2 toDog = dog - scriptedPosition;
            if (toDog.sqrMagnitude < 1.8f * 1.8f)
                break;

            Vector2 step = toDog.normalized * rushSpeed * Time.fixedDeltaTime;
            scriptedPosition += step.sqrMagnitude > toDog.sqrMagnitude ? toDog : step;
            Face(dog.x - scriptedPosition.x);
            if (scriptedPosition.x < -12f)
                break;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();

        if (!smashed && entrance != null)
            entrance.Break();
        if (!wallDropped)
        {
            wallDropped = true;
            dropWall?.Invoke();
        }

        yield return GrabAndThrow(throwTarget, onThrown);
        EndAct(0.45f);
    }

    IEnumerator GrabAndThrow(Vector2 arenaCenter, System.Action onThrown)
    {
        BindPlayer();
        if (dogMove == null || player == null)
        {
            onThrown?.Invoke();
            yield break;
        }

        Face(player.position.x - scriptedPosition.x);
        SetPose(Pose.Walk);
        Vector2 hold = scriptedPosition + new Vector2(facing * 1.05f, 0.12f);
        Vector2 pullStart = dogMove.BodyPosition;
        dogMove.BeginScripted(pullStart);
        if (dogHealth != null)
            dogHealth.ApplyDamage(16f, "Grab");

        float pull = 0f;
        while (pull < 0.2f)
        {
            pull += Time.fixedDeltaTime;
            float u = Mathf.Clamp01(pull / 0.2f);
            dogMove.SetScriptedPose(Vector2.Lerp(pullStart, hold, u), 0.25f * u);
            yield return new WaitForFixedUpdate();
        }

        yield return StartCoroutine(ShowTelegraph("GRAB", hold, new Vector2(2.1f, 1.8f), 0.32f, new Color(0.95f, 0.2f, 0.12f, 0.55f), 3f));

        float duration = 0.62f;
        float flown = 0f;
        while (flown < duration)
        {
            flown += Time.fixedDeltaTime;
            float u = Mathf.Clamp01(flown / duration);
            float hop = Mathf.Sin(u * Mathf.PI) * 2.5f;
            dogMove.SetScriptedPose(Vector2.Lerp(hold, arenaCenter, u), hop);
            yield return new WaitForFixedUpdate();
        }

        dogMove.SetScriptedPose(arenaCenter, 0f);
        yield return new WaitForFixedUpdate();
        dogMove.EndScripted();

        Vector2 hulkSpot = arenaCenter + new Vector2(6.5f, 0.35f);
        while ((scriptedPosition - hulkSpot).sqrMagnitude > 0.06f)
        {
            scriptedPosition = Vector2.MoveTowards(scriptedPosition, hulkSpot, rushSpeed * Time.fixedDeltaTime);
            Face(hulkSpot.x - scriptedPosition.x);
            yield return new WaitForFixedUpdate();
        }

        onThrown?.Invoke();
    }

    void ClearTelegraph()
    {
        if (telegraph == null)
            return;

        Destroy(telegraph);
        telegraph = null;
    }

    void OnDisable()
    {
        ClearTelegraph();
    }
}
