using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Game Config")]
    public GlobalConfig globalConfig;
    public LevelsConfig levelsConfig;
    public UIManager uiManager;
    [SerializeField] private int countdown = 3;
    public Player player;
    public float currentLevel = 1f;
    public LevelData currentLevelData;
    public bool isGameActive { get; private set; } = false;
    public bool isSpawningAllowed { get; private set; } = false;
    public int currentWave { get; private set; } = 1;
    public bool timerIsActive { get; private set; } = false;
    public float gameTimer { get; private set; } = 300f;
    [SerializeField] private float fakeTimer;
    public int score { get; private set; } = 0;
    public Dictionary<string, IGameState> gameStates = new Dictionary<string, IGameState>();
    public IGameState currentState;
    private Coroutine rutinaSpawn;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameStates.Add("Inactive", new InactiveState());
        gameStates.Add("Dialogue", new DialogueState());
        gameStates.Add("Active", new ActiveState());
        gameStates.Add("Pause", new PauseState());
        currentState = gameStates["Inactive"]; // initial state
        currentState.EnterState(this);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentState.HandleEscapeKey(this);
        }
        currentState.UpdateState(this);
    }

    public LevelData GetLevelData(float act)
    {
        return levelsConfig.GetLevel(act);
    }

    public void ChangeState(IGameState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public IEnumerator GetReferences()
    {
        // uiManager and player might not be ready yet
        while (UIManager.Instance == null)
        {
            yield return null;
        }
        uiManager = UIManager.Instance;
        Debug.Log("uiManager found: " + uiManager);
        while (FindObjectOfType<Player>() == null)
        {
            yield return null;
        }
        player = FindObjectOfType<Player>();
        Debug.Log("Player found: " + player);
    }

    private bool AreReferencesEmpty()
    {
        return uiManager == null || player == null;
    }

    public IEnumerator StartAct(float act = 1f)
    {
        // Make sure gameplay is ready:
        if (AreReferencesEmpty())
        {
            Debug.Log("Getting references...");
            yield return GetReferences();
        }
        // Start SpawnManager loop if not started
        if (SpawnManager.Instance.loopHasStarted == false)
        {
            Debug.Log("Starting SpawnManager loop...");
            // rutinaSpawn = StartCoroutine(SpawnManager.Instance.SpawnLoop());
            SpawnManager.Instance.StartEnemySpawnLoop();
        }
        yield return ResetState();
        // Load level data
        currentLevelData = GetLevelData(act);
        if (currentLevelData == null)
        {
            Debug.LogError("Act " + act + " no encontrado!");
            yield return null;
        }
        currentLevel = act;
        PlayerPrefs.SetFloat("currentLevel", currentLevel);
        PlayerPrefs.Save();
        SpawnManager.Instance.spawnDelay = currentLevelData.spawnInterval;
        if (act < 2f) UIManager.Instance.HideColorTriggers();
        else UIManager.Instance.ShowColorTriggers();
        ChangeState(gameStates["Inactive"]);
        if (currentLevelData.titles.title != "")
        {
            // Show Act Title Screen
            if (act == 2f) StartCoroutine(uiManager.title.StartColorTitleCoroutine());
            uiManager.title.SetupTitles(currentLevelData.titles);
            // StartCoroutine(uiManager.title.StopTitleIfKeyPressed());
            yield return uiManager.title.ShowTitle();
        }
        
        switch (act)
        {
            case 1f:
                UIManager.Instance.HideHearts();
                ChangeState(gameStates["Dialogue"]);
                yield return DialogueManager.Instance.PlayDialogueSequence("Act1_Intro");
                yield return new WaitUntil(() => Input.anyKey);
                break;
            case 1.5f:
                UIManager.Instance.ChangeLivesTextToHearts();
                UIManager.Instance.HideFakeBack();
                break;
            case 2f:
                UIManager.Instance.ChangeLivesTextToHearts();
                UIManager.Instance.HideFakeBack();
                SpawnManager.Instance.ChangeEnemyStatesToSeek();
                break;
            case 3f:
                UIManager.Instance.ChangeLivesTextToHearts();
                UIManager.Instance.HideFakeBack();
                ChangeState(gameStates["Dialogue"]);
                yield return DialogueManager.Instance.PlayDialogueSequence("Act3_Intro");
                yield return new WaitUntil(() => Input.anyKey);
                break;
            default:
                break;
        }
        
        ChangeState(gameStates["Inactive"]);

        // Start Countdown
        uiManager.countdownText.gameObject.SetActive(true);
        yield return WaitForCountdown();
        // Start Game
        ChangeState(gameStates["Active"]);
        if (currentLevelData.timer > 0)
        {
            timerIsActive = true;
            Debug.Log("Setting game timer to: " + currentLevelData.timer);
            gameTimer = currentLevelData.timer;
            fakeTimer = 60f;
            if (currentLevel == 1f) uiManager.InitializeTimer(fakeTimer);
            else uiManager.InitializeTimer(gameTimer);
        }
        else
        {
            Debug.Log("No timer for this act.");
            timerIsActive = false;
            uiManager.UpdateTimer(0);
            uiManager.prettyTimer.SetActive(false);
        }
        SpawnManager.Instance.SpawnFirstEnemyForCurrentLevel();
        yield return new WaitForSeconds(1);
        uiManager.HideCountdownText();
    }


    public void StartGame()
    {
        Debug.Log("Game Started!");
        if (!isGameActive) ToggleIsGameActive();
        isSpawningAllowed = true;
        uiManager.StartPlayableState();
    }

    public void StopGame()
    {
        Debug.Log("Game Stopped!");
        if (isGameActive) ToggleIsGameActive();
        isSpawningAllowed = false;
    }

    private IEnumerator WaitForCountdown()
    {
        uiManager.countdownText.gameObject.SetActive(true);
        countdown = 3;
        while (countdown > 0)
        {
            uiManager.UpdateCountdown(countdown.ToString());
            countdown--;
            AudioManager.Instance.PlaySound("UI_Countdown_0");
            yield return new WaitForSeconds(1);
        }
        AudioManager.Instance.PlaySound("UI_Countdown_1");
        uiManager.UpdateCountdown("Go!");
    }

    public IEnumerator GameOver()
    {
        Debug.Log("Game Over!");
        ChangeState(gameStates["Inactive"]);
        if (currentLevel == 3f) yield return DialogueManager.Instance.PlayDialogueSequence("DefeatAct3");
        uiManager.ShowGameOverScreen();
        yield return null;
    }

    public void PlayerKilledEnemy(GameObject enemy)
    {
        IncreaseScore(enemy.GetComponent<Enemy>().scoreValue * currentWave);
    }

    public void IncreaseScore(float points)
    {
        score += (int)points; //FLOAT TO INT
        uiManager.UpdateScore(score);
    }

    public void handleTimer()
    {
        if (!timerIsActive) return;
        gameTimer -= Time.deltaTime;

        if (currentLevel == 1f)
        {
            // Fake timer logic for Act 1
            float randomSpeed = Random.Range(0.01f, 1f);
            fakeTimer -= Time.deltaTime * randomSpeed;
            uiManager.UpdateTimer(fakeTimer);

        }
        else
        {
            uiManager.UpdateTimer(gameTimer);
        }
        // si el timer llega a 0 y el juego esta activo, termina la wave
        if (gameTimer <= 0 && isGameActive)
        {
            Debug.Log("Time's up!");
            timerIsActive = false;
            StartCoroutine(TimeIsUp());
        }
    }

    private IEnumerator TimeIsUp()
    {
        gameTimer = 0;
        uiManager.UpdateTimer(gameTimer);
        ChangeState(gameStates["Inactive"]);
        switch (currentLevel)
        {
            case 1f:
                ChangeState(gameStates["Dialogue"]);
                yield return DialogueManager.Instance.PlayDialogueSequence("Act1Ending");
                yield return new WaitUntil(() => Input.anyKey);
                ChangeState(gameStates["Inactive"]);
                StartCoroutine(StartAct(1.5f));
                break;
            case 1.5f:
                StartCoroutine(StartAct(2));
                break;
            case 2f:
                StartCoroutine(StartAct(3f));
                break;
            default:
                break;
        }
    }

    public void ToggleIsGameActive()
    {
        isGameActive = !isGameActive;
    }
    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        StartCoroutine(StartAct(PlayerPrefs.GetFloat("currentLevel", 1)));
    }

    public void SetTimerTo(float newTime)
    {
        gameTimer = newTime;
    }

    public IEnumerator BossDefeated(Boss boss)
    {
        ChangeState(gameStates["Dialogue"]);
        yield return DialogueManager.Instance.PlayDialogueSequence("VictoryAct3");
        uiManager.ShowVictoryScreen();
        AudioManager.Instance.PlaySound("Music_Victory");
        yield return DialogueManager.Instance.PlayDialogueSequence("Ending");
        yield return new WaitUntil(() => Input.anyKey);
        // TODO: Credits
        ChangeState(gameStates["Inactive"]);
        SceneManager.LoadScene("Menu");
    }

    public IEnumerator ResetState()
    {
        player.ResetPlayer();
        SpawnManager.Instance.RetrieveAllEnemies();
        SpawnManager.Instance.RetrieveAllProjectiles();
        SpawnManager.Instance.RetrieveAllPotions();
        uiManager.HideEverything();
        yield return null;
    }

    public void OnExitGameScene()
    {
        Debug.Log("Exiting Game Scene...");
        ChangeState(gameStates["Inactive"]);
        SpawnManager.Instance.ResetState();
        // StopCoroutine(rutinaSpawn);
    }
}
