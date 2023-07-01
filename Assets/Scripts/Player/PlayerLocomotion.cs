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
    [SerializeField] private string climbableTag;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private bool checkWithCollider = false;
    private RaycastHit climbHit;
    private Transform currentRug;



    [Header("DEBUG")]
    [SerializeField, ReadOnly] private Vector3 moveDirection;
    [SerializeField, ReadOnly] private Vector3 yVelocity;
    [SerializeField, ReadOnly] public bool isSprinting;
    [SerializeField, ReadOnly] public bool isJumping;
    [SerializeField, ReadOnly] public bool isJumpingFromClimb;
    [SerializeField, ReadOnly] public bool isGrounded;
    [SerializeField, ReadOnly] private bool isFalling;
    [SerializeField, ReadOnly] private bool isClimbing;
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
        if(!checkWithCollider)
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
        //Debug.DrawRay(transform.position + playerCharacterController.center, transform.forward  * ( playerCharacterController.radius + 0.1f) , Color.yellow);
        if (!isGrounded)
        {
            /*bool isClimbHit = Physics.Raycast(transform.position + playerCharacterController.center, transform.forward, out climbHit, playerCharacterController.radius + 0.1f ,
                climbableLayer);*/
            bool isClimbHit = Physics.SphereCast(transform.position + playerCharacterController.center, playerCharacterController.radius, transform.forward, out climbHit,
                playerCharacterController.radius, climbableLayer);

            if (isClimbHit)
            {
                if(currentRug == null)
                    currentRug = climbHit.transform;
                transform.rotation = Quaternion.LookRotation(climbHit.transform.up);
                Vector3 dir = (climbHit.collider.ClosestPointOnBounds(transform.position) - transform.position).normalized;
                isClimbing = true;
            }
            else
            {
                currentRug = null;
                isClimbing = false;
            }
        }
        else
        {
            isClimbing = false;

        }

        
    }
   
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == climbableTag &&  checkWithCollider)
        {
            currentRug = other.transform;
            isClimbing = true;
            isJumping = false;
            transform.rotation = Quaternion.LookRotation(other.transform.up,other.transform.forward);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == climbableTag && checkWithCollider)
        {
            isClimbing = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == climbableTag && checkWithCollider)
        {
            if (!isJumping)
                isClimbing = true;
        }
    }


    private void HandleClimbing()
    {
        // we need to:
        // 1. put cat in specific animations , do not turn orientation etc etc
        // 2. put cat exactly on top of 3D object and perpendicular to the object- DONE!
        if (isClimbing && !isJumping)
        {
            //Always goes up
            moveDirection = Vector3.up * inputManager.verticalInput;
            //Along the X axis of the rug itself
            moveDirection += currentRug.right * inputManager.horizontalInput * -1;
            moveDirection.Normalize();


            playerCharacterController.Move(moveDirection * walkSpeed * 0.5f * Time.deltaTime);

            
        }
        else
        {
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
            playerCharacterController.Move(moveDirection * (sprintSpeed - (inputManager.jumpInputTimer * 6)) * Time.deltaTime);
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
        if (!isGrounded && !isClimbing) { return; }

        //Play jump animation
        //Play jump sound
        
        isJumping = true;

        if (isClimbing)
        {
            yVelocity.y += initialJumpVelocity*2f;
            Debug.Log("climb jump before: "+ yVelocity);
            if (inputManager.horizontalInput == 0 )
            {
                //normal jump in perpendicular
                yVelocity += (currentRug.forward *2f);
            }
            else
                yVelocity += currentRug.right * inputManager.horizontalInput * -1 * 2f;

            isClimbing = false;
            //we need to force perpendicular to where the cat is
            Debug.Log("climb jump after: " + yVelocity);
            playerCharacterController.Move(yVelocity * Time.deltaTime);
            StartCoroutine("StopClimbJumpMomentum");
        }
        else
        {
            yVelocity.y += initialJumpVelocity + heldTimer;
            playerCharacterController.Move(yVelocity * Time.deltaTime);
        }
    }

    IEnumerator StopClimbJumpMomentum()
    {
        yield return new WaitForSeconds(0.25f);
        //after quarter of a second remove the momentum
        yVelocity.x = 0;
        yVelocity.z = 0;
    }

  


    //DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, groundCheckBox * 2);

        //spherecast for climbing detection
        Gizmos.color = Color.red;

    }
}
