using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLevel : MonoBehaviour
{
    const float StreetLeft = -22f;
    const float StreetRight = 132f;
    const float CrosswalkX = 100f;
    const float BossCarAngle = -48f;
    const float BossCarBlockAngle = -68f;
    const float StreetCenterY = -1.5f;
    const float StreetHeight = 6f;
    const float FirstGateX = 20f;
    const float HulkStopX = 17f;
    const float SecondGateX = 42f;

    StreetObstacle firstGate;
    StreetObstacle bus;
    HulkEnemy hulk;
    readonly List<StreetEnemy> guards = new List<StreetEnemy>();
    bool entranceStarted;
    bool finaleStarted;
    bool crosswalkOpen;
    int busMotion;

    void Awake()
    {
        StretchStreet();
        PlaceDressing();
        firstGate = StreetObstacle.Create(StreetObstacle.Kind.Gate, new Vector2(FirstGateX, StreetCenterY), new Vector2(2.2f, 8f));
        hulk = PlaceHulk(new Vector2(36f, StreetCenterY));
        PlaceSideStreet();
        PlaceCrosswalk(new Vector2(CrosswalkX, StreetCenterY));
        bus = StreetObstacle.Create(StreetObstacle.Kind.Bus, new Vector2(CrosswalkX, StreetCenterY), new Vector2(3.4f, 6.8f));
    }

    public bool ReadyForFinale()
    {
        if (finaleStarted || FindAnyObjectByType<HulkEnemy>() != null)
            return false;

        StreetEnemy[] enemies = FindObjectsByType<StreetEnemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && !enemies[i].IsCleared)
                return false;
        }

        return true;
    }

    public void BeginFinale()
    {
        if (finaleStarted || !ReadyForFinale())
            return;

        finaleStarted = true;
        StartCoroutine(BusReturn());
        StartCoroutine(RickArrival());
    }

    IEnumerator RickArrival()
    {
        GameObject car = new GameObject("RickCar");
        Vector3 approachStart = new Vector3(109.5f, 3.3f, 0f);
        Vector3 approachStop = new Vector3(105.5f, -0.85f, 0f);
        Vector3 blockStop = new Vector3(112f, -1.2f, 0f);
        car.transform.position = approachStart;
        car.transform.rotation = Quaternion.Euler(0f, 0f, BossCarAngle);
        car.transform.localScale = new Vector3(5.6f, 1.9f, 1f);
        SpriteRenderer carRenderer = car.AddComponent<SpriteRenderer>();
        carRenderer.sprite = StreetObstacle.Square;
        carRenderer.color = new Color(0.12f, 0.16f, 0.28f, 1f);
        carRenderer.sortingOrder = 5;

        GameObject carLabelObject = new GameObject("Label");
        carLabelObject.transform.SetParent(car.transform, false);
        carLabelObject.transform.localRotation = Quaternion.identity;
        carLabelObject.transform.localScale = new Vector3(0.22f, 0.58f, 1f);
        TextMesh carLabel = carLabelObject.AddComponent<TextMesh>();
        carLabel.text = "CAR";
        carLabel.anchor = TextAnchor.MiddleCenter;
        carLabel.alignment = TextAlignment.Center;
        carLabel.fontSize = 32;
        carLabel.characterSize = 0.18f;
        carLabel.color = Color.white;
        carLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        carLabelObject.GetComponent<MeshRenderer>().sortingOrder = 6;

        while (Vector3.Distance(car.transform.position, approachStop) > 0.08f)
        {
            car.transform.position = Vector3.MoveTowards(car.transform.position, approachStop, 9f * Time.deltaTime);
            carRenderer.sortingOrder = -Mathf.RoundToInt(car.transform.position.y * 100f);
            yield return null;
        }

        car.transform.position = approachStop;
        car.transform.rotation = Quaternion.Euler(0f, 0f, BossCarAngle);
        yield return new WaitForSeconds(0.28f);

        float turn = 0f;
        while (turn < 1f)
        {
            turn = Mathf.Min(1f, turn + Time.deltaTime / 0.85f);
            float eased = turn * turn * (3f - 2f * turn);
            car.transform.position = Vector3.Lerp(approachStop, blockStop, eased);
            float angle = Mathf.Lerp(BossCarAngle, BossCarBlockAngle, eased);
            car.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            carRenderer.sortingOrder = -Mathf.RoundToInt(car.transform.position.y * 100f);
            yield return null;
        }

        car.transform.position = blockStop;
        car.transform.rotation = Quaternion.Euler(0f, 0f, BossCarBlockAngle);
        carRenderer.sortingOrder = -Mathf.RoundToInt(blockStop.y * 100f);
        car.AddComponent<StreetObstacle>().SealInPlace();
        yield return new WaitForSeconds(0.25f);

        Vector3 door = car.transform.TransformPoint(new Vector3(0.04f, -0.62f, 0f));
        Vector3 stepOut = car.transform.TransformPoint(new Vector3(0.08f, -1.05f, 0f));
        GameObject rickObject = new GameObject("Rick");
        rickObject.transform.position = door;
        Rigidbody2D rickBody = rickObject.AddComponent<Rigidbody2D>();
        rickBody.bodyType = RigidbodyType2D.Kinematic;
        rickBody.gravityScale = 0f;
        rickBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        BoxCollider2D rickBox = rickObject.AddComponent<BoxCollider2D>();
        rickBox.isTrigger = true;
        rickBox.size = new Vector2(1.05f, 2.1f);
        rickObject.AddComponent<AttackDummy>();
        RickJanitor rick = rickObject.AddComponent<RickJanitor>();

        while (((Vector2)rickObject.transform.position - (Vector2)stepOut).sqrMagnitude > 0.04f)
        {
            Vector2 next = Vector2.MoveTowards(rickObject.transform.position, stepOut, 7f * Time.deltaTime);
            rickBody.position = next;
            yield return null;
        }

        rickBody.position = stepOut;
        rick.Wake();
    }

    public void AddGuard(StreetEnemy enemy)
    {
        if (enemy != null && !guards.Contains(enemy))
            guards.Add(enemy);
    }

    void Update()
    {
        WatchHulkEntrance();
        WatchBus();
    }

    void WatchHulkEntrance()
    {
        if (entranceStarted || hulk == null || guards.Count == 0)
            return;

        for (int i = 0; i < guards.Count; i++)
        {
            if (guards[i] != null && !guards[i].IsCleared)
                return;
        }

        entranceStarted = true;
        hulk.ChargeIn(firstGate, HulkStopX, SecondGateX - 4f, () =>
        {
            StreetObstacle gate = StreetObstacle.Create(
                StreetObstacle.Kind.Gate,
                new Vector2(SecondGateX, StreetCenterY),
                new Vector2(2.2f, 8f));
            hulk.SealBehind(gate);
            hulk.LockArena(FirstGateX - 4f, SecondGateX - 2.4f);
        }, new Vector2(28f, StreetCenterY), SealRearCars);
    }

    void WatchBus()
    {
        if (crosswalkOpen || bus == null || finaleStarted)
            return;
        if (!PostHulkClear())
            return;

        crosswalkOpen = true;
        StartCoroutine(BusLeave());
    }

    bool PostHulkClear()
    {
        HulkEnemy living = FindAnyObjectByType<HulkEnemy>();
        if (living != null && !living.IsOutOfFight)
            return false;

        StreetEnemy[] enemies = FindObjectsByType<StreetEnemy>();
        if (enemies.Length == 0)
            return false;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && !enemies[i].IsCleared)
                return false;
        }

        return true;
    }

    void SealRearCars()
    {
        StartCoroutine(DriveRearCars());
    }

    IEnumerator DriveRearCars()
    {
        float[] lanes = { 0.55f, -1.5f, -3.55f };
        float[] heights = { 2.25f, 2.1f, 2.25f };
        StreetObstacle[] cars = new StreetObstacle[lanes.Length];
        for (int i = 0; i < lanes.Length; i++)
            cars[i] = StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(8f, lanes[i]), new Vector2(3.2f, heights[i]));

        float x = 8f;
        const float parkX = 17.6f;
        while (x < parkX - 0.05f)
        {
            x = Mathf.MoveTowards(x, parkX, 16f * Time.deltaTime);
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i] != null)
                    cars[i].transform.position = new Vector3(x, lanes[i], 0f);
            }

            yield return null;
        }

        if (hulk != null)
            hulk.LockArena(parkX + 2.2f, SecondGateX - 2.4f);
    }

    IEnumerator BusLeave()
    {
        int token = ++busMotion;
        Vector3 away = new Vector3(CrosswalkX, 11f, 0f);
        while (token == busMotion && bus != null && Vector3.Distance(bus.transform.position, away) > 0.12f)
        {
            bus.transform.position = Vector3.MoveTowards(bus.transform.position, away, 14f * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator BusReturn()
    {
        if (bus == null)
            yield break;

        int token = ++busMotion;
        Vector3 park = new Vector3(CrosswalkX - 4.6f, StreetCenterY, 0f);
        Vector3 hover = new Vector3(park.x, 8.2f, 0f);
        while (token == busMotion && (Mathf.Abs(bus.transform.position.x - hover.x) > 0.12f || bus.transform.position.y < hover.y - 0.2f))
        {
            Vector3 position = bus.transform.position;
            position.x = Mathf.MoveTowards(position.x, hover.x, 16f * Time.deltaTime);
            position.y = Mathf.MoveTowards(position.y, hover.y, 16f * Time.deltaTime);
            bus.transform.position = position;
            yield return null;
        }

        while (token == busMotion && Vector3.Distance(bus.transform.position, park) > 0.08f)
        {
            bus.transform.position = Vector3.MoveTowards(bus.transform.position, park, 13f * Time.deltaTime);
            yield return null;
        }

        if (token == busMotion && bus != null)
            bus.transform.position = park;
    }

    void StretchStreet()
    {
        float width = StreetRight - StreetLeft;
        transform.position = new Vector3((StreetLeft + StreetRight) * 0.5f, StreetCenterY, transform.position.z);
        transform.localScale = new Vector3(width, StreetHeight, 1f);
    }

    void PlaceDressing()
    {
        StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(-8.2f, 0.55f), new Vector2(3.3f, 1.05f));
        StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(1f, -3.55f), new Vector2(3.6f, 1.2f));
        StreetObstacle.Create(StreetObstacle.Kind.Blockade, new Vector2(9f, -0.2f), new Vector2(1.05f, 3.3f));
        StreetObstacle.Create(StreetObstacle.Kind.Manhole, new Vector2(15f, -2.8f), new Vector2(1.2f, 1.2f));

        StreetObstacle.Create(StreetObstacle.Kind.Manhole, new Vector2(25f, -2.4f), new Vector2(1.15f, 1.15f));
        StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(29f, 0.45f), new Vector2(3.7f, 1.1f));
        StreetObstacle.Create(StreetObstacle.Kind.Blockade, new Vector2(35f, -3.15f), new Vector2(1f, 2.1f));

        StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(50f, -3.45f), new Vector2(3.6f, 1.15f));
        StreetObstacle.Create(StreetObstacle.Kind.Blockade, new Vector2(58f, 0.2f), new Vector2(1.05f, 2.8f));
        StreetObstacle.Create(StreetObstacle.Kind.Manhole, new Vector2(64f, -3.55f), new Vector2(1.15f, 1.15f));
        StreetObstacle.Create(StreetObstacle.Kind.Car, new Vector2(70f, 0.55f), new Vector2(3.4f, 1.05f));
        StreetObstacle.Create(StreetObstacle.Kind.Manhole, new Vector2(78f, -3.2f), new Vector2(1.2f, 1.2f));
    }

    void PlaceSideStreet()
    {
        GameObject road = new GameObject("SideStreet");
        road.transform.position = new Vector3(110f, 5.4f, 0f);
        road.transform.localScale = new Vector3(12f, 8.5f, 1f);
        SpriteRenderer renderer = road.AddComponent<SpriteRenderer>();
        renderer.sprite = StreetObstacle.Square;
        renderer.color = new Color(0.28f, 0.29f, 0.32f, 1f);
        renderer.sortingOrder = -450;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(road.transform, false);
        labelObject.transform.localScale = new Vector3(0.14f, 0.2f, 1f);
        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = "SIDE STREET";
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 32;
        label.characterSize = 0.22f;
        label.color = new Color(0.85f, 0.85f, 0.8f, 1f);
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelObject.GetComponent<MeshRenderer>().sortingOrder = -440;
    }

    static HulkEnemy PlaceHulk(Vector2 position)
    {
        GameObject hulkObject = new GameObject("The Baron Poundmaster");
        hulkObject.transform.position = position;

        Rigidbody2D body = hulkObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        BoxCollider2D box = hulkObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(1.8f, 2.5f);

        hulkObject.AddComponent<AttackDummy>();
        return hulkObject.AddComponent<HulkEnemy>();
    }

    static void PlaceCrosswalk(Vector2 position)
    {
        GameObject crosswalk = new GameObject("Crosswalk");
        crosswalk.transform.position = position;

        BoxCollider2D box = crosswalk.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(3.6f, 6.2f);

        GameObject pad = new GameObject("Pad");
        pad.transform.SetParent(crosswalk.transform, false);
        pad.transform.localScale = new Vector3(3.6f, 6.2f, 1f);
        SpriteRenderer padRenderer = pad.AddComponent<SpriteRenderer>();
        padRenderer.sprite = StreetObstacle.Square;
        padRenderer.color = new Color(0.2f, 0.2f, 0.22f, 1f);
        padRenderer.sortingOrder = -420;

        for (int i = 0; i < 6; i++)
        {
            GameObject stripe = new GameObject("Stripe");
            stripe.transform.SetParent(crosswalk.transform, false);
            stripe.transform.localPosition = new Vector3(-1.25f + i * 0.5f, 0f, 0f);
            stripe.transform.localScale = new Vector3(0.22f, 5.4f, 1f);
            SpriteRenderer stripeRenderer = stripe.AddComponent<SpriteRenderer>();
            stripeRenderer.sprite = StreetObstacle.Square;
            stripeRenderer.color = new Color(0.92f, 0.92f, 0.88f, 1f);
            stripeRenderer.sortingOrder = -410;
        }

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(crosswalk.transform, false);
        labelObject.transform.localPosition = new Vector3(0f, 2.55f, 0f);
        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = "CROSSWALK";
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 32;
        label.characterSize = 0.1f;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelObject.GetComponent<MeshRenderer>().sortingOrder = -400;

        crosswalk.AddComponent<LevelEnd>();
    }
}

public class LevelEnd : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<DogHealth>() == null)
            return;
        if (other.transform.position.x < transform.position.x + 0.6f)
            return;

        StreetLevel level = FindAnyObjectByType<StreetLevel>();
        if (level != null)
            level.BeginFinale();
    }
}
