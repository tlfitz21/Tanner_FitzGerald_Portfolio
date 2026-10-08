using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class PlayerControllerTruck : MonoBehaviour, Controls.ITruckActions
{
    [SerializeField] GameManagerTruck gameManager;
    [SerializeField] private float accelerationRate;
    [SerializeField] private float decelerationRate;
    [SerializeField] private Image deathMessage;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private Image victoryMessage;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private int carDamage;
    [SerializeField] private cutSceneManagerTruck cutSceneManager;

    
    private Controls controls;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDirection;

    private bool accelerating;

    private bool damaged;
    private bool victorious;

    [SerializeField] private int StrafeSpeed;

    void Awake()
    {
        controls = new Controls();
        controls.Truck.SetCallbacks(this);
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        accelerating = false;
        damaged = false;
        victorious = false;
        if (cutSceneManager == null)
            cutSceneManager = FindAnyObjectByType<cutSceneManagerTruck>();

    }

    void Update()   
    {
        if (!victorious)
        {
            float currDistance = gameManager.Distance += Time.deltaTime * gameManager.TruckSpeed;
            gameManager.distanceBar.fillAmount = currDistance / gameManager.desitination;

            if (gameManager.Distance >= gameManager.desitination)
            {
                StartCoroutine(beatStage());
            }

            float rate = accelerating ? accelerationRate : -decelerationRate;

            gameManager.TruckSpeed = Mathf.Clamp(
                gameManager.TruckSpeed + rate * Time.deltaTime,
                gameManager.minSpeed,
                gameManager.maxSpeed
            );
        }
        
    
    }

    void FixedUpdate()
    {
        if (victorious)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveDirection * StrafeSpeed;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!damaged && other.CompareTag("Car"))
        {
            StartCoroutine(TakeDamage());
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
            moveDirection = context.ReadValue<Vector2>();
            break;

            case InputActionPhase.Performed:
            moveDirection = context.ReadValue<Vector2>();
            break;

            case InputActionPhase.Canceled:
            moveDirection = Vector2.zero;
            break;
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
            accelerating = true;
            break;

            case InputActionPhase.Canceled:
            accelerating = false;
            break;
        }
    }

    public void HideVictoryUI()
    {
        if (victoryMessage != null)
            victoryMessage.enabled = false;
        if (victoryText != null)
            victoryText.enabled = false;
    }

    private IEnumerator TakeDamage()
    {
        // Implement damage logic here, e.g., flashing the truck or playing a sound
        if (!gameManager.takeDamage(carDamage)) 
        {
            StartCoroutine(die());
            yield break;
        }
        damaged = true;
        Color originalColor = sr.color;
        Color damageColor = new Color(1f, 0.4f, 0.4f);

        for (int i = 0; i < 5; i++)
        {
            sr.color = damageColor;
            yield return new WaitForSeconds(0.2f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        damaged = false;
    }

    private IEnumerator die()
    {
        // Implement death logic here, e.g., playing a death animation or sound
        deathMessage.enabled = true;
        deathText.enabled = true;
        controls.Disable();
        yield return new WaitForSeconds(3f);
        // Disable the truck or perform any other necessary actions
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private IEnumerator beatStage()
    {
        victorious = true;
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        controls.Disable();
        victoryMessage.enabled = true;
        victoryText.enabled = true;
        yield return new WaitForSeconds(3f);
        if (cutSceneManager == null)
        {
            Debug.LogError("Truck stage completed, but no cutSceneManagerTruck exists to run the finale.", this);
            yield break;
        }

        cutSceneManager.StartCoroutine(cutSceneManager.cutscene());
        // Disable the truck or perform any other necessary actions
    }
}
