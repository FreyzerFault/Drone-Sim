namespace DroneSim
{
    public class DroneFpvCamera : DroneCamera
    {
        protected override void OnEnable()
        {
            transform.position = Dron.FPVposition.position;
            transform.rotation = Dron.FPVposition.rotation;
        }

        protected override void LateUpdate()
        {
            transform.position = Dron.FPVposition.position;
            transform.rotation = Dron.FPVposition.rotation;
        }
    }
}
