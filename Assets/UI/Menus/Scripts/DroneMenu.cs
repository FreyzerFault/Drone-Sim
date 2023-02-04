using DroneSim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneMenu : Menu
{
    [SerializeField] private Sprite defaultDroneImage;

    private static readonly int SelectedAnimID = Animator.StringToHash("Selected");
    private static readonly int DisableAnimID = Animator.StringToHash("Disable");

    private int DroneSelectedID
    {
        get => DroneManager.Instance.currentDroneConfigurationIndex;
        set => DroneManager.Instance.currentDroneConfigurationIndex = value;
    }


    protected override void Start()
    {
        base.Start();

        SelectDrone(DroneSelectedID);

        // Carga los botones con el nombre y la imagen
        LoadButtons();

        OnClose += DroneManager.Instance.SaveSelectedDronePref;
    }

    public void SelectDrone(int newDroneID)
    {
        if (newDroneID >= DroneManager.Instance.droneConfigurations.Length)
            return;

        selectibles[DroneSelectedID].animator.SetBool(SelectedAnimID, false);
        selectibles[newDroneID].animator.SetBool(SelectedAnimID, true);
        firstSelected = selectibles[newDroneID];

        DroneSelectedID = newDroneID;

        DroneManager.Instance.currentDroneConfigurationIndex = newDroneID;
    }

    private void LoadButtons()
    {
        for (int i = 0; i < selectibles.Count; i++)
        {
            string droneName = "Locked";
            Sprite dronemage = defaultDroneImage;

            if (i < DroneManager.Instance.droneConfigurations.Length)
            {
                DroneSettingsSO droneConfig = DroneManager.Instance.droneConfigurations[i];
                droneName = droneConfig.configurationName;
                if (droneConfig.previewImage != null)
                    dronemage = droneConfig.previewImage;

                selectibles[i].GetComponent<Animator>().SetBool(DisableAnimID, false);
            }
            else
                selectibles[i].GetComponent<Animator>().SetBool(DisableAnimID, true);

            selectibles[i].GetComponentInChildren<TMP_Text>().text = droneName;
            selectibles[i].GetComponentsInChildren<Image>()[1].sprite = dronemage;
        }
    }
}