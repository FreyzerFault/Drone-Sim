using TMPro;
using UnityEngine;

public class VideoSettingsMenu : Menu
{
    private SettingsManager Settings => SettingsManager.Instance;
    
    private TMP_Dropdown resolutionDropdown;
    
    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;

        InitializeValueChangedEvents();
    }
    
    public void LoadSettings()
    {
        if (resolutionDropdown == null)
            resolutionDropdown = GetComponentsInChildren<TMP_Dropdown>(true)[0];
        
        // Busca de todas las opciones la que coincide con la resolucion guardada
        for (var i = 0; i < resolutionDropdown.options.Count; i++)
        {
            TMP_Dropdown.OptionData option = resolutionDropdown.options[i];
            
            // Resolucion en formato 1920x1080 [WidthxHeight]
            Vector2Int resolution = option.text.ParseResolution();
            
            if (resolution.x == (int) Settings.Resolution.x && resolution.y == (int) Settings.Resolution.y)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }
    
    private void InitializeValueChangedEvents()
    {
        
        resolutionDropdown = GetComponentsInChildren<TMP_Dropdown>(true)[0];
        resolutionDropdown.onValueChanged.AddListener((index) =>
        {
            // Resolucion en formato 1920x1080 [WidthxHeight]
            Vector2Int resolution = resolutionDropdown.options[index].text.ParseResolution();
            ResolutionManager.SetResolution(resolution);
        });
    }
}
