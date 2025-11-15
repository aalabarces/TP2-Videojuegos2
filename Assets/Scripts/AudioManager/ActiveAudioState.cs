using UnityEngine;

public class ActiveAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        manager.PlaySound("Music_Game");
    }

    public void ExitState(AudioManager manager)
    {
        manager.StopSound("Music_Game");
    }
}