using System.Collections;
using UnityEngine;

public class cutSceneManagerTruck : MonoBehaviour
{
    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject alien;
    [SerializeField] private GameObject house;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject mailMan;
    [SerializeField] private spawnController spawnController;
    [SerializeField] private GameManagerTruck gameManager;
    [SerializeField] private PlayerControllerTruck playerController;
    [SerializeField, Min(0f)] private float slowdownDuration = 4f;
    [SerializeField, Min(0f)] private float victoryDisplayHoldDuration = 1f;
    [SerializeField, Min(0f)] private float finalScreenHoldDuration = 3f;
    [SerializeField] private GameObject leftShoulder;
    [SerializeField] private GameObject RightShoulder;
    [SerializeField, Min(0.1f)] private float truckTurnDuration = 1.2f;
    [SerializeField, Min(0f)] private float truckTurnAngle = 35f;
    [SerializeField, Min(0f)] private float crashOverlap = 0.02f;
    [SerializeField, Min(0.1f)] private float characterMovementSpeed = 2f;
    [SerializeField, Min(0f)] private float alienRiseHeight = 2f;
    [SerializeField] GameWideManager gameWideManager;

    public bool inCutscene;

    void Awake()
    {
        inCutscene = false;
        ResolveSceneReferences();
    }

    private void ResolveSceneReferences()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManagerTruck>();

        if (spawnController == null)
            spawnController = FindAnyObjectByType<spawnController>();

        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerControllerTruck>();

        if (gameWideManager == null)
            gameWideManager = GameWideManager.EnsureExists();
    }

    public IEnumerator cutscene()
    {
        inCutscene = true;
        ResolveSceneReferences();

        if (gameManager == null)
        {
            Debug.LogError("Truck finale could not start: no GameManagerTruck exists in the scene.", this);
            inCutscene = false;
            yield break;
        }

        Debug.Log("Truck finale started: spawning is stopped and the slowdown is beginning.", this);
        float startingSpeed = Mathf.Max(0f, gameManager.TruckSpeed);
        if (spawnController == null)
        {
            Debug.LogError("Truck finale could not stop spawning: no spawnController exists in the scene.", this);
            inCutscene = false;
            yield break;
        }

        if (truck == null && playerController != null)
            truck = playerController.gameObject;

        if (truck == null)
        {
            Debug.LogError("Truck finale could not steer into the house: no truck object is assigned or found.", this);
            inCutscene = false;
            yield break;
        }

        Transform truckTransform = truck.transform;
        GameObject finalHouse = spawnController.BeginFinale(
            house, Camera.main, truckTransform, startingSpeed, slowdownDuration);
        if (finalHouse == null)
        {
            Debug.LogError("Truck finale could not create its final house.", this);
            inCutscene = false;
            yield break;
        }

        Rigidbody2D truckBody = truck.GetComponent<Rigidbody2D>();
        Vector3 truckStartPosition = truckTransform.position;
        Quaternion truckStartRotation = truckTransform.rotation;
        float turnDirection = Mathf.Sign(finalHouse.transform.position.x - truckStartPosition.x);
        if (turnDirection == 0f)
            turnDirection = 1f;
        float truckExtent = GetHorizontalColliderExtent(truck, truckStartPosition.x);
        float houseExtent = GetHorizontalColliderExtent(finalHouse, finalHouse.transform.position.x);
        float truckStopX = finalHouse.transform.position.x
            - turnDirection * Mathf.Max(0f, truckExtent + houseExtent - crashOverlap);
        float turnDuration = Mathf.Min(truckTurnDuration, slowdownDuration);
        float turnStartTime = Mathf.Max(0f, slowdownDuration - turnDuration);
        bool turnStarted = false;

        if (slowdownDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < slowdownDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / slowdownDuration);
                gameManager.TruckSpeed = Mathf.Lerp(
                    startingSpeed,
                    0f,
                    Mathf.SmoothStep(0f, 1f, progress));

                if (!turnStarted && elapsed >= turnStartTime)
                {
                    turnStarted = true;
                    DisableShoulderColliders();
                    float angle = turnDirection > 0f ? -truckTurnAngle : truckTurnAngle;
                    if (truckBody != null)
                        truckBody.rotation = truckStartRotation.eulerAngles.z + angle;
                    else
                        truckTransform.rotation = truckStartRotation * Quaternion.Euler(0f, 0f, angle);
                }

                if (turnStarted)
                {
                    float turnProgress = turnDuration > 0f
                        ? Mathf.Clamp01((elapsed - turnStartTime) / turnDuration)
                        : 1f;
                    Vector3 turnedPosition = truckStartPosition;
                    turnedPosition.x = Mathf.Lerp(
                        truckStartPosition.x,
                        truckStopX,
                        turnProgress);

                    if (truckBody != null)
                        truckBody.position = turnedPosition;
                    else
                        truckTransform.position = turnedPosition;
                }

                yield return null;
            }
        }

        gameManager.TruckSpeed = 0f;
        if (truckBody != null)
            truckBody.linearVelocity = Vector2.zero;
        yield return new WaitForFixedUpdate();
        Debug.Log("Truck finale slowdown complete: truck, map, and spawned traffic are stopped.", this);

        if (mailMan == null)
        {
            Debug.LogError("Truck finale cannot continue: Mailman prefab is not assigned.", this);
            yield break;
        }

        GameObject theMailMan = Instantiate(mailMan, truck.transform.position, Quaternion.identity);
        PutSpritesInFrontOfTruck(theMailMan);

        if (victoryDisplayHoldDuration > 0f)
            yield return new WaitForSeconds(victoryDisplayHoldDuration);

        if (playerController != null)
            playerController.HideVictoryUI();
        else
            Debug.LogWarning("PlayerControllerTruck was not found; victory UI could not be hidden.", this);

        if (finalScreenHoldDuration > 0f)
            yield return new WaitForSeconds(finalScreenHoldDuration);

        if (dog == null)
        {
            Debug.LogError("Truck finale cannot continue: Dog prefab is not assigned.", this);
            yield break;
        }

        GameObject theDog = Instantiate(dog, finalHouse.transform.position, Quaternion.identity);
        PutSpritesInFrontOfTruck(theDog);
        yield return MoveObjectTo(theDog, theMailMan.transform.position);

        if (alien == null)
        {
            Debug.LogError("Truck finale cannot continue: Alien prefab is not assigned.", this);
            yield break;
        }

        GameObject theAlien = Instantiate(alien, truck.transform.position, Quaternion.identity);
    PutSpritesInFrontOfTruck(theAlien);
        Vector3 alienRiseTarget = theAlien.transform.position + Vector3.up * alienRiseHeight;
        yield return MoveObjectTo(theAlien, alienRiseTarget);
        yield return new WaitForSeconds(0.5f);
        yield return MoveObjectTo(theAlien, theDog.transform.position);
        Destroy(theAlien);

        yield return MoveObjectTo(theDog, finalHouse.transform.position);
        yield return new WaitForSeconds(0.5f);

        if (gameWideManager == null)
            gameWideManager = GameWideManager.EnsureExists();
        if (gameWideManager != null && gameManager != null)
            gameWideManager.modScore(Mathf.RoundToInt(2000f * gameManager.health));
    
        UnityEngine.SceneManagement.SceneManager.LoadScene("DogScene");
    }

    private void PutSpritesInFrontOfTruck(GameObject character)
    {
        SpriteRenderer truckRenderer = truck.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer[] characterRenderers = character.GetComponentsInChildren<SpriteRenderer>();
        if (truckRenderer == null)
            return;

        foreach (SpriteRenderer characterRenderer in characterRenderers)
        {
            characterRenderer.sortingLayerID = truckRenderer.sortingLayerID;
            characterRenderer.sortingOrder = truckRenderer.sortingOrder + 1;
        }
    }

    private IEnumerator MoveObjectTo(GameObject movingObject, Vector3 destination)
    {
        if (movingObject == null)
            yield break;

        float speed = Mathf.Max(0.1f, characterMovementSpeed);
        Rigidbody2D body = movingObject.GetComponent<Rigidbody2D>();
        if (body == null)
            body = movingObject.GetComponentInChildren<Rigidbody2D>();

        while (Vector2.Distance(movingObject.transform.position, destination) > 0.05f)
        {
            Vector3 nextPosition = Vector3.MoveTowards(
                movingObject.transform.position,
                destination,
                speed * Time.fixedDeltaTime);

            if (body != null)
                body.MovePosition(nextPosition);
            else
                movingObject.transform.position = nextPosition;

            yield return new WaitForFixedUpdate();
        }

        if (body != null)
        {
            body.MovePosition(destination);
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            movingObject.transform.position = destination;
        }
    }

    private void DisableShoulderColliders()
    {
        DisableColliders(leftShoulder);
        DisableColliders(RightShoulder);
    }

    private static void DisableColliders(GameObject shoulder)
    {
        if (shoulder == null)
            return;

        foreach (Collider2D shoulderCollider in shoulder.GetComponentsInChildren<Collider2D>())
            shoulderCollider.enabled = false;
    }

    private static float GetHorizontalColliderExtent(GameObject target, float fallbackX)
    {
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>();
        bool foundBounds = false;
        Bounds combinedBounds = new Bounds(new Vector3(fallbackX, target.transform.position.y, target.transform.position.z), Vector3.zero);

        foreach (Collider2D targetCollider in colliders)
        {
            if (!targetCollider.enabled)
                continue;

            if (foundBounds)
                combinedBounds.Encapsulate(targetCollider.bounds);
            else
            {
                combinedBounds = targetCollider.bounds;
                foundBounds = true;
            }
        }

        if (foundBounds)
            return combinedBounds.extents.x;

        Renderer targetRenderer = target.GetComponentInChildren<Renderer>();
        return targetRenderer != null ? targetRenderer.bounds.extents.x : 0f;
    }
}
