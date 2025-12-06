using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(Title))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public Button pauseBackBtn;
    [SerializeField] public Button pauseExitBtn;
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI livesText;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI countdownText;
    [SerializeField] public TextMeshProUGUI alertsText;
    [SerializeField] public Button restartBtn;
    [SerializeField] public Button exitBtn;
    [SerializeField] public GameObject victoryScreen;
    [SerializeField] public GameObject defeatScreen;
    [SerializeField] public GameObject fakeBack;
    [SerializeField] public GameObject colorTriggersGO;
    [SerializeField] public Sprite heartFull;
    [SerializeField] public Sprite heartHalf;
    [SerializeField] public Sprite heartEmpty;
    [SerializeField] public GameObject heartsContainer;
    [SerializeField] private float heartSpacing = 100f;
    [SerializeField] Slider bossHealthBar;
    private List<GameObject> colorTriggers = new List<GameObject>();
    public GameObject dialogue;
    public Title title;
    private TextMeshProUGUI[] textElements;
    [SerializeField] public GameObject prettyTimer;
    [SerializeField] public Image clockFillImage;
    [SerializeField] public Image clockFaceImage;
    [SerializeField] private Slider sliderMasterVolume;
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private Slider sliderFXVolume;
    private float maxTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        title = GetComponent<Title>();

        textElements = new TextMeshProUGUI[] { scoreText, livesText, timerText, countdownText, alertsText };
    }

    void Start()
    {
        foreach (Transform child in colorTriggersGO.transform)
        {
            colorTriggers.Add(child.gameObject);
        }
        restartBtn.onClick.AddListener(OnRestartButtonClicked);
        exitBtn.onClick.AddListener(BackToMainMenu);
        UpdateScore(GameManager.Instance.score);
        UpdateTimer(GameManager.Instance.gameTimer);
        HideEverything();
        getVolumeSettingsFromPrefsAndApply();
        addEventListenersToVolumeSliders();
    }

    public void HideEverything()
    {
        foreach (var textElement in textElements)
        {
            textElement.text = "";
            textElement.gameObject.SetActive(false);
        }
        // dialogue.HideDialogue();
        if (title != null)
        {
            title.HideInstantly();
        }
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
        restartBtn.gameObject.SetActive(false);
        dialogue.SetActive(false);
        pauseMenu.SetActive(false);
        bossHealthBar.gameObject.SetActive(false);
        prettyTimer.SetActive(false);
        HideHearts();
    }

    public void ShowHud()
    {
        scoreText.gameObject.SetActive(true);
        UpdateScore(GameManager.Instance.score);
        if (GameManager.Instance.currentLevel < 1.5f) 
        {
            livesText.gameObject.SetActive(true);
            UpdateLives(GameManager.Instance.player.maxLives);
            prettyTimer.SetActive(false);
            timerText.gameObject.SetActive(true);
        }
        else 
        {
            ChangeLivesTextToHearts();
            if (GameManager.Instance.timerIsActive) {
                timerText.gameObject.SetActive(false);
                prettyTimer.SetActive(true);
            }
            else {
                prettyTimer.SetActive(false);
                timerText.gameObject.SetActive(false);
            }
        }
        UpdateTimer(GameManager.Instance.gameTimer);
        if (GameManager.Instance.currentLevel == 3f)
        {
            bossHealthBar.gameObject.SetActive(true);
        }
        else
        {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    public void UpdateScore(int newScore)
    {
        if (scoreText == null) return;
        scoreText.text = newScore.ToString();
    }


    public void InitializeHearts(int maxLives)
    {
        // Clear existing hearts
        foreach (Transform child in heartsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        int neededHearts = Mathf.CeilToInt(maxLives / 2f);
        for (int i = 0; i < neededHearts; i++)
        {
            GameObject newHeart = new GameObject("Heart", typeof(Image));
            newHeart.transform.SetParent(heartsContainer.transform, false);
            newHeart.GetComponent<Image>().preserveAspect = true;
            newHeart.GetComponent<Image>().sprite = heartFull;

            // Set position with offset
            RectTransform rectTransform = newHeart.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.anchoredPosition = new Vector2(i * heartSpacing, 0);
        }
    }

    public void UpdateLives(int newLives)
    {
        if (livesText == null) return;
        if (GameManager.Instance.currentLevel < 1.5f) livesText.text = "Vidas: " + newLives;
        else UpdateHearts(newLives);
    }

    private void UpdateHearts(int newLives)
    {
        if (heartsContainer == null) return;
        for (int i = 0; i < heartsContainer.transform.childCount; i++)
        {
            Image heartImage = heartsContainer.transform.GetChild(i).GetComponent<Image>();
            if (newLives >= (i + 1) * 2)
            {
                heartImage.sprite = heartFull;
            }
            else if (newLives == (i * 2) + 1)
            {
                heartImage.sprite = heartHalf;
            }
            else
            {
                heartImage.sprite = heartEmpty;
            }
        }
    }

    public void ChangeLivesTextToHearts()
    {
        ShowHearts();
        livesText.gameObject.SetActive(false);
    }
    
    public void HideHearts()
    {
        heartsContainer.SetActive(false);
    }

    public void ShowHearts()
    {
        heartsContainer.SetActive(true);
    }

    public void InitializeTimer(float maxTime)
    {
        this.maxTime = maxTime;
        if (clockFillImage != null) clockFillImage.fillAmount = 0;
        if (clockFaceImage != null) clockFaceImage.color = Color.white;
    }

    public void UpdateTimer(float newTime)
    {
        if (timerText != null)
        {
            if (newTime == 0) { timerText.text = ""; }
            else
            {
                int minutes = Mathf.FloorToInt(newTime / 60f);
                int seconds = Mathf.FloorToInt(newTime % 60f);
                timerText.text = minutes.ToString("0") + ":" + seconds.ToString("00");
            }
        }

        if (clockFillImage != null && maxTime > 0)
        {
            clockFillImage.fillAmount = 1 - (newTime / maxTime);
        }

        if (clockFaceImage != null && maxTime > 0)
        {
            if (newTime / maxTime <= 0.25f && newTime > 0)
            {
                // Flashing effect
                float t = Mathf.PingPong(Time.time * 5f, 1f);
                clockFaceImage.color = Color.Lerp(Color.white, Color.red, t);
            }
            else
            {
                clockFaceImage.color = Color.white;
            }
        }
    }

    public void UpdateCountdown(string newCountdown)
    {
        countdownText.text = newCountdown;
    }

    public void UpdateAlerts(string newAlert)
    {
        alertsText.text = newAlert;
    }
    public void ShowGameOverScreen()
    {
        HideEverything();
        restartBtn.gameObject.SetActive(true);
        defeatScreen.SetActive(true);
    }
    public void ShowVictoryScreen()
    {
        HideEverything();
        victoryScreen.SetActive(true);
    }
    public void StartPlayableState()
    {
        countdownText.gameObject.SetActive(false);
        alertsText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
    }
    public void HideCountdownText()
    {
        countdownText.gameObject.SetActive(false);
        alertsText.gameObject.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        // alertsText.gameObject.SetActive(true);
        // UpdateAlerts("Game Paused");
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        // alertsText.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked!");
        GameManager.Instance.RestartGame();
        HideEverything();
        countdownText.gameObject.SetActive(true);
    }

    public void ShowDialogueMenu()
    {
        dialogue.SetActive(true);
    }

    public void HideDialogueMenu()
    {
        dialogue.SetActive(false);
    }

    public void HideFakeBack()
    {
        fakeBack.SetActive(false);
    }

    public void ShowColorTriggers()
    {
        foreach (var trigger in colorTriggers)
        {
            trigger.SetActive(true);
        }
    }

    public void HideColorTriggers()
    {
        foreach (var trigger in colorTriggers)
        {
            trigger.SetActive(false);
        }
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.OnExitGameScene();
        GlobalSceneManager.Instance.GoToMainMenu();
    }
    public void ResumeGameplay()
    {
        GameManager.Instance.ChangeState(GameManager.Instance.gameStates["Active"]);
    }

    public void UpdateBossHealth(float current, float max)
    {
        bossHealthBar.value = current / max;
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

    private void addEventListenersToVolumeSliders()
    {
        if (sliderMasterVolume != null)
        {
            sliderMasterVolume.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetMasterVolume(value);
                PlayerPrefs.SetFloat("masterVolume", value);
                AudioManager.Instance.PlaySound("UI_Click");
            });
        }

        if (sliderMusicVolume != null)
        {
            sliderMusicVolume.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetMusicVolume(value);
                PlayerPrefs.SetFloat("musicVolume", value);
                AudioManager.Instance.PlaySound("UI_Click");
            });
        }

        if (sliderFXVolume != null)
        {
            sliderFXVolume.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetFXVolume(value);
                PlayerPrefs.SetFloat("fxVolume", value);
                AudioManager.Instance.PlaySound("UI_Click");
            });
        }
    }
}
