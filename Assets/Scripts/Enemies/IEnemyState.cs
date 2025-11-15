using UnityEngine;

public interface IEnemyState
{
    void EnterState(Enemy enemy);
    void FixedUpdateState(Enemy enemy);
    void UpdateState(Enemy enemy);
    void ExitState(Enemy enemy);
}