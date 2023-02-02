using UnityEngine;

public class ResolutionManager : SingletonPersistent<ResolutionManager>
{
    private SettingsManager Settings => SettingsManager.Instance;

    private Vector2Int resolution = new Vector2Int(1920, 1080);
    
    public Vector2Int Resolution { get => resolution; private set => resolution = value; }

    private void Start()
    {
        Settings.OnLoad += LoadSettings;
        Settings.OnSave += SaveSettings;
        LoadSettings();
    }

    private void LoadSettings()
    {
        resolution = Settings.Resolution;
        SetResolution(resolution);
    }

    private void SaveSettings() => Settings.Resolution = resolution;

    public static void SetResolution(int width, int height)
    {
        Instance.resolution = new Vector2Int(width, height);
        
        // TODO Cambiar resolucion de la pantalla
    }

    public static void SetResolution(Vector2Int resolution) => SetResolution(resolution.x, resolution.y);
    public static void SetResolution(Vector2 resolution) => SetResolution((int) resolution.x, (int)resolution.y);
}
