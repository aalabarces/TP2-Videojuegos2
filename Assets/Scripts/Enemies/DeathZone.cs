using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public float damageInterval = 1f;   // 1 vez por segundo
    private float damageTimer = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Daño inicial inmediato al entrar
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.LoseLife();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                Player player = collision.GetComponent<Player>();
                if (player != null)
                {
                    player.LoseLife();
                }

                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            damageTimer = 0f; // reset cuando sale
        }
    }
}
