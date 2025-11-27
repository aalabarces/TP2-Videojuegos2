using UnityEngine;

public class PauseAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {   
        manager.PauseAllSounds();
        manager.PlaySound("UI_Click");
    }

    public void ExitState(AudioManager manager)
    {
        manager.PlaySound("UI_Click");
    }
}