using System.Collections;
using UnityEngine;

public enum EnemyHitEffect
{
    None,
    Stagger,
    Knockdown,
    Launch
}

public class AttackDummy : MonoBehaviour
{
    [SerializeField] int sortingOffset;
    [SerializeField] float sortingScale = 100f;
    [SerializeField] Color hitColor = Color.white;
    [SerializeField] float hitFlashTime = 0.12f;
    [SerializeField] float maxHealth = 72f;

    SpriteRenderer spriteRenderer;
    Color baseColor;
    Coroutine flashRoutine;
    static Shader flashShader;
    float health;
    bool defeated;
    bool capturedColor;

    public bool IsDefeated => defeated;

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
        if (!defeated)
            health = maxHealth;
    }

    void Awake()
    {
        BindSprite();
        health = maxHealth;
    }

    void Start()
    {
        BindSprite();
    }

    void BindSprite()
    {
        Transform body = transform.Find("Visual");
        SpriteRenderer renderer = body != null ? body.GetComponent<SpriteRenderer>() : null;
        if (renderer == null)
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            if (renderers.Length == 0)
                return;
            renderer = renderers[0];
        }

        spriteRenderer = renderer;
        if (!capturedColor && spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
            capturedColor = true;
        }

        if (spriteRenderer == null)
            return;

        if (flashShader == null)
            flashShader = Shader.Find("Game/SpriteFlash");
        if (flashShader != null && spriteRenderer.material.shader != flashShader)
            spriteRenderer.material = new Material(flashShader);
    }

    void Update()
    {
        if (spriteRenderer == null || flashRoutine != null)
            return;

        spriteRenderer.sortingOrder = sortingOffset - Mathf.RoundToInt(transform.position.y * sortingScale);
    }

    public void TakeHit(string attackName, float damage, EnemyHitEffect effect = EnemyHitEffect.None, int knockDirection = 1)
    {
        if (defeated)
            return;

        health = Mathf.Max(0f, Mathf.Round((health - damage) * 10f) / 10f);
        Debug.Log($"{name} hit by {attackName} for {damage:0.0}. HP {health:0.0}");

        if (health <= 0f)
        {
            RickJanitor rickDown = GetComponent<RickJanitor>();
            if (rickDown != null && rickDown.BeginPhaseTwo())
            {
                health = maxHealth;
                StartFlash();
                return;
            }

            defeated = true;
            HulkEnemy fleeingHulk = GetComponent<HulkEnemy>();
            if (fleeingHulk != null)
            {
                fleeingHulk.BeginFlee();
                StartFlash();
                return;
            }

            if (rickDown != null)
            {
                rickDown.BeginDefeat();
                StartFlash();
                return;
            }

            Debug.Log($"{name} defeated");
            StreetEnemy beaten = GetComponent<StreetEnemy>();
            if (beaten != null)
                beaten.React(effect, knockDirection);
            AttackingDummy attacker = GetComponent<AttackingDummy>();
            if (attacker != null)
                attacker.enabled = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(
                    baseColor.r * 0.35f,
                    baseColor.g * 0.35f,
                    baseColor.b * 0.35f,
                    baseColor.a);
            }
            return;
        }

        StreetEnemy enemy = GetComponent<StreetEnemy>();
        if (enemy != null)
            enemy.React(effect, knockDirection);

        HulkEnemy hulk = GetComponent<HulkEnemy>();
        if (hulk != null)
            hulk.React(effect, knockDirection);

        RickJanitor rick = GetComponent<RickJanitor>();
        if (rick != null)
            rick.React(effect, knockDirection);

        StartFlash();
    }

    void StartFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        SetFlash(1f);
        yield return new WaitForSeconds(hitFlashTime);
        SetFlash(0f);
        flashRoutine = null;
    }

    void SetFlash(float amount)
    {
        if (spriteRenderer == null)
            return;

        if (spriteRenderer.material != null && spriteRenderer.material.HasProperty("_FlashAmount"))
        {
            spriteRenderer.material.SetFloat("_FlashAmount", amount);
            return;
        }

        spriteRenderer.color = amount > 0.5f ? hitColor : baseColor;
    }
}
