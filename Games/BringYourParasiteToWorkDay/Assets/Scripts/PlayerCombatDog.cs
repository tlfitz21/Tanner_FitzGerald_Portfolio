using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerDog))]
[DefaultExecutionOrder(-10)]
public class PlayerCombatDog : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] float lightWindup = 0.06f;
    [SerializeField] float lightActive = 0.1f;
    [SerializeField] float lightRecovery = 0.1f;
    [SerializeField] float lightBodyOverlap = 0.35f;
    [SerializeField] float lightReach = 0.95f;

    [Header("Heavy")]
    [SerializeField] float heavyWindup = 0.34f;
    [SerializeField] float heavyActive = 0.22f;
    [SerializeField] float heavyRecovery = 0.22f;
    [SerializeField] float heavyBodyOverlap = 0.45f;
    [SerializeField] float heavyReach = 1.6f;

    [Header("Hitbox")]
    [SerializeField] float hitboxHeight = 0.9f;
    [SerializeField] LayerMask hitMask = ~0;

    [Header("Block")]
    [SerializeField] float perfectBlockWindow = 0.25f;
    [SerializeField] float maxBlock = 100f;
    [SerializeField] float pressBlockCost = 8f;
    [SerializeField] float holdBlockCostPerSecond = 14f;
    [SerializeField] float lightBlockHitCost = 18f;
    [SerializeField] float heavyBlockHitCost = 40f;
    [SerializeField] float blockRegenPerSecond = 22f;
    [SerializeField] float blockRegenDelay = 1f;
    [SerializeField] float brokenBlockDelay = 3f;

    [Header("Combo")]
    [SerializeField] float lightDamage = 8f;
    [SerializeField] float heavyDamage = 18f;
    [SerializeField] float comboDamageStep = 0.55f;
    [SerializeField] float followupWindupScale = 0.5f;
    [SerializeField] float finisherWindupScale = 0.3f;
    [SerializeField] float comboLinkWindow = 0.4f;

    [Header("Feedback")]
    [SerializeField] Color lightTint = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] Color heavyTint = new Color(1f, 0.45f, 0.15f, 1f);
    [SerializeField] Color blockTint = new Color(0.55f, 0.75f, 1f, 1f);

    public bool FacingLocked => IsAttacking || (sequence.Length > 0 && Time.time <= comboExpireTime);
    public bool IsAttacking { get; private set; }
    public bool IsBlocking { get; private set; }
    public bool IsPerfectBlocking => IsBlocking && Time.time - blockStartTime <= perfectBlockWindow;
    public float BlockMeter => blockMeter;

    PlayerControllerDog controller;
    DogHealth health;
    float blockStartTime = -999f;
    float blockMeter;
    float regenReadyTime;
    Rigidbody2D body;
    SpriteRenderer spriteRenderer;
    Color baseColor;
    Coroutine attackRoutine;
    string sequence = "";
    bool hasBufferedAttack;
    bool bufferedLight;
    bool attackConnected;
    float comboExpireTime;

    static readonly string[] ComboRoutes = { "LLL", "LLH", "HLL", "HHH", "HHL", "LHH" };

    Vector2 gizmoCenter;
    Vector2 gizmoSize;
    bool gizmoActive;

    void Awake()
    {
        controller = GetComponent<PlayerControllerDog>();
        health = GetComponent<DogHealth>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            baseColor = spriteRenderer.color;

        blockMeter = maxBlock;
    }

    void Update()
    {
        if (health != null && (health.IsDead || health.InCutscene) || (controller != null && controller.IsScripted))
        {
            StopAttack();
            IsBlocking = false;
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.jKey.wasPressedThisFrame)
            TryBuffer(true);
        if (keyboard.kKey.wasPressedThisFrame)
            TryBuffer(false);

        bool wantsBlock = keyboard.lKey.isPressed && !IsAttacking && blockMeter > 0f;
        if (wantsBlock && !IsBlocking)
        {
            blockStartTime = Time.time;
            SpendBlock(pressBlockCost);
            Debug.Log("Blocking");
            wantsBlock = blockMeter > 0f;
        }
        else if (wantsBlock)
        {
            SpendBlock(holdBlockCostPerSecond * Time.deltaTime);
            wantsBlock = blockMeter > 0f;
        }

        IsBlocking = wantsBlock;
        RecoverBlockMeter();
        if (health != null)
            health.SetBlockMeter(blockMeter, maxBlock, blockMeter <= 0f);

        if (!IsAttacking)
            ApplyTint(IsBlocking ? blockTint : Color.white);
    }

    public void OnBlockHit(bool heavy)
    {
        if (IsPerfectBlocking)
            return;

        SpendBlock(heavy ? heavyBlockHitCost : lightBlockHitCost);
        if (health != null)
            health.SetBlockMeter(blockMeter, maxBlock, blockMeter <= 0f);
    }

    void SpendBlock(float amount)
    {
        if (amount <= 0f || blockMeter <= 0f)
            return;

        blockMeter = RoundTenths(Mathf.Max(0f, blockMeter - amount));
        regenReadyTime = Time.time + (blockMeter <= 0f ? brokenBlockDelay : blockRegenDelay);
    }

    void RecoverBlockMeter()
    {
        if (IsBlocking || blockMeter >= maxBlock || Time.time < regenReadyTime)
            return;

        blockMeter = RoundTenths(Mathf.Min(maxBlock, blockMeter + blockRegenPerSecond * Time.deltaTime));
    }

    static float RoundTenths(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }

    void TryBuffer(bool light)
    {
        if (controller != null && controller.IsStaggered)
            return;

        if (IsAttacking)
        {
            string next = sequence + (light ? "L" : "H");
            if (IsComboPrefix(next))
            {
                hasBufferedAttack = true;
                bufferedLight = light;
            }
            return;
        }

        BeginAttack(light);
    }

    void BeginAttack(bool light)
    {
        if (!IsAttacking && Time.time > comboExpireTime)
            sequence = "";

        string next = sequence + (light ? "L" : "H");
        if (!IsComboPrefix(next))
        {
            sequence = "";
            next = light ? "L" : "H";
        }

        sequence = next;
        hasBufferedAttack = false;
        int hitIndex = sequence.Length - 1;

        Coroutine previous = attackRoutine;
        attackRoutine = StartCoroutine(AttackRoutine(light, hitIndex));
        if (previous != null)
            StopCoroutine(previous);
    }

    IEnumerator AttackRoutine(bool light, int hitIndex)
    {
        IsAttacking = true;
        IsBlocking = false;
        attackConnected = false;

        float windupScale = hitIndex == 0 ? 1f : hitIndex == 1 ? followupWindupScale : finisherWindupScale;
        float damageScale = 1f + hitIndex * comboDamageStep;
        string attackName = light ? "Light Attack" : "Heavy Attack";
        float windup = (light ? lightWindup : heavyWindup) * windupScale;
        float active = light ? lightActive : heavyActive;
        float recovery = light ? lightRecovery : heavyRecovery;
        float overlap = light ? lightBodyOverlap : heavyBodyOverlap;
        float reach = light ? lightReach : heavyReach;
        float damage = (light ? lightDamage : heavyDamage) * damageScale;
        Color hitboxColor = light
            ? new Color(1f, 0.86f, 0.08f, 1f)
            : new Color(0.9f, 0.08f, 0.05f, 1f);

        Debug.Log($"Combo {sequence}  {attackName}  windup {windup:0.00}s  damage {damage:0.0}");
        ApplyTint(Color.white);

        if (windup > 0f)
            yield return new WaitForSeconds(windup);

        if (health != null && health.IsDead)
        {
            EndCombo();
            yield break;
        }

        bool finisher = sequence.Length >= 3;
        EnemyHitEffect effect = EnemyHitEffect.None;
        if (!light)
            effect = finisher ? EnemyHitEffect.Launch : EnemyHitEffect.Stagger;
        else if (finisher)
            effect = EnemyHitEffect.Stagger;

        SpawnHitbox(attackName, overlap, reach, active, hitboxColor, damage, effect);

        float activeLeft = active;
        while (activeLeft > 0f)
        {
            activeLeft -= Time.deltaTime;
            yield return null;
        }

        float recoveryLeft = recovery;
        while (recoveryLeft > 0f && !hasBufferedAttack)
        {
            recoveryLeft -= Time.deltaTime;
            yield return null;
        }

        if (hasBufferedAttack)
        {
            bool nextLight = bufferedLight;
            hasBufferedAttack = false;
            if (!attackConnected)
                sequence = "";
            BeginAttack(nextLight);
            yield break;
        }

        IsAttacking = false;
        ApplyTint(Color.white);
        attackRoutine = null;
        if (!attackConnected || sequence.Length >= 3)
            sequence = "";
        comboExpireTime = Time.time + comboLinkWindow;
    }

    void EndCombo()
    {
        IsAttacking = false;
        hasBufferedAttack = false;
        sequence = "";
        attackRoutine = null;
        gizmoActive = false;
        ApplyTint(Color.white);
    }

    static bool IsComboPrefix(string value)
    {
        for (int i = 0; i < ComboRoutes.Length; i++)
        {
            if (ComboRoutes[i].StartsWith(value))
                return true;
        }

        return false;
    }

    void SpawnHitbox(string attackName, float bodyOverlap, float reach, float lifetime, Color color, float damage, EnemyHitEffect effect)
    {
        StartCoroutine(ShowHitbox(attackName, bodyOverlap, reach, lifetime, color, damage, effect));
    }

    Vector2 HitboxSize(float bodyOverlap, float reach)
    {
        float halfWidth = Mathf.Max(0.05f, controller.BodyExtents.x);
        float overlap = Mathf.Clamp(bodyOverlap, 0f, halfWidth);
        float width = overlap + Mathf.Max(0.05f, reach);
        return new Vector2(width, hitboxHeight);
    }

    Vector2 HitboxCenter(float bodyOverlap, float reach)
    {
        float halfWidth = Mathf.Max(0.05f, controller.BodyExtents.x);
        float overlap = Mathf.Clamp(bodyOverlap, 0f, halfWidth);
        float reachDistance = Mathf.Max(0.05f, reach);
        float mid = halfWidth - overlap + (overlap + reachDistance) * 0.5f;
        return controller.BodyPosition + new Vector2(controller.Facing * mid, controller.VisualHeight);
    }

    public void StopAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        IsAttacking = false;
        hasBufferedAttack = false;
        sequence = "";
        gizmoActive = false;
    }

    bool IsOwnCollider(Collider2D hit)
    {
        if (hit.attachedRigidbody == body)
            return true;
        if (hit.transform == transform || hit.transform.IsChildOf(transform))
            return true;
        if (controller.WalkArea != null && hit == controller.WalkArea)
            return true;
        return false;
    }

    IEnumerator ShowHitbox(string attackName, float bodyOverlap, float reach, float lifetime, Color color, float damage, EnemyHitEffect effect)
    {
        Vector2 size = HitboxSize(bodyOverlap, reach);
        GameObject box = new GameObject(attackName + " Bite");
        SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
        renderer.sprite = BiteSprite();
        renderer.color = color;
        renderer.sortingOrder = 30000;
        int facing = controller != null && controller.Facing < 0 ? -1 : 1;

        HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();
        float elapsed = 0f;
        gizmoActive = true;
        gizmoSize = size;

        while (elapsed < lifetime)
        {
            Vector2 center = HitboxCenter(bodyOverlap, reach);
            box.transform.position = center;
            float chomp = Mathf.Lerp(1.2f, 0.45f, lifetime <= 0f ? 1f : elapsed / lifetime);
            box.transform.localScale = new Vector3(size.x * 1.15f * facing, size.y * chomp, 1f);
            gizmoCenter = center;
            gizmoSize = size;
            ApplyHitbox(attackName, center, size, damage, effect, alreadyHit);

            elapsed += Time.deltaTime;
            yield return null;
        }

        gizmoActive = false;
        Destroy(box);
    }

    void ApplyHitbox(string attackName, Vector2 center, Vector2 size, float damage, EnemyHitEffect effect, HashSet<Collider2D> alreadyHit)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, hitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || alreadyHit.Contains(hit) || IsOwnCollider(hit))
                continue;

            alreadyHit.Add(hit);
            AttackDummy dummy = hit.GetComponent<AttackDummy>();
            if (dummy != null)
            {
                attackConnected = true;
                dummy.TakeHit(attackName, damage, effect, controller.Facing);
            }
            else
                Debug.Log($"{attackName} hit {hit.name}");
        }
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

    void OnDrawGizmos()
    {
        if (!gizmoActive)
            return;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.9f);
        Gizmos.DrawWireCube(gizmoCenter, gizmoSize);
    }

    static Sprite biteSprite;
    static Sprite debugSprite;

    static Sprite BiteSprite()
    {
        if (biteSprite != null)
            return biteSprite;

        const int width = 48;
        const int height = 32;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x + 0.5f) / width;
                float ny = ((y + 0.5f) / height) * 2f - 1f;
                float jaw = Mathf.Abs(ny);
                float front = 0.12f + jaw * 0.82f;
                bool band = jaw > 0.28f && jaw < 0.78f && nx > 0.06f && nx < front;
                bool tooth = jaw > 0.16f && jaw < 0.42f && nx > 0.18f && nx < 0.86f;
                if (tooth)
                {
                    float slot = (nx - 0.18f) / 0.68f * 5f;
                    tooth = slot - Mathf.Floor(slot) < 0.42f;
                }

                texture.SetPixel(x, y, band || tooth ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        biteSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 32f);
        return biteSprite;
    }

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
