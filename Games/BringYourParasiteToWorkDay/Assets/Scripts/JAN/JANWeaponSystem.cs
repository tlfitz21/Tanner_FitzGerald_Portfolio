using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JANWeaponSystem : MonoBehaviour
{
    [Header("Mop")]
    [SerializeField] float mopRange = 3.35f;
    [SerializeField] float mopRadius = 0.55f;
    [SerializeField] float mopDamage = 36f;
    [SerializeField] float mopKnockback = 7.5f;
    [SerializeField] float mopCooldown = 0.48f;

    [Header("Spray bottle")]
    [SerializeField] int startingAmmo = 32;
    [SerializeField] int maxAmmo = 96;
    [SerializeField] int pellets = 5;
    [SerializeField] float spreadDegrees = 7f;
    [SerializeField] float pelletDamage = 8f;
    [SerializeField] float pelletSpeed = 26f;
    [SerializeField] float sprayCooldown = 0.52f;

    [Header("Viewmodel colors")]
    [SerializeField] Color mopColor = new Color(0.42f, 0.26f, 0.12f, 1f);
    [SerializeField] Color sprayColor = new Color(0.25f, 0.82f, 0.9f, 1f);

    JANPlayerController player;
    Transform mopView;
    Transform sprayView;
    Transform broomFace;
    float nextSwing;
    float nextSpray;
    int ammo;
    float swing;
    float holster;
    float groundedEyeY;
    readonly List<IJANDamageable> struck = new List<IJANDamageable>();
    const float SwingTime = 0.32f;

    public int Ammo => ammo;
    public int MaxAmmo => maxAmmo;
    public bool Swinging => swing > 0f;

    void Awake()
    {
        player = GetComponent<JANPlayerController>();
        ammo = startingAmmo;
    }

    void Start()
    {
        BuildViewmodels();
    }

    void Update()
    {
        JANGameManager game = JANGameManager.Instance;
        if (player == null || player.IsDead || (game != null && game.Ended))
            return;

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null || mouse == null || Cursor.lockState != CursorLockMode.Locked)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
            FireSpray();
        if (mouse.rightButton.wasPressedThisFrame)
            SwingMop();

        CharacterController body = player.GetComponent<CharacterController>();
        if (body != null && body.isGrounded && player.View != null)
            groundedEyeY = player.View.transform.position.y;

        TickWeapons();
    }

    public bool AddAmmo(int amount)
    {
        if (amount <= 0)
            return false;
        ammo += amount;
        return true;
    }

    void SwingMop()
    {
        if (Time.time < nextSwing || player.View == null)
            return;

        nextSwing = Time.time + mopCooldown;
        swing = 1f;
        struck.Clear();
        if (mopView != null)
            mopView.gameObject.SetActive(true);
    }

    void SweepMop()
    {
        Vector3 head = mopView.TransformPoint(new Vector3(0f, 1.02f, 0f));
        Vector3 shaft = mopView.TransformPoint(new Vector3(0f, 0.55f, 0f));
        Vector3 knock = player.View.transform.forward;
        knock.y = 0f;
        if (knock.sqrMagnitude < 0.01f)
            knock = player.transform.forward;
        knock.Normalize();
        StrikeAt(head, mopRadius + 0.17f, knock);
        StrikeAt(shaft, mopRadius, knock);

        float drop = player.View.transform.position.y - groundedEyeY;
        if (drop > 0.15f)
        {
            Vector3 down = Vector3.up * drop;
            StrikeAt(head - down, mopRadius + 0.17f, knock);
            StrikeAt(shaft - down, mopRadius, knock);
        }
    }

    void StrikeAt(Vector3 point, float radius, Vector3 knock)
    {
        Vector3 flat = point - player.transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > mopRange * mopRange)
            return;

        Collider[] hits = Physics.OverlapSphere(point, radius, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            IJANDamageable damageable = hits[i].GetComponentInParent<IJANDamageable>();
            if (damageable == null || damageable.IsDead || damageable is JANPlayerController || struck.Contains(damageable))
                continue;

            struck.Add(damageable);
            damageable.TakeDamage(mopDamage, point, knock * mopKnockback);
        }
    }

    void FireSpray()
    {
        if (Time.time < nextSpray || ammo <= 0 || player.View == null)
            return;

        nextSpray = Time.time + sprayCooldown;
        ammo--;
        Transform cam = player.View.transform;
        Vector3 aim = AutoAim(cam);
        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = Spread(cam, aim, spreadDegrees);
            SpawnPellet(cam.position + aim * 1.05f, direction, pelletDamage, pelletSpeed, true, sprayColor);
        }
    }

    static Vector3 AutoAim(Transform cam)
    {
        Vector3 flat = cam.forward;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f)
            flat = Vector3.forward;
        flat.Normalize();

        Vector3 origin = cam.position;
        Vector3 best = flat;
        float bestDistance = 30f;
        JANEnemyAI[] agents = Object.FindObjectsByType<JANEnemyAI>();
        for (int i = 0; i < agents.Length; i++)
            ConsiderAim(origin, flat, agents[i], ref best, ref bestDistance);
        PJAController boss = Object.FindAnyObjectByType<PJAController>();
        if (boss != null)
            ConsiderAim(origin, flat, boss, ref best, ref bestDistance);
        return best;
    }

    static void ConsiderAim(Vector3 origin, Vector3 flat, IJANDamageable target, ref Vector3 best, ref float bestDistance)
    {
        if (target == null || target.IsDead)
            return;

        Component body = target as Component;
        if (body == null)
            return;

        Vector3 to = body.transform.position + Vector3.up * 1.1f - origin;
        Vector3 flatTo = new Vector3(to.x, 0f, to.z);
        float distance = flatTo.magnitude;
        if (distance < 0.4f || distance > 26f || distance >= bestDistance)
            return;
        if (Vector3.Angle(flat, flatTo / distance) > 12f)
            return;
        bestDistance = distance;
        best = to.normalized;
    }

    public static void SpawnPellet(Vector3 origin, Vector3 direction, float damage, float speed, bool fromPlayer, Color color)
    {
        GameObject glob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glob.name = fromPlayer ? "Spray" : "Bullet";
        glob.transform.position = origin;
        glob.transform.localScale = Vector3.one * (fromPlayer ? 0.16f : 0.12f);
        Collider collider = glob.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
        glob.GetComponent<Renderer>().sharedMaterial = JANArt.Flat(color);
        glob.AddComponent<JANProjectile>().Launch(direction, speed, damage, fromPlayer);
    }

    static Vector3 Spread(Transform aim, Vector3 forward, float degrees)
    {
        Vector2 jitter = Random.insideUnitCircle * degrees;
        Quaternion yaw = Quaternion.AngleAxis(jitter.x, aim.up);
        Quaternion pitch = Quaternion.AngleAxis(-jitter.y, aim.right);
        return yaw * pitch * forward;
    }

    void TickWeapons()
    {
        holster = Mathf.MoveTowards(holster, swing > 0f ? 1f : 0f, Time.deltaTime * 8f);

        if (sprayView != null)
        {
            Vector3 held = new Vector3(0f, -0.46f, 0.66f);
            Vector3 away = new Vector3(0.45f, -0.95f, 0.28f);
            sprayView.gameObject.SetActive(true);
            sprayView.localPosition = Vector3.Lerp(held, away, holster);
            sprayView.localRotation = Quaternion.Euler(holster * 50f, 0f, 0f);
        }

        if (mopView == null)
            return;

        if (swing <= 0f)
        {
            mopView.gameObject.SetActive(false);
            return;
        }

        swing = Mathf.MoveTowards(swing, 0f, Time.deltaTime / SwingTime);
        float sweep = Mathf.SmoothStep(0f, 1f, 1f - swing);
        float outward = Mathf.Sin(sweep * Mathf.PI);
        mopView.gameObject.SetActive(true);
        mopView.localPosition = new Vector3(Mathf.Lerp(0.42f, -0.34f, sweep), -0.32f, 0.35f + outward * 0.55f);
        float pitch = outward * 58f;
        float yaw = Mathf.Lerp(-42f, 42f, sweep);
        float roll = Mathf.Lerp(-72f, 72f, sweep);
        mopView.localRotation = Quaternion.Euler(pitch, yaw, roll);
        LayBroomFlat();
        SweepMop();
    }

    void LateUpdate()
    {
        if (mopView != null && mopView.gameObject.activeSelf)
            LayBroomFlat();
    }

    void LayBroomFlat()
    {
        if (broomFace == null || mopView == null)
            return;

        Vector3 outward = mopView.TransformDirection(Vector3.up);
        outward.y = 0f;
        if (outward.sqrMagnitude < 0.0004f && player != null && player.View != null)
        {
            outward = player.View.transform.forward;
            outward.y = 0f;
        }

        if (outward.sqrMagnitude < 0.0001f)
            return;

        outward.Normalize();
        broomFace.rotation = Quaternion.LookRotation(-Vector3.up, -outward);
    }

    void BuildViewmodels()
    {
        if (player.View == null)
            return;

        mopView = new GameObject("MopView").transform;
        mopView.SetParent(player.View.transform, false);
        Sprite broom = LoadWeaponSprite("Assets/Sprites/Rick/rickbroom.png");
        if (broom != null)
            broomFace = SpriteQuad("Broom", broom, mopView, new Vector3(0f, 0.12f, 0f), 2.05f).transform;
        else
        {
            QuadPiece("Handle", mopColor, mopView, new Vector3(0f, 0.05f, 0f), new Vector3(0.07f, 1.85f, 1f));
            QuadPiece("Mop Head", new Color(0.82f, 0.84f, 0.8f, 1f), mopView, new Vector3(0f, 1.02f, 0f), new Vector3(0.72f, 0.28f, 1f));
            QuadPiece("Strands", new Color(0.55f, 0.62f, 0.58f, 1f), mopView, new Vector3(0f, 0.82f, -0.01f), new Vector3(0.5f, 0.22f, 1f));
        }
        mopView.gameObject.SetActive(false);

        sprayView = new GameObject("SprayView").transform;
        sprayView.SetParent(player.View.transform, false);
        Sprite spray = LoadWeaponSprite("Assets/Sprites/Rick/RickSpray.png");
        if (spray != null)
            SpriteQuad("Bottle", spray, sprayView, new Vector3(0.02f, 0.04f, 0f), 0.62f);
        else
        {
            QuadPiece("Bottle", sprayColor, sprayView, Vector3.zero, new Vector3(0.22f, 0.4f, 1f));
            QuadPiece("Nozzle", new Color(0.85f, 0.95f, 1f, 1f), sprayView, new Vector3(0f, 0.28f, 0f), new Vector3(0.08f, 0.18f, 1f));
        }
        sprayView.localPosition = new Vector3(0f, -0.05f, 0.78f);
    }

    static GameObject SpriteQuad(string name, Sprite sprite, Transform parent, Vector3 localPos, float height)
    {
        GameObject piece = JANArt.Quad(name, Color.white, parent);
        piece.transform.localPosition = localPos;
        piece.transform.localRotation = Quaternion.identity;
        float unit = sprite.pixelsPerUnit;
        float native = sprite.rect.height / unit;
        float fit = native > 0.001f ? height / native : 1f;
        piece.transform.localScale = new Vector3(sprite.rect.width / unit * fit, sprite.rect.height / unit * fit, 1f);
        Renderer renderer = piece.GetComponent<Renderer>();
        Material mat = JANArt.SpriteMaterial();
        renderer.material = mat;
        JANArt.PaintSprite(mat, sprite);
        return piece;
    }

    static Sprite LoadWeaponSprite(string path)
    {
        return SpriteLibrary.Load(path);
    }

    static void QuadPiece(string name, Color color, Transform parent, Vector3 localPos, Vector3 scale)
    {
        GameObject piece = JANArt.Quad(name, color, parent);
        piece.transform.localPosition = localPos;
        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localScale = scale;
    }
}
