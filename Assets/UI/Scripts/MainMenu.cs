using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject levelMenu;
    public GameObject droneMenu;

    public int levelSelectedID;
    public int droneSelectedID;

    private static readonly int Open = Animator.StringToHash("open");

    private void Awake()
    {
        GridLayoutGroup levelGrid = levelMenu.GetComponentInChildren<GridLayoutGroup>();
        GridLayoutGroup droneGrid = droneMenu.GetComponentInChildren<GridLayoutGroup>();

        Button[] levelButtons = levelGrid.GetComponentsInChildren<Button>();
        Button[] droneButtons = droneGrid.GetComponentsInChildren<Button>();

        // Selecciona por defecto un nivel y un dron
        if (levelSelectedID != -1 && levelSelectedID < levelButtons.Length) levelButtons[levelSelectedID].Select();
        if (droneSelectedID != -1 && droneSelectedID < droneButtons.Length) droneButtons[droneSelectedID].Select();
    }

    public void ToggleLevelSelection()
    {
        Animator animatorLevel = levelMenu.GetComponent<Animator>();
        Animator animatorDrone = droneMenu.GetComponent<Animator>();
        
        animatorLevel.SetBool(Open, !animatorLevel.GetBool(Open));
        animatorDrone.SetBool(Open, false);
    }
    
    public void ToggleDroneSelection()
    {
        Animator animatorLevel = levelMenu.GetComponent<Animator>();
        Animator animatorDrone = droneMenu.GetComponent<Animator>();
        
        animatorLevel.SetBool(Open, false);
        animatorDrone.SetBool(Open, !animatorDrone.GetBool(Open));
    }

    public void SelectLevel(int id) => levelSelectedID = id;
    public void SelectDrone(int id) => droneSelectedID = id;
}
