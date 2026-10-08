using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AlienIntroCutscene : MonoBehaviour
{
    [SerializeField] Camera view;
    [SerializeField] SpriteRenderer fade;
    [SerializeField] TextMesh caption;
    [SerializeField] TextMesh title;
    [SerializeField] Transform planet;
    [SerializeField] SpriteRenderer[] stars;

    [SerializeField] Transform mom;
    [SerializeField] SpriteRenderer momRenderer;
    [SerializeField] Sprite momClosed;
    [SerializeField] Sprite momOpen;

    [SerializeField] Transform kid;
    [SerializeField] SpriteRenderer kidRenderer;
    [SerializeField] Sprite kidIdle;
    [SerializeField] Sprite kidTalk;
    [SerializeField] Sprite kidShock;

    [SerializeField] Transform streetKid;
    [SerializeField] SpriteRenderer streetKidRenderer;

    [SerializeField] Transform agentHat;
    [SerializeField] SpriteRenderer agentHatRenderer;
    [SerializeField] Sprite hatClosed;
    [SerializeField] Sprite hatOpen;

    [SerializeField] Transform agentPlain;
    [SerializeField] SpriteRenderer agentPlainRenderer;
    [SerializeField] Sprite plainClosed;
    [SerializeField] Sprite plainOpen;

    [SerializeField] Transform van;
    [SerializeField] Transform vanHold;
    [SerializeField] Transform rocket;
    [SerializeField] Transform flame;
    [SerializeField] SpriteRenderer twinkle;

    SpriteRenderer dialogueBox;
    SpriteRenderer namePlate;
    TextMesh speakerName;
    TextMesh advancePrompt;
    SpriteRenderer[] choicePlates;
    TextMesh[] choiceLabels;
    Vector3 captionHome;
    GameObject letterboxTop;
    GameObject letterboxBottom;
    const float FootY = -2.05f;
    const float PortraitHeight = 5.7f;
    const float AgentHeight = PortraitHeight * 0.85f;

    void Awake()
    {
        SetFade(1f);
        SetTextAlpha(title, 0f);
        if (caption != null)
            caption.text = "";
        SetSparkAlpha(0f);
        letterboxTop = GameObject.Find("Letterbox Top");
        letterboxBottom = GameObject.Find("Letterbox Bottom");
        BuildNovelChrome();
    }

    void Start()
    {
        BindArt();
        SpriteMaterialFix.ApplyAll();
        StartCoroutine(Play());
    }

    void BindArt()
    {
        kidIdle = Prefer(kidIdle, "Assets/Sprites/Aliens/Layer 1_sprite_01.png");
        kidTalk = Prefer(kidTalk, "Assets/Sprites/Aliens/Layer 1_sprite_02.png");
        kidShock = Prefer(kidShock, "Assets/Sprites/Aliens/Layer 1_sprite_11.png");
        momClosed = Prefer(momClosed, "Assets/Sprites/Aliens/Layer 1_sprite_05.png");
        momOpen = Prefer(momOpen, "Assets/Sprites/Aliens/Layer 1_sprite_06.png");
        hatClosed = Prefer(hatClosed, "Assets/Sprites/Aliens/Layer 1_sprite_09.png");
        hatOpen = Prefer(hatOpen, "Assets/Sprites/Aliens/Layer 1_sprite_10.png");
        plainClosed = Prefer(plainClosed, "Assets/Sprites/Aliens/Layer 1_sprite_07.png");
        plainOpen = Prefer(plainOpen, "Assets/Sprites/Aliens/Layer 1_sprite_08.png");
        if (kidRenderer != null && kidIdle != null)
            kidRenderer.sprite = kidIdle;
        if (momRenderer != null && momClosed != null)
            momRenderer.sprite = momClosed;
        if (agentHatRenderer != null && hatClosed != null)
            agentHatRenderer.sprite = hatClosed;
        if (agentPlainRenderer != null && plainClosed != null)
            agentPlainRenderer.sprite = plainClosed;
        if (streetKidRenderer != null && kidIdle != null)
            streetKidRenderer.sprite = kidIdle;
    }

    static Sprite Prefer(Sprite current, string path)
    {
        Sprite loaded = SpriteLibrary.Load(path);
        return loaded != null ? loaded : current;
    }

    IEnumerator Play()
    {
        yield return SpaceShot();
        yield return CutTo(new Vector3(30f, 0f, -10f));
        yield return Bedroom();
        yield return CutTo(new Vector3(60f, 0f, -10f));
        yield return Street();
        yield return CutTo(new Vector3(90f, 0f, -10f));
        yield return Launch();
        yield return FadeTo(1f, 0.8f);
        SceneManager.LoadScene("SheepStealth");
    }

    IEnumerator SpaceShot()
    {
        ShowPlanet();
        yield return FadeTo(0f, 0.7f);
        Vector3 planetStart = planet != null ? planet.localPosition : Vector3.zero;
        float size0 = view != null ? view.orthographicSize : 5f;
        float duration = 5.4f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / duration);
            if (planet != null)
                planet.localPosition = planetStart + Vector3.up * (0.4f * along);
            if (view != null)
                view.orthographicSize = Mathf.Lerp(size0, size0 - 0.4f, along);
            SetTextAlpha(title, Mathf.Clamp01((elapsed - 0.7f) / 1.15f));
            TwinkleStars(elapsed);
            yield return null;
        }

        yield return new WaitForSeconds(1.7f);
    }

    IEnumerator Bedroom()
    {
        SetLetterbox(false);
        FitCover("Bedroom Backdrop");
        EnsureSprite(kidRenderer, "Assets/Sprites/Aliens/Layer 1_sprite_01.png");
        EnsureSprite(momRenderer, "Assets/Sprites/Aliens/Layer 1_sprite_05.png");
        Seat(kid, kidRenderer, 3.7f, 1);
        Seat(mom, momRenderer, -12f, -1);
        Dim(kidRenderer, false);
        Dim(momRenderer, false);

        Vector3 home = kid != null ? kid.localPosition : Vector3.zero;
        float elapsed = 0f;
        while (elapsed < 1.1f)
        {
            elapsed += Time.deltaTime;
            if (kid != null)
                kid.localPosition = home + Vector3.up * Mathf.Sin(elapsed * 2.2f) * 0.05f;
            yield return null;
        }

        if (kid != null)
            kid.localPosition = home;
        if (mom != null)
            yield return MoveLocal(mom, PortraitPlace(momRenderer, -3.55f), 1.15f);

        yield return Say(
            "Mom",
            "You sit around all do and do nothing!\nYou need to get a job, NOW!",
            momRenderer, momClosed, momOpen, kidRenderer);

        HideNovel();
        Sprite shock = LoadSprite("Assets/Sprites/Aliens/Layer 1_sprite_11.png");
        if (shock != null)
            kidShock = shock;
        if (kidRenderer != null && kidShock != null)
        {
            kidRenderer.sprite = kidShock;
            Seat(kid, kidRenderer, 3.7f, 1);
        }
        Dim(kidRenderer, false);
        Dim(momRenderer, true);
        yield return new WaitForSeconds(0.9f);

        yield return Say(
            "You",
            "But mom, I have no skills, I'm useless!",
            kidRenderer, kidIdle, kidTalk, momRenderer);
        yield return Say(
            "Mom",
            "I don't care! Just get a job, no matter how ODD it is!",
            momRenderer, momClosed, momOpen, kidRenderer);
        HideNovel();

        if (mom != null)
            yield return MoveLocal(mom, new Vector3(-12f, mom.localPosition.y, 0f), 1f);
        if (mom != null)
            mom.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        if (kid != null)
            yield return MoveLocal(kid, new Vector3(-12f, kid.localPosition.y, 0f), 1.05f);
    }

    IEnumerator Street()
    {
        SetLetterbox(false);
        FitCover("Street Backdrop");
        EnsureSprite(streetKidRenderer, "Assets/Sprites/Aliens/Layer 1_sprite_01.png");
        EnsureSprite(agentHatRenderer, "Assets/Sprites/Aliens/Layer 1_sprite_09.png");
        EnsureSprite(agentPlainRenderer, "Assets/Sprites/Aliens/Layer 1_sprite_07.png");
        Seat(streetKid, streetKidRenderer, -11f, -1);
        Seat(agentHat, agentHatRenderer, -14f, -1, AgentHeight);
        Seat(agentPlain, agentPlainRenderer, -15.6f, -1, AgentHeight);
        if (agentHatRenderer != null)
            agentHatRenderer.sortingOrder = 12;
        if (agentPlainRenderer != null)
            agentPlainRenderer.sortingOrder = 11;
        Dim(streetKidRenderer, false);
        Dim(agentHatRenderer, false);
        Dim(agentPlainRenderer, false);

        if (streetKid != null)
        {
            yield return MoveLocal(streetKid, PortraitPlace(streetKidRenderer, 3.7f), 1f);
            Seat(streetKid, streetKidRenderer, 3.7f, 1);
        }
        if (agentHat != null)
            yield return MovePair(
                agentHat, PortraitPlace(agentHatRenderer, -2.35f, AgentHeight),
                agentPlain, PortraitPlace(agentPlainRenderer, -4.7f, AgentHeight),
                1.15f);

        yield return Say("Agent", "We have an odd job for you.", agentHatRenderer, hatClosed, hatOpen, streetKidRenderer);
        Dim(agentPlainRenderer, false);
        yield return Say("Agent", "It will take other odd jobs.", agentPlainRenderer, plainClosed, plainOpen, agentHatRenderer);
        yield return Choose(
            "Agent",
            "Will you take it?",
            "I'll do it.",
            "I don't have a choice, do I?",
            agentHatRenderer, hatClosed, hatOpen, streetKidRenderer);
        Dim(streetKidRenderer, true);
        yield return Say("Agent", "We are sending you to the planet known as E-Arth.", agentHatRenderer, hatClosed, hatOpen, agentPlainRenderer);
        yield return Say("Agent", "There, you start as the lowliest of creatures.", agentPlainRenderer, plainClosed, plainOpen, agentHatRenderer);
        yield return Say("Agent", "Pass through nobler creatures until you reach the chosen one host.", agentHatRenderer, hatClosed, hatOpen, agentPlainRenderer);
        yield return Say("Agent", "That host will let you take on what humans call 'the president.'", agentPlainRenderer, plainClosed, plainOpen, agentHatRenderer);
        yield return Say("Agent", "Intel suggests he is the emperor of the world.", agentHatRenderer, hatClosed, hatOpen, agentPlainRenderer);
        HideNovel();
        Dim(streetKidRenderer, false);
        Dim(agentHatRenderer, false);
        Dim(agentPlainRenderer, false);

        if (van != null)
            yield return MoveLocal(van, new Vector3(4.4f, van.localPosition.y, 0f), 0.85f);
        if (streetKid != null && vanHold != null)
            yield return ThrowIntoVan();
        if (van != null)
            yield return MoveLocal(van, new Vector3(22f, van.localPosition.y, 0f), 1.35f);
    }

    IEnumerator ThrowIntoVan()
    {
        Vector3 start = streetKid.position;
        Vector3 end = vanHold.position;
        float elapsed = 0f;
        const float duration = 0.55f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / duration);
            Vector3 place = Vector3.Lerp(start, end, along);
            place.y += Mathf.Sin(along * Mathf.PI) * 1.35f;
            streetKid.position = place;
            streetKid.rotation = Quaternion.Euler(0f, 0f, -along * 220f);
            yield return null;
        }

        streetKid.gameObject.SetActive(false);
    }

    IEnumerator Launch()
    {
        SetLetterbox(true);
        HideNovel();
        Vector3 flameScale = flame != null ? flame.localScale : Vector3.one;
        float elapsed = 0f;
        while (elapsed < 0.55f)
        {
            elapsed += Time.deltaTime;
            if (flame != null)
            {
                float pulse = 0.75f + Mathf.PingPong(elapsed * 8f, 0.45f);
                flame.localScale = new Vector3(flameScale.x * pulse, flameScale.y * pulse, 1f);
            }
            yield return null;
        }

        Vector3 rocketStart = rocket != null ? rocket.localPosition : Vector3.zero;
        elapsed = 0f;
        const float flight = 2.5f;
        while (elapsed < flight)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.Clamp01(elapsed / flight);
            float rise = along * along;
            if (rocket != null)
            {
                rocket.localPosition = rocketStart + Vector3.up * (9.5f * rise);
                float size = Mathf.Lerp(1f, 0.22f, rise);
                rocket.localScale = new Vector3(size, size, 1f);
            }
            if (flame != null)
            {
                float pulse = 1f + Mathf.PingPong(elapsed * 10f, 0.55f);
                flame.localScale = new Vector3(flameScale.x * pulse, flameScale.y * (1.3f + rise) * pulse, 1f);
            }
            yield return null;
        }

        if (rocket != null)
            rocket.gameObject.SetActive(false);

        if (twinkle == null)
            yield break;

        Transform spark = twinkle.transform.parent != null ? twinkle.transform.parent : twinkle.transform;
        elapsed = 0f;
        const float sparkTime = 1.35f;
        while (elapsed < sparkTime)
        {
            elapsed += Time.deltaTime;
            float along = elapsed / sparkTime;
            float pop = Mathf.Sin(Mathf.Clamp01(along) * Mathf.PI);
            spark.localScale = Vector3.one * Mathf.Lerp(0.15f, 1.15f, pop);
            SetSparkAlpha(pop);
            spark.Rotate(0f, 0f, 40f * Time.deltaTime);
            yield return null;
        }
    }

    void SetSparkAlpha(float alpha)
    {
        if (twinkle == null)
            return;

        Transform spark = twinkle.transform.parent != null ? twinkle.transform.parent : twinkle.transform;
        SpriteRenderer[] pieces = spark.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < pieces.Length; i++)
        {
            Color color = pieces[i].color;
            color.a = alpha;
            pieces[i].color = color;
        }
    }

    IEnumerator Say(string speaker, string line, SpriteRenderer mouth, Sprite closed, Sprite open, SpriteRenderer listener)
    {
        ShowNovel(speaker, line);
        Dim(mouth, false);
        Dim(listener, true);
        float elapsed = 0f;
        bool ready = false;
        while (!ready)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 0.15f && AdvancePressed())
                ready = true;
            if (mouth != null && closed != null && open != null)
                mouth.sprite = Mathf.FloorToInt(elapsed * 8f) % 2 == 0 ? closed : open;
            yield return null;
        }

        if (mouth != null && closed != null)
            mouth.sprite = closed;
    }

    IEnumerator Choose(string speaker, string line, string first, string second, SpriteRenderer mouth, Sprite closed, Sprite open, SpriteRenderer listener)
    {
        ShowNovel(speaker, line);
        Dim(mouth, false);
        Dim(listener, true);
        if (caption != null)
            caption.transform.localPosition = new Vector3(captionHome.x, -2.85f, captionHome.z);
        SetChoices(first, second, true);
        int selected = 0;
        float elapsed = 0f;
        bool picked = false;
        while (!picked)
        {
            elapsed += Time.deltaTime;
            int under = ChoiceUnderCursor();
            if (under >= 0)
                selected = under;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
                    selected = 0;
                if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                    selected = 1;
                if (Keyboard.current.digit1Key.wasPressedThisFrame)
                {
                    selected = 0;
                    picked = true;
                }
                if (Keyboard.current.digit2Key.wasPressedThisFrame)
                {
                    selected = 1;
                    picked = true;
                }
            }
            HighlightChoice(selected);
            bool click = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && under >= 0;
            bool confirm = Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);
            if (elapsed > 0.15f && (click || confirm))
                picked = true;
            if (mouth != null && closed != null && open != null)
                mouth.sprite = Mathf.FloorToInt(elapsed * 8f) % 2 == 0 ? closed : open;
            yield return null;
        }

        if (mouth != null && closed != null)
            mouth.sprite = closed;
        SetChoices(first, second, false);
        if (caption != null)
            caption.transform.localPosition = captionHome;
    }

    void BuildNovelChrome()
    {
        Sprite square = fade != null ? fade.sprite : null;
        dialogueBox = MakePanel("Dialogue Box", new Vector3(0f, -3.45f, 1f), new Vector3(17.2f, 2.55f, 1f), new Color(0.04f, 0.05f, 0.08f, 0.94f), 2000, square);
        namePlate = MakePanel("Name Plate", new Vector3(-5.85f, -1.95f, 1f), new Vector3(3.5f, 0.72f, 1f), new Color(0.45f, 0.16f, 0.18f, 1f), 2001, square);

        GameObject nameObject = new GameObject("Speaker");
        nameObject.transform.SetParent(view != null ? view.transform : transform, false);
        nameObject.transform.localPosition = new Vector3(-5.85f, -1.95f, 1f);
        speakerName = nameObject.AddComponent<TextMesh>();
        speakerName.anchor = TextAnchor.MiddleCenter;
        speakerName.alignment = TextAlignment.Center;
        speakerName.fontSize = 48;
        speakerName.characterSize = 0.075f;
        speakerName.color = Color.white;
        speakerName.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        MeshRenderer nameRenderer = nameObject.GetComponent<MeshRenderer>();
        nameRenderer.sortingOrder = 2100;

        if (caption != null)
        {
            caption.transform.localPosition = new Vector3(-6.4f, -3.55f, 1f);
            captionHome = caption.transform.localPosition;
            caption.anchor = TextAnchor.MiddleLeft;
            caption.alignment = TextAlignment.Left;
            caption.characterSize = 0.065f;
            caption.fontSize = 40;
            MeshRenderer captionRenderer = caption.GetComponent<MeshRenderer>();
            if (captionRenderer != null)
                captionRenderer.sortingOrder = 2100;
        }

        advancePrompt = MakeLine("Advance Prompt", new Vector3(6.55f, -4.35f, 1f), 0.042f, TextAnchor.MiddleRight, 32);
        advancePrompt.color = new Color(0.75f, 0.75f, 0.75f, 0.55f);
        advancePrompt.alignment = TextAlignment.Right;
        advancePrompt.text = "Press Space";

        choicePlates = new SpriteRenderer[2];
        choiceLabels = new TextMesh[2];
        choicePlates[0] = MakePanel("Choice A", new Vector3(-0.8f, -3.55f, 1f), new Vector3(10.2f, 0.72f, 1f), new Color(0.16f, 0.18f, 0.22f, 1f), 2050, square);
        choicePlates[1] = MakePanel("Choice B", new Vector3(-0.8f, -4.35f, 1f), new Vector3(10.2f, 0.72f, 1f), new Color(0.16f, 0.18f, 0.22f, 1f), 2050, square);
        choiceLabels[0] = MakeLine("Choice A Label", new Vector3(-5.4f, -3.55f, 1f), 0.055f, TextAnchor.MiddleLeft, 36);
        choiceLabels[1] = MakeLine("Choice B Label", new Vector3(-5.4f, -4.35f, 1f), 0.055f, TextAnchor.MiddleLeft, 36);
        SetChoices("", "", false);

        HideNovel();
    }

    TextMesh MakeLine(string lineName, Vector3 localPosition, float characterSize, TextAnchor anchor, int fontSize)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(view != null ? view.transform : transform, false);
        lineObject.transform.localPosition = localPosition;
        TextMesh line = lineObject.AddComponent<TextMesh>();
        line.anchor = anchor;
        line.alignment = anchor == TextAnchor.MiddleRight ? TextAlignment.Right : TextAlignment.Left;
        line.fontSize = fontSize;
        line.characterSize = characterSize;
        line.color = Color.white;
        line.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lineObject.GetComponent<MeshRenderer>().sortingOrder = 2102;
        return line;
    }

    SpriteRenderer MakePanel(string panelName, Vector3 localPosition, Vector3 scale, Color color, int order, Sprite square)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(view != null ? view.transform : transform, false);
        panel.transform.localPosition = localPosition;
        panel.transform.localScale = scale;
        SpriteRenderer renderer = panel.AddComponent<SpriteRenderer>();
        renderer.sprite = square;
        renderer.color = color;
        renderer.sortingOrder = order;
        return renderer;
    }

    void ShowNovel(string speaker, string line)
    {
        if (dialogueBox != null)
            dialogueBox.enabled = true;
        if (namePlate != null)
        {
            namePlate.enabled = true;
            namePlate.color = speaker == "Mom"
                ? new Color(0.55f, 0.18f, 0.2f, 1f)
                : speaker == "Agent"
                    ? new Color(0.12f, 0.16f, 0.22f, 1f)
                    : new Color(0.12f, 0.38f, 0.28f, 1f);
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
            SetTextAlpha(caption, 1f);
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
        if (advancePrompt != null)
            advancePrompt.gameObject.SetActive(false);
        SetChoices("", "", false);
        if (caption != null)
            caption.transform.localPosition = captionHome;
        ClearCaption();
    }

    void SetChoices(string first, string second, bool visible)
    {
        for (int i = 0; i < 2; i++)
        {
            if (choicePlates != null && choicePlates[i] != null)
                choicePlates[i].enabled = visible;
            if (choiceLabels != null && choiceLabels[i] != null)
            {
                choiceLabels[i].gameObject.SetActive(visible);
                choiceLabels[i].text = i == 0 ? first : second;
            }
        }
        if (visible && advancePrompt != null)
            advancePrompt.text = "Click";
    }

    void HighlightChoice(int selected)
    {
        for (int i = 0; i < 2; i++)
        {
            if (choicePlates == null || choicePlates[i] == null)
                continue;
            bool on = i == selected;
            choicePlates[i].color = on
                ? new Color(0.28f, 0.42f, 0.34f, 1f)
                : new Color(0.14f, 0.16f, 0.2f, 1f);
            if (choiceLabels != null && choiceLabels[i] != null)
                choiceLabels[i].color = on ? Color.white : new Color(0.7f, 0.72f, 0.74f, 1f);
        }
    }

    int ChoiceUnderCursor()
    {
        if (view == null || Mouse.current == null || choicePlates == null)
            return -1;

        Vector3 screen = Mouse.current.position.ReadValue();
        screen.z = Mathf.Abs(view.transform.position.z);
        Vector2 world = view.ScreenToWorldPoint(screen);
        for (int i = 0; i < choicePlates.Length; i++)
        {
            if (choicePlates[i] == null || !choicePlates[i].enabled)
                continue;
            Vector3 place = choicePlates[i].transform.position;
            Vector3 size = choicePlates[i].transform.lossyScale;
            if (Mathf.Abs(world.x - place.x) <= Mathf.Abs(size.x) * 0.5f
                && Mathf.Abs(world.y - place.y) <= Mathf.Abs(size.y) * 0.5f)
                return i;
        }

        return -1;
    }

    void ShowPlanet()
    {
        if (planet == null)
            return;

        Sprite circle = CircleSprite();
        SpriteRenderer[] parts = planet.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
                parts[i].sprite = circle;
        }
    }

    static Sprite circleSprite;

    static Sprite CircleSprite()
    {
        if (circleSprite != null)
            return circleSprite;

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        float radius = size * 0.5f - 1.5f;
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = Mathf.Clamp01(radius - distance + 1.2f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }

    void FitCover(string objectName)
    {
        GameObject backdropObject = GameObject.Find(objectName);
        if (backdropObject == null || view == null)
            return;

        SpriteRenderer renderer = backdropObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
            return;

        Sprite art = LoadSprite(objectName == "Bedroom Backdrop"
            ? "Assets/Sprites/Backdrops/bedroom.png"
            : "Assets/Sprites/Backdrops/streetalien.png");
        if (art != null)
            renderer.sprite = art;
        if (renderer.sprite == null)
            return;

        float viewHeight = view.orthographicSize * 2f;
        float viewWidth = viewHeight * Mathf.Max(0.01f, view.aspect);
        Vector2 size = renderer.sprite.bounds.size;
        float scale = Mathf.Max(viewWidth / Mathf.Max(0.01f, size.x), viewHeight / Mathf.Max(0.01f, size.y));
        backdropObject.transform.localScale = new Vector3(scale, scale, 1f);
        backdropObject.transform.localPosition = Vector3.zero;
        renderer.color = Color.white;
        renderer.sortingOrder = -20;
    }

    static void EnsureSprite(SpriteRenderer renderer, string path)
    {
        if (renderer == null)
            return;

        Sprite art = LoadSprite(path);
        if (art != null)
            renderer.sprite = art;
    }

    static Sprite LoadSprite(string path)
    {
        return SpriteLibrary.Load(path);
    }

    void Seat(Transform actor, SpriteRenderer renderer, float localX, int faceSign, float height = PortraitHeight)
    {
        if (actor == null || renderer == null || renderer.sprite == null)
            return;

        float native = renderer.sprite.bounds.size.y;
        float scale = native > 0.01f ? height / native : 1f;
        actor.localScale = new Vector3(Mathf.Abs(scale) * faceSign, scale, 1f);
        actor.localPosition = new Vector3(localX, FootY - renderer.sprite.bounds.min.y * scale, 0f);
        renderer.sortingOrder = Mathf.Max(renderer.sortingOrder, 10);
        renderer.color = Color.white;
    }

    Vector3 PortraitPlace(SpriteRenderer renderer, float localX, float height = PortraitHeight)
    {
        if (renderer == null || renderer.sprite == null)
            return new Vector3(localX, FootY, 0f);

        float native = renderer.sprite.bounds.size.y;
        float scale = native > 0.01f ? height / native : 1f;
        return new Vector3(localX, FootY - renderer.sprite.bounds.min.y * scale, 0f);
    }

    static void Dim(SpriteRenderer renderer, bool dim)
    {
        if (renderer == null)
            return;
        renderer.color = dim ? new Color(0.45f, 0.45f, 0.5f, 1f) : Color.white;
    }

    void SetLetterbox(bool visible)
    {
        if (letterboxTop != null)
            letterboxTop.SetActive(visible);
        if (letterboxBottom != null)
            letterboxBottom.SetActive(visible);
    }

    IEnumerator MovePair(Transform first, Vector3 firstEnd, Transform second, Vector3 secondEnd, float duration)
    {
        Vector3 firstStart = first != null ? first.localPosition : Vector3.zero;
        Vector3 secondStart = second != null ? second.localPosition : Vector3.zero;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.SmoothStep(0f, 1f, duration <= 0f ? 1f : elapsed / duration);
            if (first != null)
                first.localPosition = Vector3.Lerp(firstStart, firstEnd, along);
            if (second != null)
                second.localPosition = Vector3.Lerp(secondStart, secondEnd, along);
            yield return null;
        }

        if (first != null)
            first.localPosition = firstEnd;
        if (second != null)
            second.localPosition = secondEnd;
    }

    static bool AdvancePressed()
    {
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
            return true;
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }

    IEnumerator MoveLocal(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = Mathf.SmoothStep(0f, 1f, duration <= 0f ? 1f : elapsed / duration);
            target.localPosition = Vector3.Lerp(start, destination, along);
            yield return null;
        }

        target.localPosition = destination;
    }

    IEnumerator CutTo(Vector3 position)
    {
        yield return FadeTo(1f, 0.28f);
        if (view != null)
        {
            view.transform.position = position;
            view.orthographicSize = 5f;
        }
        ClearCaption();
        yield return FadeTo(0f, 0.32f);
    }

    IEnumerator FadeTo(float alpha, float duration)
    {
        float start = fade != null ? fade.color.a : 0f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float along = duration <= 0f ? 1f : elapsed / duration;
            SetFade(Mathf.Lerp(start, alpha, along));
            yield return null;
        }

        SetFade(alpha);
    }

    void TwinkleStars(float time)
    {
        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null)
                continue;
            Color color = stars[i].color;
            color.a = 0.45f + 0.55f * Mathf.Abs(Mathf.Sin(time * (1.4f + i * 0.17f) + i));
            stars[i].color = color;
        }
    }

    void ClearCaption()
    {
        if (caption != null)
            caption.text = "";
    }

    void SetFade(float alpha)
    {
        if (fade == null)
            return;
        Color color = fade.color;
        color.a = alpha;
        fade.color = color;
    }

    static void SetTextAlpha(TextMesh text, float alpha)
    {
        if (text == null)
            return;
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
