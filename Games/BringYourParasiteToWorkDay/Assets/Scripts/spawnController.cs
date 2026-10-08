using UnityEngine;
using System.Collections.Generic;

public class spawnController : MonoBehaviour
{
    
    [SerializeField] private GameObject marker;
    [SerializeField] private GameObject[] houses;
    [SerializeField] private GameObject[] cars;
    [SerializeField] private GameManagerTruck gameManager;
    [SerializeField, Min(0.01f)] private float finaleMarkerInterval = 0.15f;
    [SerializeField, Min(1f)] private float finaleMarkerSpacingMultiplier = 1.5f;
    // private int markerCounter;
    // private int houseCounter;
    // private int carCounter;
    // private int carInterval;

    // private int houseInterval;
    // private int markerInterval;
    private float markerTimer;
    private float houseTimer;
    private float carTimer;
    private float markerInterval;
    private float houseInterval;
    private float carInterval;
    private float baseMarkerInterval;
    private float baseHouseInterval;
    private float baseCarInterval;
    private float speedFactor;
    private bool finaleStarted;
    private readonly List<MovingObject> movingObjects = new List<MovingObject>();

    private sealed class MovingObject
    {
        public Rigidbody2D body;
        public float speedMultiplier;

        public MovingObject(Rigidbody2D body, float speedMultiplier)
        {
            this.body = body;
            this.speedMultiplier = speedMultiplier;
        }
    }

    
    void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManagerTruck>();

        markerTimer = 0;
        houseTimer = 0;
        carTimer = 0;
        markerInterval = 0.2f;
        houseInterval = 1f;
        carInterval = 2f;
        baseMarkerInterval = markerInterval;
        baseHouseInterval = houseInterval;
        baseCarInterval = carInterval;
        speedFactor = gameManager != null && gameManager.minSpeed > 0f
            ? gameManager.maxSpeed / gameManager.minSpeed
            : 1f;
    }

    public GameObject BeginFinale(
        GameObject finalHousePrefab,
        Camera sceneCamera,
        Transform truckTransform,
        float startingSpeed,
        float slowdownDuration)
    {
        if (finaleStarted)
            return null;

        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManagerTruck>();

        if (gameManager == null)
        {
            Debug.LogError("Cannot begin truck finale because no GameManagerTruck exists.", this);
            return null;
        }

        finaleStarted = true;
        markerTimer = 0f;
        houseTimer = 0f;
        carTimer = 0f;

        if (finalHousePrefab == null && houses != null && houses.Length > 0)
            finalHousePrefab = houses[Random.Range(0, houses.Length)];

        if (sceneCamera == null)
            sceneCamera = Camera.main;

        float centerX = sceneCamera != null ? sceneCamera.transform.position.x : transform.position.x;
        float screenCenterY = sceneCamera != null ? sceneCamera.transform.position.y : transform.position.y;
        float houseStopY = truckTransform != null ? truckTransform.position.y : screenCenterY;
        float travelDistance = Mathf.Max(0f, startingSpeed) * Mathf.Max(0f, slowdownDuration) * 0.5f;

        if (sceneCamera != null && sceneCamera.orthographic && marker != null)
        {
            float cameraHalfHeight = sceneCamera.orthographicSize;
            Renderer markerRenderer = marker.GetComponentInChildren<Renderer>();
            float markerHalfHeight = markerRenderer != null ? markerRenderer.bounds.extents.y : 0f;
            float firstRowY = sceneCamera.transform.position.y - cameraHalfHeight + markerHalfHeight;
            float lastRowY = sceneCamera.transform.position.y + cameraHalfHeight - markerHalfHeight;
            float nominalSpacing = Mathf.Max(
                0.1f,
                gameManager.minSpeed * baseMarkerInterval * finaleMarkerSpacingMultiplier);
            float usableHeight = Mathf.Max(0f, lastRowY - firstRowY);
            int rowCount = Mathf.Max(2, Mathf.CeilToInt(usableHeight / nominalSpacing) + 1);
            float rowSpacing = usableHeight / (rowCount - 1);

            // Fill the final camera view with a slightly wider spacing between marker rows.
            for (int row = 0; row < rowCount; row++)
            {
                float rowStopY = firstRowY + rowSpacing * row;
                float spawnY = rowStopY + travelDistance;
                SpawnMovingObject(marker, new Vector3(centerX - 1.8f, spawnY, transform.position.z), 1f);
                SpawnMovingObject(marker, new Vector3(centerX + 1.8f, spawnY, transform.position.z), 1f);
            }
        }
        else if (marker != null)
        {
            float spawnY = screenCenterY + travelDistance;
            SpawnMovingObject(marker, new Vector3(centerX - 1.8f, spawnY, transform.position.z), 1f);
            SpawnMovingObject(marker, new Vector3(centerX + 1.8f, spawnY, transform.position.z), 1f);
        }

        if (finalHousePrefab == null)
        {
            Debug.LogWarning("Truck finale started without a final house prefab assigned or available.", this);
            return null;
        }

        int side = Random.Range(0, 2) == 0 ? -1 : 1;
        float targetX = transform.position.x + side * 7.5f;
        if (sceneCamera != null)
        {
            float halfScreenWidth = sceneCamera.orthographic
                ? sceneCamera.orthographicSize * sceneCamera.aspect
                : Mathf.Abs(sceneCamera.transform.position.z) * Mathf.Tan(sceneCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * sceneCamera.aspect;
            Renderer houseRenderer = finalHousePrefab.GetComponentInChildren<Renderer>();
            float houseHalfWidth = houseRenderer != null
                ? houseRenderer.bounds.extents.x
                : 0f;
            float maxOffset = Mathf.Max(0f, halfScreenWidth - houseHalfWidth - 0.25f);
            targetX = centerX + side * Mathf.Min(7.5f, maxOffset);
        }

        GameObject finalHouse = SpawnMovingObject(
            finalHousePrefab,
            new Vector3(targetX, houseStopY + travelDistance, transform.position.z),
            1f);

        if (finalHouse != null && side == 1)
            finalHouse.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        return finalHouse;
    }

    void FixedUpdate()
    {
        if (gameManager == null)
            return;

        UpdateMovingObjects();
        if (finaleStarted)
        {
            // Keep the road markings flowing during the slowdown so there is no empty stretch
            // between the existing markers and the finale's final full-screen marker field.
            if (gameManager.TruckSpeed > 0.01f && marker != null)
            {
                markerTimer += Time.fixedDeltaTime;
                if (markerTimer >= finaleMarkerInterval)
                {
                    SpawnMovingObject(marker, transform.position + new Vector3(-1.8f, 0f, 0f), 1f);
                    SpawnMovingObject(marker, transform.position + new Vector3(1.8f, 0f, 0f), 1f);
                    markerTimer = 0f;
                }
            }

            return;
        }

        markerTimer += Time.fixedDeltaTime;
        houseTimer += Time.fixedDeltaTime;
        carTimer += Time.fixedDeltaTime;
        float speed = gameManager.TruckSpeed;
        float speedRatio = speed / Mathf.Max(gameManager.minSpeed, 0.01f);

        markerInterval = Mathf.Max(0.2f / speedFactor, baseMarkerInterval / Mathf.Max(speedRatio, 0.01f));
        houseInterval = Mathf.Max(1f / speedFactor, baseHouseInterval / Mathf.Max(speedRatio, 0.01f));
        carInterval = Mathf.Max(2f / speedFactor, baseCarInterval / Mathf.Max(speedRatio, 0.01f));


        if (markerTimer >= markerInterval)
        {
            SpawnMovingObject(marker, transform.position + new Vector3(-1.8f, 0f, 0f), 1f);
            SpawnMovingObject(marker, transform.position + new Vector3(1.8f, 0f, 0f), 1f);
            markerTimer = 0;

        }

        if (houseTimer >= houseInterval)
        {
            if (houses != null && houses.Length > 0)
                SpawnHouse(houses[Random.Range(0, houses.Length)]);
            houseTimer = 0;
            houseInterval = Random.Range(1f, 2.5f);
            baseHouseInterval = houseInterval;
        }

        if (carTimer >= carInterval)
        {
            GameObject carPrefab = null;
            int randLane = Random.Range(0, 3);
            
            switch (randLane)
            {
                case 0:
                    if (cars != null && cars.Length > 0)
                        carPrefab = cars[Random.Range(0, cars.Length)];
                    SpawnMovingObject(carPrefab, transform.position + new Vector3(-3.25f, 2f, 0f), 0.5f);
                    break;
                case 1:
                    if (cars != null && cars.Length > 0)
                        carPrefab = cars[Random.Range(0, cars.Length)];
                    SpawnMovingObject(carPrefab, transform.position + new Vector3(0f, 2f, 0f), 0.5f);
                    break;
                case 2:
                    if (cars != null && cars.Length > 0)
                        carPrefab = cars[Random.Range(0, cars.Length)];
                    SpawnMovingObject(carPrefab, transform.position + new Vector3(3.25f, 2f, 0f), 0.5f);
                    break;
            }

            if (carPrefab != null)
            {
                carTimer = 0;
                carInterval = Random.Range(2f, 4f);
                baseCarInterval = carInterval;
            }
            
        }
    }

    private void SpawnHouse(GameObject housePrefab)
    {
        if (housePrefab == null)
            return;

        int side = Random.Range(0, 2) == 0 ? -1 : 1;
        GameObject spawnedHouse = SpawnMovingObject(
            housePrefab,
            transform.position + new Vector3(side * 7.5f, 2f, 0f),
            1f);

        if (spawnedHouse != null && side == 1)
            spawnedHouse.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
    }

    private GameObject SpawnMovingObject(GameObject prefab, Vector3 position, float speedMultiplier)
    {
        if (prefab == null)
            return null;

        GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody2D body = spawnedObject.GetComponent<Rigidbody2D>();
        if (body == null)
            body = spawnedObject.GetComponentInChildren<Rigidbody2D>();
        if (body != null)
        {
            movingObjects.Add(new MovingObject(body, speedMultiplier));
            body.linearVelocity = Vector2.down * gameManager.TruckSpeed * speedMultiplier;
        }

        return spawnedObject;
    }

    private void UpdateMovingObjects()
    {
        for (int i = movingObjects.Count - 1; i >= 0; i--)
        {
            MovingObject movingObject = movingObjects[i];
            if (movingObject.body == null)
            {
                movingObjects.RemoveAt(i);
                continue;
            }

            float verticalSpeed = gameManager.TruckSpeed <= 0.001f
                ? 0f
                : -gameManager.TruckSpeed * movingObject.speedMultiplier;
            movingObject.body.linearVelocity = new Vector2(0f, verticalSpeed);
        }
    }
}
