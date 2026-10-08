using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DogHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 125f;
    [SerializeField] bool showDebugReadout = true;
    [SerializeField] Animator deathAnimator;
    [SerializeField] string deathStateTrigger = "Dead";
    [SerializeField] UnityEvent onEnterDeadState;

    public bool IsDead { get; private set; }
    public bool Invincible { get; private set; }
    public bool InCutscene { get; private set; }
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    float health;
    float shownBlock = 100f;
    Image fill;
    Image blockFill;
    Text timerText;
    Text scoreText;
    GameObject gameOverPanel;
    readonly Image[] keyFaces = new Image[3];
    readonly RectTransform[] keyFaceRects = new RectTransform[3];
    readonly Vector2[] keyRest = new Vector2[3];
    readonly Color[] keyLit = new Color[3];
    readonly Color[] keyInk = new Color[3];
    readonly Text[] keyLetters = new Text[3];
    SpriteRenderer spriteRenderer;
    Color aliveColor;
    string debugLine;
    PlayerCombatDog combat;
    PlayerControllerDog movement;
    int cheatStep;
    float cheatExpire;

    void Awake()
    {
        health = RoundTenths(maxHealth);
        debugLine = $"DEBUG HP {health:0.0}";
        combat = GetComponent<PlayerCombatDog>();
        movement = GetComponent<PlayerControllerDog>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            aliveColor = spriteRenderer.color;

        GameWideManager.EnsureExists();
        BuildHud();
    }

    void Update()
    {
        RefreshClockHud();
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (IsDead)
        {
            if (keyboard.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        TickCheat(keyboard);
        if (keyFaces[0] == null)
            return;

        SetKeyPressed(0, keyboard.jKey.isPressed);
        SetKeyPressed(1, keyboard.kKey.isPressed);
        SetKeyPressed(2, keyboard.lKey.isPressed);
    }

    void RefreshClockHud()
    {
        GameWideManager clock = GameWideManager.myGameManager;
        if (clock == null)
            return;
        if (timerText != null)
            timerText.text = clock.getDisplaytime();
        if (scoreText != null)
            scoreText.text = "Score: " + clock.getScore();
    }

    public void ReceiveAttack()
    {
        ReceiveAttack(false);
    }

    public void BeginCutscene()
    {
        InCutscene = true;
    }

    void TickCheat(Keyboard keyboard)
    {
        if (cheatStep > 0 && Time.time > cheatExpire)
            cheatStep = 0;

        bool four = keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame;
        bool eight = keyboard.digit8Key.wasPressedThisFrame || keyboard.numpad8Key.wasPressedThisFrame;
        bool six = keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame;
        int presses = (four ? 1 : 0) + (eight ? 1 : 0) + (six ? 1 : 0);
        if (presses == 0)
            return;
        if (presses > 1)
        {
            cheatStep = 0;
            return;
        }

        bool matched = cheatStep % 3 == 0 ? four : cheatStep % 3 == 1 ? eight : six;
        if (!matched)
        {
            cheatStep = four ? 1 : 0;
            cheatExpire = Time.time + 0.4f;
            return;
        }

        cheatStep++;
        cheatExpire = Time.time + 0.4f;
        if (cheatStep < 6)
            return;

        cheatStep = 0;
        Invincible = !Invincible;
        debugLine = Invincible
            ? $"DEBUG HP {health:0.0}  INVINCIBLE ON"
            : $"DEBUG HP {health:0.0}  INVINCIBLE OFF";
        Debug.Log(debugLine);
    }

    public void ReceiveAttack(bool heavy)
    {
        if (IsDead || Invincible)
        {
            if (Invincible)
                debugLine = $"DEBUG HP {health:0.0}  INVINCIBLE";
            return;
        }

        float damage;
        string result;
        if (combat != null && combat.IsPerfectBlocking)
        {
            damage = 0f;
            result = heavy ? "Perfect Block Heavy" : "Perfect Block";
        }
        else if (combat != null && combat.IsBlocking)
        {
            damage = heavy ? RollRange(3f, 8f) : RollRange(1f, 5f);
            result = heavy ? "Block Heavy" : "Block";
            combat.OnBlockHit(heavy);
        }
        else
        {
            damage = heavy ? RollRange(40f, 60f) : RollRange(7f, 15f);
            result = heavy ? "Heavy Hit" : "Hit";
        }

        ApplyDamage(damage, result);
    }

    public void ApplyDamage(float amount, string result)
    {
        if (IsDead || Invincible)
        {
            if (Invincible && !IsDead)
                debugLine = $"DEBUG HP {health:0.0}  INVINCIBLE";
            return;
        }

        amount = RoundTenths(Mathf.Max(0f, amount));
        health = RoundTenths(Mathf.Max(0f, health - amount));
        debugLine = $"DEBUG HP {health:0.0}  {result} {amount:0.0}";
        Debug.Log(debugLine);
        RefreshBar();

        if (amount > 0f && movement != null && (result == "Hit" || result == "Heavy Hit" || result == "Manhole"))
            movement.Stagger(0.16f);

        if (health <= 0f)
            EnterDeadState();
    }

    public void RestorePortion(float portion)
    {
        if (IsDead)
            return;

        float amount = RoundTenths(maxHealth * Mathf.Max(0f, portion));
        health = RoundTenths(Mathf.Min(maxHealth, health + amount));
        debugLine = $"DEBUG HP {health:0.0}  Restored {amount:0.0}";
        Debug.Log(debugLine);
        RefreshBar();
    }

    void EnterDeadState()
    {
        if (IsDead)
            return;

        IsDead = true;
        health = 0f;
        debugLine = "DEBUG HP 0.0  Dead";
        Debug.Log("DEBUG Dog entered dead state. Game Over.");
        RefreshBar();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(
                aliveColor.r * 0.35f,
                aliveColor.g * 0.35f,
                aliveColor.b * 0.35f,
                aliveColor.a);
        }

        if (deathAnimator != null && !string.IsNullOrEmpty(deathStateTrigger))
            deathAnimator.SetTrigger(deathStateTrigger);

        onEnterDeadState?.Invoke();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void OnGUI()
    {
        if (!showDebugReadout)
            return;

        GUI.Label(new Rect(Screen.width - 280f, 16f, 260f, 44f), debugLine + $"\nDEBUG BLOCK {shownBlock:0.0}");
    }

    void RefreshBar()
    {
        if (fill == null || maxHealth <= 0f)
            return;

        float amount = Mathf.Clamp01(health / maxHealth);
        fill.fillAmount = amount;
        fill.color = Color.Lerp(new Color(0.75f, 0.12f, 0.1f), new Color(0.85f, 0.72f, 0.28f), amount);
    }

    void BuildHud()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject canvasObject = new GameObject("DogHealthCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform background = CreateRect("HealthBarBackground", canvasObject.transform);
        background.anchorMin = new Vector2(0f, 1f);
        background.anchorMax = new Vector2(0f, 1f);
        background.pivot = new Vector2(0f, 1f);
        background.anchoredPosition = new Vector2(28f, -28f);
        background.sizeDelta = new Vector2(360f, 36f);
        Image backgroundImage = background.gameObject.AddComponent<Image>();
        backgroundImage.sprite = WhiteSprite();
        backgroundImage.color = new Color(0.08f, 0.07f, 0.06f, 0.9f);

        RectTransform fillRect = CreateRect("HealthBarFill", background);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(4f, 4f);
        fillRect.offsetMax = new Vector2(-4f, -4f);
        fill = fillRect.gameObject.AddComponent<Image>();
        fill.sprite = WhiteSprite();
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        RefreshBar();

        RectTransform blockBackground = CreateRect("BlockBarBackground", canvasObject.transform);
        blockBackground.anchorMin = new Vector2(0f, 1f);
        blockBackground.anchorMax = new Vector2(0f, 1f);
        blockBackground.pivot = new Vector2(0f, 1f);
        blockBackground.anchoredPosition = new Vector2(28f, -72f);
        blockBackground.sizeDelta = new Vector2(360f, 18f);
        Image blockBackgroundImage = blockBackground.gameObject.AddComponent<Image>();
        blockBackgroundImage.sprite = WhiteSprite();
        blockBackgroundImage.color = new Color(0.05f, 0.07f, 0.12f, 0.9f);

        RectTransform blockFillRect = CreateRect("BlockBarFill", blockBackground);
        blockFillRect.anchorMin = Vector2.zero;
        blockFillRect.anchorMax = Vector2.one;
        blockFillRect.offsetMin = new Vector2(3f, 3f);
        blockFillRect.offsetMax = new Vector2(-3f, -3f);
        blockFill = blockFillRect.gameObject.AddComponent<Image>();
        blockFill.sprite = WhiteSprite();
        blockFill.type = Image.Type.Filled;
        blockFill.fillMethod = Image.FillMethod.Horizontal;
        blockFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        blockFill.color = new Color(0.2f, 0.55f, 1f, 1f);
        SetBlockMeter(shownBlock, 100f, false);
        BuildKeyHint(canvasObject.transform, font);

        scoreText = CreateHudLabel(canvasObject.transform, font, "Score", "Score: 0", 28, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(360f, 40f),
            new Color(0.95f, 0.93f, 0.85f, 0.9f));
        timerText = CreateHudLabel(canvasObject.transform, font, "Timer", "24:00", 32, TextAnchor.MiddleRight,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -20f), new Vector2(220f, 44f),
            new Color(0.95f, 0.93f, 0.85f, 0.95f));
        RefreshClockHud();

        gameOverPanel = new GameObject("GameOver");
        gameOverPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject borderObject = new GameObject("DeathBorder");
        borderObject.transform.SetParent(gameOverPanel.transform, false);
        RectTransform borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = new Vector2(0.5f, 0.5f);
        borderRect.anchorMax = new Vector2(0.5f, 0.5f);
        borderRect.pivot = new Vector2(0.5f, 0.5f);
        borderRect.anchoredPosition = Vector2.zero;
        borderRect.sizeDelta = new Vector2(1200f, 1020f);
        Image borderImage = borderObject.AddComponent<Image>();
        Sprite deathBorder = SpriteLibrary.Load("Assets/Sprites/DeathBorder.png");
        borderImage.sprite = deathBorder != null ? deathBorder : WhiteSprite();
        borderImage.color = Color.white;
        borderImage.raycastTarget = false;

        GameObject labelObject = new GameObject("GameOverLabel");
        labelObject.transform.SetParent(borderObject.transform, false);
        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.sizeDelta = new Vector2(520f, 220f);
        Text label = labelObject.AddComponent<Text>();
        label.text = "GAME OVER\n\nPress R to Retry";
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = 48;
        label.color = Color.white;
        label.font = font;
        label.raycastTarget = false;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;

        gameOverPanel.SetActive(false);
    }

    static Text CreateHudLabel(Transform parent, Font font, string name, string message, int size, TextAnchor align,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchored, Vector2 box, Color color)
    {
        RectTransform rect = CreateRect(name, parent);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchored;
        rect.sizeDelta = box;
        Text text = rect.gameObject.AddComponent<Text>();
        text.text = message;
        text.alignment = align;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.font = font;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    void BuildKeyHint(Transform parent, Font font)
    {
        RectTransform row = CreateRect("KeyBinds", parent);
        row.anchorMin = new Vector2(1f, 0f);
        row.anchorMax = new Vector2(1f, 0f);
        row.pivot = new Vector2(1f, 0f);
        row.anchoredPosition = new Vector2(-32f, 28f);
        row.sizeDelta = new Vector2(300f, 118f);

        CreateKeyCap(row, font, 0, "J", "LIGHT", new Color(1f, 0.84f, 0.28f), new Color(0.22f, 0.16f, 0.05f));
        CreateKeyCap(row, font, 1, "K", "HEAVY", new Color(1f, 0.46f, 0.16f), Color.white);
        CreateKeyCap(row, font, 2, "L", "BLOCK", new Color(0.38f, 0.72f, 1f), new Color(0.05f, 0.1f, 0.18f));
    }

    void CreateKeyCap(RectTransform row, Font font, int index, string letter, string action, Color lit, Color ink)
    {
        RectTransform column = CreateRect(letter + " Key", row);
        column.anchorMin = new Vector2(0f, 0f);
        column.anchorMax = new Vector2(0f, 1f);
        column.pivot = new Vector2(0f, 0.5f);
        column.anchoredPosition = new Vector2(index * 100f, 0f);
        column.sizeDelta = new Vector2(92f, 0f);

        RectTransform shadow = CreateRect("Shadow", column);
        shadow.anchorMin = new Vector2(0.5f, 0.5f);
        shadow.anchorMax = new Vector2(0.5f, 0.5f);
        shadow.pivot = new Vector2(0.5f, 0.5f);
        shadow.anchoredPosition = new Vector2(0f, 8f);
        shadow.sizeDelta = new Vector2(64f, 64f);
        Image shadowImage = shadow.gameObject.AddComponent<Image>();
        shadowImage.sprite = WhiteSprite();
        shadowImage.color = new Color(0.02f, 0.02f, 0.03f, 0.95f);
        shadowImage.raycastTarget = false;

        RectTransform face = CreateRect("Face", column);
        face.anchorMin = new Vector2(0.5f, 0.5f);
        face.anchorMax = new Vector2(0.5f, 0.5f);
        face.pivot = new Vector2(0.5f, 0.5f);
        face.anchoredPosition = new Vector2(0f, 16f);
        face.sizeDelta = new Vector2(64f, 64f);
        Image faceImage = face.gameObject.AddComponent<Image>();
        faceImage.sprite = WhiteSprite();
        faceImage.color = new Color(0.2f, 0.21f, 0.24f, 1f);
        faceImage.raycastTarget = false;

        RectTransform letterRect = CreateRect("Letter", face);
        letterRect.anchorMin = Vector2.zero;
        letterRect.anchorMax = Vector2.one;
        letterRect.offsetMin = Vector2.zero;
        letterRect.offsetMax = Vector2.zero;
        Text letterText = letterRect.gameObject.AddComponent<Text>();
        letterText.text = letter;
        letterText.alignment = TextAnchor.MiddleCenter;
        letterText.fontSize = 32;
        letterText.fontStyle = FontStyle.Bold;
        letterText.color = Color.white;
        letterText.font = font;
        letterText.raycastTarget = false;

        RectTransform captionRect = CreateRect("Caption", column);
        captionRect.anchorMin = new Vector2(0.5f, 0f);
        captionRect.anchorMax = new Vector2(0.5f, 0f);
        captionRect.pivot = new Vector2(0.5f, 0f);
        captionRect.anchoredPosition = new Vector2(0f, 4f);
        captionRect.sizeDelta = new Vector2(92f, 24f);
        Text caption = captionRect.gameObject.AddComponent<Text>();
        caption.text = action;
        caption.alignment = TextAnchor.MiddleCenter;
        caption.fontSize = 16;
        caption.fontStyle = FontStyle.Bold;
        caption.color = new Color(0.82f, 0.8f, 0.74f, 1f);
        caption.font = font;
        caption.raycastTarget = false;

        keyFaces[index] = faceImage;
        keyFaceRects[index] = face;
        keyRest[index] = face.anchoredPosition;
        keyLit[index] = lit;
        keyInk[index] = ink;
        keyLetters[index] = letterText;
    }

    void SetKeyPressed(int index, bool pressed)
    {
        Image face = keyFaces[index];
        if (face == null)
            return;

        face.color = pressed ? keyLit[index] : new Color(0.2f, 0.21f, 0.24f, 1f);
        keyFaceRects[index].anchoredPosition = keyRest[index] + (pressed ? new Vector2(0f, -6f) : Vector2.zero);
        keyLetters[index].color = pressed ? keyInk[index] : Color.white;
    }

    public void SetBlockMeter(float current, float max, bool broken)
    {
        shownBlock = current;
        if (blockFill == null || max <= 0f)
            return;

        blockFill.fillAmount = Mathf.Clamp01(current / max);
        blockFill.color = broken
            ? new Color(0.28f, 0.38f, 0.48f, 1f)
            : new Color(0.2f, 0.55f, 1f, 1f);
    }

    static RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject rectObject = new GameObject(objectName);
        rectObject.transform.SetParent(parent, false);
        return rectObject.AddComponent<RectTransform>();
    }

    static float RollRange(float min, float max)
    {
        int minSteps = Mathf.RoundToInt(min * 10f);
        int maxSteps = Mathf.RoundToInt(max * 10f);
        return Random.Range(minSteps, maxSteps + 1) * 0.1f;
    }

    static float RoundTenths(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }

    static Sprite whiteSprite;

    static Sprite WhiteSprite()
    {
        if (whiteSprite != null)
            return whiteSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }
}
