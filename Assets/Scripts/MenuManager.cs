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
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider sliderMasterVolume;
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private Slider sliderFXVolume;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        buttonContinue.gameObject.SetActive(PlayerPrefs.HasKey("currentLevel"));
    }

    void Start()
    {
        getVolumeSettingsFromPrefsAndApply();

        AudioManager.Instance.PlaySound("Music_Menu");
        if (buttonStart != null) buttonStart.onClick.AddListener(() =>
        {
            Debug.Log("Start Game clicked");
            GlobalSceneManager.Instance.StartNewGame();
        });
        if (buttonConfig != null) buttonConfig.onClick.AddListener(ShowControls);
        if (buttonBack != null) buttonBack.onClick.AddListener(GoBack);
        if (buttonContinue != null) buttonContinue.onClick.AddListener(() =>
        {
            Debug.Log("Continue Game clicked");
            Debug.Log("Saved Level: " + PlayerPrefs.GetFloat("currentLevel"));
            GlobalSceneManager.Instance.ContinueFromPrefs();
        });
        if (buttonExit != null) buttonExit.onClick.AddListener(GlobalSceneManager.Instance.exitGame);

        List<Button> buttons = new List<Button> { buttonStart, buttonContinue, buttonExit, buttonConfig, buttonBack };
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => AudioManager.Instance.PlaySound("UI_Click"));
        }
    }

    private void getVolumeSettingsFromPrefsAndApply()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        float fxVolume = PlayerPrefs.GetFloat("fxVolume", 1.0f);

        AudioManager.Instance.SetMasterVolume(masterVolume);
        AudioManager.Instance.SetMusicVolume(musicVolume);
        AudioManager.Instance.SetFXVolume(fxVolume);

        if (sliderMasterVolume != null) sliderMasterVolume.value = masterVolume;
        if (sliderMusicVolume != null) sliderMusicVolume.value = musicVolume;
        if (sliderFXVolume != null) sliderFXVolume.value = fxVolume;
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
        optionsPanel.SetActive(true);
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
        optionsPanel.SetActive(false);
    }
    public void OnVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }
    public void OnMusicVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }
    public void OnFXVolumeSliderChanged(float value)
    {
        AudioManager.Instance.SetFXVolume(value);
    }
}