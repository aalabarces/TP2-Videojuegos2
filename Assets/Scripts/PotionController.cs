using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PotionController : MonoBehaviour
{
    [SerializeField] private int healAmount = 2;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Heal(healAmount);
            AudioManager.Instance.PlaySound("FX_Heal");
            SpawnManager.Instance.RetrievePotion(gameObject);
        }
    }
}
