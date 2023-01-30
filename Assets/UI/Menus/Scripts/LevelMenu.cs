using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : Menu
{
    public Sprite defaultLevelImage;
    
    private static readonly int Selected = Animator.StringToHash("Selected");
    private static readonly int Disable = Animator.StringToHash("Disable");

    public int LevelSelectedID
    {
        get => LevelManager.Instance.currentLevel.ID;
        set => LevelManager.Instance.currentLevel = LevelManager.Instance.levels[value];
    }

    protected override void Start()
    {
        base.Start();
        
        SelectLevel(LevelSelectedID);
        
        // Carga los botones con el nombre y la imagen
        for (int i = 0; i < selectibles.Count; i++)
        {
            string levelName = "Locked";
            Sprite levelImage = defaultLevelImage;
            if (i < LevelManager.Instance.levels.Length)
            {
                levelName = LevelManager.Instance.levels[i].name;
                levelImage = LevelManager.Instance.levels[i].previewImage;
                selectibles[i].GetComponent<Animator>().SetBool(Disable, false);
            }
            else
            {
                selectibles[i].GetComponent<Animator>().SetBool(Disable, true);
            }
            
            selectibles[i].GetComponentInChildren<TMP_Text>().text = levelName;
            selectibles[i].GetComponentsInChildren<Image>()[1].sprite = levelImage;
        }
    }
    
    public void SelectLevel(int newLevelID)
    {
        if (newLevelID >= LevelManager.Instance.levels.Length)
            return;
        
        selectibles[LevelSelectedID].animator.SetBool(Selected, false);
        selectibles[newLevelID].animator.SetBool(Selected, true);
        firstSelected = selectibles[newLevelID];
            
        LevelSelectedID = newLevelID;
    }

    //
    // public void UpdateSelectedLevel(int id)
    // {
    //     firstSelected = LevelManager.;
    //     LevelMenu.selectibles[levelSelectedID].animator.SetBool(Selected, false);
    //     LevelMenu.firstSelected.animator.SetBool(Selected, true);
    //     levelSelectedID = id;
    // }
}
