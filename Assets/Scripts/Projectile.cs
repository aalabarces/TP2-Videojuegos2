using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float strength = 1f;
    private Vector2 direction = Vector2.up;
    private Color color = Color.white;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MoveAccordingToSpeedAndDirection();
        // Debug.Log($"Projectile fired in direction {direction} with speed {speed} and strength {strength}");
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
    }

    private void MoveAccordingToSpeedAndDirection()
    {
        Vector2 movement = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        if (Utils.Instance.IsOutOfBounds(transform.position, 1.0f))
        {
            SpawnManager.Instance.ReturnProjectileToPool(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().GetHit(strength, color);
        }
        SpawnManager.Instance.ReturnProjectileToPool(gameObject);
    }

}
