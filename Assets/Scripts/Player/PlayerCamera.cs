using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private InputManager playerInputManager;

    [Header("CAMERA COMPONENTS")]
    [SerializeField] private Transform playerObject;
    [SerializeField] private Transform CameraPivotTransform;

    [Header("CAMERA SETTINGS")]
    [SerializeField] private float CameraSmoothSpeed = 1;
    [SerializeField] private float LeftRightRotSpeed = 220;
    [SerializeField] private float UpDownRotSpeed = 220;

    [Header("DEBUG")]
    [SerializeField] private float LeftRightLookAngle;
    [SerializeField] private float UpDownLookAngle;

    private Vector3 CameraVelocity;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player").transform;
        playerInputManager = playerObject.GetComponent<InputManager>();
    }

    public void HandleCamera()
    {
        HandleFollowTarget();
        HandleRotations();
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerObject.position, ref CameraVelocity, CameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        LeftRightLookAngle += (playerInputManager.camHorizontalInput * LeftRightRotSpeed) * Time.deltaTime;
        UpDownLookAngle -= (playerInputManager.camVerticalInput * UpDownRotSpeed) * Time.deltaTime;

        Vector3 cameraRot = Vector3.zero;
        cameraRot.y = LeftRightLookAngle;
        Quaternion targetRot = Quaternion.Euler(cameraRot);
        transform.rotation = targetRot;

        cameraRot = Vector3.zero;
        cameraRot.x = UpDownLookAngle;
        targetRot = Quaternion.Euler(cameraRot);
        CameraPivotTransform.localRotation = targetRot;

    }

    //HandleFOV function for lerping between different FOVs based on isSprinting vs isWalking.
}
