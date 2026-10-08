using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;



public class PlayerControllerSheep : MonoBehaviour, Controls.ISheepActions
{
    
    private Controls controls;

    private Vector2 moveDirection;

    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] movementSprites = new Sprite[4];
    [SerializeField] private float animationFrameDuration = 0.12f;
    private Sprite idleSprite;
    private float animationTimer;
    private int animationFrame;
    private bool wasMoving;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Image victoryBorder;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private cutsceneManagerSheep cutsceneManager;
    private bool victorious;

    public Controls getSheepControls()
    {
        return controls;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new Controls();
        controls.Sheep.SetCallbacks(this);
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            idleSprite = spriteRenderer.sprite;
        victorious = false;
    }

    public bool getVictorious()
    {
        return victorious;
    }

    void OnEnable()
    {
        controls.Sheep.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneManager.inCutscene) return;
        rb.linearVelocity = moveDirection * moveSpeed;
        AnimateMovement();
    }

    private void AnimateMovement()
    {
        if (spriteRenderer == null)
            return;

        bool moving = moveDirection.sqrMagnitude > 0.001f;
        if (!moving)
        {
            animationTimer = 0f;
            animationFrame = 0;
            wasMoving = false;
            spriteRenderer.sprite = idleSprite;
            return;
        }

        if (moveDirection.x != 0f)
            spriteRenderer.flipX = moveDirection.x < 0f;

        if (movementSprites == null || movementSprites.Length == 0)
            return;

        if (!wasMoving)
        {
            wasMoving = true;
            animationFrame = 0;
            animationTimer = 0f;
            spriteRenderer.sprite = movementSprites[animationFrame];
        }

        animationTimer += Time.deltaTime;
        if (animationTimer >= animationFrameDuration)
        {
            animationTimer = 0f;
            animationFrame = (animationFrame + 1) % movementSprites.Length;
            spriteRenderer.sprite = movementSprites[animationFrame];
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
                // Handle move canceled
                moveDirection = Vector2.zero;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            Debug.Log("Hit the truck");
            StartCoroutine(BeatStage());
        }
    }

    private IEnumerator BeatStage()
    {
        victorious = true;
        controls.Disable();
        victoryBorder.enabled = true;
        victoryText.enabled = true;
        yield return new WaitForSeconds(3f);
        victoryBorder.enabled = false;
        victoryText.enabled = false;
        cutsceneManager.StartCoroutine(cutsceneManager.secondCutscene());
        
    }

}