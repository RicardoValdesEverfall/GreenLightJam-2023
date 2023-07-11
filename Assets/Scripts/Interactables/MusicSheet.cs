using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheet : Interactable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Interaction()
    {
        base.Interaction();
    }

    protected override void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);

        if (col.CompareTag("Player"))
        {
            playerManager.HandlePickup();
        }
    }

    protected override void OnTriggerExit(Collider col)
    {
        base.OnTriggerExit(col);
    }
}
