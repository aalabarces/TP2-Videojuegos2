using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : IEnemyState
{
    public void EnterState(Enemy enemy)
    {

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
