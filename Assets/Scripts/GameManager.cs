using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum GameMode {Drone, Pause}

public class GameManager : Singleton<GameManager>
{
    // GAME MODES
    [SerializeField]
    private GameMode mode;
    [SerializeField]
    private GameMode prevMode;
    
    public GameMode Mode { get => mode; private set => mode = value; }
    public GameMode PrevMode { get => prevMode; private set => prevMode = value; }
    public bool IsPaused => mode == GameMode.Pause;
    
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
        SwitchMode(GameMode.Drone);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void Quit () 
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
