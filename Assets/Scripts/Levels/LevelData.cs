using System.Collections.Generic;

[System.Serializable]
public class TitleData
{
    public int act;
    public string title;
    public string subtitle;
    public string description;
}

[System.Serializable]
public class PowerUpData
{
    public int SpeedBoost;
    public int DamageBoost;
}

[System.Serializable]
public class AvailableLootData
{
    public int HealthPack;
    public PowerUpData PowerUp;
}

[System.Serializable]
public class EnemyData
{
    public List<EnemyType> availableEnemies;
    public string firstEnemyType;
    public int maxEnemiesAtOnce;
}

[System.Serializable]
public class EnemyType
{
    public string name;
    public float spawnProbability;
}

[System.Serializable]
public class LevelData
{
    public float act;
    public float timer;
    public EnemyData enemyData;
    public TitleData titles;
    public AvailableLootData availableLoot;
}