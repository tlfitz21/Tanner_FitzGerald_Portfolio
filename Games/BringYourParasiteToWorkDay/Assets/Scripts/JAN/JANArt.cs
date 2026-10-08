using UnityEngine;

public static class JANArt
{
    static Sprite whiteSprite;
    static Texture2D whiteTexture;
    static Shader billboardShader;

    public static Sprite White()
    {
        if (whiteSprite != null)
            return whiteSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
        return whiteSprite;
    }

    public static Material Lit(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        return Paint(new Material(shader), color, 0.08f);
    }

    public static Material Flat(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        Material mat = Paint(new Material(shader), color, 0f);
        if (mat.HasProperty("_Cull"))
            mat.SetFloat("_Cull", 0f);
        return mat;
    }

    // Billboard PNGs need a cutout shader that ships in Resources — URP Unlit alpha-clip gets stripped in builds.
    public static Material SpriteMaterial()
    {
        Shader shader = BillboardShader();
        if (shader == null)
            return Flat(Color.white);

        Material mat = new Material(shader);
        mat.SetFloat("_Cutoff", 0.5f);
        return Paint(mat, Color.white, 0f);
    }

    public static void PaintSprite(Material mat, Sprite sprite)
    {
        if (mat == null || sprite == null)
            return;

        Texture2D tex = sprite.texture;
        tex.filterMode = FilterMode.Point;
        Rect rect = sprite.textureRect;
        Vector2 scale = new Vector2(rect.width / tex.width, rect.height / tex.height);
        Vector2 offset = new Vector2(rect.x / tex.width, rect.y / tex.height);
        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", tex);
            mat.SetTextureScale("_BaseMap", scale);
            mat.SetTextureOffset("_BaseMap", offset);
        }
        if (mat.HasProperty("_MainTex"))
        {
            mat.SetTexture("_MainTex", tex);
            mat.SetTextureScale("_MainTex", scale);
            mat.SetTextureOffset("_MainTex", offset);
        }
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", Color.white);
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", Color.white);
        if (mat.HasProperty("_Cutoff"))
            mat.SetFloat("_Cutoff", 0.5f);
        mat.color = Color.white;
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
    }

    static Shader BillboardShader()
    {
        if (billboardShader != null)
            return billboardShader;

        billboardShader = Resources.Load<Shader>("JANBillboardSprite");
        if (billboardShader == null)
            billboardShader = Shader.Find("JAN/BillboardSprite");
        return billboardShader;
    }

    static Material Paint(Material mat, Color color, float smoothness)
    {
        Texture2D white = WhiteTexture();
        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", white);
        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", white);
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
        if (smoothness > 0f && mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", smoothness);
        mat.color = color;
        return mat;
    }

    static Texture2D WhiteTexture()
    {
        if (whiteTexture != null)
            return whiteTexture;

        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        return whiteTexture;
    }

    public static GameObject Cube(string name, Vector3 position, Vector3 scale, Color color, Transform parent)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, true);
        box.transform.position = position;
        box.transform.localScale = scale;
        box.GetComponent<Renderer>().sharedMaterial = Flat(color);
        return box;
    }

    public static GameObject Quad(string name, Color color, Transform parent)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);
        quad.GetComponent<Renderer>().sharedMaterial = Flat(color);
        return quad;
    }
}
