public class MenuManager : Singleton<MenuManager>
{
    public Menu RootMenu;

    protected override void Awake()
    {
        base.Awake();
        if (RootMenu == null)
            RootMenu = GetComponent<Menu>();
    }

    public void Toggle()
    {
        if (RootMenu.isOpen)
            Close();
        else
            Open();
    }

    public void Open() => RootMenu.Open();
    public bool Close() => RootMenu.Close();
    
    public void OnCancel() => RootMenu.OnCancelRecursive();
}