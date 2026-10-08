using UnityEngine;
using System.Collections;


public class cutsceneManagerSheep : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    public bool inCutscene;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject hedge;
    [SerializeField] private GameObject truck;
    [SerializeField, Min(0f)] private float cameraPanDuration = 7f;
    [SerializeField] private GameObject alien;
    [SerializeField] private GameObject[] startCheckpoints;

    [SerializeField] private GameObject[] endCheckpoints;
    private GameWideManager gameManager;

    void Awake()
    {
        inCutscene = true;
    }

    void Start()
    {
        gameManager = GameWideManager.myGameManager;
        if (gameManager == null)
        {
            Debug.LogWarning("No GameWideManager found; skipping the sheep cutscene.", this);
            inCutscene = false;
            return;
        }

        if (gameManager.hasStarted)
        {
            inCutscene = false;
            return;
        }

        gameManager.hasStarted = true;
        StartCoroutine(firstCutscene());
    }

    private IEnumerator firstCutscene()
    {
        inCutscene = true;

        if (sceneCamera == null)
            sceneCamera = Camera.main;

        if (sceneCamera == null || player.transform == null)
        {
            inCutscene = false;
            yield break;
        }

        Vector3 startPosition = sceneCamera.transform.position;
        Vector3 targetPosition = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            startPosition.z);
        yield return new WaitForSeconds(1.5f);

        if (cameraPanDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < cameraPanDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / cameraPanDuration);
                float easedProgress = progress < 0.5f
                    ? 4f * progress * progress * progress
                    : 1f - Mathf.Pow(-2f * progress + 2f, 3f) / 2f;
                sceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
                yield return null;
            }
        }

        sceneCamera.transform.position = targetPosition;

        if (hedge != null)
        {
            hedge.transform.position += new Vector3(-0.2f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
            hedge.transform.position += new Vector3(0.4f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
            hedge.transform.position += new Vector3(-0.4f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
            hedge.transform.position += new Vector3(0.4f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
            hedge.transform.position += new Vector3(-0.4f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
            hedge.transform.position += new Vector3(0.2f, 0f, 0f);
            yield return new WaitForSeconds(0.1f);
        }

        if (alien != null)
        {
            alien.SetActive(true);

            if (startCheckpoints != null)
            {
                foreach (GameObject checkpoint in startCheckpoints)
                {
                    if (checkpoint == null)
                        continue;

                    Vector3 segmentStart = alien.transform.position;
                    Vector3 segmentEnd = checkpoint.transform.position;
                    float elapsed = 0f;
                    while (elapsed < 0.5f)
                    {
                        elapsed = Mathf.Min(elapsed + Time.deltaTime, 0.5f);
                        float progress = elapsed / 0.5f;
                        alien.transform.position = Vector3.Lerp(segmentStart, segmentEnd, progress);
                        yield return null;
                    }

                    alien.transform.position = segmentEnd;
                }
            }

            alien.SetActive(false);
        }

        inCutscene = false;
    }

    public IEnumerator secondCutscene()
    {
        inCutscene = true;

        alien.SetActive(true);
        alien.transform.position = player.transform.position;
        foreach (GameObject checkpoint in endCheckpoints)
                {
                    if (checkpoint == null)
                        continue;

                    Vector3 segmentStart = alien.transform.position;
                    Vector3 segmentEnd = checkpoint.transform.position;
                    float elapsed = 0f;
                    while (elapsed < 0.5f)
                    {
                        elapsed = Mathf.Min(elapsed + Time.deltaTime, 0.5f);
                        float progress = elapsed / 0.5f;
                        alien.transform.position = Vector3.Lerp(segmentStart, segmentEnd, progress);
                        yield return null;
                    }

                    alien.transform.position = segmentEnd;
                }
        alien.SetActive(false);
        if (truck != null)
        {
            Quaternion startingRotation = truck.transform.localRotation;
            for (int i = 0; i < 4; i++)
            {
                truck.transform.localRotation = startingRotation * Quaternion.Euler(0f, 0f, 30f);
                yield return new WaitForSeconds(0.1f);
                truck.transform.localRotation = startingRotation * Quaternion.Euler(0f, 0f, -30f);
                yield return new WaitForSeconds(0.1f);
            }

            truck.transform.localRotation = startingRotation;
        }

        truck.GetComponent<Rigidbody2D>().linearVelocityX = 5f;
        yield return new WaitForSeconds(3f);
        inCutscene = false;
        gameManager.modScore(System.Math.Max(0, (18 - gameManager.madFarmers) * 100));
        UnityEngine.SceneManagement.SceneManager.LoadScene("TruckScene");

        yield break;
    }
}
