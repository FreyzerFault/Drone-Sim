using System;
using UnityEngine;

public class SettingsManager : SingletonPersistent<SettingsManager>
{
    [SerializeField] private SettingsSO settings;
    [SerializeField] private SettingsSO defaultSettings;

    public event Action OnLoad;
    public event Action OnSave;

    #region Settings Values

    public float GlobalVolume
    {
        get => settings.globalVolume;
        set => settings.globalVolume = value;
    }

    public float MusicVolume
    {
        get => settings.musicVolume;
        set => settings.musicVolume = value;
    }
    public float SFXVolume 
    {
        get => settings.effectsVolume;
        set => settings.effectsVolume = value;
    }
    public Vector2Int Resolution 
    {
        get => settings.resolution;
        set => settings.resolution = value;
    }
    public int RefreshRate
    {
        get => settings.refreshRate;
        set => settings.refreshRate = value;
    }
    public FullScreenMode ScreenMode
    {
        get => settings.screenMode;
        set => settings.screenMode = value;
    }
    public bool GodMode 
    {
        get => settings.godMode;
        set => settings.godMode = value;
    }

    #endregion

    #region PlayerPrefs ID

    private const string GLOBAL_VOLUME_ID = "globalSound";
    private const string MUSIC_VOLUME_ID = "musicSound";
    private const string SFX_VOLUME_ID = "effectsSound";
    
    private const string RESOLUTION_WIDTH_ID = "ResolutionX";
    private const string RESOLUTION_HEIGHT_ID = "ResolutionY";
    private const string REFRESH_RATE_ID = "RefreshRate";
    private const string SCREEN_MODE_ID = "ScreenMode";
    
    private const string GOD_MODE_ID = "GodMode";

    #endregion

    protected override void Awake()
    {
        base.Awake();
        
        Load();
    }

    private void Start() => GameManager.Instance.OnQuitGame += Save;

    public void Save()
    {
        OnSave?.Invoke();
        
        // Audio
        PlayerPrefs.SetFloat(GLOBAL_VOLUME_ID, GlobalVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_ID, MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_ID, SFXVolume);

        // Graphics
        PlayerPrefs.SetInt(RESOLUTION_WIDTH_ID, Resolution.x);
        PlayerPrefs.SetInt(RESOLUTION_HEIGHT_ID, Resolution.y);
        PlayerPrefs.SetInt(REFRESH_RATE_ID, RefreshRate);
        PlayerPrefs.SetInt(SCREEN_MODE_ID, (int)ScreenMode);
        
        // Controls
        
        // Game
        PlayerPrefs.SetInt(GOD_MODE_ID, GodMode ? 1 : 0);

        PlayerPrefs.Save();

        Debug.Log("Settings Saved");
    }

    public void Load()
    {
        // Audio
        GlobalVolume = PlayerPrefs.GetFloat(GLOBAL_VOLUME_ID, .5f);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_ID, .5f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_ID, .5f);

        // Graphics
        int w = PlayerPrefs.GetInt(RESOLUTION_WIDTH_ID, ResolutionManager.CurrentMonitorResolution.width);
        int h = PlayerPrefs.GetInt(RESOLUTION_HEIGHT_ID, ResolutionManager.CurrentMonitorResolution.height);
        Resolution = new Vector2Int(w, h);
        RefreshRate = PlayerPrefs.GetInt(REFRESH_RATE_ID, ResolutionManager.CurrentMonitorRefreshRate);
        ScreenMode = (FullScreenMode) PlayerPrefs.GetInt(SCREEN_MODE_ID, (int) FullScreenMode.Windowed);

        // Controls
        
        // Game
        GodMode = PlayerPrefs.GetInt(GOD_MODE_ID, 0) == 1;
        
        OnLoad?.Invoke();
        
        Debug.Log("Settings Loaded");
    }
    
    public void ReturnToDefaultSettings()
    {
        GlobalVolume = defaultSettings.globalVolume;
        MusicVolume = defaultSettings.musicVolume;
        SFXVolume = defaultSettings.effectsVolume;
        Resolution = defaultSettings.resolution;
        RefreshRate = defaultSettings.refreshRate;
        ScreenMode = defaultSettings.screenMode;
        GodMode = defaultSettings.godMode;
        
        OnLoad.Invoke();
    }
}
