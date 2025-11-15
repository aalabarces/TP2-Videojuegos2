using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [SerializeField] private DialogueConfig dialogueConfig;
    [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject portraitPanel;
    [SerializeField] private float velocidadLetra = 0.03f;
    private DialogueData currentDialogue;
    public bool StopTyping = false;
    [SerializeField] private AudioSource typeSoundSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ChangeCharacterNameText(string newName)
    {
        characterNameText.text = newName;
    }
    private IEnumerator TypeTextCoroutine(string dialogue)
    {
        dialogueText.text = "";
        StopTyping = false;
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            PlayTypeSound();
            yield return new WaitForSeconds(velocidadLetra);
            if (StopTyping)
            {
                dialogueText.text = dialogue;
                break;
            }
        }
    }
    public void SkipDialogue()
    {
        Debug.Log("Skipping dialogue");
        StopTyping = true;
        if (currentDialogue != null)
        {
            dialogueText.text = currentDialogue.dialogueText;
            currentDialogue = null;
        }
    }

    public IEnumerator PlayDialogueSequence(string sequenceKey)
    {
        Debug.Log("Playing dialogue sequence: " + sequenceKey);
        Speaker agus = dialogueConfig.GetSpeakerByName("Agustincito");
        Speaker doctor = dialogueConfig.GetSpeakerByName("Dr. Cromático");
        Speaker narrator = dialogueConfig.GetSpeakerByName("Narrador");
        UIManager.Instance.dialogue.SetActive(true);
        switch (sequenceKey)
        {
            case "Act1_Intro":
                Debug.Log("Starting Act 1 Intro Dialogue");
                yield return Speaker_Play_Key_Lines(agus, "Act1_Intro", 3);
                break;
            
            case "Act1Ending":
                Debug.Log("Starting Act 1 Ending Dialogue");
                yield return Speaker_Play_Key_Lines(agus, "Act1_Ending", 3);
                break;
            case "Act3_Intro":
                Debug.Log("Starting Act 3 Intro Dialogue");
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Intro", 3);
                break;
            case "DefeatAct3":
                Debug.Log("Starting Act 3 Defeat Dialogue");
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Defeat", 2);
                break;
            case "VictoryAct3":
                Debug.Log("Starting Act 3 Victory Dialogue");
                // TODO: AudioManager play victory music
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Victory", 2);
                Debug.Log("Starting Narrator Ending Dialogue");
                yield return Speaker_Play_Key_Lines(narrator, "Ending", 2);
                yield return new WaitUntil(() => Input.anyKeyDown);

                break;
            default:
                Debug.LogWarning("Dialogue sequence key not found: " + sequenceKey);
                yield break;
        }
        UIManager.Instance.dialogue.SetActive(false);
    }

    private IEnumerator Speaker_Play_Key_Lines(Speaker speaker, string key, int lineCount)
    {
        for (int i = 1; i <= lineCount; i++)
        {
            string dialogueKey = key + "_" + i;
            string dialogue = speaker.GetDialogueByKey(dialogueKey).dialogueText;
            portraitPanel.GetComponent<UnityEngine.UI.Image>().sprite = speaker.speakerImage;
            ChangeCharacterNameText(speaker.speakerName);
            yield return TypeTextCoroutine(dialogue);
            yield return new WaitUntil(() => Input.anyKeyDown);
        }
    }

    private void PlayTypeSound()
    {
        if (typeSoundSource != null)
        {
            typeSoundSource.Play();
        }
    }
}
