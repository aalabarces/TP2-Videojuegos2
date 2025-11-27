using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource engineSound;
    [SerializeField]
    public GameObject cannon;
    private SpriteRenderer cannonSpriteRenderer;
    [SerializeField]
    public int currentLives { get; private set; } = 3;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private float rotationSpeed = 200.0f;
    [SerializeField]
    private float fireRate = 2f;
    [SerializeField]
    public float amplitude = 0.005f;
    [SerializeField]
    public float frequency = 20f;
    public bool isFiring { get; private set; } = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        engineSound = GetComponent<AudioSource>();
        cannonSpriteRenderer = cannon.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        UIManager.Instance.UpdateLives(currentLives);
    }
    void FixedUpdate()
    {
        if (!GameManager.Instance.isGameActive) return;
        rb.angularVelocity = 0f;
        Movement();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 move = transform.up * -verticalInput * speed;
        Vector2 newPosition = rb.position + move * Time.fixedDeltaTime;

        newPosition = Utils.Instance.GetClampedPosition(newPosition);

        // Engine vibration effect
        float vibration = Mathf.Sin(Time.time * frequency) * amplitude;
        Vector2 vibrationVector = new Vector2(vibration, vibration);
        newPosition += vibrationVector;

        rb.MovePosition(newPosition);

        // Only rotate if there's horizontal input
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float rotation = -verticalInput > 0 ? horizontalInput * rotationSpeed * Time.fixedDeltaTime
                                   : -horizontalInput * rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation + rotation);
        }

        updateAnimatorAndSoundBasedOnMovement(verticalInput);
    }

    private void updateAnimatorAndSoundBasedOnMovement(float verticalInput)
    {
        if (Mathf.Abs(verticalInput) > 0.01f)
        {   // Moving
            animator.SetBool("isMoving", true);
            if (!engineSound.isPlaying)
            {
                engineSound.Play();
            }
        }
        else
        {   // Not Moving
            animator.SetBool("isMoving", false);
            if (engineSound.isPlaying)
            {
                engineSound.Stop();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with an enemy!");
            LoseLife();
            Debug.Log("Lives remaining: " + currentLives);
            collision.gameObject.GetComponent<Enemy>().GetHit(1, collision.gameObject.GetComponent<Enemy>().color);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            return;
        if (other.gameObject.CompareTag("TriggerButton"))
        {
            Debug.Log($"Collided with a Trigger Button! Changing color from {spriteRenderer.color} to {other.gameObject.GetComponent<SpriteRenderer>().color}");
            Color otherColor = other.gameObject.GetComponent<SpriteRenderer>().color;
            Color otherColorWithoutAlpha = new Color(otherColor.r, otherColor.g, otherColor.b, 1f);
            spriteRenderer.color = otherColorWithoutAlpha;
            cannonSpriteRenderer.color = otherColorWithoutAlpha;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            return;
        if (other.gameObject.CompareTag("TriggerButton"))
        {
            Debug.Log($"Exited a Trigger Button! Changing color back to white from {spriteRenderer.color}");
            spriteRenderer.color = Color.white;
            cannonSpriteRenderer.color = Color.white;
        }
    }

    public void LoseLife()
    {
        currentLives--;
        UIManager.Instance.UpdateLives(currentLives);
        Debug.Log("Lives remaining: " + currentLives);
        if (currentLives <= 0)
        {
            Debug.Log("Game Over!");
            animator.SetTrigger("Die");
            engineSound.Stop();
            StartCoroutine(GameManager.Instance.GameOver());
        }
    }

    public void SetFiringState(bool state)
    {
        isFiring = state;
    }

    public void FireCannon(Vector3 direction)
    {
        // Debug.Log("Firing cannon...");
        if (isFiring)
        {
            Debug.Log("Already firing, cannot fire again yet.");
            return;
        }

        GameObject projectile = SpawnManager.Instance.GetProjectileFromPool();
        if (projectile != null)
        {
            Vector3 spawnPosition = cannon.transform.position + direction.normalized * 0.5f;
            projectile.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            projectile.transform.rotation = cannon.transform.rotation;
            projectile.GetComponent<Projectile>().SetDirection(direction);
            projectile.GetComponent<Projectile>().SetColor(spriteRenderer.color);
            Debug.Log($"Projectile fired towards {direction}");
            SetFiringState(true);
            cannon.GetComponent<Animator>().SetBool("isFiring", true);
            cannon.GetComponent<AimingScript>().PlayCannonSound();

            Invoke(nameof(ResetFiringState), fireRate);
        }
        else
        {
            Debug.Log("No projectiles available in pool!");
        }
    }

    public void ResetFiringState()
    {
        SetFiringState(false);
        cannon.GetComponent<Animator>().SetBool("isFiring", false);
    }

    public void ResetPlayer()
    {
        currentLives = 3;
        UIManager.Instance.UpdateLives(currentLives);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        spriteRenderer.color = Color.white;
        cannonSpriteRenderer.color = Color.white;
    }
}
