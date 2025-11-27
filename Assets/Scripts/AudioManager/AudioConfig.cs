using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Game Config/Audio Config", order = 4)]
public class AudioConfig : ScriptableObject
{
    public List<AudioData> audioDataList = new List<AudioData>();

    public AudioData GetAudioDataByKey(string key)
    {
        foreach (AudioData audioData in audioDataList)
        {
            if (audioData.audioKey == key)
            {
                return audioData;
            }
        }
        Debug.LogWarning("AudioData not found: " + key);
        return null;
    }   
}