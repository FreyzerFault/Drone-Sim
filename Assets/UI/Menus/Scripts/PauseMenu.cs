using DroneSim;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MenuManager
{
    private Menu AudioMenu => rootMenu.GetSubmenu(0).GetSubmenu(0);
    private Menu VideoMenu => rootMenu.GetSubmenu(0).GetSubmenu(0);
    
    private void Start()
    {
        rootMenu.OnOpen += Pause;
        rootMenu.OnClose += UnPause;

        AudioMenu.OnClose += SaveSettings;
        VideoMenu.OnClose += SaveSettings;
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
