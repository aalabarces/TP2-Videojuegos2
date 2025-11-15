using UnityEngine;

public class MenuAudioState : IAudioState
{
    public void EnterState(AudioManager manager)
    {
        manager.PlaySound("Music_Menu");
    }

    public void ExitState(AudioManager manager)
    {
        manager.StopSound("Music_Menu");
    }
}