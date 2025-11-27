using UnityEngine;

public class InactiveAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        manager.PauseAllSounds();
    }

    public void ExitState(AudioManager manager)
    {
    }
}