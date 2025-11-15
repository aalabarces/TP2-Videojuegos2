using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveState : IGameState
{
    public void EnterState(GameManager manager)
    {
        Debug.Log("Entering Inactive State");
        if (AudioManager.Instance.ready) AudioManager.Instance.ChangeState(AudioManager.Instance.audioStates["Inactive"]);
    }

    public void UpdateState(GameManager manager)
    {
        // Debug.Log("Updating Inactive State");
    }

    public void ExitState(GameManager manager)
    {
        Debug.Log("Exiting Inactive State");
    }
    public void HandleEscapeKey(GameManager manager)
    {
        // Do nothing or show a message that the game is inactive
        Debug.Log("Game is inactive. Escape key has no effect.");
    }
}