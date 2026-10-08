using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsRoll : MonoBehaviour
{
    Camera view;
    SpriteRenderer fade;
    TextMesh story;
    TextMesh theEnd;
    Transform roll;
    TextMesh credits;
    TextMesh jamNote;

    void Start()
    {
        view = Camera.main;
        BuildStage();
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        yield return FadeTo(0f, 0.7f);
        story.gameObject.SetActive(true);
        story.text = "And Glorp lived happily ever after,\nhis mom made him alien-spaghetti and meatballs\nto celebrate him finally having a job!";
        yield return WaitAdvance();
        story.gameObject.SetActive(false);
        theEnd.gameObject.SetActive(true);
        yield return WaitAdvance();
        theEnd.gameObject.SetActive(false);
        roll.gameObject.SetActive(true);
        yield return ScrollCredits();
    }

    IEnumerator WaitAdvance()
    {
        float elapsed = 0f;
        while (true)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 0.2f && AdvancePressed())
                yield break;
            yield return null;
        }
    }

    IEnumerator ScrollCredits()
    {
        Vector3 start = new Vector3(0f, -7.2f, 1f);
        Vector3 end = new Vector3(0f, 8.5f, 1f);
        roll.localPosition = start;
        float duration = 18f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / duration);
            roll.localPosition = Vector3.Lerp(start, end, along);
            if (AdvancePressed() && elapsed > 0.4f)
                elapsed = Mathf.Min(duration, elapsed + Time.deltaTime * 8f);
            yield return null;
        }

        roll.localPosition = end;
    }

    void BuildStage()
    {
        Transform root = view != null ? view.transform : transform;
        Sprite square = WhiteSprite();

        GameObject backdrop = new GameObject("Backdrop");
        backdrop.transform.SetParent(root, false);
        backdrop.transform.localPosition = new Vector3(0f, 0f, 1f);
        backdrop.transform.localScale = new Vector3(24f, 14f, 1f);
        SpriteRenderer backdropRenderer = backdrop.AddComponent<SpriteRenderer>();
        backdropRenderer.sprite = square;
        backdropRenderer.color = new Color(0.04f, 0.05f, 0.08f, 1f);
        backdropRenderer.sortingOrder = -20;

        story = MakeLine(root, "Story", new Vector3(0f, 0.4f, 1f), 0.085f, TextAnchor.MiddleCenter, TextAlignment.Center, 42, 2100);
        story.anchor = TextAnchor.MiddleCenter;
        story.alignment = TextAlignment.Center;
        story.gameObject.SetActive(false);

        theEnd = MakeLine(root, "The End", new Vector3(0f, 0.2f, 1f), 0.16f, TextAnchor.MiddleCenter, TextAlignment.Center, 64, 2100);
        theEnd.text = "The End";
        theEnd.gameObject.SetActive(false);

        roll = new GameObject("Credit Roll").transform;
        roll.SetParent(root, false);
        roll.localPosition = new Vector3(0f, -7.2f, 1f);
        roll.gameObject.SetActive(false);

        credits = MakeLine(roll, "Credits", new Vector3(0f, 0f, 0f), 0.09f, TextAnchor.MiddleCenter, TextAlignment.Center, 48, 2101);
        credits.text =
            "Programmers, Artists, Writers,\nand everything else under the Sun:\n\n" +
            "David O'Regan\n" +
            "Tanner FitzGerald";

        jamNote = MakeLine(roll, "Jam Note", new Vector3(0f, -3.4f, 0f), 0.055f, TextAnchor.MiddleCenter, TextAlignment.Center, 36, 2101);
        jamNote.color = new Color(0.78f, 0.8f, 0.84f, 1f);
        jamNote.text = "This build was produced for the\nVirginia Tech Game Programming Club's\nFall 2026 Game Jam";

        fade = MakePanel(root, "Fade", new Vector3(0f, 0f, 1f), new Vector3(24f, 14f, 1f), Color.black, 3000, square);
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

    static bool AdvancePressed()
    {
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
            return true;
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }
}
