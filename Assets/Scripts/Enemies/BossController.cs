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
    private float directionChangeCooldown = 1.5f;
    private float colorChangeTimer;
    private float colorChangeCooldown = 10f;
    
    protected void Start()
    {
        ChangeState(states["Simple"]);
        shootTimer = shootCooldown;
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
        // if playerIsClose, flee; else, move and shoot periodically
        if (PlayerIsClose())
        {
            ChangeState(states["Flee"]);
            return;
        }
        else if (currentState != states["Simple"])
        {
            ChangeState(states["Simple"]);
        }

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
            direction = RandomDirection();
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

    public Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
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
    }

    public void CheatKill()
    {
        Die();
    }
}