using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public Transform interactPoint;
    [SerializeField] private FMODDialogue dialogueSystem;
    [SerializeField] private bool ShowPopup;

    private CanvasGroup popupGroup;
    private Vector3 startSize;
    private Outline myOutline;
    public bool isInteractive = true;
    private Camera cam;

    [SerializeField, ReadOnly] public PlayerManager playerManager;
    [SerializeField, ReadOnly] private Transform PopupText;
    [SerializeField, ReadOnly] private bool isInRange;

    private void LateUpdate()
    {
        if(ShowPopup && isInRange)
        {
            //make text face the camera
            Vector3 relativePos = cam.transform.position - transform.position;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            popupGroup.transform.rotation = rotation;
            //popupGroup.transform.LookAt(transform.position + cam.transform.rotation*Vector3.forward, cam.transform.rotation*Vector3.up);
        }
    }
    protected virtual void Awake()
    {

        cam = Camera.main;
        if(ShowPopup)
        {
            popupGroup = GetComponentInChildren<CanvasGroup>();
            PopupText = transform.GetChild(0).GetChild(0);

            //alway put the position of the pop up top
            float popupPos = (GetComponent<Collider>().bounds.extents.y) + 0.25f;
            popupGroup.transform.position = transform.position + (Vector3.up * popupPos);

        }
        myOutline = GetComponent<Outline>();
        myOutline.enabled = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isInRange && isInteractive)
        {
            if(ShowPopup)
            {
                popupGroup.DOFade(1, .25f);
                //PopupText.localScale = Vector3.Lerp(PopupText.localScale, startSize, 0.05f);
            }

        }
        else
        {
            if(ShowPopup)
            {
                popupGroup.DOFade(0, .25f);
                //PopupText.localScale = Vector3.Lerp(PopupText.localScale, Vector3.zero, 0.1f);
            }

        }
    }

    public virtual void Interaction()
    {
        
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") )
        {
            myOutline.enabled = true;
            if(isInteractive)
            {
                if (ShowPopup)
                {
                    isInRange = true;
                }

                if (playerManager == null) { playerManager = col.GetComponent<PlayerManager>(); }
                playerManager.objectToInteractWith = this;
                playerManager.canInteract = true;

            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            myOutline.enabled = true;
        }

    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            myOutline.enabled = false;
        }
        if (isInRange) { isInRange = false; }
        playerManager.canInteract = false;
        playerManager.objectToInteractWith = null;
    }

    protected IEnumerator MakeInteractiveAgain()
    {
        yield return new WaitForSeconds(1f);
        isInteractive = true;
        if(isInRange)
        {
            playerManager.objectToInteractWith = this;
            playerManager.canInteract = true;
        }
    }
}
