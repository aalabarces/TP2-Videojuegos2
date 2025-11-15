using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonConfig;
    [SerializeField] private Button buttonBack;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject controlsPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        buttonContinue.gameObject.SetActive(PlayerPrefs.HasKey("currentLevel"));
    }

    void Start()
    {
        AudioManager.Instance.PlaySound("Music_Menu");
        if (buttonStart != null) buttonStart.onClick.AddListener(() =>
        {
            Debug.Log("Start Game clicked");
            GlobalSceneManager.Instance.ChangeScene();
        });
        if (buttonConfig != null) buttonConfig.onClick.AddListener(ShowControls);
        if (buttonBack != null) buttonBack.onClick.AddListener(GoBack);
        if (buttonContinue != null) buttonContinue.onClick.AddListener(() =>
        {
            Debug.Log("Saved Level: " + PlayerPrefs.GetFloat("currentLevel"));
            GameManager.Instance.SetContinueFromSavedLevel(true);
            Debug.Log("Continue Game clicked");
            GlobalSceneManager.Instance.ContinueFromPrefs();
        });
        if (buttonExit != null) buttonExit.onClick.AddListener(GlobalSceneManager.Instance.exitGame);

        List<Button> buttons = new List<Button> { buttonStart, buttonContinue, buttonExit, buttonConfig, buttonBack };
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => AudioManager.Instance.PlaySound("UI_Click"));
        }
    }
    public void ShowControls()
    {
        Debug.Log("Show Controls clicked");
        logo.SetActive(false);
        buttonContinue.gameObject.SetActive(false);
        buttonStart.gameObject.SetActive(false);
        buttonConfig.gameObject.SetActive(false);
        buttonExit.gameObject.SetActive(false);
        buttonBack.gameObject.SetActive(true);
        controlsPanel.SetActive(true);
    }
    public void GoBack()
    {
        Debug.Log("Go Back clicked");
        logo.SetActive(true);
        buttonContinue.gameObject.SetActive(PlayerPrefs.HasKey("currentLevel"));
        buttonStart.gameObject.SetActive(true);
        buttonConfig.gameObject.SetActive(true);
        buttonExit.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(false);
        controlsPanel.SetActive(false);
    }

}
