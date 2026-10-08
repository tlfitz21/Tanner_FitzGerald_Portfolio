using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WhiteHouseFinale : MonoBehaviour
{
    [SerializeField] Sprite backdrop;
    [SerializeField] Sprite rickSprite;
    [SerializeField] Sprite johnSprite;
    [SerializeField] Sprite alienA;
    [SerializeField] Sprite alienB;
    [SerializeField] Sprite hatA;
    [SerializeField] Sprite hatB;
    [SerializeField] Sprite plainA;
    [SerializeField] Sprite plainB;
    [SerializeField] Sprite bintedSprite;
    [SerializeField] Sprite alienShock;
    [SerializeField] Sprite alienMedal;
    [SerializeField] Sprite alienSuit;

    Camera view;
    Transform rick;
    Transform john;
    Transform alien;
    Transform hatAgent;
    Transform plainAgent;
    SpriteRenderer talker;
    Sprite talkClosed;
    Sprite talkOpen;
    SpriteRenderer dialogueBox;
    SpriteRenderer namePlate;
    TextMesh speakerName;
    TextMesh caption;
    TextMesh advancePrompt;
    SpriteRenderer fade;
    const float FootY = -1.85f;

    void Start()
    {
        view = Camera.main;
        LoadArt();
        BuildStage();
        SpriteMaterialFix.ApplyAll();
        StartCoroutine(Play());
    }

    void LoadArt()
    {
        if (backdrop == null)
            backdrop = LoadSprite("Assets/Sprites/Backdrops/oval-office.png");
        if (rickSprite == null)
            rickSprite = LoadSprite("Assets/Sprites/Rick/rick_full_sprite_0.png");
        if (johnSprite == null)
            johnSprite = LoadSprite("Assets/Sprites/JAN/JohnAmerica/john-america-finalform.png");
        if (alienA == null)
            alienA = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_01.png");
        if (alienB == null)
            alienB = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_02.png");
        if (hatA == null)
            hatA = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_09.png");
        if (hatB == null)
            hatB = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_10.png");
        if (plainA == null)
            plainA = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_07.png");
        if (plainB == null)
            plainB = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_08.png");
        if (bintedSprite == null)
            bintedSprite = LoadSprite("Assets/Sprites/JAN/JohnAmerica/john-binted.png");
        if (alienShock == null)
            alienShock = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_11.png");
        if (alienMedal == null)
            alienMedal = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_03.png");
        if (alienSuit == null)
            alienSuit = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_04.png");
    }

    IEnumerator Play()
    {
        yield return FadeTo(0f, 0.7f);
        yield return new WaitForSeconds(0.45f);
        yield return HopOut();
        yield return Say("Alien", "Rick. Thank you.");
        yield return Say("Rick", "For what? I was just mopping up.");
        yield return Say("Alien", "For carrying me all the way here... For not putting me down.");
        yield return Say("Rick", "No problem, kid. I was just doing my job.");
        yield return Say("Alien", "You may not know it, but you've just changed the course of history.");
        yield return Say("Rick", "The floors here are clean. That's all I need to know.");
        yield return Say("Alien", "I will remember you. Go home, Rick.");
        yield return Say("Rick", "Goodbye, kid.");
        HideNovel();
        yield return Leave(rick, -12f, 1.15f);
        yield return new WaitForSeconds(0.35f);
        hatAgent.gameObject.SetActive(true);
        plainAgent.gameObject.SetActive(true);
        yield return Arrive();
        yield return Say("Agent", "There he is. Honestly wasn't expecting to find him at the finish line.");
        yield return Say("Agent", "The president is down. Good job... but before we go any further...");
        yield return Say("Agent", "Zinky zoogle, zeekybooble beeple meep forp bogos binted?");
        HideNovel();
        ShowBinted();
        yield return Say("John", "Photo's printed");
        Wear(alien, alienShock, 1.7f, -1f);
        yield return Say("Alien", "Vorp?");
        HideNovel();
        yield return Tackle();
        Wear(alien, alienMedal, 1.7f, 1f);
        yield return Say("Agent", "Congratulations. The medal is yours.");
        Wear(alien, alienSuit, 1.7f, 1f);
        yield return Say("Agent", "The Alien CIA is offering you a permanent job.");
        HideNovel();
        yield return FadeTo(1f, 0.8f);
        SceneManager.LoadScene("Credits");
    }

    IEnumerator HopOut()
    {
        if (alien == null || rick == null)
            yield break;

        alien.gameObject.SetActive(true);
        Vector3 start = rick.position + new Vector3(0.25f, 1.35f, 0f);
        Vector3 end = new Vector3(0.15f, 0f, 0f);
        Seat(alien, alienA, end.x, 1.7f, 1f);
        float ground = alien.position.y;
        end.y = ground;
        alien.position = start;
        float elapsed = 0f;
        const float duration = 0.55f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / duration);
            Vector3 place = Vector3.Lerp(start, end, along);
            place.y += Mathf.Sin(along * Mathf.PI) * 1.35f;
            alien.position = place;
            yield return null;
        }

        alien.position = end;
    }

    IEnumerator Leave(Transform actor, float x, float duration)
    {
        if (actor == null)
            yield break;

        Vector3 scale = actor.localScale;
        scale.x = x < actor.position.x ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        actor.localScale = scale;
        Vector3 start = actor.position;
        Vector3 end = start;
        end.x = x;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            actor.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        actor.gameObject.SetActive(false);
    }

    IEnumerator Arrive()
    {
        Vector3 hatStart = hatAgent.position;
        Vector3 plainStart = plainAgent.position;
        Vector3 hatEnd = hatStart;
        Vector3 plainEnd = plainStart;
        hatEnd.x = -2.35f;
        plainEnd.x = -4.55f;
        float elapsed = 0f;
        const float duration = 0.9f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            hatAgent.position = Vector3.Lerp(hatStart, hatEnd, along);
            plainAgent.position = Vector3.Lerp(plainStart, plainEnd, along);
            yield return null;
        }
    }

    IEnumerator Say(string speaker, string line)
    {
        ShowNovel(speaker, line);
        if (speaker == "Alien")
        {
            SpriteRenderer shown = alien != null ? alien.GetComponent<SpriteRenderer>() : null;
            if (shown != null && (shown.sprite == alienA || shown.sprite == alienB))
                SetTalker(alien, alienA, alienB);
            else
                talker = null;
        }
        else if (speaker == "Agent")
        {
            bool hat = caption != null && (caption.text.StartsWith("There") || caption.text.StartsWith("Congratulations") || caption.text.StartsWith("The Alien CIA"));
            SetTalker(hat ? hatAgent : plainAgent, hat ? hatA : plainA, hat ? hatB : plainB);
        }
        else
            talker = null;

        float elapsed = 0f;
        bool ready = false;
        while (!ready)
        {
            elapsed += Time.deltaTime;
            if (talker != null && talkOpen != null)
                talker.sprite = Mathf.FloorToInt(elapsed * 8f) % 2 == 0 ? talkClosed : talkOpen;
            if (elapsed > 0.15f && AdvancePressed())
                ready = true;
            yield return null;
        }

        if (talker != null && talkClosed != null)
            talker.sprite = talkClosed;
    }

    void SetTalker(Transform actor, Sprite closed, Sprite open)
    {
        talker = actor != null ? actor.GetComponent<SpriteRenderer>() : null;
        talkClosed = closed;
        talkOpen = open;
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

        john = Actor("John", 4);
        rick = Actor("Rick", 8);
        alien = Actor("Alien", 12);
        hatAgent = Actor("Agent Hat", 14);
        plainAgent = Actor("Agent Plain", 13);

        PlaceJohn();
        Seat(rick, rickSprite, -2.15f, 4.35f, 1f);
        Seat(alien, alienA, 0.15f, 1.7f, 1f);
        Seat(hatAgent, hatA, -14f, 3.825f, -1f);
        Seat(plainAgent, plainA, -15.6f, 3.825f, -1f);
        alien.gameObject.SetActive(false);
        hatAgent.gameObject.SetActive(false);
        plainAgent.gameObject.SetActive(false);

        dialogueBox = MakePanel(root, "Dialogue Box", new Vector3(0f, -3.45f, 1f), new Vector3(17.2f, 2.55f, 1f), new Color(0.04f, 0.05f, 0.08f, 0.94f), 2000, square);
        namePlate = MakePanel(root, "Name Plate", new Vector3(-5.85f, -1.95f, 1f), new Vector3(3.5f, 0.72f, 1f), new Color(0.55f, 0.28f, 0.18f, 1f), 2001, square);
        speakerName = MakeLine(root, "Speaker", new Vector3(-5.85f, -1.95f, 1f), 0.075f, TextAnchor.MiddleCenter, TextAlignment.Center, 48, 2100);
        caption = MakeLine(root, "Caption", new Vector3(-6.4f, -3.55f, 1f), 0.065f, TextAnchor.MiddleLeft, TextAlignment.Left, 40, 2100);
        advancePrompt = MakeLine(root, "Advance Prompt", new Vector3(6.55f, -4.35f, 1f), 0.042f, TextAnchor.MiddleRight, TextAlignment.Right, 32, 2102);
        advancePrompt.color = new Color(0.75f, 0.75f, 0.75f, 0.55f);
        advancePrompt.text = "Press Space";
        fade = MakePanel(root, "Fade", new Vector3(0f, 0f, 1f), new Vector3(24f, 14f, 1f), Color.black, 3000, square);
        HideNovel();
    }

    void ShowBinted()
    {
        if (john == null)
            return;

        john.rotation = Quaternion.identity;
        Wear(john, bintedSprite, 3.4f, 1f);
        john.position = new Vector3(2.45f, john.position.y, 0f);
    }

    void Wear(Transform actor, Sprite sprite, float height, float faceSign)
    {
        if (actor == null || sprite == null)
            return;

        float x = actor.position.x;
        actor.rotation = Quaternion.identity;
        Seat(actor, sprite, x, height, faceSign);
    }

    IEnumerator Tackle()
    {
        if (plainAgent == null || john == null)
            yield break;

        Vector3 rushFrom = plainAgent.position;
        Vector3 rushTo = john.position + new Vector3(-0.55f, 0f, 0f);
        float elapsed = 0f;
        const float rush = 0.32f;
        while (elapsed < rush)
        {
            elapsed += Time.deltaTime;
            plainAgent.position = Vector3.Lerp(rushFrom, rushTo, Mathf.Clamp01(elapsed / rush));
            yield return null;
        }

        Vector3 plainFrom = plainAgent.position;
        Vector3 johnFrom = john.position;
        elapsed = 0f;
        const float flight = 0.45f;
        while (elapsed < flight)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / flight);
            plainAgent.position = Vector3.Lerp(plainFrom, plainFrom + new Vector3(12f, 0.35f, 0f), along);
            john.position = Vector3.Lerp(johnFrom, johnFrom + new Vector3(12f, 0.55f, 0f), along);
            plainAgent.rotation = Quaternion.Euler(0f, 0f, -35f * along);
            john.rotation = Quaternion.Euler(0f, 0f, 28f * along);
            yield return null;
        }

        plainAgent.gameObject.SetActive(false);
        john.gameObject.SetActive(false);
    }

    void PlaceJohn()
    {
        if (john == null || johnSprite == null)
            return;

        SpriteRenderer renderer = john.GetComponent<SpriteRenderer>();
        renderer.sprite = johnSprite;
        renderer.color = Color.white;
        johnSprite.texture.filterMode = FilterMode.Point;
        float native = Mathf.Max(0.01f, johnSprite.bounds.size.y);
        float scale = 3.6f / native;
        john.localScale = new Vector3(scale, scale, 1f);
        john.position = new Vector3(2.45f, -1.2f, 0f);
        john.rotation = Quaternion.Euler(0f, 0f, -78f);
    }

    static Transform Actor(string actorName, int order)
    {
        GameObject actor = new GameObject(actorName);
        SpriteRenderer renderer = actor.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = order;
        renderer.color = Color.white;
        return actor.transform;
    }

    static void Seat(Transform actor, Sprite sprite, float x, float height, float faceSign)
    {
        if (actor == null || sprite == null)
            return;

        SpriteRenderer renderer = actor.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = Color.white;
        if (sprite.texture != null)
            sprite.texture.filterMode = FilterMode.Point;
        float native = Mathf.Max(0.01f, sprite.bounds.size.y);
        float scale = height / native;
        actor.localScale = new Vector3(scale * faceSign, scale, 1f);
        actor.position = new Vector3(x, FootY - sprite.bounds.min.y * scale, 0f);
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
            if (speaker == "Rick")
                namePlate.color = new Color(0.12f, 0.38f, 0.42f, 1f);
            else if (speaker == "Agent")
                namePlate.color = new Color(0.12f, 0.16f, 0.22f, 1f);
            else if (speaker == "John")
                namePlate.color = new Color(0.45f, 0.32f, 0.12f, 1f);
            else
                namePlate.color = new Color(0.22f, 0.45f, 0.28f, 1f);
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
        {
            advancePrompt.gameObject.SetActive(true);
            advancePrompt.text = "Press Space";
        }
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
        talker = null;
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
