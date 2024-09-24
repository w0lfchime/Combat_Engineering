using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance { get; private set; }


    public InputManager inputManager;
    public WorldManager worldManager;
    public UserInterface userInterface;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        inputManager = GetComponentInChildren<InputManager>();
        worldManager = GetComponentInChildren<WorldManager>();
        userInterface = GetComponentInChildren<UserInterface>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(SceneTransition(sceneName));
    }

    private IEnumerator SceneTransition(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
