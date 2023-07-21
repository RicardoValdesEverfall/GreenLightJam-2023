using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{

    public enum PopUpDir { Top, Left, Right, Center };

    [SerializeField] public Transform interactPoint;
    //[SerializeField] private FMODDialogue dialogueSystem;
    [SerializeField] private bool floatEffect = false;
    [SerializeField] private bool showOutline = true;
    [SerializeField] private bool showPopup = true;
    [SerializeField] protected string popUpText = "E";
    [SerializeField] protected CanvasGroup popupCanvasGroup;
    [SerializeField] private PopUpDir popUpPosition;

    [SerializeField, ReadOnly] public PlayerManager playerManager;
    [SerializeField, ReadOnly] private bool isInRange;

    private Vector3 startSize;
    private Outline myOutline;
    private Camera cam;
    private Vector3 popUpWorldPos;

    protected bool isInteractive = true;
    protected TextMeshProUGUI popUpTextUI;


    private void LateUpdate()
    {
        if(showPopup && isInRange)
        {
            //always update text position
            Vector3 inScreenPos = cam.WorldToScreenPoint(popUpWorldPos);
            //popup can not leave screen
            inScreenPos.x = Mathf.Clamp(inScreenPos.x, 0, Screen.width);
            inScreenPos.y = Mathf.Clamp(inScreenPos.y, 0, Screen.height);
            popupCanvasGroup.transform.position = inScreenPos;
            
        }
    }

    protected virtual void Awake()
    {
        cam = Camera.main;
        myOutline = GetComponent<Outline>();
        myOutline.enabled = false;
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        if(popupCanvasGroup)
        {
            popUpTextUI = popupCanvasGroup.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        if(showPopup)
        {
            float popupPos = 0;
            switch (popUpPosition)
            {
                case PopUpDir.Top:
                    popupPos = (GetComponent<Collider>().bounds.extents.y) + 0.25f;
                    popUpWorldPos = transform.position + (Vector3.up * popupPos);
                    break;
                case PopUpDir.Left:
                    popupPos = (GetComponent<Collider>().bounds.extents.x) - 0.25f;
                    popUpWorldPos = transform.position + (Vector3.right * -1 * popupPos);
                    break;
                case PopUpDir.Right:
                    popupPos = (GetComponent<Collider>().bounds.extents.x) + 0.25f;
                    popUpWorldPos = transform.position + (Vector3.right * popupPos);
                    break;
                case PopUpDir.Center:
                    popUpWorldPos = transform.position;
                    break;
            }
        }
    }

    private void Start()
    {
        if(floatEffect)
        {
            transform.DOMoveY(transform.position.y + 0.1f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }

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
            isInRange = true;
            ShowInteractiveFeedback(true);

        }

    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isInteractive )
        {
            if(showOutline)
                myOutline.enabled = true;
            isInRange = true;
        }

    }*/

    protected virtual void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isInRange = false;
            ShowInteractiveFeedback(false);
            
        }
    }

    protected IEnumerator MakeInteractiveAgain()
    {
        yield return new WaitForSeconds(1.5f);
        isInteractive = true;
        if(isInRange)
        {
            ShowInteractiveFeedback(true);
        }
    }

    protected void ShowInteractiveFeedback(bool giveFeedback)
    {
        if(giveFeedback)
        {
            popUpTextUI.text = popUpText;
            Debug.Log("show feedback");
            Debug.Log(gameObject.name);
            if (showOutline)
                myOutline.enabled = true;
            if(showPopup)
                popupCanvasGroup.DOFade(1, .15f);
            playerManager.objectToInteractWith = this;
            playerManager.canInteract = true;
        }
        else
        {
            Debug.Log("hide feedback");
            if (showOutline)
                myOutline.enabled = false;
            if (showPopup)
                popupCanvasGroup.DOFade(0, .15f);
            playerManager.canInteract = false;
            playerManager.objectToInteractWith = null;
        }
    }

    protected void TogglePopUp(bool turnOn)
    {
        if(turnOn)
            popupCanvasGroup.DOFade(1, .15f);
        else
            popupCanvasGroup.DOFade(0, .15f);
    }
}
