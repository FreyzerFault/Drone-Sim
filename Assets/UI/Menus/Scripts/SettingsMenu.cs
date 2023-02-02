using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    public SettingsManager Settings => SettingsManager.Instance;

    AudioSettingsMenu AudioMenu => (AudioSettingsMenu) subMenus[0];
    VideoSettingsMenu VideoMenu => (VideoSettingsMenu) subMenus[1];
    Menu ControlsMenu => subMenus[2];
    Menu GameMenu => subMenus[3];

    [SerializeField] private DynamicNavigation botButtons;

    [SerializeField] private GameObject closeDialogueBox;

    private bool canClose = false;

    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;

        // Cada vez que abre un submenu se actualiza la navegacion de los botones inferiores
        foreach (Menu subMenu in subMenus) subMenu.OnOpen += UpdateBottomSectionNavigation;

        // GODMODE
        Toggle godModeToggle = GameMenu.GetComponentsInChildren<Toggle>()[0];
        godModeToggle.onValueChanged.AddListener((on) => Settings.GodMode = on);
    }


    public void SaveSettings() => Settings.Save();


    public void LoadSettings()
    {
        Settings.Load();
        
        AudioMenu.LoadSettings();
        VideoMenu.LoadSettings();

        // TODO GameSettingsMenu
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
