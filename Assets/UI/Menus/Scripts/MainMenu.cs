using UnityEngine;

public class MainMenu : MenuManager
{
    private static readonly int Open = Animator.StringToHash("open");

    protected override void Awake()
    {
        base.Awake();
        
        RootMenu.Open();
    }

    public void Play()
    {
        if (Close())
            LevelManager.Instance.LoadSelectedLevel();
    }

    public void Quit()
    {
        if (Close())
            GameManager.Quit();
    }
}
