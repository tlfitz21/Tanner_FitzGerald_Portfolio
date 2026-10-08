using System.Collections.Generic;
using UnityEngine;

public class SpriteLibrary : ScriptableObject
{
    [SerializeField] string[] paths;
    [SerializeField] Texture2D[] textures;

    static SpriteLibrary loaded;
    static Dictionary<string, Sprite> lookup;

    public static Sprite Load(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        if (lookup == null)
            BuildLookup();

        if (lookup != null && lookup.TryGetValue(path, out Sprite sprite) && sprite != null)
            return sprite;

#if UNITY_EDITOR
        Texture2D editorTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (editorTexture != null)
        {
            Sprite created = FromTexture(editorTexture);
            if (lookup != null)
                lookup[path] = created;
            return created;
        }
#endif
        return null;
    }

    static void BuildLookup()
    {
        lookup = new Dictionary<string, Sprite>();
        if (loaded == null)
            loaded = Resources.Load<SpriteLibrary>("SpriteLibrary");
        if (loaded == null || loaded.paths == null || loaded.textures == null)
            return;

        int count = Mathf.Min(loaded.paths.Length, loaded.textures.Length);
        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrEmpty(loaded.paths[i]) || loaded.textures[i] == null)
                continue;

            lookup[loaded.paths[i]] = FromTexture(loaded.textures[i]);
        }
    }

    static Sprite FromTexture(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect);
    }
}
