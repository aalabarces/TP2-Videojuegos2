using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    private TMPro.TextMeshProUGUI titleText;
    private TMPro.TextMeshProUGUI subtitleText;
    private TMPro.TextMeshProUGUI explanationText;
    private GameObject backgroundPanel;
    [SerializeField] private float fadeAnimationDuration = 1.0f;
    [SerializeField] private float displayDelay = 2.0f;
    private bool skipTitle = false;
    private List<TMPro.TextMeshProUGUI> textElements;

    void Awake()
    {
        titleText = titlePanel.transform.Find("TitleText").GetComponent<TMPro.TextMeshProUGUI>();
        subtitleText = titlePanel.transform.Find("SubtitleText").GetComponent<TMPro.TextMeshProUGUI>();
        explanationText = titlePanel.transform.Find("ExplanationText").GetComponent<TMPro.TextMeshProUGUI>();
        backgroundPanel = titlePanel.transform.Find("BackgroundPanel").gameObject;
        Debug.Log("Title component initialized.");
        Debug.Log("Title texts found: " + titleText.text + ", " + subtitleText.text + ", " + explanationText.text);
        Debug.Log("Background panel found: " + backgroundPanel.name);
        textElements = new List<TMPro.TextMeshProUGUI>() { titleText, subtitleText, explanationText };
        foreach (var textElement in textElements)
        {
            textElement.alpha = 0.0f;
            textElement.gameObject.SetActive(false);
        }
        backgroundPanel.SetActive(false);
    }

    public void SetupTitles(TitleData titleData)
    {
        Debug.Log("Setting up title for act: " + titleData.act);
        ChangeTitleText(titleData.title);
        ChangeSubtitleText(titleData.subtitle);
        ChangeExplanationText(titleData.description);
    }

    public void ChangeTitleText(string newTitle)
    {
        titleText.text = newTitle;
    }
    public void ChangeSubtitleText(string newSubtitle)
    {
        subtitleText.text = newSubtitle;
    }
    public void ChangeExplanationText(string newExplanation)
    {
        explanationText.text = newExplanation;
    }

    public IEnumerator ShowTitle()
    {
        skipTitle = false;
        StartCoroutine(StopTitleIfKeyPressed());
        Debug.Log("Showing title screen...");
        backgroundPanel.SetActive(true);
        titleText.alpha = 1.0f;
        titleText.gameObject.SetActive(true);
        titleText.alpha = 1.0f;
        yield return new WaitForSeconds(displayDelay);
        if (skipTitle) yield break;
        subtitleText.gameObject.SetActive(true);
        yield return PlayFadeInAnimation(subtitleText);
        if (skipTitle) yield break;
        yield return new WaitForSeconds(displayDelay);
        if (skipTitle) yield break;
        explanationText.gameObject.SetActive(true);
        yield return PlayFadeInAnimation(explanationText);
        if (skipTitle) yield break;
        yield return new WaitUntil(() => Input.anyKey);
        yield return PlayFadeOutAll();
    }

    private IEnumerator StopTitleIfKeyPressed()
    {
        while (!skipTitle)
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("Key pressed, stopping title screen...");
                skipTitle = true;
                yield return PlayFadeOutAll();
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator PlayFadeInAnimation(TMPro.TextMeshProUGUI textElement)
    {
        float elapsed = 0.0f;

        while (elapsed < fadeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeAnimationDuration);
            textElement.alpha = alpha;
            yield return null;
        }

        textElement.alpha = 1.0f;
    }
    private IEnumerator PlayFadeOutAll()
    {
        float elapsed = 0.0f;

        while (elapsed < fadeAnimationDuration)
        {
            foreach (var textElement in textElements)
            {
                float alpha = 1.0f - Mathf.Clamp01(elapsed / fadeAnimationDuration);
                textElement.alpha = alpha;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var textElement in textElements)
        {
            textElement.alpha = 0.0f;
            textElement.gameObject.SetActive(false);
        }
        backgroundPanel.SetActive(false);
    }

    public void HideInstantly()
    {
        foreach (var textElement in textElements)
        {
            textElement.alpha = 0.0f;
            textElement.gameObject.SetActive(false);
        }
        backgroundPanel.SetActive(false);
    }
}
