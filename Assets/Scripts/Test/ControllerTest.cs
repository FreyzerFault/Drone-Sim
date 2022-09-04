using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerTest : MonoBehaviour
{
    [SerializeField] public PlayerInput controller;

    private Vector2 move;
    private float lift;

    private Camera cam;
    private Vector2 camRotation;

    public float moveSpeed = 5f;
    public float liftSpeed = 5f;
    public float camRotationSpeed = 5f;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        transform.Translate(new Vector3(move.x, 0, move.y) * (Time.deltaTime * moveSpeed), Space.World);
        transform.position += Vector3.up * (lift * Time.deltaTime * liftSpeed);
        
        cam.transform.Rotate(camRotation * (Time.deltaTime * camRotationSpeed), Space.Self);
        
    }

    private void OnLift(InputValue value)
    {
        lift = value.Get<float>();
    }
    
    private void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    private void OnCamRotation(InputValue value)
    {
        camRotation = value.Get<Vector2>();
        camRotation = new Vector2(-camRotation.y, camRotation.x);
    }
}
