using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBW : Enemy
{
    [SerializeField] private GameObject DeathZonePrefab;
    private GameObject deathZoneInstance;
    private bool deathZoneSpawned = false;

    void Start()
    {
        currentState = states["Seek"];
        currentState.EnterState(this);
    } 

    protected override void Update()
    {
        base.Update();
        if (DistanceToPlayer() < 1.5f && !deathZoneSpawned)
        {
            direction = Vector2.zero;
            StartCoroutine(SpawnDeathZoneCoroutine());
        }
    }

    private float DistanceToPlayer()
    {
        if (GameManager.Instance.player != null)
        {
            return Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        }
        return Mathf.Infinity;
    }

    private IEnumerator SpawnDeathZoneCoroutine()
    {
        deathZoneSpawned = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(255f/255f, 0f/255f, 0f/255f, 1f);
        yield return new WaitForSeconds(0.2f);
        sr.color = new Color(205f/255f, 0f/255f, 0f/255f, 1f);
        yield return new WaitForSeconds(0.3f);
        sr.color = new Color(255f/255f, 0f/255f, 0f/255f, 1f);
        yield return new WaitForSeconds(0.1f);
        sr.color = new Color(155f/255f, 0f/255f, 0f/255f, 1f);
        yield return new WaitForSeconds(0.2f);
        sr.color = new Color(255f/255f, 0f/255f, 0f/255f, 1f);
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
        Die();
        SpawnDeathZone(transform.position);
    }

    public void SpawnDeathZone(Vector3 position)
    {
        if (DeathZonePrefab != null)
        {
            deathZoneInstance = Instantiate(DeathZonePrefab, position, Quaternion.identity);
            Invoke("DestroyDeathZone", 5f); // Destroy after 5 seconds
        }
        else
        {
            Debug.LogWarning("DeathZonePrefab is not assigned in EnemyBW.");
        }
    }

    private void DestroyDeathZone()
    {
        if (deathZoneInstance != null)
        {
            Destroy(deathZoneInstance);
        }
    }

    public override void Die()
    {
        animator.SetBool("isDead", true);
        AudioManager.Instance.PlaySound("FX_Explosion");
        Utils.Instance.ChangeLayerTo(this.gameObject, "Non-interactable");
        // doesn't give points
        // doesn't drop potions
        TellSpawnManagerToKillMe();
    }

    public override void ResetState()
    {
        base.ResetState();
        deathZoneSpawned = false;
    }
}
