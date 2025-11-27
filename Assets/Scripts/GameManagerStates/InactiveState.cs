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
        if (manager.uiManager.title.titlePanel.activeInHierarchy)
        {
            manager.StartCoroutine(manager.uiManager.title.StopTitleIfKeyPressed());
        }
    }
}