using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex;
    void Awake()
    {
        if (!SceneManager.GetSceneByName("Main").isLoaded)
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Additive);
            GlobalSceneManager.Instance.count = sceneIndex;
        }
    }
}
