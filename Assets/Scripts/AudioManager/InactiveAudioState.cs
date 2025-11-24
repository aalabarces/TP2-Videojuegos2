using UnityEngine;

public class InactiveAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        // manager.StopAllSounds();
    }

    public void ExitState(AudioManager manager)
    {
    }
}