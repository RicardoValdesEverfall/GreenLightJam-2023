using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : Interactable
{
    [SerializeField] private Vector3 forcePush;
    private Rigidbody bookRB;

    protected override void Awake()
    {
        base.Awake();
        bookRB = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Interaction()
    {
        base.Interaction();
        bookRB.AddForceAtPosition(forcePush, interactPoint.position, ForceMode.Impulse);
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
