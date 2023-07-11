using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls; //The input map where all input actions and types are defined & registered.
    public PlayerLocomotion playerLocomotion;
    private PlayerAudio playerAudio;

    [Header("DEBUG")]
    [SerializeField, ReadOnly] public float verticalInput;
    [SerializeField, ReadOnly] public float horizontalInput;
    [SerializeField, ReadOnly] public float camHorizontalInput;
    [SerializeField, ReadOnly] public float camVerticalInput;

    [SerializeField, ReadOnly] private bool sprintInput;
    [SerializeField, ReadOnly] public bool jumpInput;
    [SerializeField, ReadOnly] public float jumpInputTimer;
    [SerializeField, ReadOnly] public bool meowInput;


    private Vector2 movementInput;
    private Vector2 cameraInput;
    private float moveAmount;


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

                           //The input action (locomotion) in the input map "PlayerControls"
                           //followed by the input type (movement).
            playerControls.PlayerLocomotion.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                                                                      //Our variable that
                                                                      //stores the input data.

            playerControls.PlayerCamera.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.started += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            playerControls.PlayerActions.Jump.started += i => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;

            playerControls.PlayerActions.Meow.started += i => meowInput = true;
            playerControls.PlayerActions.Meow.canceled += i => meowInput = false;
        }

        HandleCursorState(CursorLockMode.Locked);
        playerControls.Enable();
    }

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleCameraInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleMeowInput();
        //HandleInteractInput
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));


        PlayerCamera.Instance.HandleCameraPivot(moveAmount);
    }

    private void HandleCameraInput()
    {
        camVerticalInput = cameraInput.y;
        camHorizontalInput = cameraInput.x;
    }


    private void HandleMeowInput()
    {
        playerAudio.PlayMeow();
    }

    private void HandleSprintInput()
    {
        if (sprintInput) { playerLocomotion.isSprinting = true; }
        else { playerLocomotion.isSprinting = false; }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            if (jumpInputTimer < 1f)
            {
                jumpInputTimer += Time.deltaTime;
            }
            return;
        }

        if (!jumpInput && jumpInputTimer != 0)
        {
            if (jumpInputTimer < 0.3f) { jumpInputTimer = 0f; }
            playerLocomotion.PerformJumpAction(jumpInputTimer * 4);
            jumpInputTimer = 0;
        }
    }

    public void HandleCursorState(CursorLockMode state)
    {
        Cursor.lockState = state;
    }
}

