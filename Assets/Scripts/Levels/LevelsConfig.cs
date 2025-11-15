using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Game Config/Levels Config", order = 2)]
public class LevelsConfig : ScriptableObject
{
    public List<LevelData> levels;

    public LevelData GetLevel(float act)
    {
        return levels.Find(l => l.act == act);
    }
}