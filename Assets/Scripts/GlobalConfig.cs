using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfig", menuName = "Game Config/Global Config", order = 1)]
public class GlobalConfig : ScriptableObject
{
    public List<SpawnableObjectsData> spawnableObjects;
    public float spawnDelay = 5.0f;
    public float minDistanceBetweenEnemies = 2.0f;
    public float spawnOffsetFromScreen = 1.0f;
}