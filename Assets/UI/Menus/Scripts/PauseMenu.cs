using DroneSim;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MenuManager
{
    private Menu audioMenu => RootMenu.subMenus[0].subMenus[0];
    private Menu videoMenu => RootMenu.subMenus[0].subMenus[1];
    
    private void Start()
    {
        RootMenu.OnOpen += Pause;
        RootMenu.OnClose += UnPause;

        audioMenu.OnClose += SaveSettings;
        videoMenu.OnClose += SaveSettings;
    }

    public void Pause()
    {
        GameManager.SwitchGameState(GameState.Pause);
        GetComponent<PlayerInput>().enabled = true;
        FindObjectOfType<DroneInputController>().GetComponent<PlayerInput>().enabled = false;
    }

    private void UnPause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GetComponent<PlayerInput>().enabled = false;
        FindObjectOfType<DroneInputController>().GetComponent<PlayerInput>().enabled = true;
        GameManager.ReturnToPreviousState();
    }

    public void ResetLevel() => LevelManager.ResetLevel();
    public void Quit() => LevelManager.QuitLevel();

    private void SaveSettings() => SettingsManager.Instance.Save();
}
