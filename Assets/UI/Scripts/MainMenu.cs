using System;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject levelMenu;
    public GameObject droneMenu;
    public GameObject settingsMenu;

    public TabsController settingsTabs;

    public EventSystem mainMenuES;
    public EventSystem settingsES;

    private Animator animatorLevel;
    private Animator animatorDrone;
    private Animator animatorSettings;

    public GameObject lastButtonSelected;
    
    public int levelSelectedID;
    public int droneSelectedID;

    private static readonly int Open = Animator.StringToHash("open");
    private static readonly int Selected = Animator.StringToHash("Selected");

    public bool isLevelSelectionOpen => levelMenu.GetComponent<Animator>().GetBool(Open);
    public bool isDroneSelectionOpen => droneMenu.GetComponent<Animator>().GetBool(Open);

    private void Start()
    {
        animatorLevel = levelMenu.GetComponent<Animator>();
        animatorDrone = droneMenu.GetComponent<Animator>();
        animatorSettings = settingsMenu.GetComponent<Animator>();

        EventSystem.current = mainMenuES;
        transform.Find("Left Side/Buttons/Play").GetComponent<Button>().Select();
    }

    private bool cancelStated = false;
    private void Update()
    {
        if (!cancelStated)
        {
            InputSystemUIInputModule iuModule = (InputSystemUIInputModule) EventSystem.current.currentInputModule;
            iuModule.cancel.action.performed += (context) =>
            {
                CloseAll();
                lastButtonSelected.GetComponent<Button>().Select();
            };
        }
        cancelStated = true;
        //
        // bool levelOpen = isLevelSelectionOpen;
        // bool droneOpen = isDroneSelectionOpen;
        // if (iuModule.cancel.action.WasPressedThisFrame() && (levelOpen || droneOpen))
        // {
        //     if (levelOpen)
        //     {
        //         ToggleLevelSelection();
        //         transform.Find("Left Side/Buttons/Select Level").GetComponent<Button>().Select();
        //     }
        //     else
        //     {
        //         ToggleDroneSelection();
        //         transform.Find("Left Side/Buttons/Select Drone").GetComponent<Button>().Select();
        //     }
        // }
    }

    public void ToggleLevelSelection()
    {
        if (ToggleMenu(animatorLevel))
        {
            // Selecciona el nivel que ya habia antes seleccionado
            Button[] levelButtons = levelMenu.GetComponentsInChildren<Button>();
            if (levelSelectedID != -1 && levelSelectedID < levelButtons.Length)
            {
                levelButtons[levelSelectedID].animator.SetBool(Selected, true);
                levelButtons[levelSelectedID].Select();
            }
        }
    }
    
    public void ToggleDroneSelection()
    {
        if (ToggleMenu(animatorDrone))
        {
            // Selecciona el dron que ya habia antes seleccionado
            Button[] droneButtons = droneMenu.GetComponentsInChildren<Button>();
            if (droneSelectedID != -1 && droneSelectedID < droneButtons.Length)
            {
                droneButtons[droneSelectedID].animator.SetBool(Selected, true);
                droneButtons[droneSelectedID].Select();
            }
        }
    }
    
    public void ToggleSettingsMenu()
    {
        if (ToggleMenu(animatorSettings))
        {
            settingsTabs.OpenTab(0);
            GameManager.SwitchEventSystem(settingsES);
        }
    }

    // Abre o Cierra el Menu
    private bool ToggleMenu(Animator menuAnim)
    {
        // Cerrar
        if (menuAnim.GetBool(Open))
        {
            menuAnim.SetBool(Open, false);
            
            GameManager.SwitchEventSystem(mainMenuES);
            
            return false;
        }
        
        // Cierra lo que hubiese antes de abrirlo
        CloseAll();
            
        // Abre el menu
        menuAnim.SetBool(Open, true);

        lastButtonSelected = EventSystem.current.currentSelectedGameObject;
        
        return true;
    }

    // Cierra cualquier menu
    private void CloseAll()
    {
        GameManager.SwitchEventSystem(mainMenuES);
        
        animatorLevel.SetBool(Open, false);
        animatorDrone.SetBool(Open, false);
        animatorSettings.SetBool(Open, false);
    }

    public void SelectLevel(int newLevelID)
    {
        levelMenu.GetComponentsInChildren<Button>()[levelSelectedID].animator.SetBool(Selected, false);
        levelMenu.GetComponentsInChildren<Button>()[newLevelID].animator.SetBool(Selected, true);
        levelSelectedID = newLevelID;
    }

    public void SelectDrone(int newDroneID)
    {
        droneMenu.GetComponentsInChildren<Button>()[droneSelectedID].animator.SetBool(Selected, false);
        droneMenu.GetComponentsInChildren<Button>()[newDroneID].animator.SetBool(Selected, true);
        droneSelectedID = newDroneID;
    }

}
