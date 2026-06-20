using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    InGame,
    Tutorial
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        StartLoadScene("MainMenu");
    }

    public void ChangeGameState(GameState newState)
    {
        // Implementation for changing game state
        if(newState == currentState) return; // FIX LOGIC: Không cần thiết phải chuyển nếu đã ở trạng thái đó rồi
        Debug.Log($"Changing game state from {currentState} to {newState}");
        currentState = newState;
        switch (currentState)
        {
            case GameState.MainMenu:
                GoToMainMenu();
                break;
            case GameState.InGame:
                break;
            case GameState.Tutorial:
                StartLoadScene("Tutorial_Scene");
                break;
        }
    }

    public GameState GetCurrState() => currentState;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToRaceScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartLoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAndSetup(sceneName));
    }

    private IEnumerator LoadSceneAndSetup(string sceneName)
    {
        UIManager.Instance.ShowLoading();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // UIManager.Instance.HideLoading();
    }
}
