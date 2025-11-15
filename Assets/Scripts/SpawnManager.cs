using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    private GlobalConfig globalConfig;
    // private LevelsConfig levelsConfig;
    public List<GameObject> currentSpawned { get; private set; } = new List<GameObject>();
    private List<GameObject> enemyPool = new List<GameObject>();
    private List<GameObject> projectilePool = new List<GameObject>();
    [SerializeField] private GameObject projectilePrefab;
    public bool loopHasStarted { get; private set; } = false;
    private float spawnDelay;
    private float minDistanceBetweenEnemies;
    private float spawnOffsetFromScreen;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Utils.Instance.SetLimits();
        // levelsConfig = GameManager.Instance.levelsConfig;
        globalConfig = GameManager.Instance.globalConfig;
        spawnDelay = globalConfig.spawnDelay;
        minDistanceBetweenEnemies = globalConfig.minDistanceBetweenEnemies;
        spawnOffsetFromScreen = globalConfig.spawnOffsetFromScreen;
        PopulateSpawnPool();
        PopulateProjectilePool();
    }

    public void StartEnemySpawnLoop()
    {
        loopHasStarted = true;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(spawnDelay); // initial delay before spawning
        while (true)
        {
            if (GameManager.Instance.isSpawningAllowed && currentSpawned.Count < GameManager.Instance.currentLevelData.enemyData.maxEnemiesAtOnce)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnFirstEnemyForCurrentLevel()
    {
        StartCoroutine(SpawnFirstEnemy());
    }

    private IEnumerator SpawnFirstEnemy()
    {
        SpawnEnemy(GameManager.Instance.currentLevelData.enemyData.firstEnemyType);
        yield return new WaitForSeconds(spawnDelay);
    }

    public void SpawnEnemy(string enemyType = null)
    {
        Vector3 spawnPosition = GetValidSpawnPosition();

        GameObject enemy = null;
        if (enemyType != null)
        {
            enemy = enemyPool.Find(e => !e.activeInHierarchy && e.name.StartsWith(enemyType));
        }
        else
        {
            enemy = WhichEnemyToSpawnAccordingToLevel();
        }

        if (!enemy)
        {
            Debug.LogWarning("No available enemies of type " + enemyType + " in the pool");
            return;
        }
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = (spawnPosition.x < 0) ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90);
        enemy.GetComponent<Enemy>().ResetState();
        Utils.Instance.ChangeLayerTo(enemy, "Interactable");
        enemy.SetActive(true);
        currentSpawned.Add(enemy);
        Debug.Log("Enemy spawned at position: " + spawnPosition);
    }

    private GameObject WhichEnemyToSpawnAccordingToLevel()
    {
        List<EnemyType> availableEnemies = GameManager.Instance.currentLevelData.enemyData.availableEnemies;
        Debug.Log("Available enemies for spawning:");
        foreach (var tempEnemy in availableEnemies)
        {
            Debug.Log("- " + tempEnemy.name + " with spawn probability " + tempEnemy.spawnProbability);
        }
        float randomPoint = Random.value;
        string enemyName = "";
        foreach (var tempEnemy in availableEnemies)
        {
            if (randomPoint < tempEnemy.spawnProbability)
                enemyName = tempEnemy.name;
            else
                randomPoint -= tempEnemy.spawnProbability;
        }
        if (enemyName == "")
        {
            Debug.LogWarning("No enemy selected based on spawn probabilities, defaulting to first available enemy.");
            enemyName = availableEnemies[0].name;
        }
        Debug.Log("Selected enemy to spawn: " + enemyName);
        GameObject enemy = enemyPool.Find(e => !e.activeInHierarchy && e.GetComponent<Enemy>().enemyType == enemyName);
        Debug.Log("Enemy found in pool: " + (enemy != null ? enemy.name : "null"));
        if (enemy != null)
        {
            enemyPool.Remove(enemy);
            return enemy;
        }
        else
        {
            Debug.LogWarning("No available base enemies in the pool!");
            return null;
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        float topBound = Utils.Instance.topLimit;
        float bottomBound = Utils.Instance.bottomLimit;
        float leftBound = Utils.Instance.leftLimit - spawnOffsetFromScreen;
        float rightBound = Utils.Instance.rightLimit + spawnOffsetFromScreen;

        Vector3 spawnPosition;
        int maxAttempts = 50;
        int attempts = 0;

        do
        {
            // Random Y between top and bottom bounds
            float spawnY = Random.Range(bottomBound, topBound);

            // Random side (left or right)
            float spawnX = Random.Range(0, 2) == 0 ? leftBound : rightBound;

            spawnPosition = new Vector3(spawnX, spawnY, 0);
            attempts++;

            // If we've tried too many times, just use the position
            if (attempts >= maxAttempts)
                break;

        } while (!IsPositionValidForSpawn(spawnPosition));

        return spawnPosition;
    }

    private bool IsPositionValidForSpawn(Vector3 position)
    {
        foreach (GameObject enemy in currentSpawned)
        {
            if (enemy.activeInHierarchy)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistanceBetweenEnemies)
                    return false;
            }
        }
        return true;
    }

    public void KillSpawn(GameObject enemy)
    {   // TODO: tendría que ser más abstracto, para cualquier tipo de spawnable object
        currentSpawned.Remove(enemy);
        enemy.SetActive(false);
    }

    private void PopulateSpawnPool()
    {
        foreach (var config in globalConfig.spawnableObjects)
        {    // key: enemy prefab, value: max pool size

            for (int i = 0; i < config.maxPoolSize; i++)
            {
                GameObject gameObject = Instantiate(config.prefab);
                gameObject.SetActive(false);
                switch (config.objectType)
                {
                    case string s when s.StartsWith("Enemy"):
                        enemyPool.Add(gameObject);
                        break;
                    default:
                        Debug.LogWarning("Unknown object type in spawnableObjects: " + config.objectType);
                        break;
                }
            }

        }

    }

    private void PopulateProjectilePool()
    {
        for (int i = 0; i < ConfigValues.Instance.maxProjectiles; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    public GameObject GetProjectileFromPool()
    {
        GameObject projectile = projectilePool.Find(p => !p.activeInHierarchy);
        if (projectile != null)
        {
            projectile.SetActive(true);
            Utils.Instance.ChangeLayerTo(projectile, "Interactable");
            projectilePool.Remove(projectile);
            return projectile;
        }
        else
        {
            Debug.LogWarning("No available projectiles in the pool!");
            // TODO: empty clicking sound
            return null;
        }
    }
    public void ReturnProjectileToPool(GameObject projectile)
    {
        projectile.SetActive(false);
        if (!projectilePool.Contains(projectile))
        {
            projectilePool.Add(projectile);
        }
    }
    public void RetrieveAllEnemies()
    {
        Debug.Log("Retrieving all enemies...");
        for (int i = currentSpawned.Count - 1; i >= 0; i--)
        {
            var enemy = currentSpawned[i];
            KillSpawn(enemy);
        }
        currentSpawned.Clear(); // Just to be sure
    }
}
