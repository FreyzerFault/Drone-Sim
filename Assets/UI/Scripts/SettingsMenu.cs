using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : TabMenu
{
    public SettingsManager Settings => SettingsManager.Instance;

    GameObject AudioMenu => menus[0];
    GameObject GraphicsMenu => menus[1];
    GameObject ControlsMenu => menus[2];
    GameObject GameMenu => menus[3];

    [SerializeField] private List<Button> botButtons;

    [SerializeField] private GameObject closeDialogueBox;
    
    protected override void Awake()
    {
        base.Awake();
        
        OnOpen += LoadSetting;
        onTabChange += OnTabChange;
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


    public void LoadSetting()
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

    private void OnTabChange()
    {
        // Cambiamos la navegacion de los botones de abajo para conectarlos con el ultimo seleccionable
        
        // El ultimo lo conectamos con el primer boton de abajo
        Selectable lastSelectable = menus[selectedTab].GetComponentsInChildren<Selectable>()[^1];
        
        Navigation lastNav = lastSelectable.navigation;
        lastNav.selectOnDown = botButtons[0];
        lastSelectable.navigation = lastNav;
            
        // Todos los botones de abajo lo conectamos con el ultimo seleccionable
        foreach (Button button in botButtons)
        {
            Navigation bNav = button.navigation;
            bNav.selectOnUp = lastSelectable;

            button.navigation = bNav;
        }
    }

    public override void Close()
    {
        closeDialogueBox.SetActive(true);
        closeDialogueBox.GetComponentsInChildren<Selectable>()[0].Select();
    }

    public void CloseTrue()
    {
        base.Close();   
    }
}
