public class PauseMenu : MenuManager
{
    private void Start()
    {
        RootMenu.OnOpen += Pause;
        RootMenu.OnClose += UnPause;
    }

    public static void Pause() => GameManager.SwitchGameState(GameState.Pause);
    public static void UnPause() => GameManager.ReturnToPreviousState();

    public void ResetLevel() => LevelManager.ResetLevel();
    public void Quit() => LevelManager.QuitLevel();
}
