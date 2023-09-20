using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private ClientBehaviour clientBehaviour;

    private static InputManager _instance;

    private Transform cameraTransform;

    public static InputManager Instance
    {
        get { return _instance; }
    }

    private PlayerInput playerInput;

    private InputAction playerMovementAction;
    private InputAction mouseMovementAction;
    private InputAction jumpAction;

    public Vector2 MovementInput;
    public Vector2 LookInput;
    public bool JumpInput;

    private CharacterController controller;

    public event Action<Vector3> SendMovementInput;
    //public event Action<Vector3> SendC

    private void Awake()
    {
        if(_instance != null && _instance != this) 
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        playerInput = GetComponent<PlayerInput>();
        playerMovementAction = playerInput.actions["Movement"];
        mouseMovementAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];

        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();   
    }

    private void Update()
    {
        MovementInput = playerMovementAction.ReadValue<Vector2>();
        LookInput = mouseMovementAction.ReadValue<Vector2>();
        JumpInput = jumpAction.triggered;
    }

    private void FixedUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward);
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        if (MovementInput != Vector2.zero)
        {
            Vector3 move = new Vector3(MovementInput.x, 0f, MovementInput.y);
            move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            move.y = 0f;

            //controller.Move(move.normalized * 10f * Time.deltaTime);

            //SendMovementInput?.Invoke(move.normalized);
        }
    }
}
