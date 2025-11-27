using UnityEngine;
using UnityEngine.SceneManagement;
public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance { get; private set; }
    public int count;
    [SerializeField]
    public GameObject _dontDestroyGO;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(_dontDestroyGO);

        count = 0;
    }

    void Start()
    {
        Debug.Log("Scene Manager Started");
        ChangeScene();
    }
    public void ChangeScene()
    {
        Debug.Log("Changing Scene...");
        var allScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        Debug.Log("Current Scene Index: " + count);
        if (count < allScenes)
        {
            count++;
            UnityEngine.SceneManagement.SceneManager.LoadScene(count);
        }
        else
        {
            GoToMainMenu();
        }
    }

    public void ContinueFromPrefs()
    {
        Debug.Log("Continuing from saved level...");
        GoToGame();
        StartCoroutine(GameManager.Instance.StartAct(PlayerPrefs.GetFloat("currentLevel", 1)));
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to Main Menu...");
        count = 1; // main menu scene index
        UnityEngine.SceneManagement.SceneManager.LoadScene(count);
    }

    public void GoToGame()
    {
        Debug.Log("Going to Game...");
        AudioManager.Instance.ChangeState(AudioManager.Instance.audioStates["Inactive"]);
        count = 2; // game scene index
        UnityEngine.SceneManagement.SceneManager.LoadScene(count);
    }

    public void StartNewGame()
    {
        Debug.Log("Starting New Game...");
        GoToGame();
        StartCoroutine(GameManager.Instance.StartAct(1));
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
