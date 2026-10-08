using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FarmerController : MonoBehaviour
{

    
    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject[] checkpoints;

    [SerializeField] private LayerMask wallMask;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private PolygonCollider2D shapeCollider;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private UnityEvent onTargetEntered;
    [SerializeField] private SpriteRenderer exclamationMark;
    [SerializeField] private FarmGrid farmGrid;
    [SerializeField] private float playerRepathInterval = 0.5f;
    [SerializeField] private float waypointReachDistance = 0.1f;
    [SerializeField] private PatrolDirection patrolDirection;
    [SerializeField] private PlayerControllerSheep playerSheep;
    [SerializeField] private Image caughtBorder;
    [SerializeField] private TextMeshProUGUI caughtText;
    [SerializeField] private cutsceneManagerSheep cutsceneManager;
    [SerializeField] private GameWideManager gameWideManager;

    private enum PatrolDirection { Forward, Backward, loop }

    private int checkpointInd;

    private Vector3 currTarget;

    int rayCount = 30;
    float angle = 60f;
    float viewDistance = 5f;
    
    bool discovered;
    bool pausing;

    private Transform playerTransform;
    private Vector2[] currentPath = new Vector2[0];
    private int pathIndex;
    private float nextPlayerRepathTime;
    private Vector2 lastPlannedPlayerPosition;

    private Rigidbody2D rb;

    Vector3[] vertices;
    Vector2[] colliderPoints;
    
    void Awake()
    {
        vertices = new Vector3[rayCount + 1];
        colliderPoints = new Vector2[rayCount + 1];
        vertices[0] = Vector3.zero;
        if (shapeCollider == null)
        {
            shapeCollider = GetComponent<PolygonCollider2D>();
        }
        shapeCollider.isTrigger = true;
        checkpointInd = 0;
        currTarget = checkpoints[checkpointInd].transform.position;
        rb = GetComponent<Rigidbody2D>();
        discovered = false;
        pausing = false;
    }
        

    void Start()
    {
        PlanPathTo(currTarget);
    }

    void Update()
    {
        bool playerWon = playerSheep != null && playerSheep.getVictorious();
        if ((cutsceneManager != null && cutsceneManager.inCutscene) || playerWon)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (discovered && !cutsceneManager.inCutscene)
        {
            if (playerTransform == null)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            currTarget = playerTransform.position;
            if (Time.time >= nextPlayerRepathTime ||
                Vector2.Distance(currTarget, lastPlannedPlayerPosition) >= 0.5f)
            {
                PlanPathTo(currTarget);
                lastPlannedPlayerPosition = currTarget;
                nextPlayerRepathTime = Time.time + playerRepathInterval;
            }
        }

        //which direction to look at
        Vector3 lookDirection = (currTarget - transform.position).normalized;

        if (pausing)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            MoveAlongPath();
        }

        // Advance patrol checkpoints when the farmer reaches the current one.
        if (!discovered && Vector2.Distance(rb.position, currTarget) < 0.3f)
        {
            AdvanceCheckpoint();
        }

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = -angle/2 + (angle * i / (rayCount - 1));
            Vector3 dir = Quaternion.Euler(0,0, currentAngle) * lookDirection;

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, dir, viewDistance, wallMask);

            float dist = hit ? hit.distance : viewDistance;
            vertices[i+1] = dir * dist;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        mesh.triangles = CreateTriangles();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        for (int i = 0; i < vertices.Length; i++)
        {
            colliderPoints[i] = vertices[i];
        }
        shapeCollider.SetPath(0, colliderPoints);
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (!discovered && player.CompareTag(targetTag))
        {
            onTargetEntered?.Invoke();
            Debug.Log("Discovered!");
            discovered = true;
            gameWideManager.madFarmers++;
            rb.linearVelocity = Vector2.zero;
            playerTransform = player.transform;
            currTarget = playerTransform.position;
            moveSpeed += 2f;
            PlanPathTo(currTarget);
            lastPlannedPlayerPosition = currTarget;
            nextPlayerRepathTime = Time.time + playerRepathInterval;
            StartCoroutine(catchPlayer());
            
        }
    }

    //method for doing something if the farmer's collider is triggered by the player
    private void OnCollisionEnter2D(Collision2D player)
    {
        if (!playerSheep.getVictorious() && player.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Caught!");
            StartCoroutine(restartStage());           
        }
    }

    private void AdvanceCheckpoint()
    {
        if (checkpoints == null || checkpoints.Length < 2)
        {
            return;
        }

        switch (patrolDirection)
        {
            case PatrolDirection.Forward:
                if (checkpointInd >= checkpoints.Length - 1)
                {
                    checkpointInd--;
                    patrolDirection = PatrolDirection.Backward;
                }
                else
                {
                    checkpointInd++;
                }
                break;
            case PatrolDirection.Backward:
                if (checkpointInd <= 0)
                {
                    checkpointInd++;
                    patrolDirection = PatrolDirection.Forward;
                }
                else
                {
                    checkpointInd--;
                }
                break;
            case PatrolDirection.loop:
                checkpointInd = (checkpointInd + 1) % checkpoints.Length;
                break;
        }
        

        currTarget = checkpoints[checkpointInd].transform.position;
        PlanPathTo(currTarget);
    }

    private void PlanPathTo(Vector2 destination)
    {
        if (farmGrid == null)
        {
            currentPath = new Vector2[0];
            Debug.LogError("FarmerController needs a FarmGrid reference.", this);
            return;
        }

        currentPath = farmGrid.aStar(rb.position, destination);
        if (currentPath.Length == 0)
        {
            Debug.Log("Pathfinding failed to find a path.");
        }
        pathIndex = 0;
    }

    private void MoveAlongPath()
    {
        while (pathIndex < currentPath.Length &&
               Vector2.Distance(rb.position, currentPath[pathIndex]) <= waypointReachDistance)
        {
            pathIndex++;
        }

        if (pathIndex >= currentPath.Length)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (currentPath[pathIndex] - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private int[] CreateTriangles()
    {
        int[] triangles = new int[(rayCount - 1) * 3];
        for (int i = 0; i < rayCount - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }
        return triangles;
    }

    private IEnumerator catchPlayer()
    {
        pausing = true;
        yield return new WaitForSeconds(0.5f);
        exclamationMark.enabled = true;
        yield return new WaitForSeconds(1f);
        exclamationMark.enabled = false;
        pausing = false;
    }

    private IEnumerator restartStage()
    {
        playerSheep.getSheepControls().Disable();
        caughtBorder.enabled = true;
        caughtText.enabled = true;
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
