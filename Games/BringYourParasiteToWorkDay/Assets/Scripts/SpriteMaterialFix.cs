using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteMaterialFix : MonoBehaviour
{
    static Material unlit;
    static SpriteMaterialFix instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null)
            return;

        GameObject host = new GameObject("Sprite Material Fix");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<SpriteMaterialFix>();
        SceneManager.sceneLoaded += (_, __) => ApplyAll();
    }

    void Start()
    {
        ApplyAll();
    }

    public static void ApplyAll()
    {
        if (unlit == null)
            unlit = ResolveMaterial();
        if (unlit == null)
            return;

        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include);
        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer renderer = renderers[i];
            if (renderer == null)
                continue;

            Material current = renderer.sharedMaterial;
            if (current != null && current.shader != null)
            {
                string name = current.shader.name;
                if (name.Contains("Sprite-Unlit") || name == "Universal Render Pipeline/2D/Sprite-Unlit-Default")
                    continue;
            }

            renderer.sharedMaterial = unlit;
        }
    }

    static Material ResolveMaterial()
    {
        Material fromAssets = Resources.Load<Material>("URPSpriteUnlit");
        if (fromAssets != null)
            return fromAssets;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");
        if (shader == null)
            return null;

        Material mat = new Material(shader);
        mat.color = Color.white;
        return mat;
    }
}
