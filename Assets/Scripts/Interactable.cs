using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private bool ShowPopup;
    [SerializeField] private Transform PopupText;

    private Vector3 startSize;
    [SerializeField, ReadOnly] private bool isInRange;

    protected virtual void Awake()
    {
        PopupText = transform.GetChild(0).GetChild(0);
        startSize = PopupText.localScale;
        PopupText.localScale = Vector3.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isInRange && ShowPopup)
        {
            PopupText.localScale = Vector3.Lerp(PopupText.localScale, startSize, 0.05f);
        }
        else if (!isInRange && ShowPopup) { PopupText.localScale = Vector3.Lerp(PopupText.localScale, Vector3.zero, 0.1f); }
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (ShowPopup && col.CompareTag("Player"))
        {
            if (!isInRange)
            { 
                isInRange = true;
                col.GetComponent<PlayerManager>().objectToInteractWith = this;
            }
        }

    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (isInRange) { isInRange = false; }

    }
}
