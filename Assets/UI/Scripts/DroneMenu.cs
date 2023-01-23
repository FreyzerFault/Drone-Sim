using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneMenu : MenuAnimated
{
    public Sprite defaultDroneImage;
    
    private static readonly int Selected = Animator.StringToHash("Selected");
    private static readonly int Disable = Animator.StringToHash("Disable");

    public int DroneSelectedID
    {
        get => DroneManager.Instance.currentDroneIndex;
        set => DroneManager.Instance.currentDroneIndex = value;
    }
    
    
    public void Start()
    {
        SelectDrone(DroneSelectedID);
        
        // Carga los botones con el nombre y la imagen
        for (int i = 0; i < selectibles.Count; i++)
        {
            string droneName = "Locked";
            Sprite dronemage = defaultDroneImage;
            
            if (i < DroneManager.Instance.drones.Length)
            {
                droneName = DroneManager.Instance.drones[i].name;
                if (DroneManager.Instance.previewImages[i] != null)
                    dronemage = DroneManager.Instance.previewImages[i];
                
                selectibles[i].GetComponent<Animator>().SetBool(Disable, false);
            }
            else
                selectibles[i].GetComponent<Animator>().SetBool(Disable, true);

            selectibles[i].GetComponentInChildren<TMP_Text>().text = droneName;
            selectibles[i].GetComponentsInChildren<Image>()[1].sprite = dronemage;
        }

        OnClose += DroneManager.Instance.SaveSelectedDronePref;
    }
    
    public void SelectDrone(int newDroneID)
    {
        if (newDroneID >= DroneManager.Instance.drones.Length)
            return;

        selectibles[DroneSelectedID].animator.SetBool(Selected, false);
        selectibles[newDroneID].animator.SetBool(Selected, true);
        firstSelected = selectibles[newDroneID];
        
        DroneSelectedID = newDroneID;
    }
}
