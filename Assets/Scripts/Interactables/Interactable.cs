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
    [SerializeField] private CanvasGroup PopupCanvasGroup;

    private Vector3 startSize;
    private Outline myOutline;
    public bool isInteractive = true;
    private Camera cam;

    [SerializeField, ReadOnly] public PlayerManager playerManager;
    [SerializeField, ReadOnly] private bool isInRange;

    private void LateUpdate()
    {
        if(ShowPopup && isInRange)
        {
            //make text face the camera
            float popupPos = (GetComponent<Collider>().bounds.extents.y) + 0.25f;
            Vector3 worldPos = transform.position + (Vector3.up * popupPos);
            PopupCanvasGroup.transform.position = cam.WorldToScreenPoint(worldPos);
        }
    }
    protected virtual void Awake()
    {
        cam = Camera.main;
        myOutline = GetComponent<Outline>();
        myOutline.enabled = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    public virtual void Interaction()
    {
        
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && isInteractive)
        {
            myOutline.enabled = true;
            if (ShowPopup)
            {
                isInRange = true;
                PopupCanvasGroup.DOFade(1, .15f);
            }

            if (playerManager == null) { playerManager = col.GetComponent<PlayerManager>(); }
            playerManager.objectToInteractWith = this;
            playerManager.canInteract = true;

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isInteractive )
        {
            myOutline.enabled = true;
            isInRange = true;
        }

    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            myOutline.enabled = false;
            PopupCanvasGroup.DOFade(0, .15f);
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
