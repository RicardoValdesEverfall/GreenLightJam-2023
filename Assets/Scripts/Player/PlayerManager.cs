using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private InputManager inputManager;
    private PlayerCamera playerCam;

    [SerializeField, ReadOnly] public Interactable objectToInteractWith;
    [SerializeField, ReadOnly] public bool isCinematicPlaying;

    private void Awake()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<PlayerCamera>();

        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (!isCinematicPlaying)
        {
            inputManager.HandleAllInputs();
            playerLocomotion.HandleAllLocomotion();
        }
      
    }

    private void LateUpdate()
    {
        if (!isCinematicPlaying)
        {
            playerCam.HandleCamera();
        }
    }
}
