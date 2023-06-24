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

    [Header("DEBUG")]
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] public bool isSprinting;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllLocomotion()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        //if is sprinting needs to be implemented here
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
}
