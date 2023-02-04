using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DroneSim
{
    /**
     * Rotor redundante
     * No se aplican fisicas, solo visual
     * Usado en drones que tienen más o menos de 4 hélices
     */
    
    public class RotorOnlyVisual : Rotor
    {
        public Rotor nearRotor1 = null;
        public Rotor nearRotor2 = null;

        protected override void Awake()
        {
            base.Awake();
            
            FindNearestRotors();
        }

        protected override void Update()
        {
            // Media de potencias
            power = (nearRotor1.power + nearRotor2.power) / 2;
            
            UpdateSmoothPower();
            
            AnimatePropeller(SmoothPower);
            SetTexture(SmoothPower);
        }

        private void FindNearestRotors()
        {
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

        protected override void FixedUpdate() {}
    }
}
