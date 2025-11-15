using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : IEnemyState
{
    public void EnterState(Enemy enemy)
    {

    }
    public void FixedUpdateState(Enemy enemy)
    {
        enemy.FleeFromPlayer();
    }
    public void UpdateState(Enemy enemy)
    {
        
    }
    public void ExitState(Enemy enemy)
    {
        
    }

}
