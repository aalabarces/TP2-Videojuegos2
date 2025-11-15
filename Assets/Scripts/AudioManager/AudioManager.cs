using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource musicMenu;
    [SerializeField] private AudioSource musicGame;
    [SerializeField] private AudioSource UI_Click;
    private Dictionary<string, AudioSource> soundDictionary = new Dictionary<string, AudioSource>();
    public Dictionary<string, IAudioState> audioStates = new Dictionary<string, IAudioState>();
    public IAudioState currentState;
    public bool ready { get; private set; } = false;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {

        audioStates.Add("Active", new ActiveAudioState());
        audioStates.Add("Inactive", new InactiveAudioState());
        audioStates.Add("Menu", new MenuAudioState());
        audioStates.Add("Pause", new PauseAudioState());
        currentState = audioStates["Menu"];

        soundDictionary.Add("Music_Menu", musicMenu);
        soundDictionary.Add("Music_Game", musicGame);
        soundDictionary.Add("UI_Click", UI_Click);
        ready = true;
    }

        public void ChangeState(IAudioState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public void PlaySound(string soundName)
    {
        // Implementation for playing sound
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Play();
        }
    }

    public void StopSound(string soundName)
    {
        // Implementation for stopping sound
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Stop();
        }
    }

    public void StopAllSounds()
    {
        foreach (var sound in soundDictionary.Values)
        {
            sound.Stop();
        }
    }
}
