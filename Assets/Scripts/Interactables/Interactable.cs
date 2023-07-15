using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public Transform interactPoint;
    [SerializeField] private FMODDialogue dialogueSystem;
    [SerializeField] private bool ShowPopup;

    private Vector3 startSize;
    private Outline myOutline;
    public bool isInteractive = true;

    [SerializeField, ReadOnly] public PlayerManager playerManager;
    [SerializeField, ReadOnly] private Transform PopupText;
    [SerializeField, ReadOnly] private bool isInRange;

    protected virtual void Awake()
    {
        

        if(ShowPopup)
        {
            PopupText = transform.GetChild(0).GetChild(0);
            startSize = PopupText.localScale;
            PopupText.localScale = Vector3.zero;

        }
        myOutline = GetComponent<Outline>();
        myOutline.enabled = false;
        Debug.Log("disable outline at first");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isInRange && isInteractive)
        {
            if(ShowPopup)
                PopupText.localScale = Vector3.Lerp(PopupText.localScale, startSize, 0.05f);

        }
        else
        {
            if(ShowPopup)
                PopupText.localScale = Vector3.Lerp(PopupText.localScale, Vector3.zero, 0.1f);

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
