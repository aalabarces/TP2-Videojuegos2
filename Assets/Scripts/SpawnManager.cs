using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    private GlobalConfig globalConfig;
    // private LevelsConfig levelsConfig;
    public List<GameObject> currentSpawned { get; private set; } = new List<GameObject>();
    private List<GameObject> enemyPool = new List<GameObject>();
    private List<GameObject> projectilePool = new List<GameObject>();
    private List<GameObject> activeProjectiles = new List<GameObject>();
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject potionPrefab;
    private List<GameObject> potionPool = new List<GameObject>();
    private List<GameObject> activePotions = new List<GameObject>();
    public bool loopHasStarted { get; private set; } = false;
    private float spawnDelay;
    private float minDistanceBetweenEnemies;
    private float spawnOffsetFromScreen;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // levelsConfig = GameManager.Instance.levelsConfig;
        globalConfig = GameManager.Instance.globalConfig;
        spawnDelay = globalConfig.spawnDelay;
        minDistanceBetweenEnemies = globalConfig.minDistanceBetweenEnemies;
        spawnOffsetFromScreen = globalConfig.spawnOffsetFromScreen;
    }
    public void StartEnemySpawnLoop()
    {
        loopHasStarted = true;
        Utils.Instance.SetLimits();
        PopulateSpawnPool();
        PopulateProjectilePool();
        PopulatePotionPool();
        StartCoroutine(SpawnLoop());
    }

    public IEnumerator SpawnLoop()
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
        Vector3 spawnPosition;
        if (enemyType == "EnemyBoss")
        {
            spawnPosition = new Vector3(0, Utils.Instance.topLimit + 2.0f, 0);
        }
        else
        {
            spawnPosition = GetValidSpawnPosition();
        }

        GameObject enemy = null;
        if (enemyType != null)
        {
            enemy = enemyPool.Find(e => !e.activeInHierarchy && e.name.StartsWith(enemyType));
        }
        else
        {
            enemy = WhichEnemyToSpawnAccordingToLevel();
        }
        Debug.Log("Spawning enemy of type: " + (enemy != null ? enemy.name : "null"));
        if (!enemy)
        {
            Debug.LogWarning("No available enemies of type " + enemyType + " in the pool");
            return;
        }
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = (spawnPosition.x < 0) ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90);
        Utils.Instance.ChangeLayerTo(enemy, "Interactable");
        enemy.SetActive(true);
        enemy.GetComponent<Enemy>().ResetState();
        currentSpawned.Add(enemy);
        Debug.Log("Enemy spawned at position: " + spawnPosition);
    }

    private GameObject WhichEnemyToSpawnAccordingToLevel()
    {
        List<EnemyType> availableEnemies = GameManager.Instance.currentLevelData.enemyData.availableEnemies;
        float randomPoint = Random.value;

        foreach (var e in availableEnemies)
        {
            if (randomPoint < e.spawnProbability)
            {
                return GetEnemyFromPool(e.name);
            }
            else
            {
                randomPoint -= e.spawnProbability;
            }
        }

        // fallback
        return GetEnemyFromPool(availableEnemies[0].name);
    }
    private GameObject GetEnemyFromPool(string name)
    {
        GameObject enemy = enemyPool.Find(e => !e.activeInHierarchy && e.GetComponent<Enemy>().enemyType == name);
        if (enemy != null) enemyPool.Remove(enemy);
        return enemy;
    }

    private Vector3 GetValidSpawnPosition()
    {
        float topBound = Utils.Instance.topLimit - 2.0f;
        float bottomBound = Utils.Instance.bottomLimit + 2.0f;
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
                float distance = Mathf.Abs(position.y - enemy.transform.position.y);
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
        for (int i = 0; i < globalConfig.maxProjectiles; i++)
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
            activeProjectiles.Add(projectile);
            projectile.GetComponent<Projectile>().ResetState();
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
            activeProjectiles.Remove(projectile);
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
    }
    public void RetrieveAllProjectiles()
    {
        Debug.Log("Retrieving all projectiles...");
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            var projectile = activeProjectiles[i];
            ReturnProjectileToPool(projectile);
        }
    }

    public void ChangeEnemyStatesToSeek()
    {
        Debug.Log("Changing all enemies to Seek State...");

        List<GameObject> enemies = currentSpawned
            .Concat(enemyPool)
            .ToList();

        foreach (GameObject enemyGO in enemies)
        {
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            Debug.Log("Changing enemy " + enemy.name + " to Seek State.");
            Debug.Log("Current State: " + enemy.currentState.GetType().Name);
            Debug.Log("Enemy Seek State instance: " + enemy.states["Seek"].GetType().Name);
            enemy.ChangeState(enemy.states["Seek"]);
        }
    }

    private void PopulatePotionPool()
    {
        for (int i = 0; i < globalConfig.maxPotions; i++)
        {
            GameObject potion = Instantiate(potionPrefab);
            potion.SetActive(false);
            potionPool.Add(potion);
        }
    }

    public void SpawnPotionAtPosition(Vector3 position)
    {
        GameObject potion = potionPool.Find(p => !p.activeInHierarchy);
        if (potion != null)
        {
            potion.transform.position = position;
            Utils.Instance.ChangeLayerTo(potion, "Interactable");
            potion.SetActive(true);
            potionPool.Remove(potion);
            activePotions.Add(potion);
        }
        else
        {
            Debug.LogWarning("No available potions in the pool!");
        }
    }

    public void RetrievePotion(GameObject potion)
    {
        potion.SetActive(false);
        potionPool.Add(potion);
        activePotions.Remove(potion);
    }

    public void RetrieveAllPotions()
    {
        Debug.Log("Retrieving all potions...");
        Debug.Log("Active potions count: " + activePotions.Count);
        for (int i = activePotions.Count - 1; i >= 0; i--)
        {
            var potion = activePotions[i];
            Debug.Log("Retrieving potion: " + potion.name);
            RetrievePotion(potion);
        }
    }

    public void ResetState()
    {
        loopHasStarted = false;
        StopAllCoroutines();
        CleanLists();
    }

    public void CleanLists()
    {
        enemyPool.Clear();
        projectilePool.Clear();
        potionPool.Clear();
        currentSpawned.Clear();
        activeProjectiles.Clear();
        activePotions.Clear();
    }
}
