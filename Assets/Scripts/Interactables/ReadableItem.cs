using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Yarn.Unity;
using TMPro;

public class ReadableItem : Interactable
{
	[SerializeField] string yarnNode;
	DialogueRunner dialogueRunner;
    private bool isCurrentConversation = false;
    private bool alreadyCollected = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetupItem(DialogueRunner dr, CanvasGroup popup)
    {
        dialogueRunner = dr;
        popupCanvasGroup = popup;

        //events
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        dialogueRunner.onDialogueStart.AddListener(BeforeStartingItemDialogue);

        popUpTextUI = popupCanvasGroup.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        popUpTextUI.text = popUpText;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Interaction()
    {
        base.Interaction();
        if(!isCurrentConversation && !dialogueRunner.IsDialogueRunning)
        {
            isCurrentConversation = true;
		    dialogueRunner.StartDialogue(yarnNode);
            //hide popup
            TogglePopUp(false);
            //week 4 temp
            if (!alreadyCollected)
            {
                alreadyCollected = true;
                //playerManager.CollectScroll();
            }
        }
	}

    private void EndConversation()
    {
		Debug.Log("End dialogue event");
        if(isCurrentConversation)
        {
            isCurrentConversation = false;
            playerManager.isCinematicPlaying = false;
            ShowInteractiveFeedback(false);
            StartCoroutine(MakeInteractiveAgain());
        }
    }

    private void BeforeStartingItemDialogue()
    {
        if(isCurrentConversation)
        {
            playerManager.isCinematicPlaying = true;
            isInteractive = false;
            playerManager.canInteract = false;
            playerManager.objectToInteractWith = null;

        }
        
    }

    protected override void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);
    }

    protected override void OnTriggerExit(Collider col)
    {
        base.OnTriggerExit(col);
    }

   
}
