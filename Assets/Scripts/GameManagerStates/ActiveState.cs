using UnityEngine;

public class ActiveState : IGameState
{
    public void EnterState(GameManager manager)
    {
        Debug.Log("Entering Active State");
        manager.StartGame();
        AudioManager.Instance.ChangeState(AudioManager.Instance.audioStates["Active"]);
    }

    public void UpdateState(GameManager manager)
    {
        if (!manager.isGameActive) return;
        manager.handleTimer();
        if (Input.GetMouseButtonDown(0) && manager.isGameActive)
        {
            handleClick(manager);
        }
        if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftShift))
        {
            manager.SetTimerTo(1f);
        }
    }

    public void ExitState(GameManager manager)
    {
        Debug.Log("Exiting Active State");
        manager.StopGame();
        if (manager.backgroundMusicSource != null) 
        {
            manager.backgroundMusicSource.Pause();
        }
    }

    private void handleClick(GameManager manager)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = manager.player.cannon.transform.position.z;
        Vector3 direction = (mouseWorld - manager.player.cannon.transform.position).normalized;
        manager.player.FireCannon(direction);
    }

    public void HandleEscapeKey(GameManager manager)
    {
        manager.ChangeState(manager.gameStates["Pause"]);
    }
}
