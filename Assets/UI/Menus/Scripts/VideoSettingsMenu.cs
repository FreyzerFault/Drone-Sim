public class VideoSettingsMenu : Menu
{
    private SettingsManager Settings => SettingsManager.Instance;
    
    private ResolutionDropdown resolutionDropdown;
    private WindowModeDropdown windowModeDropdown;
    
    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;
    }
    
    public void LoadSettings()
    {
        if (resolutionDropdown == null)
            resolutionDropdown = GetComponentInChildren<ResolutionDropdown>(true);
        if (windowModeDropdown == null)
            windowModeDropdown = GetComponentInChildren<WindowModeDropdown>(true);

        resolutionDropdown.UpdateDropdownOptions();
        windowModeDropdown.UpdateDropdownOptions();
    }
}
