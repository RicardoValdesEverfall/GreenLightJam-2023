using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : Interactable
{
    protected override void Awake()
    {
        base.Awake();
    }

  

    public override void Interaction()
    {
        base.Interaction();
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
