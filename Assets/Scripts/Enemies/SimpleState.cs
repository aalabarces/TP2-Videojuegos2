using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleState : IEnemyState
{
    public void EnterState(Enemy enemy)
    {

    }
    public void FixedUpdateState(Enemy enemy)
    {
        // simply go forward
        enemy.currentVelocity += enemy.direction * enemy.maxSpeed * Time.fixedDeltaTime;
    }
    public void UpdateState(Enemy enemy)
    {
        
    }
    public void ExitState(Enemy enemy)
    {
        
    }
}
