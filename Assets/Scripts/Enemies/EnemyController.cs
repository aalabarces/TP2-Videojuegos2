using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer sr;
    public Vector2 direction;
    public Vector2 velocity;
    protected AudioSource audioSource;
    protected float extraMargin; // for out of bounds detection
    [SerializeField] public float growthMultiplier = 1.5f; // multiplier for after first act
    [SerializeField] public float scoreValue = 10;
    [SerializeField] public float maxSpeed = 1.0f;
    public Vector2 currentVelocity = Vector2.zero;
    [SerializeField] public float baseHealth = 1f;
    [SerializeField] protected float health = 1f;
    [SerializeField] public Color color = Color.white;
    [SerializeField] public string enemyType = "EnemyBasic";
    [SerializeField] public Dictionary<string, IEnemyState> states {get; private set;} = new Dictionary<string, IEnemyState>();
    public IEnemyState currentState;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Debug.Log("Animator found: " + animator);
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        states.Add("Simple", new SimpleState());
        states.Add("Flee", new FleeState());
        states.Add("Seek", new SeekState());
        currentState = states["Simple"];
        currentState.EnterState(this);
        
        extraMargin = Mathf.Max(sr.bounds.extents.x, sr.bounds.extents.y) + 0.1f;
    }

    protected virtual void FixedUpdate()
    {
        if (!GameManager.Instance.isGameActive) return;
        currentState.FixedUpdateState(this);
        Move();
    }
    protected virtual void Update()
    {
        if (!GameManager.Instance.isGameActive) return;
        currentState.UpdateState(this);
        RotateSpriteAccordingToVelocity();
        if (Utils.Instance.IsOutOfBounds(transform.position, extraMargin, direction))
        {
            HandleOutOfBounds();
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
    protected virtual void Move()
    {
        if (!GameManager.Instance.isGameActive) return;
        // inherited by subclasses
        Vector2 newPosition = rb.position + currentVelocity * Time.fixedDeltaTime;
        // newPosition = Utils.Instance.GetClampedPosition(newPosition);
        rb.MovePosition(newPosition);
    }

    public void GetHit(float damage, Color col)
    {
        Debug.Log($"Enemy of type {enemyType} got hit with damage {damage} and color {col}. Current health: {health}. Current color: {color}");
        if (color != col) return;   // ALPHA ERROR
        health -= damage;
        if (health <= 0)
        {
            Debug.Log($"Enemy of type {enemyType} has died.");
            Die();
        }
    }

    public virtual void Die()
    {
        animator.SetBool("isDead", true);
        audioSource.Play();
        Utils.Instance.ChangeLayerTo(this.gameObject, "Non-interactable");
        GameManager.Instance.PlayerKilledEnemy(gameObject);
        // Invoke(nameof(TellSpawnManagerToKillMe), 0.5f);
    }

    private void OnEnable()
    {
        ResetState();
    }

    public virtual void ResetState()
    {
        health = baseHealth;
        rb.velocity = Vector2.zero;
        Utils.Instance.ChangeLayerTo(this.gameObject, "Interactable");
        if (animator != null) animator.SetBool("isDead", false);
        direction = (transform.position.x > 0) ? Vector3.left : Vector3.right;
    }
    
    public void TellSpawnManagerToKillMe()
    {
        SpawnManager.Instance.KillSpawn(gameObject);
    }

    public virtual void HandleOutOfBounds()
    {
        if (currentState == states["Simple"])
        {
            GameManager.Instance.player.LoseLife();
        }
        Debug.Log("Enemy out of bounds, destroying...");
        TellSpawnManagerToKillMe();
    }
    public virtual void FleeFromPlayer()
    {
        Vector2 position = transform.position;
        Vector2 playerPos = GameManager.Instance.player.transform.position;
        Vector2 desiredVelocity = (position - playerPos).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - currentVelocity;
        currentVelocity += steering * Time.fixedDeltaTime;

        currentVelocity = currentVelocity.normalized * maxSpeed;
    }
    public void GoToPlayer()
    {
        Vector2 position = transform.position;
        Vector2 playerPos = GameManager.Instance.player.transform.position;
        Vector2 desiredVelocity = (playerPos - position).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - currentVelocity;
        currentVelocity += steering * Time.fixedDeltaTime;

        currentVelocity = currentVelocity.normalized * maxSpeed;
    }

    public void RotateSpriteAccordingToVelocity()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 180f;
            sr.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    private void LevelUp()
    {
        baseHealth *= growthMultiplier;
        maxSpeed *= growthMultiplier;
        health = baseHealth;
    }
}

