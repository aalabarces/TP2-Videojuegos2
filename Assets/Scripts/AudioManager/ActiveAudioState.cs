using UnityEngine;

public class ActiveAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        if (GameManager.Instance.currentLevel == 1f) return;
        manager.PlaySound("Music_Game");
    }

    public void ExitState(AudioManager manager)
    {
    }
}