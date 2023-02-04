using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{

    #region Submenus

    AudioSettingsMenu AudioMenu => (AudioSettingsMenu) subMenus[0];
    VideoSettingsMenu VideoMenu => (VideoSettingsMenu) subMenus[1];
    Menu ControlsMenu => subMenus[2];
    Menu GameMenu => subMenus[3];

    #endregion
    
    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;

        // Cada vez que abre un submenu se actualiza la navegacion de los botones inferiores
        foreach (Menu subMenu in subMenus) subMenu.OnOpen += UpdateBottomSectionNavigation;

        HandleChangedEvents();
    }

    #region Save/Load Settings

    private SettingsManager Settings => SettingsManager.Instance;

    // Prepara los items seleccionables con la actualizacion de las Settings cuando se modifica (onValueChanged)
    private void HandleChangedEvents()
    {
        // GODMODE
        Toggle godModeToggle = GameMenu.GetComponentsInChildren<Toggle>()[0];
        godModeToggle.onValueChanged.AddListener((on) => Settings.GodMode = on);
    }
    
    public void SaveSettings() => Settings.Save();
    
    public void LoadSettings()
    {
        AudioMenu.LoadSettings();
        VideoMenu.LoadSettings();

        // TODO GameSettingsMenu
        GameMenu.GetComponentsInChildren<Toggle>()[0].isOn = Settings.GodMode;
    }

    public void ApplyChanges() => Settings.Save();
    public void UndoChanges()
    {
        Settings.Load();
        LoadSettings();
    }
    public void ReturnToDefaultSettings()
    {
        Settings.ReturnToDefaultSettings();
        LoadSettings();
    }

    #endregion


    #region BottomSection Buttons: Save / Discard / Default

    [SerializeField] private DynamicNavigation botButtons;
    
    // Cambia la navegacion de los items seleccionables para que el ultimo siempre te lleve a la seccion de los botones inferiores
    private void UpdateBottomSectionNavigation() => botButtons.UpdateTopSection(SubmenuOpened.gameObject);

    #endregion


    #region Dialogue Box to Confirm Close

    private bool canClose = false;
    
    [SerializeField] private GameObject closeDialogueBox;
    
    public void OpenDialogueBox()
    {
        if (closeDialogueBox.activeSelf) return;
        
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
        
        OpenDialogueBox();
        return false;
    }
    
    public void CloseTrue() => base.Close();

    #endregion
    

    public override void OnCancelRecursive() => Close();
}
