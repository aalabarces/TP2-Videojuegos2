using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioMixer audioMixer;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float fxVolume = 1.0f;
    [SerializeField] public AudioConfig audioConfig;
    private Dictionary<string, AudioSource> soundDictionary = new Dictionary<string, AudioSource>();
    public Dictionary<string, IAudioState> audioStates = new Dictionary<string, IAudioState>();
    public IAudioState currentState;
    public bool ready { get; private set; } = false;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
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

        audioConfig.audioDataList.ForEach(audioData =>
        {
            GameObject audioObject = new GameObject("Audio_" + audioData.audioKey);
            audioObject.transform.parent = this.transform;
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = audioData.audioClips;
            AudioMixerGroup group = audioData.audioKey.Contains("Music") ? musicGroup : sfxGroup;
            audioSource.outputAudioMixerGroup = group;
            audioSource.playOnAwake = false;
            audioSource.loop = audioData.audioKey.Contains("Music") && !audioData.audioKey.Contains("Victory") ? true : false;
            if (audioData.audioKey.Contains("Countdown"))
            {
                audioSource.volume = 0.5f;
            }
            soundDictionary.Add(audioData.audioKey, audioSource);
        });
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

    public void PauseAllSounds()
    {
        foreach (var sound in soundDictionary.Values)
        {
            sound.Pause();
        }
    }

    public bool IsSoundPlaying(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            return soundDictionary[soundName].isPlaying;
        }
        return false;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;

        // Convertimos volumen lineal (0–1) a dB (-80 a 0)
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);

        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        // Convertimos volumen lineal (0–1) a dB (-80 a 0)
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    public void SetFXVolume(float volume)
    {
        fxVolume = volume;

        // Convertimos volumen lineal (0–1) a dB (-80 a 0)
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("FXVolume", dB);
        PlayerPrefs.SetFloat("FXVolume", volume);
    }
}
