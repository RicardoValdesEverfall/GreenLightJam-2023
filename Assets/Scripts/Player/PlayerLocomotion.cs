using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private InputManager inputManager;

    [Header("PLAYER COMPONENTS")]
    [SerializeField] private Transform cameraObject;
    [SerializeField] private CharacterController playerCharacterController;

    [Header("LOCOMOTION SETTINGS")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float initialFallVelocity;   
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float maxJumpTime;
   
    [SerializeField, Range(0.01f, 0.1f)] private float fallingSpeed;   

    [Header("GRAVITY & GROUND SETTINGS")]
    [SerializeField] private float gravity;
    [SerializeField] private float groundedGravity;
    [SerializeField] private Vector3 groundCheckBox;
    [SerializeField] private LayerMask groundLayer;

    [Header("DEBUG")]
    [SerializeField, ReadOnly] private Vector3 moveDirection;
    [SerializeField, ReadOnly] private Vector3 yVelocity;
    [SerializeField, ReadOnly] public bool isSprinting;
    [SerializeField, ReadOnly] public bool isJumping;
    [SerializeField, ReadOnly] public bool isGrounded;
    [SerializeField, ReadOnly] private bool isFalling;
    [SerializeField, ReadOnly] private float inAirTimer;
    [SerializeField, ReadOnly] private float initialJumpVelocity;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;

        initialJumpVelocity = Mathf.Sqrt(-2 * gravity * maxJumpHeight);
    }

    public void HandleAllLocomotion()
    {
        HandleMovement();
        HandleRotation();

        HandleGravity();
        HandleGroundCheck();
        //HandleAnimations();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        if (isSprinting) { playerCharacterController.Move(moveDirection * sprintSpeed * Time.deltaTime); }
        else { playerCharacterController.Move(moveDirection * walkSpeed * Time.deltaTime); }
        
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero) { targetDirection = transform.forward; } //If there is no input we want to keep whatever last rotation we had.

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckBox(transform.position, groundCheckBox, Quaternion.identity, groundLayer);
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                isFalling = false;
                isJumping = false;
                yVelocity.y = groundedGravity;
            }
        }

        else
        {
            if (!isJumping && !isFalling)
            {
                isFalling = true;
                yVelocity.y = initialFallVelocity;
            }

            if (isJumping)
            {   
                if (inAirTimer >= maxJumpTime)
                {
                    isJumping = false;
                }
            }

            if (isFalling)
            {
                yVelocity.y -= fallingSpeed;
            }

            inAirTimer += Time.deltaTime;
            yVelocity.y += gravity * Time.deltaTime;
            //Set Animator falling float here
        }

        playerCharacterController.Move(yVelocity * Time.deltaTime);
    }

    public void PerformJumpAction()
    {
        if (isJumping) { return; }
        if (!isGrounded) { return; }

        //Play jump animation
        
        isJumping = true;

        yVelocity.y += initialJumpVelocity;
        playerCharacterController.Move(yVelocity * Time.deltaTime);
    }


    //DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, groundCheckBox * 2);
    }
}
