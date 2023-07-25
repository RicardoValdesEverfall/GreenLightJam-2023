using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Yarn.Unity;
using TMPro;
using UnityEngine.Events;

public class ReadableItem : Interactable
{
	[SerializeField] string yarnNode;
	[SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] public UnityEvent AfterNarrativeInteraction;
    private bool isCurrentConversation = false;
    private bool alreadyCollected = false;

    protected override void Awake()
    {
        base.Awake();
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
            if (!alreadyCollected)
            {
                alreadyCollected = true;
                AfterNarrativeInteraction.Invoke();
            }
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
