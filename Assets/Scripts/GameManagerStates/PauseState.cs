using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : IGameState
{
    public void EnterState(GameManager manager)
    {
        Debug.Log("Entering Pause State");
        manager.StopGame();
        manager.uiManager.ShowPauseMenu();
        AudioManager.Instance.ChangeState(AudioManager.Instance.audioStates["Pause"]);
    }

    public void UpdateState(GameManager manager)
    {
        // Debug.Log("Updating Pause State");
    }

    public void ExitState(GameManager manager)
    {
        Debug.Log("Exiting Pause State");
        manager.uiManager.HidePauseMenu();
    }
    public void HandleEscapeKey(GameManager manager)
    {
        manager.ChangeState(manager.gameStates["Active"]);
    }
}