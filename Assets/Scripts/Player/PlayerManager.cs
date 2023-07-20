using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using TMPro;
using Yarn.Unity;

public class PlayerManager : MonoBehaviour
{
    private PlayerLocomotion playerLocomotion;
    private InputManager inputManager;
    private PlayerCamera playerCam;

    [Header("Narrative Management")]
    [SerializeField] private DialogueRunner itemDialogues;
    [SerializeField] private CanvasGroup itemsPopup;

    [Header("DEBUG VALUES")]
    [SerializeField, ReadOnly] public PlayableDirector endCinematic;
    [SerializeField, ReadOnly] public Interactable objectToInteractWith;
    [SerializeField, ReadOnly] public Transform interactIKTarget;
    [SerializeField, ReadOnly] public Animator catAnimator;

    [SerializeField, ReadOnly] public bool isCinematicPlaying;
    [SerializeField, ReadOnly] public int musicSheetCounter;
    [SerializeField, ReadOnly] public bool churchComplete;
    [SerializeField, ReadOnly] public bool libraryComplete;
    [SerializeField, ReadOnly] public bool kitchenComplete;
    [SerializeField, ReadOnly] public bool dormComplete;
    [SerializeField, ReadOnly] public bool canInteract;
    [SerializeField, ReadOnly] private ReadableItem[] narrativeItems;

    //Week 4 - production Temp
    private int numScrolls = 0;
    private int scrollsCollected = 0;
    [SerializeField] TextMeshProUGUI scrollsTxt;


    //Unity Events
    public UnityEvent<int> itemCounter;

    private void Awake()
    {
        //playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<PlayerCamera>();
        catAnimator = gameObject.GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();

        //numScrolls = GameObject.FindGameObjectsWithTag("NarrativeItem").Length;
        //scrollsTxt.text = scrollsCollected + "/" + numScrolls;
        narrativeItems = FindObjectsOfType<ReadableItem>();
        foreach (ReadableItem item in narrativeItems)
        {
            item.SetupItem(itemDialogues, itemsPopup);
        }
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
            //playerCam.HandleCamera();
        }
    }

    //Week 4 tempo
    /*public void CollectScroll()
    {
        scrollsCollected++;
        scrollsTxt.text = scrollsCollected + "/" + numScrolls;
        if(scrollsCollected >= numScrolls)
        {
            //endCinematic.Play();
            //inputManager.HandleCursorState(CursorLockMode.Confined);
            scrollsTxt.text = "all collected! Thanks for playing";
        }
    }*/

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
        itemCounter.Invoke(musicSheetCounter); //What's this for?

        if (musicSheetCounter == 5)
        {
            endCinematic.Play();
            inputManager.HandleCursorState(CursorLockMode.Confined);
        }
    }
}
