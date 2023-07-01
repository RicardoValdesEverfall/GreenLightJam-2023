using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private bool ShowPopup;
    [SerializeField] private Transform PopupText;
    [SerializeField] private bool ShowOutline = true;

    private Vector3 startSize;
    private Outline myOutline;
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
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isInRange)
        {
            if(ShowPopup)
                PopupText.localScale = Vector3.Lerp(PopupText.localScale, startSize, 0.05f);
            if (ShowOutline)
                myOutline.enabled = true;

        }
        else
        {
            if(ShowPopup)
                PopupText.localScale = Vector3.Lerp(PopupText.localScale, Vector3.zero, 0.1f);
            if(ShowOutline)
                myOutline.enabled = false;

        }
    }

    protected virtual void HandleInteract()
    {

    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (ShowPopup)
            {
                if (!isInRange)
                {
                    isInRange = true;
                   
                }
            }

            col.GetComponent<PlayerManager>().objectToInteractWith = this;
        }

    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (isInRange) { isInRange = false; }

    }
}
