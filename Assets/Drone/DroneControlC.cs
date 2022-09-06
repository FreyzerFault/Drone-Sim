using UnityEngine;
using System.Collections;

public class DroneControlC : MonoBehaviour {
	public Rigidbody Drone;
	public GameObject RButton;
	public GameObject LButton;

		
	   /*Speed*/public int forwardBackwardForce = 50; 
	   /*Speed*/public int panForce = 50; 
	   /*Speed*/public int leftRightForce = 50;  
	   /*Speed*/public int UpDownForce = 50; 

	public Vector3 DroneRotation;
	public bool Mobile;
	private float Rx;
	private float Ry;
	private float Lx;
	private float Ly;

	public float maxTorque = 10;
	public float enginePower = 100;

	void Update () {
		if (Mobile) 
		{
			Rx = RButton.GetComponent<DroneCanvasC>().Rx;
			Ry = RButton.GetComponent<DroneCanvasC>().Ry;
			Lx = LButton.GetComponent<DroneCanvasC>().Lx;
			Ly = LButton.GetComponent<DroneCanvasC>().Ly;
		}
	}

	void FixedUpdate () {
		DroneRotation = Drone.transform.localEulerAngles;

		enginePower = 100;
		
		// STABILIZATION Z axis
		if (DroneRotation.z > maxTorque && DroneRotation.z <= 180)
		{
			Drone.AddRelativeTorque (0, 0, -maxTorque);
		}
		else if (DroneRotation.z > 180 && DroneRotation.z <= 360 - maxTorque)
		{
			Drone.AddRelativeTorque (0, 0, maxTorque);
		}
		
		if (DroneRotation.z > 1 && DroneRotation.z <= maxTorque)
		{
			Drone.AddRelativeTorque (0, 0, -3);
		}
		else if (DroneRotation.z > maxTorque && DroneRotation.z <= 359)
		{
			Drone.AddRelativeTorque (0, 0, 3);
		}

		
		// STABILIZATION X axis
		if (DroneRotation.x > maxTorque && DroneRotation.x <= 180)
		{
			Drone.AddRelativeTorque (Vector3.right * -maxTorque);
		}
		else if (DroneRotation.x > 180 && DroneRotation.x <= 360 - maxTorque)
		{
			Drone.AddRelativeTorque (Vector3.right * maxTorque);
		}

		if (DroneRotation.x > 1 && DroneRotation.x <= 10)
		{
			Drone.AddRelativeTorque (Vector3.right * -3);
		}
		else if (DroneRotation.x > 350 && DroneRotation.x < 359)
		{
			Drone.AddRelativeTorque (Vector3.right * 3);
		}

		
		// PAN
		if(Mobile)
		{
			Drone.AddRelativeTorque(Vector3.up * Lx / 5);
		}
		else
		{
			// LEFT
			if (Input.GetKey(KeyCode.A))
			{
				Drone.AddRelativeTorque(-Vector3.up * panForce);
			}
			// RIGHT
			if (Input.GetKey(KeyCode.D))
			{
				Drone.AddRelativeTorque(Vector3.up * panForce);
			}
		}

		// MOVEMENT
		if(Mobile)
		{
			Drone.AddForce(transform.forward * Ly/2);
			if (Ly > 5)
			{
				Drone.AddRelativeTorque (maxTorque, 0, 0);
			}
			else if (Ly < -5)
			{
				Drone.AddRelativeTorque (-maxTorque, 0, 0);
			}

			Drone.AddRelativeForce(Rx,0,0);if(Rx>5){Drone.AddRelativeTorque (0, 0,-10);};if(Rx<-5){Drone.AddRelativeTorque (0, 0,10);}


			Drone.AddRelativeForce(0,Ry/2,0);//drone fly up or down
		}
		else 
		{
			// FORWARD
			if(Input.GetKey(KeyCode.W))
			{
				enginePower += forwardBackwardForce;
				//Drone.AddForce(transform.forward * forwardBackwardForce);
				Drone.AddRelativeTorque (Vector3.right * maxTorque);
			}
			
			// BACKWARD
			if (Input.GetKey(KeyCode.S))
			{
				enginePower += forwardBackwardForce;
				//Drone.AddForce(transform.forward * -forwardBackwardForce);
				Drone.AddRelativeTorque (Vector3.right * -maxTorque);
			}

			// LEFT
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				enginePower += leftRightForce;
				//Drone.AddForce(transform.right * -leftRightForce);
				Drone.AddRelativeTorque (Vector3.forward * maxTorque);
			}

			// RIGHT
			if (Input.GetKey(KeyCode.RightArrow))
			{
				enginePower += leftRightForce;
				//Drone.AddForce(transform.right * leftRightForce);
				Drone.AddRelativeTorque (Vector3.forward * -maxTorque);
			}
			
			// UP
			if (Input.GetKey(KeyCode.UpArrow))
			{
				enginePower += UpDownForce;
				//Drone.AddRelativeForce(Vector3.up * UpDown);
			}
			
			// DOWN
			if (Input.GetKey(KeyCode.DownArrow))
			{
				enginePower -= UpDownForce;
				//Drone.AddRelativeForce(Vector3.up * -UpDown);
			}
		}
		
		enginePower = Physics.gravity.magnitude / Mathf.Cos(Mathf.Max(DroneRotation.x, DroneRotation.z) * Mathf.Deg2Rad);
		
		// HOVER
		Drone.AddRelativeForce(Vector3.up * enginePower);
	}

}