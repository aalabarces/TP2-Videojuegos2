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
    private List<GameObject> colorTriggers = new List<GameObject>();
    public GameObject dialogue;
    public Title title;
    private TextMeshProUGUI[] textElements;

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
        HideEverything();
        UpdateScore(GameManager.Instance.score);
        UpdateTimer(GameManager.Instance.gameTimer);
        restartBtn.onClick.AddListener(OnRestartButtonClicked);
        exitBtn.onClick.AddListener(() => GlobalSceneManager.Instance.GoToMainMenu());
        HideEverything();
    }

    void Update()
    {
        if (GameManager.Instance.timerIsActive)
        {
            UpdateTimer(GameManager.Instance.gameTimer);
        }
    }

    private void HideEverything()
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
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    public void UpdateLives(int newLives)
    {
        livesText.text = "Lives: " + newLives;
    }

    public void UpdateTimer(float newTime)
    {
        if (newTime == 0) { timerText.text = ""; return; }
        int minutes = Mathf.FloorToInt(newTime / 60f);
        int seconds = Mathf.FloorToInt(newTime % 60f);
        timerText.text = minutes.ToString("0") + ":" + seconds.ToString("00");
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
        GlobalSceneManager.Instance.GoToMainMenu();
    }
    public void ResumeGameplay()
    {
        GameManager.Instance.ChangeState(GameManager.Instance.gameStates["Active"]);
    }
}
