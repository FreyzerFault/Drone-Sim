using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class DroneCamera : MonoBehaviour
{
    public enum CameraType
    {
        FPV = 0,
        TPV = 1,
        Static = 2,
    }
    public CameraType type;

    protected CinemachineVirtualCamera virtualCamera;
    private const int ACTIVE_PRIORITY = 10;
    private const int INACTIVE_PRIORITY = 5;

    protected virtual void Awake() => virtualCamera = GetComponent<CinemachineVirtualCamera>();

    public virtual void Enable() => virtualCamera.Priority = ACTIVE_PRIORITY;
    public virtual void Disable() => virtualCamera.Priority = INACTIVE_PRIORITY;
}
