using UnityEngine;

[System.Serializable]
public class SpawnableObjectsData
{
    public GameObject prefab;
    public int maxPoolSize;
    public string objectType; // e.g., "Enemy", "Projectile", "Power up"
}