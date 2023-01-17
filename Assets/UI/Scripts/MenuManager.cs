using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public List<SubMenu> subMenus;

    private LevelMenu levelMenu;
    private SettingsMenu settingsMenu;
    
    private SubMenu DroneMenu => subMenus[1];

    private int menuOpened = -1;
    public int droneSelectedID;

    private static readonly int Open = Animator.StringToHash("open");

    public bool isLevelSelectionOpen => levelMenu.GetComponent<Animator>().GetBool(Open);
    public bool isDroneSelectionOpen => DroneMenu.GetComponent<Animator>().GetBool(Open);

    private void Start()
    {
        UpdateSelectedDrone(droneSelectedID);

        for (int i = 0; i < subMenus.Count; i++)
        {
            int menuID = i;
            subMenus[i].OnClose += () => menuOpened = -1;
            subMenus[i].OnOpen += () => menuOpened = menuID;
        }

        levelMenu = (LevelMenu) subMenus[0];
        settingsMenu = (SettingsMenu) subMenus[2];
    }

    public void ToggleLevelSelection() => ToggleMenu(0);

    public void ToggleDroneSelection() => ToggleMenu(1);

    public void ToggleSettingsMenu() => ToggleMenu(2);

    // Abre o Cierra el Menu
    private void ToggleMenu(int menuID)
    {
        // Si ya estaba abierto lo cierra
        if (menuOpened == menuID)
        {
            CloseMenu();
            return;
        }
        
        // Si no cerramos si hay alguno otro abierto y abrimos el menu
        if (CloseMenu())
            OpenMenu(menuID);
    }

    private void OpenMenu(int menuID)
    {
        if (menuOpened == menuID) return;
        subMenus[menuID].Open();
    }
    private bool CloseMenu()
    {
        if (menuOpened == -1) return true;
        return subMenus[menuOpened].Close();
    }

    public void SelectDrone(int newDroneID)
    {
        DroneMenu.GetComponentsInChildren<Button>()[droneSelectedID].animator.SetBool(Selected, false);
        DroneMenu.GetComponentsInChildren<Button>()[newDroneID].animator.SetBool(Selected, true);
        droneSelectedID = newDroneID;
    }


    
    private static readonly int Selected = Animator.StringToHash("Selected");
    
    public void UpdateSelectedDrone(int id)
    {
        DroneMenu.firstSelected = DroneMenu.selectibles[id];
        DroneMenu.selectibles[droneSelectedID].animator.SetBool(Selected, false);
        DroneMenu.firstSelected.animator.SetBool(Selected, true);
        droneSelectedID = id;
    }

    public void OnCancel() => CloseMenu();

    public void Play()
    {
        CloseMenu();
        
        LevelManager.Instance.LoadSelectedLevel();
    }
}
