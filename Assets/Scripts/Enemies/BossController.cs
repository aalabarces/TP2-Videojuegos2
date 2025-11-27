using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private GameObject cannon;
    public bool isFiring { get; private set; } = false;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] public float fleeDistance = 5f;
    [SerializeField] public float shootCooldown = 2f;
    private float shootTimer;
    private float directionChangeTimer;
    [SerializeField] private float directionChangeCooldown = 1.5f;
    private float colorChangeTimer;
    private float colorChangeCooldown = 10f;
    [SerializeField] private float boundaryAvoidanceMargin = 2.0f;
    [SerializeField] private float fleeBias = 1.5f;
    [SerializeField] private float wanderBias = 1.0f;
    
    protected void Start()
    {
        ChangeState(states["Simple"]);
        shootTimer = shootCooldown;
        directionChangeTimer = 0f;
        colorChangeTimer = colorChangeCooldown;
    }

    protected override void FixedUpdate()
    {
        if (!GameManager.Instance.isGameActive) return;
        base.FixedUpdate();
        if (Utils.Instance.IsOutOfBounds(transform.position))
        {
            rb.MovePosition(Utils.Instance.GetClampedPosition(transform.position));
        }
    }

    protected override void Update(){
        if (!GameManager.Instance.isGameActive) return;
        base.Update();
        HandleBossBehaviour();
    }

    private void HandleBossBehaviour()
    {
        // Lógica de cambio de estado
        if (PlayerIsClose())
        {
            if (currentState != states["Flee"])
            {
                ChangeState(states["Flee"]);
            }
        }
        else if (currentState != states["Simple"])
        {
            ChangeState(states["Simple"]);
        }

        // Lógica de disparo
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            FireAtPlayer();
            shootTimer = shootCooldown;
        }

        // Cambiar dirección de movimiento
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            if (currentState == states["Flee"])
            {
                direction = GetFleeDirection();
            }
            else // currentState == states["Simple"]
            {
                direction = GetWanderDirection();
            }
            
            directionChangeTimer = directionChangeCooldown;
        }
        
        // Cambiar color
        colorChangeTimer -= Time.deltaTime;
        if (colorChangeTimer <= 0f)
        {
            ChangeColor();
            colorChangeTimer = colorChangeCooldown;
        }
    }

    public Vector3 GetWanderDirection()
    {
        Vector2 currentPosition = transform.position;
        float left = Utils.Instance.leftLimit;
        float right = Utils.Instance.rightLimit;
        float bottom = Utils.Instance.bottomLimit;
        float top = Utils.Instance.topLimit;

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        
        Vector3 edgeAvoidance = Vector3.zero;

        if (currentPosition.x < left + boundaryAvoidanceMargin)
        {
            edgeAvoidance += Vector3.right;
        }
        else if (currentPosition.x > right - boundaryAvoidanceMargin)
        {
            edgeAvoidance += Vector3.left;
        }

        if (currentPosition.y < bottom + boundaryAvoidanceMargin)
        {
            edgeAvoidance += Vector3.up;
        }
        else if (currentPosition.y > top - boundaryAvoidanceMargin)
        {
            edgeAvoidance += Vector3.down;
        }

        Vector3 finalDirection = (randomDir * wanderBias + edgeAvoidance).normalized;

        return finalDirection;
    }

    public Vector3 GetFleeDirection()
    {
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 bossPos = transform.position;

        // 1. Dirección base: vector alejándose del jugador.
        Vector3 fleeFromPlayer = (bossPos - playerPos).normalized;

        // 2. Comprobación y ajuste de bordes para evitar ser acorralado.
        Vector3 edgeAvoidance = Vector3.zero;

        float left = Utils.Instance.leftLimit;
        float right = Utils.Instance.rightLimit;
        float bottom = Utils.Instance.bottomLimit;
        float top = Utils.Instance.topLimit;

        // Lógica de detección de proximidad al borde
        bool nearLeft = bossPos.x < left + boundaryAvoidanceMargin;
        bool nearRight = bossPos.x > right - boundaryAvoidanceMargin;
        bool nearBottom = bossPos.y < bottom + boundaryAvoidanceMargin;
        bool nearTop = bossPos.y > top - boundaryAvoidanceMargin;

        // Si cerca de un borde Y la huida del jugador nos lleva *hacia* ese borde (fleeFromPlayer.x/y tiene el mismo signo), 
        // necesitamos añadir una corrección fuerte.
        if (nearLeft && fleeFromPlayer.x < 0) edgeAvoidance += Vector3.right;
        if (nearRight && fleeFromPlayer.x > 0) edgeAvoidance += Vector3.left;
        if (nearBottom && fleeFromPlayer.y < 0) edgeAvoidance += Vector3.up;
        if (nearTop && fleeFromPlayer.y > 0) edgeAvoidance += Vector3.down;

        // 3. Combinar huida del jugador (prioridad) con evasión de bordes.
        Vector3 finalDirection;

        if (edgeAvoidance != Vector3.zero)
        {
            // Si hay peligro de esquina, priorizamos el alejamiento del borde (multiplicando por fleeBias).
            finalDirection = (fleeFromPlayer * 1.0f + edgeAvoidance * fleeBias).normalized;
        }
        else
        {
            // Si no hay peligro de esquina, solo huir del jugador.
            finalDirection = fleeFromPlayer;
        }

        return finalDirection;
    }
    public bool PlayerIsClose()
    {
        float d = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
        return d < fleeDistance;
    }

    public void FireAtPlayer()
    {
        Vector3 dir = (GameManager.Instance.player.transform.position - transform.position).normalized;
        FireCannon(dir);
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
            projectile.GetComponent<Projectile>().SetColor(GameManager.Instance.player.GetComponent<SpriteRenderer>().color); // Boss projectiles match player color
            // Debug.Log($"Projectile fired towards {direction}");
            SetFiringState(true);
            cannon.GetComponent<Animator>().SetBool("isFiring", true);
            cannon.GetComponent<AimingScript>().PlayCannonSound();

            Invoke(nameof(ResetFiringState), fireRate);
            // se suponía que esto (resetear el estado de disparo) iba en el fin de la animación
            // pero se bugueaba si disparabas mucho rápido
        }
        else
        {
            Debug.Log("No projectiles available in pool!");
        }
    }

    public override void ResetState()
    {
        base.ResetState();
        shootTimer = shootCooldown;
        directionChangeTimer = 0f;
        colorChangeTimer = colorChangeCooldown;
        ResetFiringState();
    }

    public void SetFiringState(bool state)
    {
        isFiring = state;
    }

    private void ResetFiringState()
    {
        SetFiringState(false);
        cannon.GetComponent<Animator>().SetBool("isFiring", false);
    }

    public override void HandleOutOfBounds()
    {
        // no sé si pasa algo, pero tenía que overridear esto
        Debug.Log("Boss out of bounds, but ignoring...");
    }
    public override void Die()
    {
        StartCoroutine(GameManager.Instance.BossDefeated(this));
    }

    public void ChangeColor()
    {
        Color newColor = Utils.Instance.GetRandomColorDifferentFrom(GetComponent<SpriteRenderer>().color);
        GetComponent<SpriteRenderer>().color = newColor;
        color = newColor;
        cannon.GetComponent<SpriteRenderer>().color = newColor;
    }

    public void CheatKill()
    {
        Die();
    }
}