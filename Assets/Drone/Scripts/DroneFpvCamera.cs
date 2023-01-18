namespace DroneSim
{
    public class DroneFpvCamera : DroneCamera
    {
        protected override void OnEnable()
        {
            transform.position = dron.FPVposition.position;
            transform.rotation = dron.FPVposition.rotation;
        }

        protected override void LateUpdate()
        {
            transform.position = dron.FPVposition.position;
            transform.rotation = dron.FPVposition.rotation;
        }
    }
}
