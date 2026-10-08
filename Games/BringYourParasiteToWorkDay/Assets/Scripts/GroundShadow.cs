using UnityEngine;

public static class GroundShadow
{
    static Sprite blob;

    public static SpriteRenderer Attach(Transform owner, float footY, float width, float depth)
    {
        Transform existing = owner.Find("Shadow");
        if (existing != null)
            return existing.GetComponent<SpriteRenderer>();

        GameObject shadow = new GameObject("Shadow");
        shadow.transform.SetParent(owner, false);
        Vector3 lossy = owner.lossyScale;
        float scaleX = Mathf.Abs(lossy.x) < 0.01f ? 1f : Mathf.Abs(lossy.x);
        float scaleY = Mathf.Abs(lossy.y) < 0.01f ? 1f : Mathf.Abs(lossy.y);
        shadow.transform.localPosition = new Vector3(0f, footY, 0f);
        shadow.transform.localScale = new Vector3(width / scaleX, depth / scaleY, 1f);

        SpriteRenderer renderer = shadow.AddComponent<SpriteRenderer>();
        renderer.sprite = Blob;
        renderer.color = new Color(0f, 0f, 0f, 0.4f);
        Sort(renderer, owner.position.y, 0);
        return renderer;
    }

    public static void Sort(SpriteRenderer shadow, float worldY, int offset)
    {
        if (shadow == null)
            return;

        shadow.sortingOrder = offset - Mathf.RoundToInt(worldY * 100f) - 2;
    }

    static Sprite Blob
    {
        get
        {
            if (blob != null)
                return blob;

            const int size = 32;
            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size * 2f - 1f;
                    float ny = (y + 0.5f) / size * 2f - 1f;
                    float falloff = Mathf.Clamp01(1f - (nx * nx + ny * ny));
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, falloff * falloff));
                }
            }

            texture.Apply();
            blob = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            return blob;
        }
    }
}
