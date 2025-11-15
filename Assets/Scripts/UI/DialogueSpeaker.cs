using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Speaker", menuName = "Game Config/Speaker", order = 4)]
public class Speaker : ScriptableObject
{
    public Sprite speakerImage;
    public string speakerName;
    public List<DialogueData> script;

    public DialogueData GetDialogueByKey(string key)
    {
        foreach (DialogueData dialogue in script)
        {
            if (dialogue.dialogueKey == key)
            {
                return dialogue;
            }
        }
        Debug.LogWarning("Dialogue not found: " + key);
        return null;
    }
}
