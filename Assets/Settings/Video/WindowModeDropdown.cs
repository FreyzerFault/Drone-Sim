using System.Collections.Generic;
using UnityEngine;

public class WindowModeDropdown : BetterDropdown
{
    private FullScreenMode ScreenMode => (FullScreenMode) screenModeIndex;
    private int screenModeIndex;

    private FullScreenMode CurrentScreenMode => Screen.fullScreenMode;
    private int CurrentScreenModeIndex => (int) Screen.fullScreenMode;
    
    
    private const int SCREEN_MODE_COUNT = 4;


    private void Start()
    {
        UpdateDropdownOptions();
        InitializeValueChangedEvents();
    }

    public void UpdateDropdownOptions()
    {
        screenModeIndex = (int) ResolutionManager.Instance.screenMode;

        List<string> options = new List<string>();
        for (int i = 0; i < SCREEN_MODE_COUNT; i++) options.Add(((FullScreenMode) i).ToString());

        base.UpdateDropdownOptions(options, screenModeIndex);
    }

    private void InitializeValueChangedEvents()
    {
        if (Dropdown == null && !TryGetComponent(out Dropdown)) return;

        Dropdown.onValueChanged.AddListener(
            index =>
            {
                screenModeIndex = index;
                UpdateScreenMode();
            });
    }

    private void UpdateScreenMode() =>
        ResolutionManager.SetScreenMode(ScreenMode);
}