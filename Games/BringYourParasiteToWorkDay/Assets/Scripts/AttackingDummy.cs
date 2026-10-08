using System.Collections;
using UnityEngine;

public class AttackingDummy : MonoBehaviour
{
    [SerializeField] float attackInterval = 3f;
    [SerializeField] int facing = 1;
    [SerializeField] bool heavy;
    [SerializeField] float telegraphTime = 0.4f;
    [SerializeField] float telegraphPulses = 2.5f;
    [SerializeField] float active = 0.18f;
    [SerializeField] float bodyOverlap = 0.35f;
    [SerializeField] float reach = 1.15f;
    [SerializeField] float hitboxHeight = 0.9f;
    [SerializeField] Color attackTint = new Color(1f, 0.55f, 0.2f, 1f);
    [SerializeField] Color telegraphColor = new Color(1f, 0.86f, 0.25f, 0.45f);
    [SerializeField] LayerMask hitMask = ~0;

    Collider2D body;
    SpriteRenderer spriteRenderer;
    Color baseColor;
    DogHealth dog;
    float timer;
    bool swinging;

    void Awake()
    {
        body = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            baseColor = spriteRenderer.color;

        facing = facing >= 0 ? 1 : -1;
    }

    void Update()
    {
        if (dog == null)
            dog = FindAnyObjectByType<DogHealth>();
        if (dog != null && dog.IsDead)
            return;

        timer += Time.deltaTime;
        if (swinging || timer < attackInterval)
            return;

        timer = 0f;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        swinging = true;
        ApplyTint(attackTint);

        Vector2 center = HitboxCenter(out Vector2 size);
        Vector2 warningSize = heavy ? size * 1.2f : size;
        if (telegraphTime > 0f)
            yield return StartCoroutine(ShowTelegraph(center, warningSize));

        if (dog != null && dog.IsDead)
        {
            EndSwing();
            yield break;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, hitMask);
        bool damagedDog = false;
        for (int i = 0; i < hits.Length; i++)
        {
            DogHealth health = hits[i].GetComponent<DogHealth>();
            if (health == null || damagedDog)
                continue;

            health.ReceiveAttack(heavy);
            damagedDog = true;
        }

        yield return StartCoroutine(ShowHitbox(center, size, active));
        EndSwing();
    }

    IEnumerator ShowTelegraph(Vector2 center, Vector2 size)
    {
        GameObject marker = new GameObject(heavy ? "Heavy Telegraph" : "Light Telegraph");
        marker.transform.position = center;

        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDebugSprite();
        renderer.sortingOrder = 29000;

        float elapsed = 0f;
        while (elapsed < telegraphTime)
        {
            float pulse = Mathf.PingPong((elapsed / telegraphTime) * telegraphPulses, 1f);
            float scale = 0.72f + 0.28f * pulse;
            marker.transform.localScale = new Vector3(size.x * scale, size.y * scale, 1f);
            Color color = telegraphColor;
            color.a = telegraphColor.a * (0.35f + 0.65f * pulse);
            renderer.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(marker);
    }

    void EndSwing()
    {
        swinging = false;
        ApplyTint(Color.white);
    }

    Vector2 HitboxCenter(out Vector2 size)
    {
        float halfWidth = body != null ? Mathf.Max(0.05f, body.bounds.extents.x) : 0.4f;
        float overlap = Mathf.Clamp(bodyOverlap, 0f, halfWidth);
        float inner = halfWidth - overlap;
        float outer = halfWidth + Mathf.Max(0.05f, reach);
        float width = outer - inner;
        float mid = (inner + outer) * 0.5f;

        size = new Vector2(width, hitboxHeight);
        return (Vector2)transform.position + new Vector2(facing * mid, 0f);
    }

    IEnumerator ShowHitbox(Vector2 center, Vector2 size, float lifetime)
    {
        GameObject box = new GameObject("Dummy Attack Hitbox");
        box.transform.position = center;
        box.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = box.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDebugSprite();
        renderer.color = new Color(0.35f, 0.55f, 1f, 0.45f);
        renderer.sortingOrder = 30000;

        if (lifetime > 0f)
            yield return new WaitForSeconds(lifetime);

        Destroy(box);
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
