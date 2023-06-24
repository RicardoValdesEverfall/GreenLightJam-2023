using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private InputManager inputManager;
    private PlayerCamera playerCam;

    private void Awake()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<PlayerCamera>();

        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        playerLocomotion.HandleAllLocomotion();
    }

    private void LateUpdate()
    {
     
        playerCam.HandleCamera();
    }
}
