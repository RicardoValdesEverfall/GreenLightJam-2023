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

    [Header("CLIMBING SETTINGS")]
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private float climbFallSpeed;
    [SerializeField] private float detectLength;
    [SerializeField] private float castRadius;
    [SerializeField] private float maxLookAtAngle;
    private float lookAtAngle;
    private bool isClimbing;
    private RaycastHit climbHit;
    private bool isClimbHit;



    [Header("DEBUG")]
    [SerializeField, ReadOnly] private Vector3 moveDirection;
    [SerializeField, ReadOnly] private Vector3 yVelocity;
    [SerializeField, ReadOnly] public bool isSprinting;
    [SerializeField, ReadOnly] public bool isJumping;
    [SerializeField, ReadOnly] public bool isGrounded;
    [SerializeField, ReadOnly] private bool isFalling;
    [SerializeField, ReadOnly] private float inAirTimer;
    [SerializeField, ReadOnly] private float initialJumpVelocity;
    [SerializeField, ReadOnly] public float currentSpeed;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;

        initialJumpVelocity = Mathf.Sqrt(-2 * gravity * maxJumpHeight);
        isClimbing = false;
    }

    public void HandleAllLocomotion()
    {
        HandleGroundCheck();
        CheckClimbing();
        HandleClimbing();

        if(!isClimbing)
        {
            //Handle normal movement
            HandleMovement();
            HandleRotation();

            HandleGravity();
        }
        //HandleAnimations();

    }

    private void CheckClimbing()
    {
        if (!isGrounded)
        {
            isClimbHit = Physics.SphereCast(transform.position, castRadius, transform.forward,
                out climbHit, detectLength, climbableLayer);

            //Debug.Log("It can climb? " + isClimbHit);
            lookAtAngle = Vector3.Angle(transform.forward, -climbHit.normal);
            //Debug.Log("look angle: " + wallLookAngle);
            if(isClimbHit && lookAtAngle < maxLookAtAngle)
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }
        }
        else
        {
            isClimbing = isClimbHit = false;

        }

        
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            moveDirection = Vector3.up * inputManager.verticalInput;
            moveDirection += transform.right * inputManager.horizontalInput;
            moveDirection.Normalize();


            playerCharacterController.Move(moveDirection * walkSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("no more climbing. Should fall");
            isClimbing = false;
        }
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        if (isSprinting)
        { 
            playerCharacterController.Move(moveDirection * (sprintSpeed - (inputManager.jumpInputTimer * 3)) * Time.deltaTime);
            currentSpeed = sprintSpeed;
        }

        else
        { 
            playerCharacterController.Move(moveDirection * (walkSpeed - (inputManager.jumpInputTimer * 3)) * Time.deltaTime);
            currentSpeed = sprintSpeed;
        }
        
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
            //if was in the air before, play landing sound once

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

    public void PerformJumpAction(float heldTimer) //The amount of time the player held the jump input. This is to perform a charged jump.
    {
        if (isJumping) { return; }
        if (!isGrounded) { return; }

        //Play jump animation
        //Play jump sound
        
        isJumping = true;

        yVelocity.y += initialJumpVelocity + heldTimer;
        playerCharacterController.Move(yVelocity * Time.deltaTime);
    }


    //DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, groundCheckBox * 2);
    }
}
