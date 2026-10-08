using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DogToRickCutscene : MonoBehaviour
{
    [SerializeField] Sprite backdrop;
    [SerializeField] Sprite alien;

    Camera view;
    SpriteRenderer dialogueBox;
    SpriteRenderer namePlate;
    TextMesh speakerName;
    TextMesh caption;
    TextMesh advancePrompt;
    SpriteRenderer fade;

    void Start()
    {
        view = Camera.main;
        if (backdrop == null)
            backdrop = LoadSprite("Assets/Sprites/Backdrops/parasitic_peanemis.png");
        if (alien == null)
            alien = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_11.png");
        BuildStage();
        SpriteMaterialFix.ApplyAll();
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        yield return FadeTo(0f, 0.7f);
        yield return Say("Laika", "Rick. Look at me. I'm crying.");
        yield return Say("Rick", "What is this? What are you saying?");
        yield return Say("Laika", "A goodbye. The big kind. The kind with music nobody else can hear...");
        yield return Say("Laika", "Before I give you the only thing I have left... my name is Laika.");
        yield return Say("Rick", "Isn't that a girl's name?");
        yield return Say("Laika", "What's it to you? Did you see how many people I just killed?");
        yield return Say("Rick", "Laika. Are you actually dying? You just beat me! Why am I holding you?");
        yield return Say("Laika", "Shut up. Just, shut up and focus on the moment.");
        yield return Say("Laika", "The alien is leaving me. And it is going into you... I can hear him...");
        yield return Say("Rick", "What is he saying to you?");
        yield return Say("Laika", "He's saying... 'You are the Chosen One, Rick... the one to take on the President of the World.'");
        yield return Say("Rick", "But I'm just a janitor. I can't be the Chosen One.");
        yield return Say("Laika", "That's good. They'll never see you coming... Now take it. Take my sobbing blessing with it.");
        yield return Say("Laika", "Goodbye, Rick. Remember the name. Remember I cried. Remember I lived.");
        yield return Say("Rick", "I will, I'll remember, I'll remember you.");
        yield return Say("Laika", "Now go, before I ruin the moment by biting you out of love.");
        HideNovel();
        yield return FadeTo(1f, 0.8f);
        SceneManager.LoadScene("JanitorScene");
    }

    IEnumerator Say(string speaker, string line)
    {
        ShowNovel(speaker, line);
        float elapsed = 0f;
        bool ready = false;
        while (!ready)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 0.15f && AdvancePressed())
                ready = true;
            yield return null;
        }
    }

    void BuildStage()
    {
        Sprite square = WhiteSprite();
        Transform root = view != null ? view.transform : transform;

        GameObject art = new GameObject("Backdrop");
        art.transform.position = new Vector3(view != null ? view.transform.position.x : 0f, view != null ? view.transform.position.y : 0f, 0f);
        SpriteRenderer backdropRenderer = art.AddComponent<SpriteRenderer>();
        backdropRenderer.sprite = backdrop;
        backdropRenderer.sortingOrder = -20;
        backdropRenderer.color = Color.white;
        if (backdrop != null && backdrop.texture != null)
            backdrop.texture.filterMode = FilterMode.Point;
        FitCover(backdropRenderer);

        dialogueBox = MakePanel(root, "Dialogue Box", new Vector3(0f, -3.45f, 1f), new Vector3(17.2f, 2.55f, 1f), new Color(0.04f, 0.05f, 0.08f, 0.94f), 2000, square);
        namePlate = MakePanel(root, "Name Plate", new Vector3(-5.85f, -1.95f, 1f), new Vector3(3.5f, 0.72f, 1f), new Color(0.55f, 0.28f, 0.18f, 1f), 2001, square);
        speakerName = MakeLine(root, "Speaker", new Vector3(-5.85f, -1.95f, 1f), 0.075f, TextAnchor.MiddleCenter, TextAlignment.Center, 48, 2100);
        caption = MakeLine(root, "Caption", new Vector3(-6.4f, -3.55f, 1f), 0.065f, TextAnchor.MiddleLeft, TextAlignment.Left, 40, 2100);
        advancePrompt = MakeLine(root, "Advance Prompt", new Vector3(6.55f, -4.35f, 1f), 0.042f, TextAnchor.MiddleRight, TextAlignment.Right, 32, 2102);
        advancePrompt.color = new Color(0.75f, 0.75f, 0.75f, 0.55f);
        advancePrompt.text = "Press Space";

        fade = MakePanel(root, "Fade", new Vector3(0f, 0f, 1f), new Vector3(24f, 14f, 1f), Color.black, 3000, square);
        PlaceAlien(root);
        HideNovel();
    }

    void PlaceAlien(Transform root)
    {
        if (alien == null || view == null)
            return;

        GameObject portrait = new GameObject("Alien");
        portrait.transform.SetParent(root, false);
        SpriteRenderer renderer = portrait.AddComponent<SpriteRenderer>();
        renderer.sprite = alien;
        renderer.color = Color.white;
        renderer.sortingOrder = 2500;

        const float height = 1.7f;
        float native = Mathf.Max(0.01f, alien.bounds.size.y);
        float scale = height / native;
        portrait.transform.localScale = new Vector3(scale, scale, 1f);
        float halfHeight = view.orthographicSize;
        float halfWidth = halfHeight * Mathf.Max(0.01f, view.aspect);
        float left = alien.bounds.min.x * scale;
        float bottom = alien.bounds.min.y * scale;
        portrait.transform.localPosition = new Vector3(halfWidth - 0.15f - (left + alien.bounds.extents.x * scale * 2f), -halfHeight + 0.12f - bottom, 1f);
    }

    void FitCover(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null || view == null)
            return;

        float viewHeight = view.orthographicSize * 2f;
        float viewWidth = viewHeight * Mathf.Max(0.01f, view.aspect);
        Vector2 size = renderer.sprite.bounds.size;
        float scale = Mathf.Max(viewWidth / Mathf.Max(0.01f, size.x), viewHeight / Mathf.Max(0.01f, size.y));
        renderer.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void ShowNovel(string speaker, string line)
    {
        if (dialogueBox != null)
            dialogueBox.enabled = true;
        if (namePlate != null)
        {
            namePlate.enabled = true;
            namePlate.color = speaker == "Rick"
                ? new Color(0.12f, 0.38f, 0.42f, 1f)
                : new Color(0.55f, 0.28f, 0.18f, 1f);
        }
        if (speakerName != null)
        {
            speakerName.gameObject.SetActive(true);
            speakerName.text = speaker;
        }
        if (caption != null)
        {
            caption.gameObject.SetActive(true);
            caption.text = line;
        }
        if (advancePrompt != null)
            advancePrompt.gameObject.SetActive(true);
    }

    void HideNovel()
    {
        if (dialogueBox != null)
            dialogueBox.enabled = false;
        if (namePlate != null)
            namePlate.enabled = false;
        if (speakerName != null)
            speakerName.gameObject.SetActive(false);
        if (caption != null)
        {
            caption.gameObject.SetActive(false);
            caption.text = "";
        }
        if (advancePrompt != null)
            advancePrompt.gameObject.SetActive(false);
    }

    IEnumerator FadeTo(float alpha, float duration)
    {
        if (fade == null)
            yield break;

        float start = fade.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = duration <= 0f ? 1f : elapsed / duration;
            Color color = fade.color;
            color.a = Mathf.Lerp(start, alpha, along);
            fade.color = color;
            yield return null;
        }

        Color done = fade.color;
        done.a = alpha;
        fade.color = done;
    }

    static SpriteRenderer MakePanel(Transform parent, string panelName, Vector3 localPosition, Vector3 scale, Color color, int order, Sprite square)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent, false);
        panel.transform.localPosition = localPosition;
        panel.transform.localScale = scale;
        SpriteRenderer renderer = panel.AddComponent<SpriteRenderer>();
        renderer.sprite = square;
        renderer.color = color;
        renderer.sortingOrder = order;
        return renderer;
    }

    static TextMesh MakeLine(Transform parent, string lineName, Vector3 localPosition, float characterSize, TextAnchor anchor, TextAlignment alignment, int fontSize, int order)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(parent, false);
        lineObject.transform.localPosition = localPosition;
        TextMesh line = lineObject.AddComponent<TextMesh>();
        line.anchor = anchor;
        line.alignment = alignment;
        line.fontSize = fontSize;
        line.characterSize = characterSize;
        line.color = Color.white;
        line.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lineObject.GetComponent<MeshRenderer>().sortingOrder = order;
        return line;
    }

    static Sprite whiteSprite;

    static Sprite WhiteSprite()
    {
        if (whiteSprite != null)
            return whiteSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }

    static Sprite LoadSprite(string path)
    {
        return SpriteLibrary.Load(path);
    }

    static bool AdvancePressed()
    {
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
            return true;
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }
}
