using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;
    private InputManager playerInputManager;

    [Header("CAMERA COMPONENTS")]
    [SerializeField] private Transform playerObject;
    [SerializeField] private Transform cameraPivotTransform;
    [SerializeField] private Camera playerCam;

    [Header("CAMERA SETTINGS")]
    [SerializeField] private float CameraSmoothSpeed;
    [SerializeField] private float LeftRightRotSpeed;
    [SerializeField] private float UpDownRotSpeed;
    [SerializeField] private float camFOVwalk;
    [SerializeField] private float camFOVsprint;
    [SerializeField] private float camFOVidle;
    [SerializeField] private float camZOffsetwalk;
    [SerializeField] private float camZOffsetprint;
    [SerializeField] private float camZOffsetidle;

    [Header("DEBUG")]
    [SerializeField, ReadOnly] private float LeftRightLookAngle;
    [SerializeField, ReadOnly] private float UpDownLookAngle;

    private Vector3 CameraVelocity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else { Destroy(this.gameObject); }
    }

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

    public void HandleCameraPivot(float moveSpeed)
    {
        if (moveSpeed <= 0.5) { HandleSmoothCamera(camZOffsetidle, camFOVidle); }

                              //Absolutely awful implementation, I am sorry.
                              //I should be using PlayerManager for the flags but I kinda gave up lol.
        if (moveSpeed > 0.5 && !playerInputManager.playerLocomotion.isSprinting) { HandleSmoothCamera(camZOffsetwalk, camFOVwalk); }
        else if (moveSpeed > 0.5 && playerInputManager.playerLocomotion.isSprinting) { HandleSmoothCamera(camZOffsetprint, camFOVsprint); }
    }

    private void HandleSmoothCamera(float zPos, float FOV)    //HandleFOV function for lerping between different FOVs based on isSprinting vs isWalking.
    {
        Vector3 newPos = cameraPivotTransform.localPosition;
        newPos.z = Mathf.Lerp(newPos.z, zPos, 1.8f * Time.deltaTime);

        cameraPivotTransform.localPosition = newPos;
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, FOV, 1.8f * Time.deltaTime);
    }

    private void HandleRotations()
    {
        LeftRightLookAngle += (playerInputManager.camHorizontalInput * LeftRightRotSpeed) * Time.deltaTime;
        UpDownLookAngle -= (playerInputManager.camVerticalInput * UpDownRotSpeed) * Time.deltaTime;
        UpDownLookAngle = Mathf.Clamp(UpDownLookAngle, -30, 60);

        Vector3 cameraRot = Vector3.zero;
        cameraRot.y = LeftRightLookAngle;
        Quaternion targetRot = Quaternion.Euler(cameraRot);
        transform.rotation = targetRot;

        cameraRot = Vector3.zero;
        cameraRot.x = UpDownLookAngle;
        targetRot = Quaternion.Euler(cameraRot);
        cameraPivotTransform.localRotation = targetRot;

    }
}
