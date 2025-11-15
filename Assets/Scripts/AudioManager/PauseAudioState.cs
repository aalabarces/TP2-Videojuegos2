using UnityEngine;

public class PauseAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        manager.PlaySound("UI_Pause");
    }

    public void ExitState(AudioManager manager)
    {
        manager.PlaySound("UI_Pause");
    }
}