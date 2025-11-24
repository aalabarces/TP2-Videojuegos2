using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : IEnemyState
{
    public void EnterState(Enemy enemy)
    {
        Debug.Log("Entering Seek State");
        Debug.Log("Enemy Type: " + enemy.enemyType);

    }
    public void FixedUpdateState(Enemy enemy)
    {
        enemy.GoToPlayer();
    }
    public void UpdateState(Enemy enemy)
    {
    }
    public void ExitState(Enemy enemy)
    {
        
    }

}
