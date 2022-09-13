using UnityEngine;

public class DronCamera3rdP : MonoBehaviour
{
    public DroneController drone;
    
    public float distanceOrbit;
    public float heightOrbit;
    
    private void Awake()
    {
        if (drone == null)
            drone = transform.parent.GetChild(0).GetComponent<DroneController>();
    }

    Vector3 smoothVel = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 dronePos = drone.transform.position;
        Vector3 droneForward = drone.transform.forward;

        
        Vector3 targetPos = dronePos
            - Vector3.ProjectOnPlane(droneForward, Vector3.up).normalized * distanceOrbit
            + Vector3.up * heightOrbit;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref smoothVel, 0.01f
            );


        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dronePos + droneForward - transform.position), 0.1f);
    }
}
