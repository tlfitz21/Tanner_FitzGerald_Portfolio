using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttackDummy))]
public class RickJanitor : MonoBehaviour
{
    [SerializeField] float maxHealth = 460f;
    [SerializeField] float walkSpeed = 3.2f;
    [SerializeField] float depthSpeed = 2f;
    [SerializeField] float staggerTime = 0.4f;

    Rigidbody2D body;
    AttackDummy dummy;
    Transform visual;
    Transform mop;
    Transform bottle;
    SpriteRenderer spriteRenderer;
    SpriteRenderer shadowRenderer;
    Transform player;
    DogHealth dogHealth;
    Collider2D walkArea;
    bool fighting;
    bool acting;
    bool defeated;
    bool phaseTwo;
    float cooldown = 0.6f;
    const float FootY = -1.1f;
    float portraitScale = 1f;
    float mopFit = 1f;
    bool mopIsArt;
    Sprite phaseTwoSprite;
    float staggerTimer;
    int facing = -1;
    GameObject telegraph;

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

    public void Wake()
    {
        fighting = true;
    }

    void Update()
    {
        BindPlayer();
        UpdateSorting();
        if (defeated || !fighting)
            return;
        if (dogHealth != null && dogHealth.IsDead)
            return;

        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;
            return;
        }

        if (acting || player == null)
            return;

        cooldown -= Time.deltaTime;
        Vector2 toPlayer = (Vector2)player.position - body.position;
        Face(toPlayer.x);

        float mopReach = phaseTwo ? 2.9f : 2.35f;
        float mopDepth = phaseTwo ? 1.45f : 1.25f;
        float squirtReach = phaseTwo ? 10f : 8.5f;
        bool inMopRange = Mathf.Abs(toPlayer.x) < mopReach && Mathf.Abs(toPlayer.y) < mopDepth;

        // Recover by walking — don't plant in place between attacks.
        if (cooldown > 0f)
        {
            if (!inMopRange)
                Approach(toPlayer);
            return;
        }

        if (inMopRange)
            StartCoroutine(MopSwing());
        else if (Mathf.Abs(toPlayer.x) < squirtReach && Mathf.Abs(toPlayer.y) < mopDepth * 1.35f)
            StartCoroutine(Squirt());
        else
            Approach(toPlayer);
    }

    public void React(EnemyHitEffect effect, int direction)
    {
        if (effect == EnemyHitEffect.None || defeated)
            return;

        acting = false;
        ClearTelegraph();
        StopAllCoroutines();
        HideBottle();
        if (mopIsArt)
            PoseMop();
        else if (mop != null)
            mop.localRotation = Quaternion.identity;
        int knock = direction >= 0 ? 1 : -1;
        if (effect == EnemyHitEffect.Launch)
            StartCoroutine(Slide(knock * 3.2f, 0.22f));
        else
            staggerTimer = staggerTime;
    }

    public bool BeginPhaseTwo()
    {
        if (phaseTwo || defeated)
            return false;

        phaseTwo = true;
        acting = false;
        ClearTelegraph();
        StopAllCoroutines();
        walkSpeed = 4.8f;
        depthSpeed = 3.1f;
        staggerTime = 0.24f;
        if (mopIsArt)
            mopFit *= 1.12f;
        PoseMop();
        if (spriteRenderer != null && phaseTwoSprite != null)
        {
            spriteRenderer.sprite = phaseTwoSprite;
            SeatOnShadow(phaseTwoSprite);
        }
        staggerTimer = 0.45f;
        cooldown = 1.1f;
        return true;
    }

    public void BeginDefeat()
    {
        if (defeated)
            return;

        defeated = true;
        fighting = false;
        acting = false;
        ClearTelegraph();
        StopAllCoroutines();
        HideBottle();
        if (mop != null)
        {
            mop.SetParent(null, true);
            mop.rotation = mopIsArt ? Quaternion.Euler(0f, 0f, 90f * facing) : Quaternion.identity;
        }

        if (visual != null)
            visual.localRotation = Quaternion.Euler(0f, 0f, facing * 70f);
        StartCoroutine(PlayCutscene());
    }

    void Approach(Vector2 toPlayer)
    {
        Vector2 aim = toPlayer;
        if (aim.sqrMagnitude > 1f)
            aim.Normalize();
        Vector2 step = new Vector2(aim.x * walkSpeed, aim.y * depthSpeed);
        body.MovePosition(ClampToStreet(body.position + step * Time.deltaTime));
    }

    IEnumerator MopSwing()
    {
        acting = true;
        cooldown = phaseTwo ? 0.95f : 1.15f;
        Vector2 size = phaseTwo ? new Vector2(2.95f, 1.5f) : new Vector2(2.5f, 1.35f);
        Vector2 center = body.position + new Vector2(facing * (phaseTwo ? 1.7f : 1.55f), 0.1f);
        float windup = phaseTwo ? 0.3f : 0.48f;
        yield return StartCoroutine(ShowTelegraph("MOP", center, size, windup, new Color(0.72f, 0.78f, 0.7f, 0.55f)));
        if (!CanFinish())
            yield break;

        TryHit(center, size, true);
        yield return new WaitForSeconds(0.16f);
        EndAct(phaseTwo ? 0.85f : 1.05f);
    }

    IEnumerator Squirt()
    {
        acting = true;
        cooldown = phaseTwo ? 1.05f : 1.25f;
        Vector2 origin = body.position + new Vector2(facing * 0.7f, 0.35f);
        Vector2 aim = player != null ? (Vector2)player.position - origin : Vector2.right * facing;
        if (aim.sqrMagnitude < 0.01f)
            aim = Vector2.right * facing;
        aim.Normalize();
        float reach = phaseTwo ? 3.8f : 3.2f;
        Vector2 center = origin + aim * reach;
        float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        Vector2 marker = phaseTwo ? new Vector2(7.4f, 0.55f) : new Vector2(6.2f, 0.45f);
        float windup = phaseTwo ? 0.22f : 0.36f;
        ShowBottle(aim);
        yield return StartCoroutine(ShowTelegraph("SQUIRT", center, marker, windup, new Color(0.35f, 0.75f, 1f, 0.55f), angle));
        if (!CanFinish())
        {
            HideBottle();
            yield break;
        }

        int shots = phaseTwo ? 2 : 1;
        float speed = phaseTwo ? 11f : 9f;
        for (int i = 0; i < shots; i++)
        {
            float spread = phaseTwo ? (i == 0 ? -0.18f : 0.18f) : 0f;
            Vector2 shot = aim + new Vector2(-aim.y, aim.x) * spread;
            if (shot.sqrMagnitude > 0.01f)
                shot.Normalize();
            SpawnSquirt(origin, shot * speed);
        }
        yield return new WaitForSeconds(phaseTwo ? 0.08f : 0.12f);
        HideBottle();
        EndAct(phaseTwo ? 0.95f : 1.15f);
    }

    void SpawnSquirt(Vector2 origin, Vector2 velocity)
    {
        GameObject drop = new GameObject("Squirt");
        drop.transform.position = origin;
        drop.transform.localScale = new Vector3(0.45f, 0.28f, 1f);
        SpriteRenderer renderer = drop.AddComponent<SpriteRenderer>();
        renderer.sprite = StreetObstacle.Square;
        renderer.color = new Color(0.45f, 0.82f, 1f, 0.9f);
        renderer.sortingOrder = 40;
        BoxCollider2D box = drop.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = Vector2.one;
        drop.AddComponent<RickSquirt>().Launch(velocity);
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

    IEnumerator ShowTelegraph(string attackName, Vector2 center, Vector2 size, float duration, Color color, float angle = 0f)
    {
        ClearTelegraph();
        GameObject marker = new GameObject(attackName);
        marker.transform.position = center;
        marker.transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
        label.characterSize = 0.1f;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelObject.GetComponent<MeshRenderer>().sortingOrder = 29001;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (defeated || staggerTimer > 0f)
            {
                ClearTelegraph();
                yield break;
            }

            float pulse = Mathf.PingPong(elapsed * 6f, 1f);
            marker.transform.localScale = new Vector3(size.x, size.y, 1f) * (0.86f + 0.14f * pulse);
            Color drawn = color;
            drawn.a = color.a * (0.45f + 0.55f * pulse);
            renderer.color = drawn;
            elapsed += Time.deltaTime;
            yield return null;
        }

        ClearTelegraph();
    }

    IEnumerator Slide(float velocity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            body.MovePosition(ClampToStreet(body.position + Vector2.right * velocity * Time.deltaTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        staggerTimer = 0.15f;
    }

    IEnumerator PlayCutscene()
    {
        DogHealth dog = FindAnyObjectByType<DogHealth>();
        if (dog != null)
            dog.BeginCutscene();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject canvasObject = new GameObject("RickCutscene");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        CreateBar(canvasObject.transform, true);
        CreateBar(canvasObject.transform, false);

        GameObject card = new GameObject("Card");
        card.transform.SetParent(canvasObject.transform, false);
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(1200f, 220f);
        Text text = card.AddComponent<Text>();
        text.font = font;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 42;
        text.color = Color.white;
        text.text = "RICK\n<size=28>Chief Executive Presidential Sanitation Technician</size>";

        yield return new WaitForSeconds(1.4f);
        SceneManager.LoadScene("DogToRick");
    }

    static void CreateBar(Transform parent, bool top)
    {
        GameObject bar = new GameObject(top ? "LetterboxTop" : "LetterboxBottom");
        bar.transform.SetParent(parent, false);
        RectTransform rect = bar.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, top ? 1f : 0f);
        rect.anchorMax = new Vector2(1f, top ? 1f : 0f);
        rect.pivot = new Vector2(0.5f, top ? 1f : 0f);
        rect.sizeDelta = new Vector2(0f, 140f);
        rect.anchoredPosition = Vector2.zero;
        Image image = bar.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;
    }

    bool CanFinish()
    {
        if (defeated || staggerTimer > 0f || dogHealth == null || dogHealth.IsDead)
        {
            EndAct(0.35f);
            return false;
        }

        return true;
    }

    void EndAct(float nextCooldown)
    {
        acting = false;
        ClearTelegraph();
        cooldown = nextCooldown;
        if (mopIsArt)
            PoseMop();
        else if (mop != null)
            mop.localRotation = Quaternion.identity;
        HideBottle();
    }

    void BuildBody()
    {
        visual = new GameObject("Visual").transform;
        visual.SetParent(transform, false);
        visual.localScale = new Vector3(1.05f, 2.15f, 1f);
        spriteRenderer = visual.gameObject.AddComponent<SpriteRenderer>();
        Sprite portrait = LoadRickSprite(1);
        phaseTwoSprite = LoadRickSprite(0);
        if (portrait != null)
        {
            spriteRenderer.sprite = portrait;
            spriteRenderer.color = Color.white;
            portrait.texture.filterMode = FilterMode.Point;
            if (phaseTwoSprite != null)
                phaseTwoSprite.texture.filterMode = FilterMode.Point;
            float native = portrait.rect.height / portrait.pixelsPerUnit;
            portraitScale = native > 0.01f ? 2.2f / native : 1f;
            visual.localScale = new Vector3(portraitScale * facing, portraitScale, 1f);
            SeatOnShadow(portrait);
        }
        else
        {
            spriteRenderer.sprite = StreetObstacle.Square;
            spriteRenderer.color = new Color(0.12f, 0.48f, 0.5f, 1f);
        }

        mop = new GameObject("Mop").transform;
        mop.SetParent(transform, false);
        mop.localPosition = new Vector3(0.62f * facing, -0.05f, 0f);
        SpriteRenderer handle = mop.gameObject.AddComponent<SpriteRenderer>();
        Sprite broom = LoadWeaponSprite("Assets/Sprites/Rick/rickbroom.png");
        if (broom != null)
        {
            handle.sprite = broom;
            handle.color = Color.white;
            broom.texture.filterMode = FilterMode.Point;
            float native = broom.rect.height / broom.pixelsPerUnit;
            mopFit = native > 0.01f ? 2.05f / native : 1f;
            mopIsArt = true;
            PoseMop();
        }
        else
        {
            mop.localScale = new Vector3(1.35f, 0.16f, 1f);
            handle.sprite = StreetObstacle.Square;
            handle.color = new Color(0.45f, 0.32f, 0.16f, 1f);
            Transform head = new GameObject("MopHead").transform;
            head.SetParent(mop, false);
            head.localPosition = new Vector3(0.62f, 0f, 0f);
            head.localScale = new Vector3(0.28f, 2.6f, 1f);
            SpriteRenderer headRenderer = head.gameObject.AddComponent<SpriteRenderer>();
            headRenderer.sprite = StreetObstacle.Square;
            headRenderer.color = new Color(0.75f, 0.75f, 0.7f, 1f);
            headRenderer.sortingOrder = 3;
        }

        handle.sortingOrder = 2;
        bottle = new GameObject("Spray").transform;
        bottle.SetParent(transform, false);
        SpriteRenderer bottleRenderer = bottle.gameObject.AddComponent<SpriteRenderer>();
        Sprite spray = LoadWeaponSprite("Assets/Sprites/Rick/RickSpray.png");
        if (spray != null)
        {
            bottleRenderer.sprite = spray;
            bottleRenderer.color = Color.white;
            spray.texture.filterMode = FilterMode.Point;
            float native = spray.rect.height / spray.pixelsPerUnit;
            float fit = native > 0.01f ? 0.85f / native : 1f;
            bottle.localScale = new Vector3(fit, fit, 1f);
        }
        else
        {
            bottleRenderer.sprite = StreetObstacle.Square;
            bottleRenderer.color = new Color(0.25f, 0.72f, 0.9f, 1f);
            bottle.localScale = new Vector3(0.22f, 0.48f, 1f);
        }

        bottle.gameObject.SetActive(false);

        GameObject nameObject = new GameObject("Name");
        nameObject.transform.SetParent(transform, false);
        nameObject.transform.localPosition = new Vector3(0f, 1.7f, 0f);
        TextMesh nameLabel = nameObject.AddComponent<TextMesh>();
        nameLabel.text = "RICK";
        nameLabel.anchor = TextAnchor.MiddleCenter;
        nameLabel.alignment = TextAlignment.Center;
        nameLabel.fontSize = 32;
        nameLabel.characterSize = 0.13f;
        nameLabel.color = Color.white;
        nameLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameObject.GetComponent<MeshRenderer>().sortingOrder = 30;

        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(transform, false);
        titleObject.transform.localPosition = new Vector3(0f, 2.15f, 0f);
        TextMesh title = titleObject.AddComponent<TextMesh>();
        title.text = "Chief Executive Presidential Sanitation Technician";
        title.anchor = TextAnchor.MiddleCenter;
        title.alignment = TextAlignment.Center;
        title.fontSize = 32;
        title.characterSize = 0.045f;
        title.color = new Color(0.85f, 0.9f, 0.85f, 1f);
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleObject.GetComponent<MeshRenderer>().sortingOrder = 30;
        if (mop != null && portrait != null && !mopIsArt)
            mop.gameObject.SetActive(false);
        shadowRenderer = GroundShadow.Attach(transform, portrait != null ? FootY : -0.95f, 0.9f, 0.22f);
    }

    void SeatOnShadow(Sprite sprite)
    {
        if (visual == null || sprite == null)
            return;

        Vector3 place = visual.localPosition;
        place.y = FootY - sprite.bounds.min.y * portraitScale;
        visual.localPosition = place;
    }

    static Sprite LoadRickSprite(int index)
    {
        return LoadWeaponSprite("Assets/Sprites/Rick/rick_full_sprite_" + index + ".png");
    }

    static Sprite LoadWeaponSprite(string path)
    {
        return SpriteLibrary.Load(path);
    }

    void PoseMop()
    {
        if (mop == null || !mopIsArt)
            return;

        mop.localScale = new Vector3(Mathf.Abs(mopFit), Mathf.Abs(mopFit), 1f);
        mop.localRotation = Quaternion.Euler(0f, 0f, 90f * facing);
        mop.localPosition = new Vector3(0.62f * facing, -0.05f, 0f);
    }

    void ShowBottle(Vector2 aim)
    {
        if (bottle == null)
            return;

        bottle.gameObject.SetActive(true);
        bottle.localPosition = new Vector3(0.48f * facing, 0.35f, 0f);
        float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        bottle.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void HideBottle()
    {
        if (bottle != null)
            bottle.gameObject.SetActive(false);
    }

    void BindPlayer()
    {
        if (player != null)
            return;

        dogHealth = FindAnyObjectByType<DogHealth>();
        if (dogHealth == null)
            return;

        player = dogHealth.transform;
        PlayerControllerDog movement = dogHealth.GetComponent<PlayerControllerDog>();
        if (movement != null)
            walkArea = movement.WalkArea;
    }

    void Face(float direction)
    {
        if (Mathf.Abs(direction) < 0.05f || visual == null)
            return;

        facing = direction > 0f ? 1 : -1;
        Vector3 scale = visual.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        visual.localScale = scale;
        if (mopIsArt)
            PoseMop();
        else if (mop != null)
        {
            Vector3 mopScale = mop.localScale;
            mopScale.x = Mathf.Abs(mopScale.x) * facing;
            mop.localScale = mopScale;
            mop.localPosition = new Vector3(0.85f * facing, mop.localPosition.y, 0f);
        }
    }

    void UpdateSorting()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100f);
        int front = spriteRenderer.sortingOrder + 1;
        if (mop != null)
        {
            SpriteRenderer mopRenderer = mop.GetComponent<SpriteRenderer>();
            if (mopRenderer != null)
                mopRenderer.sortingOrder = front;
        }
        if (bottle != null)
        {
            SpriteRenderer bottleRenderer = bottle.GetComponent<SpriteRenderer>();
            if (bottleRenderer != null)
                bottleRenderer.sortingOrder = front + 1;
        }
        GroundShadow.Sort(shadowRenderer, transform.position.y, 0);
    }

    Vector2 ClampToStreet(Vector2 position)
    {
        if (walkArea == null)
            return position;

        Bounds bounds = walkArea.bounds;
        position.x = Mathf.Clamp(position.x, bounds.min.x + 0.8f, bounds.max.x - 0.8f);
        position.y = Mathf.Clamp(position.y, bounds.min.y + 0.8f, bounds.max.y - 0.8f);
        return position;
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

public class RickSquirt : MonoBehaviour
{
    Vector2 velocity;
    float life = 1.5f;
    bool spent;

    public void Launch(Vector2 shotVelocity)
    {
        velocity = shotVelocity;
    }

    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        life -= Time.deltaTime;
        if (life <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (spent)
            return;

        DogHealth health = other.GetComponent<DogHealth>();
        if (health == null)
            return;

        spent = true;
        health.ReceiveAttack(false);
        Destroy(gameObject);
    }
}
