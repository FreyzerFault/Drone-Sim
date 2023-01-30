using DroneSim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneMenu : Menu
{
    public Sprite defaultDroneImage;
    
    private static readonly int Selected = Animator.StringToHash("Selected");
    private static readonly int Disable = Animator.StringToHash("Disable");

    public int DroneSelectedID
    {
        get => DroneManager.Instance.currentDroneConfigurationIndex;
        set => DroneManager.Instance.currentDroneConfigurationIndex = value;
    }
    
    
    protected override void Start()
    {
        base.Start();
        
        SelectDrone(DroneSelectedID);
        
        // Carga los botones con el nombre y la imagen
        for (int i = 0; i < selectibles.Count; i++)
        {
            string droneName = "Locked";
            Sprite dronemage = defaultDroneImage;
            
            if (i < DroneManager.Instance.droneConfigurations.Length)
            {
                DroneSettingsSO droneConfig = DroneManager.Instance.droneConfigurations[i];
                droneName = droneConfig.name;
                if (droneConfig.previewImage != null)
                    dronemage = droneConfig.previewImage;
                
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
        if (newDroneID >= DroneManager.Instance.droneConfigurations.Length)
            return;

        selectibles[DroneSelectedID].animator.SetBool(Selected, false);
        selectibles[newDroneID].animator.SetBool(Selected, true);
        firstSelected = selectibles[newDroneID];
        
        DroneSelectedID = newDroneID;

        DroneManager.Instance.currentDroneConfigurationIndex = newDroneID;
    }
}
