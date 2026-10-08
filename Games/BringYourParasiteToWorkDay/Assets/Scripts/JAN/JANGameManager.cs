using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Drop this on an empty object in JanitorScene and press Play.
// Awake builds the White House, the janitor, the HUD, and the fight from primitives.
// No imported art. Restart reloads this scene, so JanitorScene must be in the build list.
public class JANGameManager : MonoBehaviour
{
    public static JANGameManager Instance { get; private set; }

    public JANPlayerController Player { get; private set; }
    public bool Ended { get; private set; }
    public float PeaceUntil { get; private set; }
    public int AgentsRemaining { get; private set; }

    PJAController boss;
    int living;
    int zone;
    float bannerTimer;
    float hurtAlpha;
    bool won;

    Text bannerText;
    Text endText;
    Image portrait;
    Image hurtFlash;
    Image bossFill;
    Image staminaFill;
    JANPixelLabel ammoReadout;
    JANPixelLabel healthReadout;
    JANPixelLabel armorReadout;
    JANPixelLabel enemiesReadout;
    JANPixelLabel timerReadout;
    Text scoreText;
    GameObject bossRoot;
    GameObject endPanel;
    Image endDimmer;
    Image deathBorder;
    Text restartHint;
    [SerializeField] Sprite[] rickFaces;

    void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
        PeaceUntil = Time.time + 3.5f;
        GameWideManager.EnsureExists();
        JANWhiteHouse.Build(transform);
        BuildHud();
        JANDoomView.Install(Camera.main);
    }

    void Start()
    {
        ShowBanner("ENTRANCE HALL — LEFT CLICK SPRAYS, RIGHT CLICK SWINGS");
    }

    public void BindPlayer(JANPlayerController player)
    {
        Player = player;
    }

    public void RegisterAgent()
    {
        AgentsRemaining++;
        RegisterHostile();
    }

    public void AgentDown()
    {
        AgentsRemaining = Mathf.Max(0, AgentsRemaining - 1);
        HostileDown();
    }

    public void RegisterHostile()
    {
        living++;
    }

    public void HostileDown()
    {
        living = Mathf.Max(0, living - 1);
    }

    public void BossEngaged(PJAController president)
    {
        boss = president;
        if (bossRoot != null)
            bossRoot.SetActive(true);
        zone = 3;
        ShowBanner("OVAL OFFICE — PRESIDENT JOHN AMERICAN");
    }

    public void BossHealthChanged()
    {
    }

    public void ShowBanner(string message)
    {
        if (bannerText == null || Ended)
            return;

        bannerText.text = message;
        bannerTimer = 4f;
    }

    public void PlayerHurt()
    {
        hurtAlpha = 0.55f;
    }

    public void PlayerDied()
    {
        if (Ended)
            return;

        Ended = true;
        won = false;
        PresentEnd("SECURITY ESCORTED YOU OUT", new Color(0.85f, 0.2f, 0.18f), true);
    }

    public void Victory()
    {
        if (Ended)
            return;

        Ended = true;
        won = true;
        PresentEnd("MISSION ACCOMPLISHED: THE FLOORS ARE CLEAN", new Color(1f, 0.86f, 0.25f), false);
        StartCoroutine(LeaveForFinale());
    }

    IEnumerator LeaveForFinale()
    {
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("Finale");
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        TrackZone();
        RefreshHud();
    }

    void TrackZone()
    {
        if (Ended || Player == null || zone >= 2)
            return;

        if (Player.transform.position.z > JANWhiteHouse.Across(50f))
        {
            zone = 2;
            ShowBanner("WEST WING — WATCH THE STAIRS");
        }
    }

    void RefreshHud()
    {
        if (Player == null)
            return;

        float healthPct = Player.MaxHealth <= 0f ? 0f : 100f * Player.Health / Player.MaxHealth;
        int healthNumber = Mathf.CeilToInt(healthPct);
        int armorNumber = Mathf.CeilToInt(Player.Armor);
        int ammo = Player.Weapons != null ? Player.Weapons.Ammo : 0;
        Color red = new Color(0.86f, 0.08f, 0.06f, 1f);
        Color yellow = new Color(0.93f, 0.78f, 0.15f, 1f);

        ammoReadout.Set(ammo.ToString(), red, 8);
        healthReadout.Set(healthNumber + "%", red, 8);
        armorReadout.Set(armorNumber + "%", red, 8);
        if (enemiesReadout != null)
            enemiesReadout.Set(living.ToString(), yellow, 8);
        GameWideManager clock = GameWideManager.myGameManager;
        if (timerReadout != null && clock != null)
            timerReadout.Set(clock.getDisplaytime(), yellow, 3);
        if (scoreText != null && clock != null)
            scoreText.text = "Score: " + clock.getScore();
        if (staminaFill != null && Player.MaxStamina > 0f)
            staminaFill.fillAmount = Player.Stamina / Player.MaxStamina;

        if (portrait != null && rickFaces != null && rickFaces.Length > 0)
        {
            int face = RickFace(Player.Health);
            if (face < rickFaces.Length && rickFaces[face] != null)
                portrait.sprite = rickFaces[face];
            portrait.color = Color.white;
        }

        if (hurtFlash != null)
        {
            hurtAlpha = Mathf.MoveTowards(hurtAlpha, 0f, Time.deltaTime * 1.6f);
            Color flash = hurtFlash.color;
            flash.a = hurtAlpha;
            hurtFlash.color = flash;
        }

        if (bannerText != null)
        {
            bannerTimer -= Time.deltaTime;
            Color color = bannerText.color;
            color.a = bannerTimer > 0.4f ? 1f : Mathf.Clamp01(bannerTimer / 0.4f);
            bannerText.color = color;
        }

        if (bossRoot != null && boss != null && boss.IsAggro)
        {
            bossRoot.SetActive(!boss.IsDead && !Ended);
            if (bossFill != null && boss.MaxHealth > 0f)
                bossFill.fillAmount = boss.Health / boss.MaxHealth;
        }

        if (Ended && endText != null && won)
        {
            float pulse = Mathf.PingPong(Time.time * 2.2f, 1f);
            endText.color = Color.Lerp(Color.white, new Color(1f, 0.82f, 0.15f), pulse);
        }
    }

    void PresentEnd(string message, Color color, bool death)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (endPanel != null)
            endPanel.SetActive(true);
        if (endDimmer != null)
            endDimmer.enabled = !death;
        if (deathBorder != null)
            deathBorder.enabled = death;
        if (restartHint != null)
            restartHint.gameObject.SetActive(true);
        if (endText != null)
        {
            endText.text = message;
            endText.color = color;
            endText.fontSize = death ? 36 : 42;
        }
    }

    void BuildHud()
    {
        GameObject canvasObject = new GameObject("Janitor HUD");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        Font font = BuiltinFont();
        Color metal = new Color(0.62f, 0.62f, 0.64f, 1f);
        Color inset = new Color(0.16f, 0.16f, 0.16f, 1f);
        Color label = new Color(0.78f, 0.78f, 0.78f, 1f);
        const float bar = JANDoomView.BarHeight;

        GameObject view = new GameObject("View");
        view.transform.SetParent(canvasObject.transform, false);
        RectTransform viewRect = view.AddComponent<RectTransform>();
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        viewRect.offsetMin = new Vector2(0f, bar);
        viewRect.offsetMax = Vector2.zero;

        scoreText = Label(view.transform, "Score: 0", 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -14f), new Vector2(320f, 28f), new Color(0.92f, 0.9f, 0.82f, 0.85f), font);
        bannerText = Label(view.transform, "", 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(1200f, 40f), new Color(0.95f, 0.86f, 0.35f), font);
        Label(view.transform, "LMB SPRAY    RMB MOP    WASD    SHIFT    CTRL    SPACE    R RESTART", 16, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 10f), new Vector2(1100f, 24f), new Color(0.8f, 0.8f, 0.8f, 0.7f), font);

        Vector2 viewCenter = new Vector2(0.5f, 0.5f);
        Image cross = Panel(view.transform, viewCenter, viewCenter, viewCenter, Vector2.zero, new Vector2(4f, 16f), Color.white);
        Panel(view.transform, viewCenter, viewCenter, viewCenter, Vector2.zero, new Vector2(16f, 4f), Color.white);
        cross.transform.SetAsLastSibling();

        hurtFlash = Panel(view.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.7f, 0.05f, 0.05f, 0f));
        hurtFlash.raycastTarget = false;

        bossRoot = new GameObject("Boss Bar");
        bossRoot.transform.SetParent(view.transform, false);
        RectTransform bossRect = bossRoot.AddComponent<RectTransform>();
        bossRect.anchorMin = new Vector2(0.5f, 1f);
        bossRect.anchorMax = new Vector2(0.5f, 1f);
        bossRect.pivot = new Vector2(0.5f, 1f);
        bossRect.anchoredPosition = new Vector2(0f, -96f);
        bossRect.sizeDelta = new Vector2(520f, 28f);
        Panel(bossRoot.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, inset);
        bossFill = Panel(bossRoot.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.75f, 0.12f, 0.08f));
        bossFill.type = Image.Type.Filled;
        bossFill.fillMethod = Image.FillMethod.Horizontal;
        bossFill.fillAmount = 1f;
        Label(bossRoot.transform, "PRESIDENT JOHN AMERICAN", 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.white, font);
        bossRoot.SetActive(false);

        GameObject barRoot = new GameObject("Status Bar");
        barRoot.transform.SetParent(canvasObject.transform, false);
        RectTransform barRect = barRoot.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(1f, 0f);
        barRect.pivot = new Vector2(0.5f, 0f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta = new Vector2(0f, bar);
        Panel(barRoot.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, metal);
        Panel(barRoot.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 6f), new Color(0.25f, 0.25f, 0.25f, 1f));

        ammoReadout = Stat(barRoot.transform, 70f, "AMMO", label, 8);
        healthReadout = Stat(barRoot.transform, 390f, "HEALTH", label, 8);
        enemiesReadout = Stat(barRoot.transform, 700f, "ENEMIES", label, 8);
        portrait = Face(barRoot.transform, 900f);
        armorReadout = Stat(barRoot.transform, 1120f, "ARMOR", label, 8);
        timerReadout = Pixel(barRoot.transform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1480f, 108f), 3);
        timerReadout.Set("24:00", new Color(0.93f, 0.78f, 0.15f, 1f), 3);
        StaminaBar(barRoot.transform, 1480f);

        endPanel = new GameObject("End");
        endPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform endRect = endPanel.AddComponent<RectTransform>();
        endRect.anchorMin = Vector2.zero;
        endRect.anchorMax = Vector2.one;
        endRect.offsetMin = Vector2.zero;
        endRect.offsetMax = Vector2.zero;
        endDimmer = Panel(endPanel.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.04f, 0.03f, 0.03f, 0.82f));
        deathBorder = Panel(endPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1200f, 1020f), Color.white);
        Sprite borderSprite = SpriteLibrary.Load("Assets/Sprites/DeathBorder.png");
        if (borderSprite != null)
            deathBorder.sprite = borderSprite;
        deathBorder.enabled = false;
        endText = Label(endPanel.transform, "", 42, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(700f, 160f), Color.white, font);
        restartHint = Label(endPanel.transform, "R  RESTART", 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(400f, 40f), Color.white, font);
        endPanel.SetActive(false);
    }

    JANPixelLabel Stat(Transform bar, float x, string caption, Color captionColor, int numberScale)
    {
        Pixel(bar, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(x, 28f), 3).Set(caption, captionColor, 3);
        return Pixel(bar, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(x, 78f), numberScale);
    }

    Image Face(Transform bar, float x)
    {
        LoadRickFaces();
        Panel(bar, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(x, 28f), new Vector2(112f, 124f), new Color(0.08f, 0.08f, 0.08f, 1f));
        Image skin = Panel(bar, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(x + 6f, 34f), new Vector2(100f, 112f), Color.white);
        skin.sprite = rickFaces != null && rickFaces.Length > 0 ? rickFaces[0] : JANArt.White();
        skin.preserveAspect = true;
        skin.raycastTarget = false;
        return skin;
    }

    static int RickFace(float health)
    {
        if (health >= 80f)
            return 0;
        if (health >= 60f)
            return 1;
        if (health >= 40f)
            return 2;
        if (health >= 20f)
            return 3;
        return 4;
    }

    void LoadRickFaces()
    {
        if (rickFaces == null || rickFaces.Length < 5)
        {
            Sprite[] loaded = new Sprite[5];
            if (rickFaces != null)
            {
                for (int i = 0; i < rickFaces.Length && i < loaded.Length; i++)
                    loaded[i] = rickFaces[i];
            }

            rickFaces = loaded;
        }

        for (int i = 0; i < rickFaces.Length; i++)
        {
            if (rickFaces[i] != null)
                continue;

            rickFaces[i] = SpriteLibrary.Load("Assets/Sprites/Rick/sprite_rick" + i + ".png");
        }
    }

    void StaminaBar(Transform bar, float x)
    {
        GameObject root = new GameObject("Stamina");
        root.transform.SetParent(bar, false);
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(x, 22f);
        rect.sizeDelta = new Vector2(380f, 58f);
        Panel(root.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.12f, 0.12f, 0.12f, 1f));
        staminaFill = Panel(root.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.86f, 0.72f, 0.16f, 1f));
        staminaFill.type = Image.Type.Filled;
        staminaFill.fillMethod = Image.FillMethod.Horizontal;
        staminaFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        staminaFill.fillAmount = 1f;
    }

    static JANPixelLabel Pixel(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchored, int pixelScale)
    {
        GameObject go = new GameObject("Pixel");
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = anchored;
        JANPixelLabel label = go.AddComponent<JANPixelLabel>();
        label.Set("", Color.white, pixelScale);
        return label;
    }

    static Font BuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return font;
    }

    static Image Panel(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchored, Vector2 size, Color color)
    {
        GameObject box = new GameObject("Panel");
        box.transform.SetParent(parent, false);
        RectTransform rect = box.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchored;
        rect.sizeDelta = size;
        if (anchorMin == Vector2.zero && anchorMax == Vector2.one)
        {
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        Image image = box.AddComponent<Image>();
        image.sprite = JANArt.White();
        image.color = color;
        return image;
    }

    static Text Label(Transform parent, string message, int size, TextAnchor anchor, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchored, Vector2 box, Color color, Font font)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchored;
        rect.sizeDelta = box;
        Text text = go.AddComponent<Text>();
        text.font = font;
        text.text = message;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }
}
