using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    public SettingsManager Settings => SettingsManager.Instance;

    Menu AudioMenu => subMenus[0];
    Menu GraphicsMenu => subMenus[1];
    Menu ControlsMenu => subMenus[2];
    Menu GameMenu => subMenus[3];

    [SerializeField] private DynamicNavigation botButtons;

    [SerializeField] private GameObject closeDialogueBox;

    private bool canClose = false;

    protected override void Awake()
    {
        base.Awake();

        OnOpen += LoadSettings;

        // Cada vez que abre un submenu se actualiza la navegacion de los botones inferiores
        foreach (Menu subMenu in subMenus)
        {
            subMenu.OnOpen += UpdateBottomSectionNavigation;
        }
    }

    public void SaveSettings()
    {
        // AUDIO
        Slider[] audioSliders = AudioMenu.GetComponentsInChildren<Slider>();
        Settings.GlobalVolume = audioSliders[0].value * 100;
        Settings.MusicVolume = audioSliders[1].value * 100;
        Settings.EffectsVolume = audioSliders[2].value * 100;
        
        // RESOLUTION
        TMP_Dropdown resolutionDropdown = GraphicsMenu.GetComponentsInChildren<TMP_Dropdown>()[0];
        int index = resolutionDropdown.value;
        
        // Resolucion en formato 1920x1080 [WidthxHeight]
        string[] res = resolutionDropdown.options[index].text.Split("x");
        int w = int.Parse(res[0]);
        int h = int.Parse(res[1]);
        Settings.Resolution = new Vector2(w, h);

        Settings.GodMode = GameMenu.GetComponentsInChildren<Toggle>()[0].isOn;
        
        Settings.Save();
    }


    public void LoadSettings()
    {
        // AUDIO
        Slider[] audioSliders = AudioMenu.GetComponentsInChildren<Slider>();
        audioSliders[0].value = Settings.GlobalVolume / 100;
        audioSliders[1].value = Settings.MusicVolume / 100;
        audioSliders[2].value = Settings.EffectsVolume / 100;
        
        // RESOLUTION
        TMP_Dropdown resolutionDropdown = GraphicsMenu.GetComponentsInChildren<TMP_Dropdown>()[0];
        for (var i = 0; i < resolutionDropdown.options.Count; i++)
        {
            TMP_Dropdown.OptionData option = resolutionDropdown.options[i];
            
            // Resolucion en formato 1920x1080 [WidthxHeight]
            string[] res = option.text.Split("x");
            int w = int.Parse(res[0]);
            int h = int.Parse(res[1]);
            
            if (w == (int) Settings.Resolution.x && h == (int) Settings.Resolution.y)
                resolutionDropdown.value = i;
        }

        GameMenu.GetComponentsInChildren<Toggle>()[0].isOn = Settings.GodMode;
    }

    private void UpdateBottomSectionNavigation()
    {
        botButtons.topSection = subMenus[menuOpened].gameObject;
        botButtons.UpdateNavigation();
    }

    private void OpenDialogueBox()
    {
        closeDialogueBox.SetActive(true);
        closeDialogueBox.GetComponentsInChildren<Selectable>()[0].Select();
    }

    public override void Open()
    {
        canClose = false;
        base.Open();
    }

    public override bool Close()
    {
        if (canClose)
            return base.Close();
        else
        {
            OpenDialogueBox();
            canClose = true;
            return false;
        }
    }

    public void CloseTrue() => base.Close();

     public override void OnCancelRecursive()
     {
         Close();
     }
}
