using DroneSim;
using Unity.VisualScripting;
using UnityEngine;

public class Wing : Rotor
{
    public enum WingType
    { FrontLeft, BackLeft, FrontRight, BackRight}

    public WingType wingType;

    private Animator animator;
    private int wingTypeID;

    private float angle = 0;
    private bool open = true;
    
    public float minAngle = -20;
    public float maxAngle = 80;
    public float velocity = 10;

    private Quaternion initRotation;
    private Quaternion maxRotation;

    protected override void Awake()
    {
        base.Awake();
        blurActivated = false;
        
        switch (wingType)
        {
            case WingType.FrontLeft:
                wingTypeID = Animator.StringToHash("fl");
                counterclockwise = true;
                break;
            case WingType.BackLeft:
                wingTypeID = Animator.StringToHash("bl");
                counterclockwise = true;
                break;
            case WingType.FrontRight:
                wingTypeID = Animator.StringToHash("fr");
                counterclockwise = false;
                break;
            case WingType.BackRight:
                wingTypeID = Animator.StringToHash("br");
                counterclockwise = false;
                break;
        }
    }

    protected override void Start()
    {
        base.Start();
        
        animator = drone.GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (animationActivated) AnimatePropeller(Power * velocity);
    }
    
    protected override void AnimatePropeller(float power_t)
    {
        //animator.SetFloat(wingTypeID, power_t);

        // Cuando pasa del maximo o baja de 0 => cambia el sentido
        if (angle > maxAngle || angle < minAngle)
        {
            open = !open;
            angle = angle > maxAngle ? maxAngle : minAngle;
        }
        
        angle += power_t * (open ? 1 : -1);
        
        float deltaAngle = power_t * (counterclockwise ? 1 : -1) * (open ? 1 : -1);
        transform.RotateAround(transform.parent.position, transform.parent.up, deltaAngle);
    }

    protected override void FixedUpdate() { }

    public override float Power
    {

        get
        {
            switch (wingType)
            {
                case WingType.FrontLeft:
                    return drone.rotorCW1.Power;
                case WingType.BackLeft:
                    return drone.rotorCCW1.Power;
                case WingType.FrontRight:
                    return drone.rotorCCW2.Power;
                case WingType.BackRight:
                    return drone.rotorCW2.Power;
            }
            return power;
        }
    }
    
}
