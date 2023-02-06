using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResolutionManager : SingletonPersistent<ResolutionManager>
{
    private SettingsManager Settings => SettingsManager.Instance;

    private Vector2Int resolution = new Vector2Int(1920, 1080);
    private int refreshRate = 60;
    public FullScreenMode screenMode = FullScreenMode.Windowed;

    #region Query Current Monitor Settings

    public static Resolution CurrentMonitorResolution => Screen.currentResolution;
    public static int CurrentMonitorRefreshRate => Screen.currentResolution.refreshRate;

    public static Resolution[] CurrentMonitorResolutions => Screen.resolutions;

    public static Resolution[] FilteredResolutionsByRefreshRate(int refreshRate)
    {
        List<Resolution> filteredResolutions = CurrentMonitorResolutions.ToList();
        filteredResolutions.RemoveAll((res) => res.refreshRate != refreshRate);
        return filteredResolutions.ToArray();
    }

    public static FullScreenMode CurrentScreenMode => Screen.fullScreenMode;

    #endregion

    public Vector2Int Resolution
    {
        get => resolution;
        private set => resolution = value;
    }

    private void Start()
    {
        Settings.OnLoad += LoadSettings;
        Settings.OnSave += SaveSettings;
        LoadSettings();
    }

    public void LoadSettings() => 
        SetSettings(Settings.Resolution.x, Settings.Resolution.y, Settings.RefreshRate, Settings.ScreenMode);

    private void SaveSettings()
    {
        Settings.Resolution = resolution;
        Settings.RefreshRate = refreshRate;
        Settings.ScreenMode = screenMode;
    }

    public static void SetSettings(int width, int height, int refreshRate, FullScreenMode screenMode)
    {
        Instance.resolution = new Vector2Int(width, height);
        Instance.refreshRate = refreshRate;
        Instance.screenMode = screenMode;
        
        Screen.SetResolution(width, height, screenMode, refreshRate);
    }

    public static void SetResolution(int width, int height)
    {
        Instance.resolution = new Vector2Int(width, height);

        Screen.SetResolution(width, height, Instance.screenMode);

        Debug.Log("Resolution changed to " + width + "x" + height);
    }

    public static void SetResolution(Vector2Int resolution) => SetResolution(resolution.x, resolution.y);
    public static void SetResolution(Vector2 resolution) => SetResolution((int) resolution.x, (int) resolution.y);

    public static void SetResolution(Resolution resolution) =>
        SetResolution((int) resolution.width, (int) resolution.height);

    public static void SetRefreshRate(int refreshRate)
    {
        Instance.refreshRate = refreshRate;
        Screen.SetResolution(Instance.resolution.x, Instance.resolution.y, Instance.screenMode,
            refreshRate);
    }

    public static void SetScreenMode(FullScreenMode mode)
    {
        Instance.screenMode = mode;

        #if UNITY_EDITOR
            Debug.Log("Game is in " + mode);
        #else
            Screen.SetResolution(Instance.Resolution.x, Instance.Resolution.y, mode);
        #endif
    }
}