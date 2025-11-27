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
    public void ChangeCharacterPortraitSprite(Sprite newSprite)
    {
        portraitPanel.GetComponent<UnityEngine.UI.Image>().sprite = newSprite;
    }
    private IEnumerator TypeTextCoroutine(string dialogue)
    {
        dialogueText.text = "";
        StopTyping = false;
        Debug.Log("Typing dialogue: " + dialogue);
        Debug.Log(dialogue.ToCharArray().Length + " characters to type.");
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
            // Acá tendria que haber un código que se ejecute automáticamente
            // Si hay intro, la pone. Si no, saltea
            // Si hay outro, la pone. Si no, saltea
            // La duración ya venga definida en el ScriptableObject
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
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Intro", 2);
                break;
            case "DefeatAct3":
                Debug.Log("Starting Act 3 Defeat Dialogue");
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Defeat", 1);
                break;
            case "VictoryAct3":
                Debug.Log("Starting Act 3 Victory Dialogue");
                // TODO: AudioManager play victory music
                yield return Speaker_Play_Key_Lines(doctor, "Act3_Victory", 1);
                break;
            case "Ending":
                Debug.Log("Starting Narrator Ending Dialogue");
                yield return Speaker_Play_Key_Lines(narrator, "Ending", 3);
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
            ChangeCharacterPortraitSprite(speaker.speakerImage);
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
