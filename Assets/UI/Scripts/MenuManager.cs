using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public List<SubMenu> subMenus;

    private SubMenu LevelMenu => subMenus[0];
    private SubMenu DroneMenu => subMenus[1];
    private SubMenu SettingsMenu => subMenus[2];

    private int menuOpened = -1;
    
    public TabsController settingsTabController;

    public int levelSelectedID;
    public int droneSelectedID;

    private static readonly int Open = Animator.StringToHash("open");
    private static readonly int Selected = Animator.StringToHash("Selected");

    public bool isLevelSelectionOpen => LevelMenu.GetComponent<Animator>().GetBool(Open);
    public bool isDroneSelectionOpen => DroneMenu.GetComponent<Animator>().GetBool(Open);

    private void Start()
    {
        UpdateSelectedLevel(levelSelectedID);

        for (int i = 0; i < subMenus.Count; i++)
        {
            int menuID = i;
            subMenus[i].OnClose += () => menuOpened = -1;
            subMenus[i].OnOpen += () => menuOpened = menuID;
        }
    }

    public void ToggleLevelSelection()
    {
        ToggleMenu(0);
    }
    
    public void ToggleDroneSelection()
    {
        ToggleMenu(1);
    }
    
    public void ToggleSettingsMenu()
    {
        ToggleMenu(2);
        
        if (subMenus[2].isOpen)
            settingsTabController.OpenTab(0);
    }

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
        CloseMenu();
        OpenMenu(menuID);
    }

    private void OpenMenu(int menuID)
    {
        if (menuOpened == menuID) return;
        subMenus[menuID].Open();
    }
    private void CloseMenu()
    {
        if (menuOpened == -1) return;
        subMenus[menuOpened].Close();
    }

    public void SelectLevel(int newLevelID)
    {
        LevelMenu.GetComponentsInChildren<Button>()[levelSelectedID].animator.SetBool(Selected, false);
        LevelMenu.GetComponentsInChildren<Button>()[newLevelID].animator.SetBool(Selected, true);
        levelSelectedID = newLevelID;
    }

    public void SelectDrone(int newDroneID)
    {
        DroneMenu.GetComponentsInChildren<Button>()[droneSelectedID].animator.SetBool(Selected, false);
        DroneMenu.GetComponentsInChildren<Button>()[newDroneID].animator.SetBool(Selected, true);
        droneSelectedID = newDroneID;
    }


    public void UpdateSelectedLevel(int id)
    {
        LevelMenu.firstSelected = LevelMenu.selectibles[id];
        LevelMenu.selectibles[levelSelectedID].animator.SetBool(Selected, false);
        LevelMenu.firstSelected.animator.SetBool(Selected, true);
        levelSelectedID = id;
    }
}
