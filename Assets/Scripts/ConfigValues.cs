using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigValues : MonoBehaviour
{
    public static ConfigValues Instance { get; private set; }

    [SerializeField] public int maxBaseEnemyPoolSize = 10;
    [SerializeField] public int maxFastEnemyPoolSize = 10;
    [SerializeField] public int maxColoredEnemyPoolSize = 10;
    [SerializeField] public int maxHeavyEnemyPoolSize = 10;
    [SerializeField] public int maxProjectiles = 20;

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
