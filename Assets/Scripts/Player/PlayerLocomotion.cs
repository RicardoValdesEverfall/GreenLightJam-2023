using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerManager playerManager;

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
    [SerializeField] private float fallingClimbingSpeed = 0;
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
        playerManager = GetComponent<PlayerManager>();
        cameraObject = Camera.main.transform;

        initialJumpVelocity = Mathf.Sqrt(-2 * gravity * maxJumpHeight);
        isClimbing = false;

        if(fallingClimbingSpeed == 0)
        {
            fallingClimbingSpeed = fallingSpeed / 2;
        }

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
        HandleAnimations();

    }
    
    //Method using spherecast and not colliders. OLD
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

    private void HandleClimbing()
    {
        // we need to:
        // 1. put cat in specific animations , do not turn orientation etc etc
        if (isClimbing && !isJumping)
        {
            bool notMoving = false;
            if(inputManager.verticalInput == 0 && inputManager.horizontalInput == 0)
            {
                //falldown
                moveDirection = currentRug.up * -1;
                notMoving = true;
            }
            else
            {
                moveDirection = currentRug.up * inputManager.verticalInput;
                moveDirection += transform.right * inputManager.horizontalInput;
            }
            //Along the X axis
            
            Debug.Log("vertical input: " + inputManager.verticalInput);
            Debug.Log("move dir: " + moveDirection);

            moveDirection.Normalize();
            playerCharacterController.Move(moveDirection * (notMoving ? fallingClimbingSpeed : walkSpeed * 0.3f) * Time.deltaTime);

        }
        else
            isClimbing = false;
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;
        Debug.Log(moveDirection.magnitude);

        if (moveDirection.magnitude < 0.2f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, 1.8f * Time.deltaTime);
        }

        else if (isSprinting)
        { 
            playerCharacterController.Move(moveDirection * (sprintSpeed - (inputManager.jumpInputTimer * 6)) * Time.deltaTime);
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, 2.8f * Time.deltaTime);
        }

        else
        { 
            playerCharacterController.Move(moveDirection * (walkSpeed - (inputManager.jumpInputTimer * 3)) * Time.deltaTime);
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, 1.8f * Time.deltaTime);
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
            //is jump from climbing
            yVelocity.y = initialJumpVelocity*2f;
            if (inputManager.horizontalInput == 0 )
            {
                //normal jump in perpendicular
                yVelocity += (currentRug.forward * 2f);
            }
            else
                yVelocity += transform.right * inputManager.horizontalInput * -1 * 2.5f;

            isClimbing = false;
            //we need to force perpendicular to where the cat is
            playerCharacterController.Move(yVelocity * Time.deltaTime);
            StartCoroutine("StopClimbJumpMomentum");
        }
        else
        {
            yVelocity.y += initialJumpVelocity + heldTimer;
            playerCharacterController.Move(yVelocity * Time.deltaTime);
        }
    }

    private void HandleAnimations()
    {
        playerManager.catAnimator.SetFloat("Movement", currentSpeed);
        playerManager.catAnimator.SetFloat("Jump", inputManager.jumpInputTimer);
        playerManager.catAnimator.SetBool("Climbing", isClimbing);
        playerManager.catAnimator.SetBool("Falling", isFalling);
    }

    //stops the jump from climbing momentum after .25 secs
    IEnumerator StopClimbJumpMomentum()
    {
        yield return new WaitForSeconds(0.25f);
        yVelocity.x = 0;
        yVelocity.z = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(climbableTag) && checkWithCollider)
        {
            //if the collision is on the side or bottom-up the nothing should happen
            Vector3 contactPointNormal = collision.GetContact(0).normal;
            if (contactPointNormal.x != 0 && contactPointNormal.z != 0)
            {
                currentRug = collision.transform;
                isClimbing = true;
                isJumping = false;

                transform.rotation = Quaternion.LookRotation(collision.transform.up, contactPointNormal);

            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(climbableTag) && checkWithCollider)
        {
            isClimbing = false;
        }
    }

    //DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, groundCheckBox * 2);

        //spherecast for climbing detection
        Gizmos.color = Color.red;

    }
}
