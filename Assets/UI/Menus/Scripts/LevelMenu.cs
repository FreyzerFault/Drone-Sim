using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : Menu
{
    [SerializeField] private Sprite defaultLevelImage;
    
    private static readonly int SelectedAnimID = Animator.StringToHash("Selected");
    private static readonly int DisableAnimID = Animator.StringToHash("Disable");

    private int LevelSelectedID
    {
        get => LevelManager.Instance.currentLevel.ID;
        set => LevelManager.Instance.currentLevel = LevelManager.Instance.levels[value];
    }

    protected override void Start()
    {
        base.Start();
        
        SelectLevel(LevelSelectedID);
        
        // Carga los botones con el nombre y la imagen
        UpdateButtons();
        
        OnClose += LevelManager.Instance.SaveSelectedLevelPref;
    }
    
    public void SelectLevel(int newLevelID)
    {
        if (newLevelID >= LevelManager.Instance.levels.Length)
            return;
        
        selectibles[LevelSelectedID].animator.SetBool(SelectedAnimID, false);
        selectibles[newLevelID].animator.SetBool(SelectedAnimID, true);
        firstSelected = selectibles[newLevelID];
            
        LevelSelectedID = newLevelID;
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < selectibles.Count; i++)
        {
            string levelName = "Locked";
            Sprite levelImage = defaultLevelImage;
            if (i < LevelManager.Instance.levels.Length)
            {
                levelName = LevelManager.Instance.levels[i].name;
                levelImage = LevelManager.Instance.levels[i].previewImage;
                selectibles[i].GetComponent<Animator>().SetBool(DisableAnimID, false);
            }
            else
            {
                selectibles[i].GetComponent<Animator>().SetBool(DisableAnimID, true);
            }
            
            selectibles[i].GetComponentInChildren<TMP_Text>().text = levelName;
            selectibles[i].GetComponentsInChildren<Image>()[1].sprite = levelImage;
        }
    }
}
