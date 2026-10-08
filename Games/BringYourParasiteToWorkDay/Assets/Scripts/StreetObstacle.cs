using System.Collections.Generic;
using UnityEngine;

public class StreetObstacle : MonoBehaviour
{
    public enum Kind
    {
        Blockade,
        Car,
        Manhole,
        Gate,
        Bus
    }

    static readonly List<StreetObstacle> solids = new List<StreetObstacle>();
    static Sprite square;

    [SerializeField] Kind kind;
    [SerializeField] float manholeDamage = 6f;
    [SerializeField] float manholeInterval = 0.55f;

    Collider2D area;
    float nextManholeHit;
    bool useTransformShape;

    public static Sprite Square
    {
        get
        {
            if (square != null)
                return square;

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            square = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return square;
        }
    }

    public static StreetObstacle Create(Kind kind, Vector2 position, Vector2 size)
    {
        GameObject root = new GameObject(kind.ToString());
        root.transform.position = position;

        BoxCollider2D box = root.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = size;

        StreetObstacle obstacle = root.AddComponent<StreetObstacle>();
        obstacle.Setup(kind, size);
        return obstacle;
    }

    public static Vector2 ResolveMovement(Vector2 from, Vector2 to, Vector2 extents)
    {
        if (!Blocked(to, extents))
            return to;

        Vector2 alongX = new Vector2(to.x, from.y);
        Vector2 alongY = new Vector2(from.x, to.y);
        bool xFree = !Blocked(alongX, extents);
        bool yFree = !Blocked(alongY, extents);
        if (xFree && yFree)
            return Mathf.Abs(to.x - from.x) >= Mathf.Abs(to.y - from.y) ? alongX : alongY;
        if (xFree)
            return alongX;
        if (yFree)
            return alongY;
        return from;
    }

    void Awake()
    {
        area = GetComponent<Collider2D>();
    }

    void Setup(Kind value, Vector2 size)
    {
        kind = value;
        if (kind == Kind.Manhole)
            solids.Remove(this);
        else if (!solids.Contains(this))
            solids.Add(this);

        BuildVisual(size);
    }

    void OnEnable()
    {
        if (kind != Kind.Manhole && !solids.Contains(this))
            solids.Add(this);
    }

    void OnDisable()
    {
        solids.Remove(this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (kind != Kind.Manhole || Time.time < nextManholeHit)
            return;

        DogHealth health = other.GetComponent<DogHealth>();
        if (health == null || health.IsDead)
            return;

        nextManholeHit = Time.time + manholeInterval;
        health.ApplyDamage(manholeDamage, "Manhole");
    }

    void BuildVisual(Vector2 size)
    {
        Color color = kind switch
        {
            Kind.Car => new Color(0.46f, 0.16f, 0.13f, 1f),
            Kind.Manhole => new Color(0.08f, 0.08f, 0.1f, 1f),
            Kind.Gate => new Color(0.18f, 0.16f, 0.14f, 1f),
            Kind.Bus => new Color(0.86f, 0.62f, 0.12f, 1f),
            _ => new Color(0.38f, 0.34f, 0.28f, 1f)
        };

        GameObject graphic = new GameObject("Visual");
        graphic.transform.SetParent(transform, false);
        graphic.transform.localScale = new Vector3(size.x, size.y, 1f);
        SpriteRenderer renderer = graphic.AddComponent<SpriteRenderer>();
        renderer.sprite = Square;
        renderer.color = color;
        renderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100f);

        string tag = kind switch
        {
            Kind.Car => "CAR",
            Kind.Manhole => "MANHOLE",
            Kind.Gate => "BLOCKED",
            Kind.Bus => "BUS",
            _ => "BLOCKADE"
        };

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(transform, false);
        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = tag;
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 32;
        label.characterSize = 0.1f;
        label.color = kind == Kind.Manhole || kind == Kind.Gate
            ? new Color(1f, 0.85f, 0.25f, 1f)
            : Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        float textWidth = Mathf.Max(1f, tag.Length * 0.28f);
        float fit = size.x * 0.82f / textWidth;
        labelObject.transform.localScale = Vector3.one * Mathf.Clamp(fit, 0.35f, 1.1f);

        MeshRenderer mesh = labelObject.GetComponent<MeshRenderer>();
        mesh.sortingOrder = renderer.sortingOrder + 1;
    }

    public void SealInPlace()
    {
        BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
        if (box == null)
            box = gameObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = Vector2.one;
        area = box;
        useTransformShape = true;
        kind = Kind.Car;
        if (!solids.Contains(this))
            solids.Add(this);
    }

    static bool Blocked(Vector2 point, Vector2 extents)
    {
        for (int i = 0; i < solids.Count; i++)
        {
            StreetObstacle obstacle = solids[i];
            if (obstacle.area == null)
                continue;

            if (obstacle.useTransformShape)
            {
                if (obstacle.Covers(point, extents))
                    return true;
                continue;
            }

            Bounds bounds = obstacle.area.bounds;
            bounds.Expand(extents * 2f);
            if (bounds.Contains(point))
                return true;
        }

        return false;
    }

    bool Covers(Vector2 point, Vector2 extents)
    {
        Vector2 axisX = transform.right;
        Vector2 axisY = transform.up;
        Vector2 offset = point - (Vector2)transform.position;
        float alongX = Vector2.Dot(offset, axisX);
        float alongY = Vector2.Dot(offset, axisY);
        float halfX = transform.lossyScale.x * 0.5f
            + Mathf.Abs(extents.x * axisX.x) + Mathf.Abs(extents.y * axisX.y);
        float halfY = transform.lossyScale.y * 0.5f
            + Mathf.Abs(extents.x * axisY.x) + Mathf.Abs(extents.y * axisY.y);
        return Mathf.Abs(alongX) <= halfX && Mathf.Abs(alongY) <= halfY;
    }

    public void Break()
    {
        Bounds footprint = area != null ? area.bounds : new Bounds(transform.position, new Vector3(2f, 6f, 0f));
        solids.Remove(this);
        if (area != null)
            area.enabled = false;

        for (int i = 0; i < 14; i++)
        {
            float x = footprint.center.x + Random.Range(-3.2f, 3.2f);
            float span = Mathf.Max(1.5f, footprint.size.y);
            float y = footprint.center.y + Random.Range(-span * 0.55f, span * 0.55f);
            GameObject bit = new GameObject("Rubble");
            bit.transform.position = new Vector3(x, y, 0f);
            bit.transform.localScale = Vector3.one * Random.Range(0.28f, 0.8f);
            bit.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            SpriteRenderer bitRenderer = bit.AddComponent<SpriteRenderer>();
            bitRenderer.sprite = Square;
            float shade = Random.Range(0.3f, 0.62f);
            bitRenderer.color = new Color(shade, shade * 0.9f, shade * 0.75f, 1f);
            bitRenderer.sortingOrder = -Mathf.RoundToInt(y * 100f);
        }

        Destroy(gameObject);
    }
}
