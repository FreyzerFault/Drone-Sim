using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DroneSim
{
    public class RotorOnlyVisual : Rotor
    {
        public Rotor nearRotor1 = null;
        public Rotor nearRotor2 = null;

        protected override void Awake()
        {
            base.Awake();

            Rotor[] rotors =
            {
                drone.rotorCW1,
                drone.rotorCW2,
                drone.rotorCCW1,
                drone.rotorCCW2,
            };
            
            float[] distanceToRotors =
            {
                Vector3.Distance(transform.position, drone.rotorCW1.transform.position),
                Vector3.Distance(transform.position, drone.rotorCW2.transform.position),
                Vector3.Distance(transform.position, drone.rotorCCW1.transform.position),
                Vector3.Distance(transform.position, drone.rotorCCW2.transform.position),
            };

            float distToDrone = Vector3.Distance(transform.position, drone.transform.position);

            for (int i = 0; i < 4; i++)
            {
                if (distanceToRotors[i] < distToDrone)
                {
                    if (nearRotor1 == null)
                        nearRotor1 = rotors[i];
                    else
                        nearRotor2 = rotors[i];
                }
            }
        }

        protected override void Update()
        {
            // Media de potencias
            power = (nearRotor1.power + nearRotor2.power) / 2;
            
            UpdateSmoothPower();
            
            AnimatePropeller(smoothPower);
            SetTexture(smoothPower);
        }

        protected override void FixedUpdate() {}
    }
}
