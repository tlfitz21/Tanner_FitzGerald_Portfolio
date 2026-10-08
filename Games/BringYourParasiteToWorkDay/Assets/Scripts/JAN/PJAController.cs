using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PJAController : MonoBehaviour, IJANDamageable
{
    [SerializeField] float maxHealth = 820f;
    [SerializeField] float rifleDamage = 11f;
    [SerializeField] float bashDamage = 48f;

    CharacterController controller;
    JANBillboardSprite billboard;
    Renderer bodyRenderer;
    Transform shield;
    Vector3 velocity;
    Vector3 shove;
    Vector3 home;
    float health;
    float nextShot;
    float nextDash;
    bool phaseTwo;
    bool aggro;
    bool charging;
    bool transforming;
    bool usingAk;
    Material akMat;
    float walkTime;
    float flashUntil;
    int shownFrame;
    Vector3 lastPlanar;
    Vector3 visualScale;
    Vector3 visualPos;
    static Sprite[] akFrames;
    static Sprite finalForm;
    static Sprite podiumSprite;
    float shake;
    float shakeMag;
    bool shook;
    Transform officePodium;
    Transform podiumSpin;
    float orbit;
    Renderer whiteout;
    const float FinalHeight = 4.7f;

    public bool IsDead { get; private set; }
    public bool IsAggro => aggro;
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool PhaseTwo => phaseTwo;

    public static PJAController Create(Vector3 feet)
    {
        GameObject body = new GameObject("President John American");
        body.transform.position = feet;
        CharacterController capsule = body.AddComponent<CharacterController>();
        capsule.height = 2.15f;
        capsule.radius = 0.55f;
        capsule.center = new Vector3(0f, 1.05f, 0f);
        capsule.stepOffset = 0.4f;
        capsule.slopeLimit = 48f;

        PJAController boss = body.AddComponent<PJAController>();
        Color gold = new Color(0.86f, 0.68f, 0.18f, 1f);
        GameObject visual = JANArt.Quad("Billboard", gold, body.transform);
        visual.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        visual.transform.localScale = new Vector3(1.15f, 2.2f, 1f);
        boss.billboard = visual.AddComponent<JANBillboardSprite>();
        boss.bodyRenderer = visual.GetComponent<Renderer>();
        boss.visualScale = visual.transform.localScale;
        boss.visualPos = visual.transform.localPosition;
        boss.lastPlanar = feet;
        boss.usingAk = AkSprite(0) != null;
        if (boss.usingAk)
            boss.ApplyAk(0);

        boss.health = boss.maxHealth;
        JANGameManager.Instance?.RegisterHostile();
        return boss;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        home = transform.position;
        if (health <= 0f)
            health = maxHealth;
    }

    public void SetOfficePodium(Transform podium)
    {
        officePodium = podium;
        shield = podium;
    }

    public void Aggro()
    {
        if (aggro || IsDead)
            return;
        aggro = true;
        nextShot = Time.time + 0.6f;
        nextDash = Time.time + 1.4f;
        JANGameManager.Instance?.BossEngaged(this);
    }

    void Update()
    {
        if (IsDead || !aggro)
            return;

        JANPlayerController player = JANGameManager.Instance != null ? JANGameManager.Instance.Player : null;
        if (player == null || player.IsDead)
        {
            Stick();
            return;
        }

        if (transforming)
        {
            Stick();
            return;
        }

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);

        if (charging)
        {
            Stick();
            return;
        }

        if (!phaseTwo)
            TickPhaseOne(player, toPlayer);
        else
            TickPhaseTwo(player, toPlayer);
    }

    void TickPhaseOne(JANPlayerController player, Vector3 toPlayer)
    {
        if (Time.time >= nextDash)
        {
            nextDash = Time.time + 4.2f;
            StartCoroutine(Dash(RingPoint()));
        }

        if (Time.time >= nextShot)
        {
            nextShot = Time.time + 1.45f;
            StartCoroutine(RifleBurst(player));
        }

        Move(toPlayer.normalized * 2.2f);
    }

    void TickPhaseTwo(JANPlayerController player, Vector3 toPlayer)
    {
        if (Time.time >= nextDash)
        {
            nextDash = Time.time + 2.35f;
            StartCoroutine(PodiumSwing(player));
            return;
        }

        Move(toPlayer.normalized * 6.4f);
        IdleOrbit();
    }

    IEnumerator RifleBurst(JANPlayerController player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (IsDead || phaseTwo || player == null || player.IsDead || player.View == null)
                yield break;

            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 aim = (player.View.transform.position - origin).normalized;
            aim = Quaternion.Euler(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f), 0f) * aim;
            JANWeaponSystem.SpawnPellet(origin, aim, rifleDamage, 20f, false, new Color(1f, 0.82f, 0.2f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Dash(Vector3 target)
    {
        charging = true;
        float duration = 0.28f;
        float elapsed = 0f;
        Vector3 start = transform.position;
        while (elapsed < duration && !IsDead)
        {
            elapsed += Time.deltaTime;
            Vector3 next = Vector3.Lerp(start, target, elapsed / duration);
            Vector3 step = next - transform.position;
            step.y = 0f;
            controller.Move(step);
            yield return null;
        }

        charging = false;
    }

    Vector3 RingPoint()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector3(home.x + Mathf.Cos(angle) * 14f, transform.position.y, home.z + Mathf.Sin(angle) * 11f);
    }

    void Move(Vector3 planar)
    {
        planar += shove;
        shove = Vector3.MoveTowards(shove, Vector3.zero, 14f * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += -24f * Time.deltaTime;
        planar.y = velocity.y;
        controller.Move(planar * Time.deltaTime);
    }

    void Stick()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += -24f * Time.deltaTime;
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
    }

    void LateUpdate()
    {
        Vector3 now = transform.position;
        Vector3 delta = now - lastPlanar;
        delta.y = 0f;
        lastPlanar = now;

        ApplyShake();
        if (phaseTwo && !transforming && bodyRenderer != null && akMat != null)
        {
            if (flashUntil > Time.time)
                bodyRenderer.sharedMaterial = JANArt.Flat(Color.white);
            else if (bodyRenderer.sharedMaterial != akMat)
                bodyRenderer.material = akMat;
        }
        if (!usingAk || phaseTwo || transforming || IsDead)
            return;

        if (flashUntil > Time.time)
            return;

        if (bodyRenderer != null && bodyRenderer.sharedMaterial != akMat && akMat != null)
            bodyRenderer.material = akMat;

        float speed = Time.deltaTime > 0f ? delta.magnitude / Time.deltaTime : 0f;
        if (speed < 0.35f)
        {
            ApplyAk(0);
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
        {
            ApplyAk(0);
            return;
        }

        forward.Normalize();
        right.Normalize();
        Vector3 move = delta.normalized;
        float forwardDot = Vector3.Dot(move, forward);
        float backDot = -forwardDot;
        float rightDot = Vector3.Dot(move, right);
        float leftDot = -rightDot;
        float best = forwardDot;
        int facing = 0;
        if (backDot > best)
        {
            best = backDot;
            facing = 0;
        }
        if (rightDot > best)
        {
            best = rightDot;
            facing = 1;
        }
        if (leftDot > best)
            facing = 2;

        if (facing == 1)
        {
            ApplyAk(3);
            return;
        }
        if (facing == 2)
        {
            ApplyAk(4);
            return;
        }

        walkTime += Time.deltaTime;
        ApplyAk(1 + Mathf.FloorToInt(walkTime * 8f) % 2);
    }

    void ApplyAk(int index)
    {
        shownFrame = index;
        Sprite sprite = AkSprite(index);
        Sprite stand = AkSprite(0);
        if (sprite == null || stand == null || bodyRenderer == null)
            return;

        if (akMat == null)
        {
            akMat = JANArt.SpriteMaterial();
            bodyRenderer.material = akMat;
        }
        else if (bodyRenderer.sharedMaterial != akMat)
            bodyRenderer.material = akMat;

        JANArt.PaintSprite(akMat, sprite);

        float unit = stand.pixelsPerUnit;
        float standHeight = stand.rect.height / unit;
        float fit = standHeight > 0.001f ? 2.4f / standHeight : 1f;
        float worldW = sprite.rect.width / sprite.pixelsPerUnit * fit;
        float worldH = sprite.rect.height / sprite.pixelsPerUnit * fit;
        float bodyW = stand.rect.width / unit * fit;
        Transform view = bodyRenderer.transform;
        view.localScale = new Vector3(worldW, worldH, 1f);
        view.localPosition = new Vector3((worldW - bodyW) * 0.5f, worldH * 0.5f, 0f);
    }

    static Sprite AkSprite(int index)
    {
        if (akFrames == null)
            akFrames = new Sprite[5];
        if (index < 0 || index >= akFrames.Length)
            return null;
        if (akFrames[index] != null)
            return akFrames[index];

        Sprite loaded = SpriteLibrary.Load("Assets/Sprites/JAN/JohnAmerica/john_p1_sprite_" + index + ".png");
        if (loaded != null)
        {
            loaded.texture.filterMode = FilterMode.Point;
            akFrames[index] = loaded;
        }
        return akFrames[index];
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 knockDirection)
    {
        if (IsDead || !aggro || transforming)
            return;

        health = Mathf.Max(0f, health - amount);
        shove += new Vector3(knockDirection.x, 0f, knockDirection.z) * 0.35f;
        if (!phaseTwo && !transforming && health > 0f && health <= maxHealth * 0.5f)
            EnterPhaseTwo();
        if (bodyRenderer != null && !transforming)
        {
            if ((usingAk && !phaseTwo) || phaseTwo)
            {
                flashUntil = Time.time + 0.14f;
                bodyRenderer.sharedMaterial = JANArt.Flat(Color.white);
            }
            else
                StartCoroutine(Flash());
        }
        JANGameManager.Instance?.BossHealthChanged();
        if (health <= 0f && !transforming)
            Die();
    }

    void EnterPhaseTwo()
    {
        if (transforming || phaseTwo)
            return;

        transforming = true;
        StopAllCoroutines();
        charging = false;
        shove = Vector3.zero;
        transform.localScale = Vector3.one;
        StartCoroutine(BecomeFinal());
    }

    IEnumerator BecomeFinal()
    {
        JANGameManager.Instance?.ShowBanner("THE OFFICE SHAKES");
        float elapsed = 0f;
        const float rumble = 1.15f;
        while (elapsed < rumble && !IsDead)
        {
            elapsed += Time.deltaTime;
            float along = elapsed / rumble;
            shake = 0.35f;
            shakeMag = Mathf.Lerp(0.04f, 0.2f, along);
            if (bodyRenderer != null)
                bodyRenderer.sharedMaterial = JANArt.Flat(Color.Lerp(new Color(0.86f, 0.68f, 0.18f, 1f), Color.white, along));
            SetWhiteout(Mathf.Clamp01((along - 0.45f) / 0.35f));
            yield return null;
        }

        ApplyFinalForm();
        GrowCapsule();
        SetWhiteout(1f);
        shakeMag = 0.24f;
        elapsed = 0f;
        while (elapsed < 0.45f && !IsDead)
        {
            elapsed += Time.deltaTime;
            shake = 0.2f;
            SetWhiteout(1f - elapsed / 0.45f);
            yield return null;
        }

        SetWhiteout(0f);
        shake = 0f;
        yield return SummonPodium();
        maxHealth = Mathf.Round(maxHealth * 1.25f);
        health = maxHealth;
        JANGameManager.Instance?.BossHealthChanged();
        transforming = false;
        phaseTwo = true;
        nextDash = Time.time + 0.45f;
        JANGameManager.Instance?.ShowBanner("HE GRABBED THE PODIUM");
    }

    void ApplyFinalForm()
    {
        Sprite sprite = FinalSprite();
        if (sprite == null || bodyRenderer == null)
        {
            bodyRenderer.sharedMaterial = JANArt.Flat(new Color(0.75f, 0.22f, 0.12f, 1f));
            return;
        }

        usingAk = false;
        if (akMat == null)
            akMat = JANArt.SpriteMaterial();
        bodyRenderer.material = akMat;
        PaintSprite(akMat, sprite);
        float unit = sprite.pixelsPerUnit;
        float native = sprite.rect.height / unit;
        float fit = native > 0.001f ? FinalHeight / native : 1f;
        float worldW = sprite.rect.width / unit * fit;
        float worldH = sprite.rect.height / unit * fit;
        Transform view = bodyRenderer.transform;
        view.localScale = new Vector3(worldW, worldH, 1f);
        view.localPosition = new Vector3(0f, worldH * 0.5f, 0f);
    }

    void GrowCapsule()
    {
        if (controller == null)
            return;
        controller.height = 3.5f;
        controller.center = new Vector3(0f, 1.75f, 0f);
        controller.radius = 0.68f;
    }

    IEnumerator SummonPodium()
    {
        if (officePodium == null)
            officePodium = GameObject.Find("Presidential Podium")?.transform;
        if (officePodium == null)
            yield break;

        shield = officePodium;
        Vector3 start = officePodium.position;
        float elapsed = 0f;
        const float flight = 0.85f;
        while (elapsed < flight && !IsDead)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.SmoothStep(0f, 1f, elapsed / flight);
            Vector3 hover = transform.position + Vector3.up * 1.7f + transform.right * 1.15f;
            Vector3 arc = Vector3.Lerp(start, hover, along);
            arc.y += Mathf.Sin(along * Mathf.PI) * 2.4f;
            officePodium.position = arc;
            shake = 0.12f;
            shakeMag = 0.05f;
            yield return null;
        }

        officePodium.SetParent(transform, true);
        podiumSpin = officePodium;
        orbit = 0f;
        IdleOrbit();
    }

    void IdleOrbit()
    {
        if (podiumSpin == null)
            return;

        orbit += Time.deltaTime * 2.4f;
        podiumSpin.localPosition = new Vector3(Mathf.Sin(orbit) * 1.25f, 1.85f, Mathf.Cos(orbit) * 1.25f);
        podiumSpin.localRotation = Quaternion.identity;
    }

    IEnumerator PodiumSwing(JANPlayerController player)
    {
        charging = true;
        bool hit = false;
        float elapsed = 0f;
        const float windup = 0.22f;
        while (elapsed < windup && !IsDead)
        {
            elapsed += Time.deltaTime;
            orbit += Time.deltaTime * 14f;
            if (podiumSpin != null)
            {
                float radius = Mathf.Lerp(1.25f, 0.85f, elapsed / windup);
                podiumSpin.localPosition = new Vector3(Mathf.Sin(orbit) * radius, 1.7f, Mathf.Cos(orbit) * radius);
            }
            shake = 0.08f;
            shakeMag = 0.03f;
            yield return null;
        }

        Vector3 direction = player != null ? player.transform.position - transform.position : transform.forward;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;
        direction.Normalize();
        elapsed = 0f;
        const float swing = 0.48f;
        while (elapsed < swing && !IsDead)
        {
            elapsed += Time.deltaTime;
            float along = elapsed / swing;
            float sweep = Mathf.Lerp(-1.15f, 1.15f, along);
            if (podiumSpin != null)
                podiumSpin.localPosition = new Vector3(sweep * 1.55f, 0.2f + Mathf.Sin(along * Mathf.PI) * 0.35f, 1.25f + Mathf.Sin(along * Mathf.PI) * 0.7f);
            controller.Move(direction * 5.5f * Time.deltaTime);
            shake = 0.1f;
            shakeMag = 0.07f;
            if (!hit && player != null && !player.IsDead)
            {
                Vector3 podium = podiumSpin != null ? podiumSpin.position : transform.position + direction;
                Vector3 under = player.transform.position - podium;
                float drop = podium.y - player.transform.position.y;
                under.y = 0f;
                bool reachedDown = under.magnitude < 2.05f && drop < 3.6f && drop > -1.2f;
                Vector3 flat = player.transform.position - transform.position;
                flat.y = 0f;
                bool inFront = flat.magnitude < 3.4f && Vector3.Dot(flat.sqrMagnitude > 0.01f ? flat.normalized : direction, direction) > -0.15f;
                if (reachedDown || inFront)
                {
                    hit = true;
                    player.TakeDamage(bashDamage, player.transform.position, direction);
                }
            }

            yield return null;
        }

        charging = false;
    }

    void ApplyShake()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 local = cam.transform.localPosition;
        if (shake > 0f)
        {
            shake -= Time.deltaTime;
            float mag = shakeMag;
            local.x = (Mathf.PerlinNoise(Time.time * 52f, 0.2f) - 0.5f) * 2f * mag;
            local.z = (Mathf.PerlinNoise(0.2f, Time.time * 52f) - 0.5f) * 2f * mag;
            cam.transform.localPosition = local;
            shook = true;
            return;
        }

        if (!shook)
            return;

        local.x = 0f;
        local.z = 0f;
        cam.transform.localPosition = local;
        shook = false;
    }

    void SetWhiteout(float alpha)
    {
        if (whiteout == null)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;
            GameObject veil = JANArt.Quad("Transform White", Color.white, cam.transform);
            veil.transform.localPosition = new Vector3(0f, 0f, 0.6f);
            veil.transform.localRotation = Quaternion.identity;
            veil.transform.localScale = new Vector3(24f, 16f, 1f);
            whiteout = veil.GetComponent<Renderer>();
        }

        if (alpha <= 0.01f)
        {
            whiteout.enabled = false;
            return;
        }

        whiteout.enabled = true;
        whiteout.transform.localScale = new Vector3(24f, 16f, 1f);
    }

    static void PaintSprite(Material mat, Sprite sprite)
    {
        JANArt.PaintSprite(mat, sprite);
    }

    static Sprite FinalSprite()
    {
        if (finalForm != null)
            return finalForm;

        finalForm = SpriteLibrary.Load("Assets/Sprites/JAN/JohnAmerica/john-america-finalform.png");
        return finalForm;
    }

    public static Transform CreateOfficePodium(Vector3 feet)
    {
        GameObject root = new GameObject("Presidential Podium");
        root.transform.position = feet;
        GameObject quad = JANArt.Quad("Podium Sprite", Color.white, root.transform);
        quad.AddComponent<JANBillboardSprite>();
        Sprite sprite = PodiumArt();
        Renderer renderer = quad.GetComponent<Renderer>();
        if (sprite != null && renderer != null)
        {
            Material mat = JANArt.SpriteMaterial();
            renderer.material = mat;
            PaintSprite(mat, sprite);
            float unit = sprite.pixelsPerUnit;
            float native = sprite.rect.height / unit;
            float fit = native > 0.001f ? 1.55f / native : 1f;
            float worldW = sprite.rect.width / unit * fit;
            float worldH = sprite.rect.height / unit * fit;
            quad.transform.localScale = new Vector3(worldW, worldH, 1f);
            quad.transform.localPosition = new Vector3(0f, worldH * 0.5f, 0f);
        }
        else
        {
            quad.transform.localScale = new Vector3(0.9f, 1.45f, 1f);
            quad.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        }

        return root.transform;
    }

    static Sprite PodiumArt()
    {
        if (podiumSprite != null)
            return podiumSprite;

        podiumSprite = SpriteLibrary.Load("Assets/Sprites/JAN/JohnAmerica/presidential-podium.png");
        if (podiumSprite != null)
            podiumSprite.texture.filterMode = FilterMode.Point;
        return podiumSprite;
    }

    IEnumerator Flash()
    {
        Color restore = phaseTwo ? new Color(0.75f, 0.22f, 0.12f, 1f) : new Color(0.86f, 0.68f, 0.18f, 1f);
        bodyRenderer.sharedMaterial = JANArt.Flat(Color.white);
        yield return new WaitForSeconds(0.14f);
        if (!IsDead && bodyRenderer != null)
            bodyRenderer.sharedMaterial = JANArt.Flat(restore);
    }

    void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        transform.localScale = Vector3.one;
        StopAllCoroutines();
        flashUntil = 0f;
        if (usingAk && !phaseTwo)
            ApplyAk(shownFrame);
        else if (phaseTwo && bodyRenderer != null && akMat != null)
            bodyRenderer.material = akMat;
        if (billboard != null)
            billboard.Freeze();
        if (controller != null)
            controller.enabled = false;
        transform.Rotate(75f, 0f, 0f);
        JANGameManager.Instance?.HostileDown();
        JANGameManager.Instance?.Victory();
    }
}
