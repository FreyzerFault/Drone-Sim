public class MenuManager : Singleton<MenuManager>
{
    public Menu rootMenu;

    protected override void Awake()
    {
        base.Awake();
        if (rootMenu == null)
            rootMenu = GetComponent<Menu>();
    }

    public void Toggle()
    {
        if (rootMenu.isOpen)
            Close();
        else
            Open();
    }

    private void Open() => rootMenu.Open();
    protected bool Close() => rootMenu.Close();

    #region Inputs

    public virtual void OnCancel() => rootMenu.OnCancelRecursive();
    public virtual void OnCloseMenu() => Close();    

    #endregion

}