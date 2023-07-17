using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Yarn.Unity;

public class ReadableItem : Interactable
{
	[SerializeField] DialogueRunner dialogueRunner;
	[SerializeField] string yarnNode;
    private bool isCurrentConversation = false;

    protected override void Awake()
    {
        base.Awake();
        if (dialogueRunner)
        {
            dialogueRunner.onDialogueComplete.AddListener(EndConversation);
            dialogueRunner.onDialogueStart.AddListener(BeforeStartingItemDialogue);

        }
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
		    dialogueRunner.StartDialogue(yarnNode);
            isCurrentConversation = true;
        }
	}

    private void EndConversation()
    {
				Debug.Log("End dialogue event");
        if(isCurrentConversation)
        {
            isCurrentConversation = false;
        }
        //playerManager.isCinematicPlaying = false;
        StartCoroutine(MakeInteractiveAgain());
    }

    private void BeforeStartingItemDialogue()
    {
        //playerManager.isCinematicPlaying = true;
        isInteractive = false;
        playerManager.canInteract = false;
        playerManager.objectToInteractWith = null;
        
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
