using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float strength = 1f;
    private Vector2 direction = Vector2.up;
    [SerializeField] private Color color = Color.white;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isGameActive) return;
        MoveAccordingToSpeedAndDirection();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetColor(Color col)
    {
        color = col;
        spriteRenderer.color = col;
    }

    public void ResetState()
    {
        spriteRenderer.color = Color.white;
        ResetAnimator();
    }

    public void ResetAnimator()
    {
        if (animator == null) return;
        animator.enabled = true;
        animator.Rebind();
        animator.SetBool("die", false);
        animator.Play("Move", 0, 0f);
        animator.Update(0f);
    }

    private void MoveAccordingToSpeedAndDirection()
    {
        Vector2 movement = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        if (Utils.Instance.IsOutOfBounds(transform.position, 1.0f))
        {
            TellSpawnManagerToKillMe();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Projectile collided with " + collision.gameObject.name);
        if (collision.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().GetHit(strength, color);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().LoseLife();
            // por qué no tiene un GetHit? Eso no es muy "objetoso"
            // todos los que pueden recibir daño tendrían que tener un componente común
        }
        if (!collision.gameObject.CompareTag("TriggerButton"))
        {
            audioSource.Play();
            animator.SetBool("die", true);
            direction = Vector2.zero; // stop moving
        }
    }

    public void TellSpawnManagerToKillMe()
    {
        ResetAnimator();
        SpawnManager.Instance.ReturnProjectileToPool(gameObject);
    }
}
