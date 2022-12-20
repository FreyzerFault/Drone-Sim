using UnityEngine;

// BAROMETER SENSOR
public class Barometer : MonoBehaviour {

    private float height;
    public float getHeight() { return nHeight.getNoise(height); }
    private float lastHeight;

    private float verticalSpeed;
    public float getverticalSpeed() { return nSpeed.getNoise(verticalSpeed); }
    private float lastSpeed;

    private float verticalAcc;
    public float getverticalAcc() { return nAcc.getNoise(verticalAcc); }

    NoiseAdder nHeight;
    NoiseAdder nSpeed;
    NoiseAdder nAcc;

    void Awake()
    {
        nHeight = new NoiseAdder();
        nSpeed = new NoiseAdder();
        nAcc = new NoiseAdder();

        lastHeight = height = transform.position.y;
        lastSpeed = verticalSpeed = 0;
        verticalAcc = 0;
    }
    
    void FixedUpdate()
    {
        height = transform.position.y;
        verticalSpeed = (height - lastHeight) / Time.deltaTime;
        verticalAcc = (verticalSpeed - lastSpeed) / Time.deltaTime;

        lastHeight = height;
        lastSpeed = verticalSpeed;
    }
}
