using System;
using DroneSim;
using UnityEngine;
using UnityEngine.UI;

public class DroneCameraManager : MonoBehaviour
{
    public enum CameraType
    {
        FPV = 0,
        TPV = 1,
        Static = 2,
    }
    [Serializable]
    public struct DroneCamera
    {
        public int Index => (int) type;
        public CameraType type;
        public Camera camera;
        public Sprite icon;
    }
    
    private DroneController dron;

    private int activeCamera = 0;
    public CameraType initialCam = CameraType.TPV;
    
    public DroneCamera[] Cameras;
    
    // UI
    public Image cameraIcon;

    public event Action OnCameraSwitched;

    private void Awake()
    {
        cameraIcon = GameObject.FindWithTag("Camera Icon").GetComponent<Image>();

        activeCamera = (int) initialCam;
        
        UpdateCamera();
        UpdateSprite();
    }

    public void SwitchCamera()
    {
        activeCamera = (activeCamera + 1) % Cameras.Length;

        UpdateCamera();
        UpdateSprite();

        OnCameraSwitched?.Invoke();
    }
    
    private void UpdateCamera() => GameManager.Camera = Cameras[activeCamera].camera;
    private void UpdateSprite() => cameraIcon.sprite = Cameras[activeCamera].icon;
}
