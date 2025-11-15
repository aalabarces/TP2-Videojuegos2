using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : IGameState
{
    public void EnterState(GameManager manager)
    {
        Debug.Log("Entering Dialogue State");
        manager.uiManager.ShowDialogueMenu();
        AudioManager.Instance.ChangeState(AudioManager.Instance.audioStates["Inactive"]);
    }

    public void UpdateState(GameManager manager)
    {
        // Debug.Log("Updating Menu State");
        if (Input.anyKeyDown)
        {
            DialogueManager.Instance.SkipDialogue();
        }
    }

    public void ExitState(GameManager manager)
    {
        Debug.Log("Exiting Dialogue State");
        manager.uiManager.HideDialogueMenu();
    }
    public void HandleEscapeKey(GameManager manager)
    {
        DialogueManager.Instance.SkipDialogue();
    }

}
