using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueConfig", menuName = "Game Config/Dialogue Config", order = 3)]
public class DialogueConfig : ScriptableObject
{
    public List<Speaker> Speakers = new List<Speaker>();

    public Speaker GetSpeakerByName(string name)
    {
        foreach (Speaker speaker in Speakers)
        {
            if (speaker.speakerName == name)
            {
                return speaker;
            }
        }
        Debug.LogWarning("Speaker not found: " + name);
        return null;
    }
   
}