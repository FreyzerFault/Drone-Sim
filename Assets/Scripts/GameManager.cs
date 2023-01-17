using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum GameMode {Drone, Pause}

public class GameManager : SingletonPersistent<GameManager>
{
    // GAME MODES
    public GameMode defaultGameMode = GameMode.Drone;
    
    [SerializeField]
    private GameMode mode;
    [SerializeField]
    private GameMode prevMode;
    
    public GameMode Mode { get => mode; private set => mode = value; }
    public GameMode PrevMode { get => prevMode; private set => prevMode = value; }
    public bool GameIsPaused => mode == GameMode.Pause;

    public static event Action OnQuitGame;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += (scene, loadMode) => mode = defaultGameMode; 
    }

    public void SwitchMode(GameMode newMode)
    {
        // Al salir de un Modo
        switch (mode)
        {
            case GameMode.Drone:
                // TODO
                break;
            case GameMode.Pause:
                Time.timeScale = 1;
                break;
        }
        
        // Al entrar en ese modo
        switch (newMode)
        {
            case GameMode.Drone:
                // TODO
                break;
            case GameMode.Pause:
                Time.timeScale = 0;
                break;
        }

        prevMode = mode;
        mode = newMode;
    }
    
    public void SwitchToPreviousMode() => SwitchMode(prevMode);
    
    
    // CAMERA MANAGEMENT
    public static Camera Camera { 
        get => Camera.main;
        set
        {
            Camera.main.gameObject.SetActive(false);
            value.gameObject.SetActive(true);
        }
    }

    public void ResetGame()
    {
        SwitchMode(defaultGameMode);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public static void Quit () 
    {
        OnQuitGame?.Invoke();
        
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
