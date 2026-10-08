using UnityEngine;

// The player's camera draws the world into a chunky texture.
// A second camera paints that texture onto Display 1, so the game view has a camera again.
// The status bar is a separate overlay and is not part of this texture.
public class JANDoomView : MonoBehaviour
{
    public const float BarHeight = 168f;

    Camera world;
    Camera display;
    Transform screenQuad;
    RenderTexture buffer;
    float aspect = -1f;
    int blitLayer = 31;

    public static void Install(Camera camera)
    {
        if (camera == null || camera.GetComponent<JANDoomView>() != null)
            return;

        JANDoomView view = camera.gameObject.AddComponent<JANDoomView>();
        view.world = camera;
    }

    void OnDestroy()
    {
        if (world != null)
            world.targetTexture = null;
        if (buffer != null)
        {
            buffer.Release();
            Destroy(buffer);
        }
        if (display != null)
            Destroy(display.gameObject);
    }

    void LateUpdate()
    {
        if (world == null)
            world = GetComponent<Camera>();
        if (world == null)
            return;

        float screenAspect = Screen.height > 0 ? (float)Screen.width / Screen.height : 1.6f;
        if (display == null || buffer == null || Mathf.Abs(screenAspect - aspect) > 0.02f)
            Rebuild(screenAspect);
    }

    void Rebuild(float screenAspect)
    {
        aspect = Mathf.Max(0.5f, screenAspect);
        int height = 200;
        int width = Mathf.Max(64, Mathf.RoundToInt(height * aspect));

        if (buffer != null)
        {
            world.targetTexture = null;
            buffer.Release();
            Destroy(buffer);
        }

        buffer = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        buffer.filterMode = FilterMode.Point;
        buffer.wrapMode = TextureWrapMode.Clamp;
        buffer.Create();

        world.allowMSAA = false;
        world.targetTexture = buffer;
        world.cullingMask &= ~(1 << blitLayer);
        float bar = BarHeight / 1080f;
        world.rect = new Rect(0f, bar, 1f, 1f - bar);

        if (display == null)
        {
            GameObject displayObject = new GameObject("Display Camera");
            display = displayObject.AddComponent<Camera>();
            display.clearFlags = CameraClearFlags.SolidColor;
            display.backgroundColor = Color.black;
            display.orthographic = true;
            display.orthographicSize = 0.5f;
            display.nearClipPlane = 0.01f;
            display.farClipPlane = 5f;
            display.depth = 10f;
            display.allowMSAA = false;
            display.cullingMask = 1 << blitLayer;
            display.transform.position = new Vector3(0f, 0f, -2f);
            EnsureRenderer(displayObject);

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "Doom Blit";
            Destroy(quad.GetComponent<Collider>());
            quad.layer = blitLayer;
            quad.transform.SetParent(display.transform, false);
            quad.transform.localPosition = new Vector3(0f, 0f, 2f);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            screenQuad = quad.transform;
        }

        screenQuad.localScale = new Vector3(-aspect, 1f, 1f);
        Renderer renderer = screenQuad.GetComponent<Renderer>();
        Material material = JANArt.Flat(Color.white);
        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", buffer);
        if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", buffer);
        material.mainTexture = buffer;
        renderer.sharedMaterial = material;
    }

    static void EnsureRenderer(GameObject cameraObject)
    {
        System.Type dataType = System.Type.GetType(
            "UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (dataType != null && cameraObject.GetComponent(dataType) == null)
            cameraObject.AddComponent(dataType);
    }
}
