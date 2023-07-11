using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private InputManager inputManager;
    private PlayerCamera playerCam;

    [Header("DEBUG VALUES")]
    [SerializeField, ReadOnly] public PlayableDirector endCinematic;
    [SerializeField, ReadOnly] public Interactable objectToInteractWith;
    [SerializeField, ReadOnly] public Transform interactIKTarget;
    [SerializeField, ReadOnly] public Animator catAnimator;

    [SerializeField, ReadOnly] public bool isCinematicPlaying;
    [SerializeField, ReadOnly] private int musicSheetCounter;
    [SerializeField, ReadOnly] public bool churchComplete;
    [SerializeField, ReadOnly] public bool libraryComplete;
    [SerializeField, ReadOnly] public bool kitchenComplete;
    [SerializeField, ReadOnly] public bool dormComplete;
    [SerializeField, ReadOnly] public bool canInteract;

    private void Awake()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<PlayerCamera>();
        catAnimator = gameObject.GetComponentInChildren<Animator>();
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

    public void HandleInteraction()
    {
        if (objectToInteractWith != null)
        {
            if (canInteract)
            {
                objectToInteractWith.Interaction();
            }
        }
    }

    public void HandlePickup()
    {
        //Technically should add an IF or Switch statement here to check the type of pickup, but so far only the music sheets are pickups.

        objectToInteractWith.gameObject.SetActive(false);
        musicSheetCounter++;

        if (musicSheetCounter == 5)
        {
            endCinematic.Play();
            inputManager.HandleCursorState(CursorLockMode.Confined);
        }
    }
}
