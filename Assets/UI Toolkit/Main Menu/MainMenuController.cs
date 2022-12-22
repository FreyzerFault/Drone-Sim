using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private UIDocument doc;

    private VisualElement levelSelectorScreen;
    private VisualElement droneSelectorScreen;

    private Button levelBackButton;
    private Button droneBackButton;
    
    private void OnEnable()
    {
        doc = GetComponent<UIDocument>();

        VisualElement root = doc.rootVisualElement;

        levelSelectorScreen = root.Q<VisualElement>("level-selector");
        droneSelectorScreen = root.Q<VisualElement>("drone-selector");
        
        VisualElement levelGrid = levelSelectorScreen.Q<VisualElement>("level-grid");
        VisualElement droneGrid = droneSelectorScreen.Q<VisualElement>("drone-grid");

        Button playButton = root.Q<Button>("play-button");
        Button selectLevelButton = root.Q<Button>("level-button");
        Button selectDroneButton = root.Q<Button>("drone-button");
        Button settingsButton = root.Q<Button>("settings-button");
        Button statsButton = root.Q<Button>("stats-button");
        Button quitButton = root.Q<Button>("quit-button");

        levelBackButton = levelSelectorScreen.Q<Button>(className: "back-button");
        droneBackButton = droneSelectorScreen.Q<Button>(className: "back-button");

        playButton.clicked += () => SceneManager.LoadScene(2);
        
        selectLevelButton.clicked += openLevelSelection;
        selectDroneButton.clicked += openDroneSelection;
        levelBackButton.clicked += openLevelSelection;
        droneBackButton.clicked += openDroneSelection;
        
        // TODO ===============================================================
        settingsButton.clicked += () => Debug.Log("Play Clicked");
        statsButton.clicked += () => Debug.Log("Play Clicked");
        // ====================================================================
        
        quitButton.clicked += GameManager.Quit;
    }

    private void openLevelSelection()
    {
        if (levelSelectorScreen.ClassListContains("selector-closed-level"))
            levelSelectorScreen.RemoveFromClassList("selector-closed-level");
        else
            levelSelectorScreen.AddToClassList("selector-closed-level");
        
        if (!droneSelectorScreen.ClassListContains("selector-closed-drone"))
            droneSelectorScreen.AddToClassList("selector-closed-drone");
    }

    private void openDroneSelection()
    {
        if (droneSelectorScreen.ClassListContains("selector-closed-drone"))
            droneSelectorScreen.RemoveFromClassList("selector-closed-drone");
        else
            droneSelectorScreen.AddToClassList("selector-closed-drone");
        
        if (!levelSelectorScreen.ClassListContains("selector-closed-level"))
            levelSelectorScreen.AddToClassList("selector-closed-level");
    }
}
